// library for color mixing and hsv to rgb
#include <Color.h>

// nice helper for dealing with timing related code
#include <elapsedMillis.h>

// my gem definition to encapsulate individual led sections for unique behavior, color, state.. etc
#include "Gem.h"

// shaders define pattern behaviors
#include "TopShader.h"
#include "StarFallDown.h"
#include "StarRiseUp.h"
#include "StarLinger.h"
#include "TwinkleShader.h"
#include "GlowShader.h"
#include "PulsingShader.h"
#include "RisingShader.h"
#include "MultiColorShader.h"
#include "MultiGlowShader.h"
#include "MultiTwinkleShader.h"
#include "BodyTwinkler.h"
#include "BassShader.h"
#include "StrobingShader.h"
#include "CandyStrobingShader.h"
#include "StarConstellationFull.h"
#include "StarCaught.h"
#include "PulsingLinger.h"

// my color presets
#include "colors.h"

// the dma to ws2811 magic
#include <OctoWS2811.h>


// osc messaging makes serial easy
#include <OSCBundle.h>
#include <OSCBoards.h>

// makes osc over serial work
#ifdef BOARD_HAS_USB_SERIAL
#include <SLIPEncodedUSBSerial.h>
SLIPEncodedUSBSerial SLIPSerial( thisBoardsSerialUSB );
#else
#include <SLIPEncodedSerial.h>
SLIPEncodedSerial SLIPSerial(Serial1);
#endif

#define DEBUG false


////
// LED CONFIG
//const int ledsPerStrip = 90;
//const int ledsPerStrip = 80;
//const int ledsPerStrip = 128;  // 8*16

// one long stick
const int ledsPerStrip = 144;  // 32 + 7*16


// declaring some memory for led buffer
DMAMEM int displayMemory[ledsPerStrip*6];
int drawingMemory[ledsPerStrip*6];
const int config = WS2811_GRB | WS2811_800kHz;
OctoWS2811 leds(ledsPerStrip, displayMemory, drawingMemory, config);


// create an instance of each shader to use from here on
Shader* behavior[] = {
  new StarFallDown(), // falling 0
  new StarLinger(), // lingering 1
  new TwinkleShader(), // 2
  new GlowShader(), //glow 3
  new PulsingLinger(), // 4
  new RisingShader(), // rise 5
  new BassShader(), // bass 6
  new MultiColorShader(), // colors 7
  new MultiGlowShader(), // multiglow 8
  new MultiTwinkleShader(), // multitwinkle 9
  new StrobingShader(), // strobe 10
  new CandyStrobingShader(), // candystrobe 11
  new StarConstellationFull(), // 12
  new StarCaught(), //13
  new StarRiseUp() //14

  };

// nice to keep an array size value around for validation and looping
int behaviorCount = 15; 

#define LINGER 1
#define FALL 0
#define CAUGHT 13
#define CONSTELLATION 12
#define PULSE_LINGER 4
#define RETURN 5
#define GLOW 3



// these are used for run-time configuration from message events
int currentColor;
int currentSecondaryColor;
int currentDuration;


float currentHue = 0.12;
float currentSaturation = 0.3;
float currentBrightness = 0.5;

float colorSkew = 0.01;

boolean hitSync = false;

long loopElapsed;
int fps;

// keep track of the gems
const int gemCount = 64;
Gem gems[gemCount];

// todo : pixels per-gem
// led count of strip within each gem
int shortGemPixelCount = 16;
int longGemPixelCount = 2*shortGemPixelCount;

// serial event globals 
String inputString = "";         // a string to hold incoming data
boolean stringComplete = false;  // whether the string is complete


