namespace TicTacToe;

public enum GameResult
{
    PlayerWin,
    ComputerWin,
    Draw
}

public class GameRecord
{
    public int Id { get; set; }
    public DateTime PlayedAt { get; set; }
    public GameResult Result { get; set; }
    public int TotalMoves { get; set; }
}
