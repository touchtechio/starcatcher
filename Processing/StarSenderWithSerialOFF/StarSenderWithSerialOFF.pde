import processing.serial.*;
//download at http://ubaa.net/shared/processing/udp/
import hypermedia.net.*;
//download at www.sojamo.de/libraries/controlp5
import controlP5.*;
import oscP5.*;
import netP5.*;
import java.net.*;


/************************************************************************************
 GUI
 ************************************************************************************/

ControlP5 cp5;

DropdownList serialddl;
DropdownList baudddl;
Textlabel arduinoLabel;
Textlabel UDPLabel;
Textlabel incomingPacket;
Button startButton;
Button stopButton;
Textfield ipAddressField;
Textfield incomingPortField;
Textfield outgoingPortField;
Textlabel serialPortName;

boolean ENABLE_SERIAL = false;
boolean SERIAL_OUT_LOG = true;
boolean UDP_LOG = false;

public class OscMessageLogger extends OscMessage {
  public OscMessageLogger(final byte[] theBytes,
                         final InetAddress theInetAddress,
                         final int thePort,
                         final TcpClient theClient) {
    super(theBytes, theInetAddress, thePort, theClient);
  }
  
}

void setupGUI() {
  //the ControlP5 object
  cp5 = new ControlP5(this);

  //start button
  startButton = cp5.addButton("START")
    .setPosition(200, 200)
      .setSize(200, 19)
        ;

  //stop button
  stopButton = cp5.addButton("STOP")
    .setPosition(200, 200)
      .setSize(200, 19)
        ;
  stopButton.hide();

  //Serial Port selector
  serialddl = cp5.addDropdownList("SerialPort")
    .setPosition(50, 100)
      .setSize(200, 200)
        ;
  serialddl.setItemHeight(20);
  serialddl.setBarHeight(15);
  serialddl.getCaptionLabel().set("SELECT ARDUINO SERIAL PORT");
  serialddl.getCaptionLabel().getStyle().marginTop = 3;
  serialddl.getCaptionLabel().getStyle().marginLeft = 3;
  serialddl.getValueLabel().getStyle().marginTop = 3;
  //set the serial options
  String SerialList[] = Serial.list(); 
  for (int i=0;i<SerialList.length;i++) {
    String portName = SerialList[i];
    serialddl.addItem(portName, i);
  }
  
  serialddl.setValue(1);

  //setup the baud list
  baudddl = cp5.addDropdownList("BaudRate")
    .setPosition(50, 50)
      .setSize(200, 200)
        ;
  baudddl.setItemHeight(20);
  baudddl.setBarHeight(15);
  baudddl.getCaptionLabel().set("SELECT THE BAUD RATE");
  baudddl.getCaptionLabel().getStyle().marginTop = 3;
  baudddl.getCaptionLabel().getStyle().marginLeft = 3;
  baudddl.getValueLabel().getStyle().marginTop = 3;
  //the baud options
  for (int i=0;i<serialRateStrings.length;i++) {
    String baudString = serialRateStrings[i];
    baudddl.addItem(baudString, i);
  }
  
  baudddl.setValue(4);

  //udp IP/port
  ipAddressField = cp5.addTextfield("IP address")
    .setPosition(300, 30)
      .setAutoClear(false)
        .setText(ipAddress)
          ;
  incomingPortField = cp5.addTextfield("Incoming Port Number")
    .setPosition(300, 80)
      .setAutoClear(false)
        .setText(str(inPort))
          ;

  outgoingPortField = cp5.addTextfield("Outgoing Port Number")
    .setPosition(300, 130)
      .setAutoClear(false)
        .setText(str(outPort))
          ;

  //text labels
  arduinoLabel = cp5.addTextlabel("arduinoLabel")
    .setText("Serial")
      .setPosition(50, 10)
        .setColorValue(0xffffff00)
          .setFont(createFont("SansSerif", 11))
            ;
  UDPLabel = cp5.addTextlabel("UDPLabel")
    .setText("UDP")
      .setPosition(300, 10)
        .setColorValue(0xffffff00)
          .setFont(createFont("SansSerif", 11))
            ;

  incomingPacket = cp5.addTextlabel("incomingPacketLabel")
    .setText("Incoming")
      .setPosition(210, 100)
        .setColorValue(0xffffff00)
          .setFont(createFont("SansSerif", 10))
            ;
  incomingPacket.hide();
  
  serialPortName = cp5.addTextlabel("serialPortNameLabel")
    .setText("Serial Port Name")
      .setPosition(210, 160)
        .setColorValue(0xffffff00)
          .setFont(createFont("SansSerif", 10))
            ;
  serialPortName.hide();
}

void controlEvent(ControlEvent theEvent) {
  String eventName = theEvent.getName();
  if (theEvent.isGroup()) {
    if (eventName == "SerialPort") {
      //set the serial port 
      serialListNumber = int(theEvent.getValue());
    } 
    else if (eventName == "BaudRate") {
      int index = int(theEvent.getValue());
      baud = Integer.parseInt(serialRateStrings[index]);
    } 
    else {
    }
  } 
  else if (theEvent.isAssignableFrom(Textfield.class)) {
    if (eventName == "IP address") {
      ipAddressField.setFocus(false);
      ipAddress = theEvent.getStringValue();
    } 
    else if (eventName == "Incoming Port Number") {
      incomingPortField.setFocus(false);
      inPort = Integer.parseInt(theEvent.getStringValue());
    } 
    else if (eventName == "Outgoing Port Number") {
      outgoingPortField.setFocus(false);
      outPort = Integer.parseInt(theEvent.getStringValue());
    }
  }
}

boolean applicationRunning = false;

//start everything
public void START(int theValue) {
  setupUDP();
  setupSerial();
  hideControls();
  applicationRunning = true;
}

