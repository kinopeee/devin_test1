namespace TicTacToe;

public interface IGameRepository
{
    void SaveGameResult(GameResult result, int totalMoves);
    GameStatistics GetStatistics();
}
