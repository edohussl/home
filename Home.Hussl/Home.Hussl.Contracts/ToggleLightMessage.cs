namespace Home.Hussl.Contracts
{
	public record ToggleLightMessage
	{
		public long Address { get; set; }

		public short? Device { get; set; }

		public bool TurnOn { get; set; }
	}
}