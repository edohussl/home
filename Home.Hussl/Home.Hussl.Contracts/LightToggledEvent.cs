namespace Home.Hussl.Contracts
{
	public record LightToggledEvent
	{
		public long Address { get; set; }

		public short? Device { get; set; }

		public bool TurnedOn { get; set; }
	}
}