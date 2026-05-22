namespace TicTacToe;

public class GameStatistics
{
    public int PlayerWins { get; set; }
    public int ComputerWins { get; set; }
    public int Draws { get; set; }
    public int TotalGames => PlayerWins + ComputerWins + Draws;
}
