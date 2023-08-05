#include <wiringPi.h>
#include <stdio.h>
#include <stdlib.h>
#include <getopt.h>
#include <unistd.h>
#include <ctype.h>
#include <iostream>
#include <string.h>
#include <fcntl.h>           /* For O_* constants */
#include <sys/stat.h>        /* For mode constants */
#include <semaphore.h>
#include <stdio.h>
#include <fcntl.h>
#include "NewRemoteTransmitter.cpp"
#include "NewRemoteReceiver.cpp"
#include "RfWrapper.h"

using namespace std;

void setupWiringPi() {
    if(wiringPiSetup() == -1)
    {
        printf("WiringPi setup failed.");
        exit(1);
    }
}

void sendUnitSignal(int pinNumber, int address, int device, bool state) {

    pinMode(PIN_OUT, OUTPUT);
    digitalWrite(PIN_OUT, LOW);
    NewRemoteTransmitter transmitter(address, PIN_OUT, 260, pinNumber);
    transmitter.sendUnit(device, state);
}

void sendGroupSignal(int pinNumber, int address, bool state) {

    pinMode(PIN_OUT, OUTPUT);
    digitalWrite(PIN_OUT, LOW);
    NewRemoteTransmitter transmitter(address, PIN_OUT, 260, pinNumber);
    transmitter.sendGroup(state);
}

int main(int argc, char *argv[])
{
    int intAdr1;
    int intAdr2;
    int intCmd;
    deviceCommand devCmd;
    int intLev;
    
    // load wiringPi
    if(wiringPiSetup() == -1)
    {
        printf("WiringPi setup failed.");
        exit(1);
    }
                            
    if( argc < 4 ) 
    { // not enough arguments
        printf("Usage: %s address device state level (yet only for new KaKu type A) \n", argv[0]);
        printf("Example: %s 1234567 10 dim 5 \n", argv[0]);
        printf("Example: %s 1234567 10 [on|off] \n", argv[0]);
        exit(1);
    }
    else
    {
        intAdr1 = atoi(argv[1]);
        intAdr2 = atoi(argv[2]);
        intCmd = atoi(argv[3]);
        if (string(argv[3]) == "on") devCmd = on;  //  std::string(argv[1]) == "yes"
        if (string(argv[3]) == "off") devCmd = off;
        if (string(argv[3]) == "dim") devCmd = dim;
        if (argc > 4) intLev = atoi(argv[4]);
        else intLev = 0;
        printf("Send code: %d %d %s %d \n", intAdr1, intAdr2, argv[3], intLev);

        sendUnitSignal(3, intAdr1, intAdr2, devCmd == on);
    }
    return 0;
}