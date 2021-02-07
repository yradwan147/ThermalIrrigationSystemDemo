#include <Servo.h>
#include <Wire.h>
#include <Adafruit_MLX90614.h>

Adafruit_MLX90614 mlx = Adafruit_MLX90614();


Servo myservo;
Servo myservo2;
void setup() {
  Serial.print("test");
  mlx.begin();
  Serial.print("test2");
  // put your setup code here, to run once:
    myservo.attach(9);
    myservo2.attach(10);
    //Serial.begin(9600);
}

void loop() {
  for(int pos = 30; pos < 130; pos+= 5){  
  delay(200);
  //Serial.print("1");
  myservo2.write(pos);
  for (int ff = 0; ff <= 180; ff +=1){
       myservo.write(ff);
    //   Serial.print("2");
       double reading = mlx.readObjectTempC();
      // Serial.print("3");
       //Serial.print("*"+(String)(pos)+";"+(String)(ff)+";"+reading+"*");
       delay(25);
     }

  
  //myservo.write(0);
    delay(500); 
  }                     
   myservo.write(0);
   myservo2.write(0);
  delay(1000);
}
