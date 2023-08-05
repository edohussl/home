namespace Home.Hussl.Contracts
{
	public record ApiLight : ApiLightBody
	{
		public Guid Id { get; set; }
	}
}