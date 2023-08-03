#!/bin/bash 
cp * ~/rpi433/rpi433/
gcc -c -o ~/rpi433/rpi433/RfWrapper.o ~/rpi433/rpi433/RfWrapper.cpp
gcc -shared -o ~/rpi433/rpi433/RfWrapper.so ~/rpi433/rpi433/RfWrapper.o