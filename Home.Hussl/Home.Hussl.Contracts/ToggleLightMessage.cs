namespace Home.Hussl.Contracts
{
	public record ToggleLightMessage
	{
		public int Address { get; set; }

		public int? Device { get; set; }

		public bool TurnOn { get; set; }
	}
}