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

int setupWiringPi() {
	return wiringPiSetup();
}

void sendUnitSignal(int pinNumber, int address, int device, bool state) {

    NewRemoteTransmitter transmitter(address, PIN_OUT, 260, pinNumber);
    transmitter.sendUnit(device, state);
}