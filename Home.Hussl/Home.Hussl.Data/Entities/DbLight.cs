namespace Home.Hussl.Data.Entities
{
    public class DbLight
    {
        public Guid Id { get; set; }

        public string Room { get; set; } = null!;

        public string Name { get; set; } = null!;

        public long Address { get; set; }

        public short? Device { get; set; }

        public bool IsOn { get; set; }
    }
}