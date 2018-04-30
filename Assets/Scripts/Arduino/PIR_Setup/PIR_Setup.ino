/*
 * Unity: Right side = 1, Left side = 2, Front side = 3, No motion = 0;
 */

int ledPin = 13;                // choose the pin for the LED
int inputPinL = 2;
int inputPinR = 3; 
int inputPinF = 4; // choose the input pin (for PIR sensor)
int pirState = LOW;             // we start, assuming no motion detected
int valL = 0; 
int valR = 0;
int valF = 0;// variable for reading the pin status
int pirValue = 0;
 
void setup() {
  Serial.begin(9600);
  pinMode(ledPin, OUTPUT);      // declare LED as output
  pinMode(inputPinL, INPUT);     // declare sensor as input
  pinMode(inputPinR, INPUT);     // declare sensor as input
  pinMode(inputPinF, INPUT);     // declare sensor as input
  Serial.begin(9600);
}
 
void loop(){
  valL = digitalRead(inputPinL);  // read input value
  valR = digitalRead(inputPinR);  // read input value
  valF = digitalRead(inputPinF);  // read input value
  if (valL == HIGH) {            // check if the input is HIGH
    digitalWrite(ledPin, HIGH);  // turn LED ON
    if (pirState == LOW) {
      Serial.println(1);
      pirState = HIGH;
    }
  } 
    else if (valR == HIGH) {            // check if the input is HIGH
    digitalWrite(ledPin, HIGH);  // turn LED ON
    if (pirState == LOW) {
      Serial.println(2);
      pirState = HIGH;
    }
  }
    else if (valF == HIGH) {            // check if the input is HIGH
    digitalWrite(ledPin, HIGH);  // turn LED ON
    if (pirState == LOW) {
      Serial.println(3);
      pirState = HIGH;
    }
  }
  else {
    digitalWrite(ledPin, LOW); // turn LED OFF
    if (pirState == HIGH){
      Serial.println(0); 
      pirState = LOW;
    }
  }
}
