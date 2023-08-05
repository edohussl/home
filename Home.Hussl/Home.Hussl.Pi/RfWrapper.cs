using System.Runtime.InteropServices;

namespace Home.Hussl.Pi;

public class RfWrapper
{
	//public struct NewRemoteCode
	//{
	//	public enum SwitchType
	//	{
	//		off = 0,
	//		on = 1,
	//		dim = 2,
	//		on_with_dim = 3
	//	};

	//	public uint period;            // Detected duration in microseconds of 1T in the received signal
	//	public uint address;          // Address of received code. [0..2^26-1]
	//	public bool groupBit;                // Group bit set or not
	//	public SwitchType switchType;          // off, on, dim, on_with_dim.
	//	public ushort unit;             // Unit code of received code [0..15]
	//	public ushort dimLevel;         // Dim level [0..15] iff switchType == 2
	//};

	//public delegate void NewRemoteReceiverCallBack(NewRemoteCode code);

	[DllImport("/home/pi/rpi433/rpi433/RfWrapper.so", EntryPoint = "setupWiringPi")]
	public static extern int setupWiringPi();

	[DllImport("/home/pi/rpi433/rpi433/RfWrapper.so", EntryPoint = "sendUnitSignal")]
	public static extern void sendUnitSignal(int pinNumber, int address, int device, bool state);

	[DllImport("/home/pi/rpi433/rpi433/RfWrapper.so", EntryPoint = "sendGroupSignal")]
	public static extern void sendGroupSignal(int pinNumber, int address, bool state);

	//[DllImport("../lib/domotics/raspKaku/libkakuWrapper.so", EntryPoint = "initReceiver")]
	//public static extern void initReceiver(int pinNumber, NewRemoteReceiverCallBack callback);

	//[DllImport("../lib/domotics/raspKaku/libkakuWrapper.so", EntryPoint = "disableReceiver")]
	//public static extern void disableReceiver();

	//[DllImport("../lib/domotics/raspKaku/libkakuWrapper.so", EntryPoint = "enableReceiver")]
	//public static extern void enableReceiver();

	//[DllImport("../lib/domotics/raspKaku/libkakuWrapper.so", EntryPoint = "deinitReceiver")]
	//public static extern void deinitReceiver();
}