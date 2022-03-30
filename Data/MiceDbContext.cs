using Microsoft.EntityFrameworkCore;
using Aweton.Labs.CurrencyRates.Cli.Models;
using Microsoft.Extensions.Options;

namespace Aweton.Labs.CurrencyRates.Cli.Data
{
  public partial class MiceDbContext : DbContext
  {
    public MiceDbContext()
    {      
    }

    public MiceDbContext(DbContextOptions<MiceDbContext> options)
        : base(options)
    {
      
    }

    public virtual DbSet<CurrencyRate> CurrencyRates { get; set; } = null!;
    public virtual DbSet<CurrencyType> CurrencyTypes { get; set; } = null!;
    public virtual DbSet<awjCbrDailyLog> awjCbrDailyLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.UseCollation("Cyrillic_General_CI_AS");

      modelBuilder.Entity<CurrencyRate>(entity =>
      {
        entity.HasKey(e => e.CurrencyRatesID);

        entity.HasIndex(e => new { e.CurrencyTypesID, e.RateTypesID, e.ADate }, "nci_wi_CurrencyRates_Reordered")
                  .IsUnique();

        entity.Property(e => e.ADate)
                  .HasColumnType("datetime");

        entity.Property(e => e.RateTypesID);

        entity.Property(e => e.UserName)
                  .HasMaxLength(128);
      });

      modelBuilder.Entity<CurrencyType>(entity =>
      {
        entity.HasKey(e => e.CurrencyTypesID);

        entity.HasIndex(e => e.CurrCodeChr, "UC_CurrencyTypes_CurrCodeChr")
                  .IsUnique();

        entity.Property(e => e.CurrencyTypesID).ValueGeneratedNever();

        entity.Property(e => e.Case1)
                  .HasMaxLength(32)
                  .IsUnicode(false);

        entity.Property(e => e.Case2)
                  .HasMaxLength(32)
                  .IsUnicode(false);

        entity.Property(e => e.Case5)
                  .HasMaxLength(32)
                  .IsUnicode(false);

        entity.Property(e => e.Case_1)
                  .HasMaxLength(32)
                  .IsUnicode(false);

        entity.Property(e => e.Case_2)
                  .HasMaxLength(32)
                  .IsUnicode(false);

        entity.Property(e => e.Case_5)
                  .HasMaxLength(32)
                  .IsUnicode(false);

        entity.Property(e => e.CurrCodeChr)
                  .HasMaxLength(3)
                  .IsUnicode(false)
                  .IsFixedLength();

        entity.Property(e => e.CurrCodeInt)
                  .HasMaxLength(3)
                  .IsUnicode(false)
                  .IsFixedLength();

        entity.Property(e => e.Description)
                  .HasMaxLength(60)
                  .IsUnicode(false);

        entity.Property(e => e.Eng_description)
                  .HasMaxLength(64)
                  .IsUnicode(false);
      });

      modelBuilder.Entity<awjCbrDailyLog>(entity =>
      {
        entity.ToTable("awjCbrDailyLog");

        entity.Property(e => e.HashBytes).HasMaxLength(80);

        entity.Property(e => e.PostedAt)
                  .HasColumnType("datetime");

        entity.Property(e => e.aDate)
                  .HasColumnType("datetime");
      });

      OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
  }
}