void setup() {

  // config led pins
  leds.begin();
  leds.show();

  // setup OSC listener
  SLIPSerial.begin(38400);   // set this as high as you can reliably run on your platform

  // fps calculation
  loopElapsed = millis();

  // create neighbors for multi gem effects
  behavior[CAUGHT]->neighbors = 3;


  int pixelStart = 0;
  
  // initialize our gems
  for (int i = 0; i < gemCount; i++ ) {
    int pixelCount = shortGemPixelCount;
    // if first stick is long
    //if (i%8 == 0) pixelCount = longGemPixelCount;


    // if last stick is long
    if (i%8 == 7) pixelCount = longGemPixelCount;

    
    // todo Gem Number and Gem Count need to respect more Gems per Octo Channel
    gems[i] = Gem(i, pixelCount, pixelStart, &leds, behavior[0]);
    gems[i].setColor(color[i%7]);
    gems[i].setSecondaryColor(2);
    gems[i].setDuration(2000);

    pixelStart+=pixelCount;

  }

  pinMode(0,OUTPUT);
  digitalWrite(0,HIGH);

  setColorsWithHsb();

  return; 
}


void loop() {
  pollForNewOscMessages();

  updateGems();

  leds.show();

  fps = 1000 / (millis() - loopElapsed);
  loopElapsed = millis();
  return; 
}


// check serial for pending hits
void pollForNewOscMessages() {
  // process all serial stream

  // NotSerialEvent();

  OSCMessage msg;  
  int size;

  // to test hit all
  for (int i = 0; i<gemCount; i++){
    // hit(i);
  }

  if( (size =SLIPSerial.available()) > 0) {
    while(!SLIPSerial.endofPacket())
      if( (size =SLIPSerial.available()) > 0)
      {
        while(size--) {
          msg.fill(SLIPSerial.read());
        }
      }


    if(!msg.hasError()) {

      msg.dispatch("/allfall", routeAllFall);
      msg.dispatch("/allcaught", routeAllCaught);
      msg.dispatch("/starfall", routeFallingStar);
      msg.dispatch("/starreturn", routeReturnStar);
      msg.dispatch("/starcool", routeFallingStarCool);
      msg.dispatch("/starwarm", routeFallingStarWarm);
      msg.dispatch("/starglow", routeFallingStarGlow);
      msg.dispatch("/starlinger", routeLingeringStar);
      msg.dispatch("/faintstarlinger", routePulseLingeringStar);
      msg.dispatch("/starcaught", routeCaughtStar);
      msg.dispatch("/constellationfull", routeConstellationFull);
      msg.route("/color", routeColor);
      msg.route("/behavior", routeBehavior);
      msg.route("/info", routeInfo);
      msg.route("/duration", routeDuration);
      msg.route("/neighbors", routeNeighbors);

    }

    msg.empty();

  }


  return;
}


void routeAllFall(OSCMessage &msg){
  // triggers animation on all stars with last set behavior id
  for (int i = 0; i<gemCount; i++){
    hit(i);
  }
}

void routeAllCaught(OSCMessage &msg){
  for (int i = 0; i < gemCount; i++) {
    triggerStar(i, 3500, CAUGHT);
  }
  
}

void routeCaughtStar(OSCMessage &msg){
  triggerStar( msg, CAUGHT);
}


void routeFallingStarCool(OSCMessage &msg){
  currentColor = 0x508679;
  triggerStar( msg, FALL);
}

void routeFallingStarWarm(OSCMessage &msg){
  currentColor = 0xF1AF62;
  triggerStar( msg, FALL);
}

void routeFallingStarGlow(OSCMessage &msg){
  triggerStar( msg, GLOW);
}

void routeFallingStar(OSCMessage &msg){
  currentColor = 0xFDF1E4;
  triggerStar( msg, FALL);
}

void routeReturnStar(OSCMessage &msg){
  currentColor = 0xFDF1E4;
  triggerStar( msg, RETURN);
}



void routeLingeringStar(OSCMessage &msg){
  triggerStar( msg, LINGER);
}



void routePulseLingeringStar(OSCMessage &msg){
  triggerStar( msg, PULSE_LINGER);
}


void routeConstellationFull(OSCMessage &msg){
  //triggerStar( msg, LINGER);
  for (int i = 0; i < gemCount; i++) {
    triggerStar(i,1500, CONSTELLATION);
  }
  
}