//hide all the controls and show the stop button
void hideControls() {
  serialddl.hide();
  baudddl.hide();
  startButton.hide();
  outgoingPortField.hide();
  incomingPortField.hide();
  ipAddressField.hide();
  incomingPacket.show();
  serialPortName.show();
  arduinoLabel.hide();
  //show the stop button
  stopButton.show();
}

void showControls() {
  serialddl.show();
  baudddl.show();
  startButton.show();
  outgoingPortField.show();
  incomingPortField.show();
  ipAddressField.show();
  incomingPacket.hide();
  serialPortName.hide();
  arduinoLabel.show();
  //hide the stop button
  stopButton.hide();
}

public void STOP() {
  stopSerial();
  stopUDP();
  showControls();
  applicationRunning = false;
}

/************************************************************************************
 SERIAL
 ************************************************************************************/

//the Serial communcation to the Arduino
Serial serial;

String[] serialRateStrings = {
  "300", "1200", "2400", "4800", "9600", "14400", 
  "19200", "28800", "38400", "57600", "115200"
};
int baud = 38400;
int serialListNumber = 0;

ArrayList<Byte> serialBuffer = new ArrayList<Byte>();

void setupSerial() {
  if (ENABLE_SERIAL) {
     println(Serial.list());
  
    serial = new Serial(this, Serial.list()[serialListNumber], baud);
    println(Serial.list()[serialListNumber]);
    serialPortName.setText(Serial.list()[serialListNumber] + "\n"
    + ipAddress + "\n"+ inPort + "\n");
  }
}

void stopSerial() {
  serial.stop();
}

void serialEvent(Serial serial) { 
  //decode the message
  while (serial.available () > 0) {
    slipDecode(byte(serial.read())); 
  }
}

void SerialSendToUDP() {
  byte [] buffer = new byte[serialBuffer.size()];
  //copy the buffer over
  for (int i = 0; i < serialBuffer.size(); i++) {
    buffer[i] = serialBuffer.get(i);
  }
  //send it off
  UDPSendBuffer(buffer);
  //clear the buffer
  serialBuffer.clear();
  //light up the indicator
  drawIncomingSerial();
}

void printOscData(byte[] data) {
 // println(new String(data));
  try {
  OscMessageLogger logger = new OscMessageLogger(data, InetAddress.getLocalHost(), 9000, null);
//  logger.print();
  print(logger.toString() + ": ");
  Object[] oscArgs = logger.arguments();
  for (Object o: oscArgs) {
    print(o);
    print(", ");
  }
  println();
} catch(UnknownHostException e) {}
}

void serialSend(byte[] data) {
  
  if (SERIAL_OUT_LOG) {
    printOscData(data);
  }
  //encode the message and send it
  for (int i = 0; i < data.length; i++){
     slipEncode(data[i]);
  }
  //write the eot
  serial.write(eot);
}

/************************************************************************************
 SLIP ENCODING
 ************************************************************************************/

byte eot = byte(192);
byte slipesc = byte(219);
byte slipescend = byte(220);
byte slipescesc = byte(221);

byte previousByte;

void slipDecode(byte incoming) {
  byte previous = previousByte;
  previousByte = incoming;
  //if the previous was the escape char
  if (previous == slipesc) {
    //if this one is the esc eot
    if (incoming==slipescend) { 
      serialBuffer.add(eot);
    } 
    else if (incoming==slipescesc) {
      serialBuffer.add(slipesc);
    }
  } 
  else if (incoming==eot) {
    //if it's the eot
    //send off the packet
    SerialSendToUDP();
  } 
  else if (incoming != slipesc) {
    serialBuffer.add(incoming);
  }
}

void slipEncode(byte incoming) {
  if(incoming == eot){ 
    serial.write(slipesc);
    serial.write(slipescend); 
  } else if(incoming==slipesc) {  
    serial.write(slipesc);
    serial.write(slipescesc); 
  } else {
    serial.write(incoming);
  }  
}


/************************************************************************************
 UDP
 ************************************************************************************/

//UDP communication
UDP udp;

int inPort = 9000;
int outPort = 9001;
String ipAddress = "127.0.0.1";

void setupUDP() {
  udp = new UDP( this, inPort );
  udp.log( UDP_LOG );     // <-- printout the connection activity
  udp.listen( true );
}

void stopUDP() {
  udp.close();
}

void UDPSendBuffer(byte[] data) {
  udp.send( data, ipAddress, outPort );
}

//called when UDP recieves some data
void receive( byte[] data) {
  drawIncomingUDP();
  //send it over to serial
 
  if(ENABLE_SERIAL) {
    serialSend(data);
  }
}

/************************************************************************************
 SETUP/DRAW
 ************************************************************************************/

void setup() {
  // configure the screen size and frame rate
  size(550, 300, P3D);
  frameRate(30);
  setupGUI();
  
  //START(-1);
}

void draw() {
  background(128);
  if (applicationRunning) {
    drawIncomingPackets();
  }
}


/************************************************************************************
 VISUALIZING INCOMING PACKETS
 ************************************************************************************/

int lastSerialPacket = 0;
int lastUDPPacket = 0;

void drawIncomingPackets() {
  //the serial packet
  fill(0);
//  rect(75, 50, 100, 100);
  //the udp packet
  rect(325, 50, 100, 100);
  int now = millis();
  int lightDuration = 75;
/*  if (now - lastSerialPacket < lightDuration) {
    fill(255);
    rect(85, 60, 80, 80);
  }
*/
  if (now - lastUDPPacket < lightDuration) {
    fill(255);
    rect(335, 60, 80, 80);
  }
}

void drawIncomingSerial() {
  lastSerialPacket = millis();
}

void drawIncomingUDP() {
  lastUDPPacket = millis();
}
