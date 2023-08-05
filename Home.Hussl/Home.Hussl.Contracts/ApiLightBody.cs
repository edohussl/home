namespace Home.Hussl.Contracts
{
	public record ApiLightBody
	{
		public string Room { get; set; } = null!;

		public string Name { get; set; } = null!;

		public int Address { get; set; }

		public int? Device { get; set; }

		public bool IsOn { get; set; }
	}
}