void triggerStar(OSCMessage &msg, int behaviorId) {
  int starId = 0;
  int duration = 5000;
  
  switch (msg.size()) {
    case 2:
      if (msg.isInt(1))
        duration = msg.getInt(1);

    case 1:
      if (msg.isInt(0))
        starId = msg.getInt(0);
  }
  
  triggerStar( starId,  duration, behaviorId);
}


void triggerStar(int starId, int duration, int behaviorId) {
    gems[starId].setShader(behavior[behaviorId]);
    gems[starId].setDuration(duration);
    gems[starId].setColor(currentColor); 

    hit(starId);
    return;
}



void hit(int gemIndex) {
  if (DEBUG) {

    Serial.print("HIT:");
    Serial.println(gemIndex);
    Serial.print("fps: ");
    Serial.println(fps);
  }

  if (hitSync) {
    gems[gemIndex].setDuration(currentDuration);
    setGemColor(gemIndex, currentColor, currentSecondaryColor);
  }

  gems[gemIndex].hit();
  hitNeighbors(gemIndex);
  return;
}

void hitNeighbors(int gemIndex) {

  int currentGemsMaxNeighborEffect = gems[gemIndex].getShader()->neighbors;

  for (int i = 1; i < currentGemsMaxNeighborEffect; i++) {
    hitNeighbor(gemIndex, i, gems[gemIndex].getShader());
  }

}

void hitNeighbor(int gemIndex, int distance, Shader* shader) {
  
  if (isValidGem(gemIndex-distance)) {
    gems[gemIndex-distance].setShader(shader);
    gems[gemIndex-distance].neighborHit(distance);
  }
  if (isValidGem(gemIndex+distance)) {
    gems[gemIndex+distance].setShader(shader);
    gems[gemIndex+distance].neighborHit(distance);
  }
  return;
}



void hitNeighbor(int gemIndex, int distance) {
  if (isValidGem(gemIndex-distance)) {
    gems[gemIndex-distance].neighborHit(distance);
  }
  if (isValidGem(gemIndex+distance)) {
    gems[gemIndex+distance].neighborHit(distance);
  }
  return;
}


boolean isValidGem(int gemIndex) {
  if ((gemIndex >= 0) && (gemIndex < 8)) 
    return true;

  return false;

}


void updateGems() {

  // to test hit all
  for (int i = 0; i<gemCount; i++){
    gems[i].animate();
  }


  return;    
}



/*
  SerialEvent occurs whenever a new data comes in the
 hardware serial RX.  This routine is run between each
 time loop() runs, so using delay inside loop can delay
 response.  Multiple bytes of data may be available.
 */
void NotSerialEvent() {
  while (Serial.available()) {
    // get the new byte:
    char inChar = (char)Serial.read(); 

    int gemIn = (int)inChar - 48;

    if ((gemIn > -1) && ( gemIn < 9)) {
      if (DEBUG) {
        Serial.print("HIT: ");
        Serial.println(inChar);
      }
      hit(gemIn);
    }

    if (DEBUG) {
      Serial.print(inChar);  
    }
  }

}



//called after matching '/0'
void routeGem(OSCMessage &msg, int patternOffset){
  //Serial.println("Match: '/gem'");
  msg.route("/1", routeGemOne, patternOffset);
  msg.route("/2", routeGemTwo, patternOffset);

}


//called after matching '/0'
void routeGemOne(OSCMessage &msg, int patternOffset){
  //Serial.println("Match: '/gem/1'");
  //msg.route("/1", routeGemOne, patternOffset);
  matchGemHit(msg, patternOffset, 0);
}


//called after matching '/0'
void routeGemTwo(OSCMessage &msg, int patternOffset){
  //Serial.println("Match: '/gem/2'");
  //msg.route("/2", routeGemOne, patternOffset);
  matchGemHit(msg, patternOffset, 4);
}


void matchGemHit(OSCMessage &msg, int patternOffset, int gemOffset) {   

  if ( 0 < msg.match("/1",patternOffset)) {
    hit(gemOffset+0);
  }
  if ( 0 < msg.match("/2",patternOffset)) {
    hit(gemOffset+1);
  }
  if ( 0 < msg.match("/3",patternOffset)) {
    hit(gemOffset+2);
  }
  if ( 0 < msg.match("/4",patternOffset)) {
    hit(gemOffset+3);
  }

}





