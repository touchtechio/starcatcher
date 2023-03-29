import themidibus.*; //Import the library

import oscP5.*;
import netP5.*;
  
OscP5 oscP5;
NetAddress myRemoteLocation;


MidiBus myBus; // The MidiBus

void setup() {
  size(400, 400);
  background(0);

  MidiBus.list(); // List all available Midi devices on STDOUT. This will show each device's index and name.
  myBus = new MidiBus(this, "Akai MPD18", -1); // Create a new MidiBus with no input device and the default Java Sound Synthesizer as the output device.

  /* start oscP5, listening for incoming messages at port 12000 */
  oscP5 = new OscP5(this,12000);
  
  /* myRemoteLocation is a NetAddress. a NetAddress takes 2 parameters,
   * an ip address and a port number. myRemoteLocation is used as parameter in
   * oscP5.send() when sending osc packets to another computer, device, 
   * application. usage see below. for testing purposes the listening port
   * and the port of the remote location address are the same, hence you will
   * send messages back to this sketch.
   */
  myRemoteLocation = new NetAddress("127.0.0.1", 9000);

}

void draw() {
  delay(200);
}


void sendOscNoteOn(int channel, int pitch, int velocity) {
  
  String address = "/behavior/" + (pitch-36);
  /* in the following different ways of creating osc messages are shown by example */
  OscMessage myMessage = new OscMessage(address);
  
  myMessage.add(pitch-36); /* add an int to the osc message */
  myMessage.add(velocity); /* add an int to the osc message */
  println("SEND OSC ON");

  /* send the message */
  oscP5.send(myMessage, myRemoteLocation); 
    print("### send an osc message.");
    
    

}

void sendOscNoteOff(int channel, int pitch, int velocity) {
  
  String address = "/allfall";
  /* in the following different ways of creating osc messages are shown by example */
  OscMessage myMessage = new OscMessage(address);
  
  myMessage.add(pitch-36); /* add an int to the osc message */
  myMessage.add(velocity); /* add an int to the osc message */
  println("SEND OSC OFF");

  /* send the message */
  oscP5.send(myMessage, myRemoteLocation); 
    print("### send an osc message.");
    
    

}


void sendOscCC(int channel,  int number, int value) {
  
  String address = "/color/hue";
  /* in the following different ways of creating osc messages are shown by example */
  OscMessage myMessage = new OscMessage(address);
  
  myMessage.add(((float)value)/127.0); /* add an int to the osc message */
  println("SEND OSC CC ");

  /* send the message */
  oscP5.send(myMessage, myRemoteLocation); 
    print("### send an osc message.");
    
    

}



void noteOn(int channel, int pitch, int velocity) {
  // Receive a noteOn
  println();
  println("Note On:");
  println("--------");
  println("Channel:"+channel);
  println("Pitch:"+pitch);
  println("Velocity:"+velocity);
    sendOscNoteOn(channel, pitch, velocity);

}

void noteOff(int channel, int pitch, int velocity) {
  // Receive a noteOff
  println();
  println("Note Off:");
  println("--------");
  println("Channel:"+channel);
  println("Pitch:"+pitch);
  println("Velocity:"+velocity);
      sendOscNoteOff(channel, pitch, velocity);

}

void controllerChange(int channel, int number, int value) {
  // Receive a controllerChange
  println();
  println("Controller Change:");
  println("--------");
  println("Channel:"+channel);
  println("Number:"+number);
  println("Value:"+value);
  sendOscCC(channel, number, value);
}

void delay(int time) {
  int current = millis();
  while (millis () < current+time) Thread.yield();
}
