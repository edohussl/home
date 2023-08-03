using Home.Hussl.Pi;

namespace Home.Hussl.Console
{
	internal class Program
	{
		static void Main(string[] args)
		{
			System.Console.WriteLine("Hello, World!");

			var result = RfWrapper.setupWiringPi();

			if (result == -1)
			{
				System.Console.WriteLine("WiringPi setup failed.");
				return;
			}

			RfWrapper.sendUnitSignal(3, int.Parse(args[0]), int.Parse(args[1]), bool.Parse(args[2]));
			System.Console.WriteLine("Command sent.");
		}
	}
}