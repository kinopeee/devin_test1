using Microsoft.EntityFrameworkCore;

namespace TicTacToe;

public class GameDbContext : DbContext
{
    public DbSet<GameRecord> GameRecords { get; set; } = null!;

    private readonly string _dbPath = string.Empty;

    public GameDbContext()
    {
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _dbPath = Path.Combine(folder, "TicTacToe", "games.db");
    }

    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var dir = Path.GetDirectoryName(_dbPath)!;
            Directory.CreateDirectory(dir);
            optionsBuilder.UseSqlite($"Data Source={_dbPath}");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GameRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PlayedAt).IsRequired();
            entity.Property(e => e.Result).IsRequired();
            entity.Property(e => e.TotalMoves).IsRequired();
        });
    }
}
