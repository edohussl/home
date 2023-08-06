#!/bin/bash 
cp Home.Hussl.Rpi433/* ~/rpi433/rpi433/
gcc -c -o ~/rpi433/rpi433/RfWrapper.o ~/rpi433/rpi433/RfWrapper.cpp -lwiringPi
gcc -shared -o ~/rpi433/rpi433/RfWrapper.so ~/rpi433/rpi433/RfWrapper.o -lwiringPi

dotnet build Home.Hussl.Console/Home.Hussl.Console.csproj -c Release