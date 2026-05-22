using TicTacToe;

namespace TicTacToe.E2ETests;

internal class MockGameRepository : IGameRepository
{
    public List<(GameResult Result, int TotalMoves)> SavedResults { get; } = new();
    private readonly GameStatistics _stats = new();

    public void SaveGameResult(GameResult result, int totalMoves)
    {
        SavedResults.Add((result, totalMoves));
        switch (result)
        {
            case GameResult.PlayerWin:
                _stats.PlayerWins++;
                break;
            case GameResult.ComputerWin:
                _stats.ComputerWins++;
                break;
            case GameResult.Draw:
                _stats.Draws++;
                break;
        }
    }

    public GameStatistics GetStatistics()
    {
        return _stats;
    }
}
