#include "FastLED.h"

// How many leds in your strip?
#define NUM_LEDS 32

// For led chips like Neopixels, which have a data line, ground, and power, you just
// need to define DATA_PIN.  For led chipsets that are SPI based (four wires - data, clock,
// ground, and power), like the LPD8806, define both DATA_PIN and CLOCK_PIN
#define DATA_PIN 11

#define DEFAULT_DURATION 3000
#define CAUGHT_DURATION 1500

// Define the array of leds
CRGB leds[NUM_LEDS];


void setup() {
  Serial.begin(9600);
  Serial.println("resetting");
  LEDS.addLeds<WS2812, DATA_PIN, RGB>(leds, NUM_LEDS);
  LEDS.setBrightness(84);


  pinMode(7, OUTPUT);
  digitalWrite(7, HIGH);  // enable access to LEDs

  fireLED(DEFAULT_DURATION / 10);

}

void fadeall() {
  for (int i = 0; i < NUM_LEDS; i++) {
    leds[i].nscale8(200);
  }
}
void alloff() {
  for (int i = 0; i < NUM_LEDS; i++) {
    leds[i] = CRGB(0, 0, 0);
  }
}

void loop() {

  int incomingByte;

  if (Serial.available() > 0) {
    incomingByte = Serial.read();
  }

  if (incomingByte == 'a') {
    fireLED(DEFAULT_DURATION);
  }

  if (incomingByte == 'x') {
    caughtLED(CAUGHT_DURATION);

  }
}

void fireLED(int duration) {

  static uint8_t hue = 0;

  int delayIncrement = duration / NUM_LEDS;

  // First slide the led in one direction
  for (int i = 0; i < NUM_LEDS; i++) {
    // Set the i'th led to red
    leds[i] = CHSV(75, 128, 255);
    // Show the leds
    FastLED.show();
    // now that we've shown the leds, reset the i'th led to black
    // leds[i] = CRGB::Black;
    fadeall();
    // Wait a little bit before we loop around and do it again
    delay(delayIncrement);
  }

  for (int i = 0; i < 200; i++) {

    fadeall();
    leds[30] = CHSV(75, 128, 255);
    leds[29] = CHSV(75, 128, 255);
    FastLED.show();
    delay(10);
  }

  alloff();
  FastLED.show();
}

void caughtLED(int duration) {
  static uint8_t hue = 0;
  int delayIncrement = duration / NUM_LEDS;

  for (int i = NUM_LEDS - 1; i >= 0; i--) {
    // Set the i'th led to red
    leds[i] = CHSV(200, 128, 255);
    // Show the leds
    FastLED.show();
    // now that we've shown the leds, reset the i'th led to black
    // leds[i] = CRGB::Black;
    fadeall();
    // Wait a little bit before we loop around and do it again
    delay(delayIncrement);
  }
  alloff();
  FastLED.show();
}







