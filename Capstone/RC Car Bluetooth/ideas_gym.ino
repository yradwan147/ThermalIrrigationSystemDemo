void back(){

digitalWrite(4,HIGH);
  digitalWrite(5,LOW);
  digitalWrite(6,LOW);
  digitalWrite(7,HIGH);

}

void forward(){

digitalWrite(4,LOW);
  digitalWrite(5,HIGH);
  digitalWrite(6,HIGH);
  digitalWrite(7,LOW);

}


void Left(){

digitalWrite(4,LOW);
  digitalWrite(5,LOW);
  digitalWrite(6,HIGH);
  digitalWrite(7,LOW);

}

void Right(){

digitalWrite(4,LOW);
  digitalWrite(5,HIGH);
  digitalWrite(6,LOW);
  digitalWrite(7,LOW);

}

void Stop(){

digitalWrite(4,LOW);
  digitalWrite(5,LOW);
  digitalWrite(6,LOW);
  digitalWrite(7,LOW);

}

void setup() {
  // put your setup code here, to run once:
  pinMode(4,OUTPUT);
  pinMode(5,OUTPUT);
  pinMode(6,OUTPUT);
  pinMode(7,OUTPUT);
Serial.begin(9600);
}

void loop() {
 char data;
  // put your main code here, to run repeatedly:

  if(Serial.available()>0){

data=Serial.read();
    
  }

  if(data=='F'){

 forward();
    
  }

    if(data=='B'){

 back();
    
  }

   if(data=='L'){

 Left();
    
  }

if(data=='R'){

Right();
    
  }

  if(data=='S'){

Stop();
    
  }
 
  
}
