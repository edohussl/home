using Home.Hussl.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Home.Hussl.Data;

public class HomeDbContext : DbContext
{
	public DbSet<DbLight> Lights { get; set; } = null!;

	public HomeDbContext(DbContextOptions<HomeDbContext> options)
		: base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.HasDefaultContainer("Lights");

		modelBuilder.Entity<DbLight>()
			.ToContainer("Lights");

		modelBuilder.Entity<DbLight>().Property(o => o.Id).ToJsonProperty("id");
		modelBuilder.Entity<DbLight>().Property(o => o.Room).ToJsonProperty("room");
		modelBuilder.Entity<DbLight>().Property(o => o.Name).ToJsonProperty("name");
		modelBuilder.Entity<DbLight>().Property(o => o.Address).ToJsonProperty("address");
		modelBuilder.Entity<DbLight>().Property(o => o.Device).ToJsonProperty("device");
		modelBuilder.Entity<DbLight>().Property(o => o.IsOn).ToJsonProperty("isOn");

		modelBuilder.Entity<DbLight>()
			.HasKey(o => o.Id);

		modelBuilder.Entity<DbLight>().HasPartitionKey(o => o.Room);
		modelBuilder.Entity<DbLight>().HasNoDiscriminator();
		modelBuilder.Entity<DbLight>().UseETagConcurrency();
	}
}