/**
 * OSC ROUTE : /neighbors <float>
 */
void routeNeighbors(OSCMessage &msg, int patternOffset) {

  if (msg.isFloat(0)) {
    setNeighbors(msg.getFloat(0) * 7);
  }

}



/**
 * OSC ROUTE : /info
 */
void routeInfo(OSCMessage &msg, int patternOffset){
  if (DEBUG) {
    Serial.print("fps: ");
    Serial.println(fps);
  }
}



/**
 * OSC parsing helper : returns number portion from touchOsc buttons
 */
int matchNumbers(OSCMessage &msg, int patternOffset) {  
  if ( 0 < msg.match("/1/1",patternOffset)) {
    return 0;
  }
  if ( 0 < msg.match("/2/1",patternOffset)) {
    return 1;
  }
  if ( 0 < msg.match("/3/1",patternOffset)) {
    return 2;
  }
  if ( 0 < msg.match("/4/1",patternOffset)) {
    return 3;
  }
  if ( 0 < msg.match("/5/1",patternOffset)) {
    return 4;
  }
  if ( 0 < msg.match("/6/1",patternOffset)) {
    return 5;
  }
  if ( 0 < msg.match("/7/1",patternOffset)) {
    return 6;
  }
  if ( 0 < msg.match("/8/1",patternOffset)) {
    return 7;
  }

  return 0;
} 


/**
 * OSC ROUTE : /behavior
 */
void routeBehavior(OSCMessage &msg, int patternOffset){

  int newBehavior = 0;

  if ( 0 < msg.match("/0",patternOffset)) {
    newBehavior = 0;
  }
  if ( 0 < msg.match("/1",patternOffset)) {
    newBehavior = 1;
  }
  if ( 0 < msg.match("/2",patternOffset)) {
    newBehavior = 2;
  }
  if ( 0 < msg.match("/3",patternOffset)) {
    newBehavior = 3;
  }
  if ( 0 < msg.match("/4",patternOffset)) {
    newBehavior = 4;
  }
  if ( 0 < msg.match("/5",patternOffset)) {
    newBehavior = 5;
  }
  if ( 0 < msg.match("/6",patternOffset)) {
    newBehavior = 6;
  }
  if ( 0 < msg.match("/7",patternOffset)) {
    newBehavior = 7;
  }
  if ( 0 < msg.match("/8",patternOffset)) {
    newBehavior = 8;
  }

  if ( 0 < msg.match("/9",patternOffset)) {
    newBehavior = 9;
  }

  if ( 0 < msg.match("/10",patternOffset)) {
    newBehavior = 10;
  }

  if ( 0 < msg.match("/11",patternOffset)) {
    newBehavior = 11;
  }

  if ( 0 < msg.match("/12",patternOffset)) {
    newBehavior = 12;
  }

  if ( 0 < msg.match("/13",patternOffset)) {
    newBehavior = 13;
  }

  if ( 0 < msg.match("/14",patternOffset)) {
    newBehavior = 14;
  }



  if ( 0 < msg.match("/body",patternOffset)) {
    newBehavior = 0;
  }
  if ( 0 < msg.match("/twinkler",patternOffset)) {
    newBehavior = 1;
  }
  if ( 0 < msg.match("/glow",patternOffset)) {
    newBehavior = 2;
  }
  if ( 0 < msg.match("/pulse",patternOffset)) {
    newBehavior = 3;
  }
  if ( 0 < msg.match("/rise",patternOffset)) {
    newBehavior = 4;
  }
  if ( 0 < msg.match("/bass",patternOffset)) {
    newBehavior = 5;
  }
  if ( 0 < msg.match("/colors",patternOffset)) {
    newBehavior = 6;
  }
  if ( 0 < msg.match("/multiglow",patternOffset)) {
    newBehavior = 7;
  }
  if ( 0 < msg.match("/multitwinkle",patternOffset)) {
    newBehavior = 8;
  }

  if ( 0 < msg.match("/strobe",patternOffset)) {
    newBehavior = 9;
  }

  if ( 0 < msg.match("/candystrobe",patternOffset)) {
    newBehavior = 10;
  }

  if ( 0 < msg.match("/b",patternOffset)) {
    newBehavior = 11;
  }


  setBehaviors(newBehavior);

  return;

}


