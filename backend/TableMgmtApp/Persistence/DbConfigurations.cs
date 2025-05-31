using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TableMgmtApp.Persistence;

public class TableMgmtAppDbContext : DbContext {
    public DbSet<Player> Players { get; set; }
    public DbSet<PoolTable> PoolTables { get; set; }
    public DbSet<PlaySession> PlaySessions { get; set; }
    public DbSet<ScheduleDTO> Schedules { get; set; }
    public DbSet<Discount> Discounts { get; set; }

    #nullable disable
    public TableMgmtAppDbContext(DbContextOptions<TableMgmtAppDbContext> options) : base(options) {}

    // DB configurations get initialized here
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.ApplyConfiguration(new PlayerConfiguration());
        modelBuilder.ApplyConfiguration(new TableConfiguration());
        modelBuilder.ApplyConfiguration(new PlaySessionConfiguration());
        modelBuilder.ApplyConfiguration(new ScheduleConfiguration());
        modelBuilder.ApplyConfiguration(new DiscountConfiguration());
    }
}


public class TableConfiguration : IEntityTypeConfiguration<PoolTable> {
    public void Configure(EntityTypeBuilder<PoolTable> builder) {
        builder.ToTable("PoolTables");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).IsRequired();
        builder.Property(t => t.Number).IsRequired();
        builder.Property(t => t.ScheduleId).IsRequired();
    }
}

public class PlayerConfiguration : IEntityTypeConfiguration<Player> {
    public void Configure(EntityTypeBuilder<Player> builder) {
        builder.ToTable("Players");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.CreatedAt).IsRequired();
        builder.Property(u => u.Name).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Surname).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Email).HasMaxLength(200);
        builder.Property(u => u.Discount);
    }
}

public class PlaySessionConfiguration : IEntityTypeConfiguration<PlaySession> {
    public void Configure(EntityTypeBuilder<PlaySession> builder) {
        builder.ToTable("PlaySessions");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.StartTime).IsRequired();
        builder.Property(p => p.PlayTime).IsRequired();
        builder.Property(p => p.Price).IsRequired();
        builder.Property(p => p.TableNumber).IsRequired();
        builder.Property(p => p.TableName).IsRequired();
        builder.Property(p => p.PlayerId);

        builder.HasOne(p => p.Player)
            .WithMany()
            .HasForeignKey(p => p.PlayerId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}   

public class ScheduleConfiguration : IEntityTypeConfiguration<ScheduleDTO> {
    public void Configure(EntityTypeBuilder<ScheduleDTO> builder) {
        builder.ToTable("Schedules");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
        builder.Property(s => s.WeeklyRates);
        builder.Property(s => s.DefaultRate).IsRequired();
    }
}
public class DiscountConfiguration : IEntityTypeConfiguration<Discount> {
    public void Configure(EntityTypeBuilder<Discount> builder) {
        builder.ToTable("Discounts");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Type).IsRequired();
        builder.Property(d => d.Name).IsRequired();
        builder.Property(d => d.Rate).IsRequired();
    }
}


