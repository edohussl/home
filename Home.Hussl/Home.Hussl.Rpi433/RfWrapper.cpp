#include <wiringPi.h>
#include <stdio.h>
#include <stdlib.h>
#include <getopt.h>
#include <unistd.h>
#include <ctype.h>
#include <iostream>
#include <string.h>
#include <fcntl.h>    /* For O_* constants */
#include <sys/stat.h> /* For mode constants */
#include <semaphore.h>
#include <stdio.h>
#include <fcntl.h>
#include "NewRemoteTransmitter.cpp"
#include "NewRemoteReceiver.cpp"
#include "RfWrapper.h"

using namespace std;

NewRemoteReceiverCallBack _callback;
bool _shouldRun = false;

void setupWiringPi()
{
    if (wiringPiSetup() == -1)
    {
        printf("WiringPi setup failed.");
        exit(1);
    }
}

void sendUnitSignal(int pinNumber, int address, int device, bool state)
{
    pinMode(PIN_OUT, OUTPUT);
    digitalWrite(PIN_OUT, LOW);
    NewRemoteTransmitter transmitter(address, PIN_OUT, 260, pinNumber);
    transmitter.sendUnit(device, state);
}

void sendGroupSignal(int pinNumber, int address, bool state)
{
    pinMode(PIN_OUT, OUTPUT);
    digitalWrite(PIN_OUT, LOW);
    NewRemoteTransmitter transmitter(address, PIN_OUT, 260, pinNumber);
    transmitter.sendGroup(state);
}

void initReceiver(int pinNumber, NewRemoteReceiverCallBack callback)
{
    _callback = callback;
    _shouldRun = true;
    NewRemoteReceiver receiver(pinNumber, 2, _callback);

    while (_shouldRun)
    {
        sleep(1);
    }

    printf("Receiver stopped.");
}

void disableReceiver()
{
    NewRemoteReceiver::disable();
}

void enableReceiver()
{
    NewRemoteReceiver::enable();
}

void deinitReceiver()
{
    NewRemoteReceiver::deinit();
    _shouldRun = false;
}