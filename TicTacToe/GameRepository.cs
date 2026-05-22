using Microsoft.EntityFrameworkCore;

namespace TicTacToe;

public class GameRepository : IGameRepository
{
    private readonly Func<GameDbContext> _contextFactory;

    public GameRepository() : this(() => new GameDbContext())
    {
    }

    public GameRepository(Func<GameDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
        EnsureDatabase();
    }

    private void EnsureDatabase()
    {
        using var context = _contextFactory();
        context.Database.EnsureCreated();
    }

    public void SaveGameResult(GameResult result, int totalMoves)
    {
        using var context = _contextFactory();
        context.GameRecords.Add(new GameRecord
        {
            PlayedAt = DateTime.UtcNow,
            Result = result,
            TotalMoves = totalMoves
        });
        context.SaveChanges();
    }

    public GameStatistics GetStatistics()
    {
        using var context = _contextFactory();
        var records = context.GameRecords.ToList();
        return new GameStatistics
        {
            PlayerWins = records.Count(r => r.Result == GameResult.PlayerWin),
            ComputerWins = records.Count(r => r.Result == GameResult.ComputerWin),
            Draws = records.Count(r => r.Result == GameResult.Draw)
        };
    }
}
