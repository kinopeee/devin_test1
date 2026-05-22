using Microsoft.EntityFrameworkCore;
using Xunit;
using TicTacToe;

namespace TicTacToe.Tests;

public class GameRepositoryTests : IDisposable
{
    private readonly string _dbPath;
    private readonly GameRepository _repository;

    public GameRepositoryTests()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"tictactoe_test_{Guid.NewGuid()}.db");
        _repository = new GameRepository(() => CreateContext());
    }

    private GameDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseSqlite($"Data Source={_dbPath}")
            .Options;
        return new GameDbContext(options);
    }

    public void Dispose()
    {
        try { File.Delete(_dbPath); } catch { }
    }

    [Fact]
    public void SaveGameResult_PlayerWin_SavesCorrectly()
    {
        _repository.SaveGameResult(GameResult.PlayerWin, 5);

        var stats = _repository.GetStatistics();
        Assert.Equal(1, stats.PlayerWins);
        Assert.Equal(0, stats.ComputerWins);
        Assert.Equal(0, stats.Draws);
    }

    [Fact]
    public void SaveGameResult_ComputerWin_SavesCorrectly()
    {
        _repository.SaveGameResult(GameResult.ComputerWin, 6);

        var stats = _repository.GetStatistics();
        Assert.Equal(0, stats.PlayerWins);
        Assert.Equal(1, stats.ComputerWins);
        Assert.Equal(0, stats.Draws);
    }

    [Fact]
    public void SaveGameResult_Draw_SavesCorrectly()
    {
        _repository.SaveGameResult(GameResult.Draw, 9);

        var stats = _repository.GetStatistics();
        Assert.Equal(0, stats.PlayerWins);
        Assert.Equal(0, stats.ComputerWins);
        Assert.Equal(1, stats.Draws);
    }

    [Fact]
    public void GetStatistics_NoGames_ReturnsZeros()
    {
        var stats = _repository.GetStatistics();

        Assert.Equal(0, stats.PlayerWins);
        Assert.Equal(0, stats.ComputerWins);
        Assert.Equal(0, stats.Draws);
        Assert.Equal(0, stats.TotalGames);
    }

    [Fact]
    public void GetStatistics_MultipleGames_ReturnsCorrectCounts()
    {
        _repository.SaveGameResult(GameResult.PlayerWin, 5);
        _repository.SaveGameResult(GameResult.PlayerWin, 7);
        _repository.SaveGameResult(GameResult.ComputerWin, 6);
        _repository.SaveGameResult(GameResult.Draw, 9);
        _repository.SaveGameResult(GameResult.Draw, 9);

        var stats = _repository.GetStatistics();

        Assert.Equal(2, stats.PlayerWins);
        Assert.Equal(1, stats.ComputerWins);
        Assert.Equal(2, stats.Draws);
        Assert.Equal(5, stats.TotalGames);
    }

    [Fact]
    public void SaveGameResult_StoresMovesCount()
    {
        _repository.SaveGameResult(GameResult.PlayerWin, 7);

        using var context = CreateContext();
        var record = context.GameRecords.First();
        Assert.Equal(7, record.TotalMoves);
    }

    [Fact]
    public void SaveGameResult_StoresPlayedAtTime()
    {
        var before = DateTime.UtcNow;
        _repository.SaveGameResult(GameResult.Draw, 9);
        var after = DateTime.UtcNow;

        using var context = CreateContext();
        var record = context.GameRecords.First();
        Assert.True(record.PlayedAt >= before.AddSeconds(-1));
        Assert.True(record.PlayedAt <= after.AddSeconds(1));
    }

    [Fact]
    public void GameStatistics_TotalGames_CalculatesCorrectly()
    {
        var stats = new GameStatistics
        {
            PlayerWins = 3,
            ComputerWins = 2,
            Draws = 1
        };

        Assert.Equal(6, stats.TotalGames);
    }
}
