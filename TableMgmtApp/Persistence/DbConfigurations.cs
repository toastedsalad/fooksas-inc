using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TableMgmtApp.Persistence;

public class TableMgmtAppDbContext : DbContext {
    public DbSet<Player> Players { get; set; }
    public DbSet<Table> Tables { get; set; }

    #nullable disable
    public TableMgmtAppDbContext(DbContextOptions<TableMgmtAppDbContext> options) : base(options) {}

    // Tables get initialized here
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.ApplyConfiguration(new PlayerConfiguration());
        modelBuilder.ApplyConfiguration(new TableConfiguration());
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

public class TableConfiguration : IEntityTypeConfiguration<Table> {
    public void Configure(EntityTypeBuilder<Table> builder) {
        builder.ToTable("Tables");
        builder.HasKey(u => u.Number);
        builder.Property(u => u.Id).IsRequired();
    }
}