/**
 * sets behavior of all gems
 */
void setBehaviors(int matchingIndex) {
  for (int i = 0; i < gemCount; i++ ) {
    gems[i].setShader(behavior[matchingIndex%behaviorCount]);

    // set colors

  }
}



void setNeighbors(int neighbors) {
  for (int i = 0; i < gemCount; i++ ) {
    gems[i].getShader()->neighbors = neighbors;
  }
  return;
}



////
// COLORS

/**
 * OSC ROUTE : /color
 */
void routeColor(OSCMessage &msg, int patternOffset){

  msg.route("/hue", hue, patternOffset);
  msg.route("/saturation", saturation, patternOffset);
  msg.route("/brightness", brightness, patternOffset);
  msg.route("/skew", skew, patternOffset);
  msg.route("/hitsync", routeHitSync, patternOffset);


}

/**
 * OSC ROUTE : /color/hue <float>
 */
void hue(OSCMessage &msg, int patternOffset){

  if (msg.isFloat(0)) {
    currentHue = msg.getFloat(0);
  }

  setColorsWithHsb();

  return;

}

/**
 * OSC ROUTE : /color/saturation <float>
 */
void saturation(OSCMessage &msg, int patternOffset){

  if (msg.isFloat(0)) {
    currentSaturation = msg.getFloat(0);
  }

  setColorsWithHsb();

  return;
}



/**
 * OSC ROUTE : /color/brightness <float>
 */
void brightness(OSCMessage &msg, int patternOffset){

  if (msg.isFloat(0)) {
    currentBrightness = msg.getFloat(0);
  }

  setColorsWithHsb();

  return;

}


/**
 * OSC ROUTE : /color/skew <float>
 */
void skew(OSCMessage &msg, int patternOffset){

  if (msg.isFloat(0)) {
    colorSkew = msg.getFloat(0);
  }

  setColorsWithHsb();

  return;

}


void setGemColor(int gemIndex, int color, int secondaryColor) {
  gems[gemIndex].setColor(color); 

  gems[gemIndex].setSecondaryColor(secondaryColor); 

  return;
}

void setColorsWithHsb() {

  Color color = Color(1,1,1);

  color.convert_hcl_to_rgb(currentHue,currentSaturation,currentBrightness);

  currentColor = ((uint32_t)color.red << 16) | ((uint32_t)color.green <<  8) | color.blue;

  currentSecondaryColor = currentColor;


  if (0 != colorSkew)  {

    float hueShift = (random((int)(colorSkew*1000.0))) / 1000.0;
    color.convert_hcl_to_rgb(currentHue+hueShift,currentSaturation,currentBrightness);
    currentSecondaryColor = ((uint32_t)color.red << 16) | ((uint32_t)color.green <<  8) | color.blue;

  }

  if (false == hitSync) {
    for (int i = 0; i < gemCount; i++ ) {
      setAllGemColors();
    }

  }
  return;
}



void setAllGemColors() {

  for (int i = 0; i < gemCount; i++ ) {
    setGemColor(i, currentColor, currentSecondaryColor);
  }
  return;
}



////
// DURATION

/**
 * OSC ROUTE : /duration
 *  - accepts float from 0.0 to 1.0 as duration percent of 6sec
 */
void routeDuration(OSCMessage &msg, int patternOffset){


  if (msg.isFloat(0)){
    currentDuration = msg.getFloat(0) * 6000;
    
    if (false == hitSync) {
      setAllDurations(currentDuration);
    }
  } 


  return;
}


void setAllDurations(int duration) {
  for (int i = 0; i < gemCount; i++ ) {
    gems[i].setDuration(duration);
  }
  return;
}


void routeHitSync(OSCMessage &msg, int patternOffset) {
  
  
  if (msg.isFloat(0)){
    hitSync = (msg.getFloat(0) > 0.5);
  } 


  return;

}
