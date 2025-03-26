using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TableMgmtApp.Persistence;

public class TableMgmtAppDbContext : DbContext {
    public DbSet<Player> Players { get; set; }

    #nullable disable
    public TableMgmtAppDbContext(DbContextOptions<TableMgmtAppDbContext> options) : base(options) {}

    // Players get initialized here
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.ApplyConfiguration(new PlayerConfiguration());
    }
}

public class PlayerConfiguration : IEntityTypeConfiguration<Player> {
    public void Configure(EntityTypeBuilder<Player> builder) {
        builder.ToTable("Players");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Name).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Surname).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Email).HasMaxLength(200);
        builder.Property(u => u.Discount);
    }
}

