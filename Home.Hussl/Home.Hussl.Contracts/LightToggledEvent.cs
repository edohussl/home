namespace Home.Hussl.Contracts
{
	public record LightToggledEvent
	{
		public int Address { get; set; }

		public int? Device { get; set; }

		public bool TurnedOn { get; set; }
	}
}