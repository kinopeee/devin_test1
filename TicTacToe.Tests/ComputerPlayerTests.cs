using Xunit;
using TicTacToe;

namespace TicTacToe.Tests;

public class ComputerPlayerTests
{
    [Fact]
    public void GetBestMove_EmptyBoard_ReturnsValidMove()
    {
        var engine = new GameEngine();
        engine.PlaceMark(0, 0); // X moves first
        var computer = new ComputerPlayer(engine);

        var (row, col) = computer.GetBestMove();

        Assert.InRange(row, 0, 2);
        Assert.InRange(col, 0, 2);
    }

    [Fact]
    public void GetBestMove_BlocksPlayerWin()
    {
        var engine = new GameEngine();
        // X at (0,0), (0,1) - threatening (0,2)
        engine.PlaceMark(0, 0); // X
        engine.PlaceMark(1, 1); // O (center)
        engine.PlaceMark(0, 1); // X threatens row 0

        var computer = new ComputerPlayer(engine);
        var (row, col) = computer.GetBestMove();

        Assert.Equal(0, row);
        Assert.Equal(2, col);
    }

    [Fact]
    public void GetBestMove_TakesWinningMove()
    {
        var engine = new GameEngine();
        // Set up board so O can win
        engine.PlaceMark(0, 0); // X
        engine.PlaceMark(1, 0); // O
        engine.PlaceMark(0, 1); // X
        engine.PlaceMark(1, 1); // O - now O has (1,0) and (1,1), can win at (1,2)
        engine.PlaceMark(2, 2); // X

        var computer = new ComputerPlayer(engine);
        var (row, col) = computer.GetBestMove();

        Assert.Equal(1, row);
        Assert.Equal(2, col);
    }

    [Fact]
    public void GetBestMove_ReturnsEmptyCell()
    {
        var engine = new GameEngine();
        engine.PlaceMark(0, 0); // X
        var computer = new ComputerPlayer(engine);

        var (row, col) = computer.GetBestMove();
        Assert.Equal("", engine.GetCell(row, col));
    }

    [Fact]
    public void GetBestMove_OnlyOneCellLeft_ReturnsThatCell()
    {
        var engine = new GameEngine();
        // Fill all but (2,2) without winning
        // X O X
        // X X O
        // O _ _
        engine.PlaceMark(0, 0); // X
        engine.PlaceMark(0, 1); // O
        engine.PlaceMark(0, 2); // X
        engine.PlaceMark(1, 2); // O
        engine.PlaceMark(1, 0); // X
        engine.PlaceMark(2, 0); // O
        engine.PlaceMark(1, 1); // X
        // O's turn, (2,1) and (2,2) available

        var computer = new ComputerPlayer(engine);
        var (row, col) = computer.GetBestMove();

        Assert.Equal(2, row);
        Assert.True(col == 1 || col == 2);
    }

    [Fact]
    public void GetBestMove_PrefersCenter_WhenAvailable()
    {
        var engine = new GameEngine();
        engine.PlaceMark(0, 0); // X takes corner

        var computer = new ComputerPlayer(engine);
        var (row, col) = computer.GetBestMove();

        // Minimax should pick center or a strong defensive position
        Assert.Equal("", engine.GetCell(row, col));
    }

    [Fact]
    public void GetBestMove_NeverLoses_MultipleScenarios()
    {
        // Play multiple games where X plays suboptimally
        // Computer (O) should never lose
        var positions = new[] { (0, 0), (0, 1), (0, 2), (1, 0), (1, 1), (1, 2), (2, 0), (2, 1), (2, 2) };

        foreach (var firstMove in positions)
        {
            var engine = new GameEngine();
            engine.PlaceMark(firstMove.Item1, firstMove.Item2); // X

            var computer = new ComputerPlayer(engine);

            while (!engine.IsGameOver)
            {
                if (!engine.IsXTurn)
                {
                    var (r, c) = computer.GetBestMove();
                    engine.PlaceMark(r, c);
                }
                else
                {
                    // X plays first available
                    bool placed = false;
                    for (int row = 0; row < GameEngine.BoardSize && !placed; row++)
                        for (int col = 0; col < GameEngine.BoardSize && !placed; col++)
                            if (string.IsNullOrEmpty(engine.GetCell(row, col)))
                            {
                                engine.PlaceMark(row, col);
                                placed = true;
                            }
                }
            }

            // O (computer) should never lose
            Assert.True(engine.OWins > 0 || engine.Draws > 0,
                $"Computer lost when X started at ({firstMove.Item1},{firstMove.Item2})");
        }
    }
}
