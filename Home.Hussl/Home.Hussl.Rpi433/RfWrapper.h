#ifdef __cplusplus
extern "C" {
#endif
#ifdef _WIN32
#  ifdef MODULE_API_EXPORTS
#    define MODULE_API __declspec(dllexport)
#  else
#    define MODULE_API __declspec(dllimport)
#  endif
#else
#  define MODULE_API
#endif

#include <cstdint>
#include <wiringPi.h>
#include "NewRemoteReceiver.h"

MODULE_API void setupWiringPi();
MODULE_API void sendUnitSignal(int pinNumber, long address, short device, bool state);
MODULE_API void sendGroupSignal(int pinNumber, long address, bool state);

MODULE_API void initReceiver(int pinNumber, NewRemoteReceiverCallBack callback);
MODULE_API void disableReceiver();
MODULE_API void enableReceiver();
MODULE_API void deinitReceiver();
#ifdef __cplusplus
}
#endif