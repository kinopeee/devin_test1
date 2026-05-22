using Xunit;
using TicTacToe;

namespace TicTacToe.Tests;

public class GameEngineTests
{
    #region Initial State

    [Fact]
    public void NewGame_IsXTurn_ReturnsTrue()
    {
        var engine = new GameEngine();
        Assert.True(engine.IsXTurn);
    }

    [Fact]
    public void NewGame_IsGameOver_ReturnsFalse()
    {
        var engine = new GameEngine();
        Assert.False(engine.IsGameOver);
    }

    [Fact]
    public void NewGame_AllCellsAreEmpty()
    {
        var engine = new GameEngine();
        for (int r = 0; r < GameEngine.BoardSize; r++)
            for (int c = 0; c < GameEngine.BoardSize; c++)
                Assert.Equal("", engine.GetCell(r, c));
    }

    [Fact]
    public void NewGame_AllScoresAreZero()
    {
        var engine = new GameEngine();
        Assert.Equal(0, engine.XWins);
        Assert.Equal(0, engine.OWins);
        Assert.Equal(0, engine.Draws);
    }

    [Fact]
    public void NewGame_CurrentPlayerMark_IsX()
    {
        var engine = new GameEngine();
        Assert.Equal("X", engine.CurrentPlayerMark);
    }

    #endregion

    #region PlaceMark - Valid Moves

    [Fact]
    public void PlaceMark_FirstMoveOnEmptyCell_ReturnsSuccess()
    {
        var engine = new GameEngine();
        var result = engine.PlaceMark(0, 0);
        Assert.Equal(MoveResult.Success, result);
    }

    [Fact]
    public void PlaceMark_FirstMove_PlacesXOnBoard()
    {
        var engine = new GameEngine();
        engine.PlaceMark(0, 0);
        Assert.Equal("X", engine.GetCell(0, 0));
    }

    [Fact]
    public void PlaceMark_SecondMove_PlacesOOnBoard()
    {
        var engine = new GameEngine();
        engine.PlaceMark(0, 0);
        engine.PlaceMark(1, 0);
        Assert.Equal("O", engine.GetCell(1, 0));
    }

    [Fact]
    public void PlaceMark_AlternatesTurnsBetweenXAndO()
    {
        var engine = new GameEngine();
        Assert.True(engine.IsXTurn);

        engine.PlaceMark(0, 0);
        Assert.False(engine.IsXTurn);
        Assert.Equal("O", engine.CurrentPlayerMark);

        engine.PlaceMark(1, 0);
        Assert.True(engine.IsXTurn);
        Assert.Equal("X", engine.CurrentPlayerMark);
    }

    [Fact]
    public void PlaceMark_SuccessfulMove_DoesNotEndGame()
    {
        var engine = new GameEngine();
        engine.PlaceMark(0, 0);
        Assert.False(engine.IsGameOver);
    }

    [Fact]
    public void PlaceMark_OnlyAffectsTargetCell()
    {
        var engine = new GameEngine();
        engine.PlaceMark(1, 1);

        for (int r = 0; r < GameEngine.BoardSize; r++)
            for (int c = 0; c < GameEngine.BoardSize; c++)
                if (r == 1 && c == 1)
                    Assert.Equal("X", engine.GetCell(r, c));
                else
                    Assert.Equal("", engine.GetCell(r, c));
    }

    #endregion

    #region PlaceMark - Invalid Moves

    [Fact]
    public void PlaceMark_OnOccupiedCell_ReturnsCellOccupied()
    {
        var engine = new GameEngine();
        engine.PlaceMark(0, 0);
        var result = engine.PlaceMark(0, 0);
        Assert.Equal(MoveResult.CellOccupied, result);
    }

    [Fact]
    public void PlaceMark_OnOccupiedCell_DoesNotOverwriteExistingMark()
    {
        var engine = new GameEngine();
        engine.PlaceMark(0, 0); // X
        engine.PlaceMark(0, 0); // O tries same cell
        Assert.Equal("X", engine.GetCell(0, 0));
    }

    [Fact]
    public void PlaceMark_OnOccupiedCell_DoesNotSwitchTurn()
    {
        var engine = new GameEngine();
        engine.PlaceMark(0, 0); // X → O's turn
        Assert.False(engine.IsXTurn);
        engine.PlaceMark(0, 0); // rejected
        Assert.False(engine.IsXTurn); // still O's turn
    }

    [Fact]
    public void PlaceMark_AfterGameOver_ReturnsGameAlreadyOver()
    {
        var engine = new GameEngine();
        PlayXWinsTopRow(engine);

        var result = engine.PlaceMark(2, 2);
        Assert.Equal(MoveResult.GameAlreadyOver, result);
    }

    [Fact]
    public void PlaceMark_AfterGameOver_DoesNotModifyBoard()
    {
        var engine = new GameEngine();
        PlayXWinsTopRow(engine);

        engine.PlaceMark(2, 2);
        Assert.Equal("", engine.GetCell(2, 2));
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    [InlineData(3, 0)]
    [InlineData(0, 3)]
    [InlineData(-1, -1)]
    [InlineData(3, 3)]
    [InlineData(100, 0)]
    public void PlaceMark_OutOfRange_ReturnsInvalidPosition(int row, int col)
    {
        var engine = new GameEngine();
        var result = engine.PlaceMark(row, col);
        Assert.Equal(MoveResult.InvalidPosition, result);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, 3)]
    public void PlaceMark_OutOfRange_DoesNotSwitchTurn(int row, int col)
    {
        var engine = new GameEngine();
        Assert.True(engine.IsXTurn);
        engine.PlaceMark(row, col);
        Assert.True(engine.IsXTurn);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(3, 3)]
    public void GetCell_OutOfRange_ThrowsArgumentOutOfRangeException(int row, int col)
    {
        var engine = new GameEngine();
        Assert.Throws<ArgumentOutOfRangeException>(() => engine.GetCell(row, col));
    }

    #endregion

    #region Win Detection - All 8 Patterns

    [Fact]
    public void PlaceMark_XWinsRow0_ReturnsWinAndSetsGameOver()
    {
        var engine = new GameEngine();
        // X: (0,0), (0,1), (0,2) / O: (1,0), (1,1)
        engine.PlaceMark(0, 0); // X
        engine.PlaceMark(1, 0); // O
        engine.PlaceMark(0, 1); // X
        engine.PlaceMark(1, 1); // O
        var result = engine.PlaceMark(0, 2); // X wins

        Assert.Equal(MoveResult.Win, result);
        Assert.True(engine.IsGameOver);
        Assert.True(engine.IsXTurn); // winner's turn preserved
    }

    [Fact]
    public void PlaceMark_XWinsRow1_ReturnsWin()
    {
        var engine = new GameEngine();
        engine.PlaceMark(1, 0); // X
        engine.PlaceMark(0, 0); // O
        engine.PlaceMark(1, 1); // X
        engine.PlaceMark(0, 1); // O
        var result = engine.PlaceMark(1, 2); // X wins

        Assert.Equal(MoveResult.Win, result);
        Assert.True(engine.IsGameOver);
    }

    [Fact]
    public void PlaceMark_XWinsRow2_ReturnsWin()
    {
        var engine = new GameEngine();
        engine.PlaceMark(2, 0); // X
        engine.PlaceMark(0, 0); // O
        engine.PlaceMark(2, 1); // X
        engine.PlaceMark(0, 1); // O
        var result = engine.PlaceMark(2, 2); // X wins

        Assert.Equal(MoveResult.Win, result);
        Assert.True(engine.IsGameOver);
    }

    [Fact]
    public void PlaceMark_XWinsCol0_ReturnsWin()
    {
        var engine = new GameEngine();
        engine.PlaceMark(0, 0); // X
        engine.PlaceMark(0, 1); // O
        engine.PlaceMark(1, 0); // X
        engine.PlaceMark(1, 1); // O
        var result = engine.PlaceMark(2, 0); // X wins

        Assert.Equal(MoveResult.Win, result);
        Assert.True(engine.IsGameOver);
    }

    [Fact]
    public void PlaceMark_XWinsCol1_ReturnsWin()
    {
        var engine = new GameEngine();
        engine.PlaceMark(0, 1); // X
        engine.PlaceMark(0, 0); // O
        engine.PlaceMark(1, 1); // X
        engine.PlaceMark(1, 0); // O
        var result = engine.PlaceMark(2, 1); // X wins

        Assert.Equal(MoveResult.Win, result);
        Assert.True(engine.IsGameOver);
    }

    [Fact]
    public void PlaceMark_XWinsCol2_ReturnsWin()
    {
        var engine = new GameEngine();
        engine.PlaceMark(0, 2); // X
        engine.PlaceMark(0, 0); // O
        engine.PlaceMark(1, 2); // X
        engine.PlaceMark(1, 0); // O
        var result = engine.PlaceMark(2, 2); // X wins

        Assert.Equal(MoveResult.Win, result);
        Assert.True(engine.IsGameOver);
    }

    [Fact]
    public void PlaceMark_XWinsMainDiagonal_ReturnsWin()
    {
        var engine = new GameEngine();
        // X: (0,0), (1,1), (2,2) / O: (0,1), (0,2)
        engine.PlaceMark(0, 0); // X
        engine.PlaceMark(0, 1); // O
        engine.PlaceMark(1, 1); // X
        engine.PlaceMark(0, 2); // O
        var result = engine.PlaceMark(2, 2); // X wins

        Assert.Equal(MoveResult.Win, result);
        Assert.True(engine.IsGameOver);
    }

    [Fact]
    public void PlaceMark_XWinsAntiDiagonal_ReturnsWin()
    {
        var engine = new GameEngine();
        // X: (0,2), (1,1), (2,0) / O: (0,0), (1,0)
        engine.PlaceMark(0, 2); // X
        engine.PlaceMark(0, 0); // O
        engine.PlaceMark(1, 1); // X
        engine.PlaceMark(1, 0); // O
        var result = engine.PlaceMark(2, 0); // X wins

        Assert.Equal(MoveResult.Win, result);
        Assert.True(engine.IsGameOver);
    }

    [Fact]
    public void PlaceMark_OWinsRow0_ReturnsWin()
    {
        var engine = new GameEngine();
        // X: (1,0), (1,1), (2,2) / O: (0,0), (0,1), (0,2)
        engine.PlaceMark(1, 0); // X
        engine.PlaceMark(0, 0); // O
        engine.PlaceMark(1, 1); // X
        engine.PlaceMark(0, 1); // O
        engine.PlaceMark(2, 2); // X
        var result = engine.PlaceMark(0, 2); // O wins

        Assert.Equal(MoveResult.Win, result);
        Assert.True(engine.IsGameOver);
        Assert.False(engine.IsXTurn); // O's turn preserved
    }

    [Fact]
    public void PlaceMark_OWinsCol0_ReturnsWin()
    {
        var engine = new GameEngine();
        // X: (0,1), (1,1), (2,2) / O: (0,0), (1,0), (2,0)
        engine.PlaceMark(0, 1); // X
        engine.PlaceMark(0, 0); // O
        engine.PlaceMark(1, 1); // X
        engine.PlaceMark(1, 0); // O
        engine.PlaceMark(2, 2); // X
        var result = engine.PlaceMark(2, 0); // O wins

        Assert.Equal(MoveResult.Win, result);
        Assert.True(engine.IsGameOver);
    }

    [Fact]
    public void PlaceMark_OWinsMainDiagonal_ReturnsWin()
    {
        var engine = new GameEngine();
        // X: (0,1), (0,2), (1,0) / O: (0,0), (1,1), (2,2)
        engine.PlaceMark(0, 1); // X
        engine.PlaceMark(0, 0); // O
        engine.PlaceMark(0, 2); // X
        engine.PlaceMark(1, 1); // O
        engine.PlaceMark(1, 0); // X
        var result = engine.PlaceMark(2, 2); // O wins

        Assert.Equal(MoveResult.Win, result);
        Assert.True(engine.IsGameOver);
    }

    [Fact]
    public void PlaceMark_OWinsAntiDiagonal_ReturnsWin()
    {
        var engine = new GameEngine();
        // X: (0,0), (1,0), (2,2) / O: (0,2), (1,1), (2,0)
        engine.PlaceMark(0, 0); // X
        engine.PlaceMark(0, 2); // O
        engine.PlaceMark(1, 0); // X
        engine.PlaceMark(1, 1); // O
        engine.PlaceMark(2, 2); // X
        var result = engine.PlaceMark(2, 0); // O wins

        Assert.Equal(MoveResult.Win, result);
        Assert.True(engine.IsGameOver);
    }

    #endregion

    #region Draw Detection

    [Fact]
    public void PlaceMark_BoardFullNoWinner_ReturnsDraw()
    {
        var engine = new GameEngine();
        // Board:
        //   X | O | X
        //   X | X | O
        //   O | X | O
        engine.PlaceMark(0, 0); // X
        engine.PlaceMark(0, 1); // O
        engine.PlaceMark(0, 2); // X
        engine.PlaceMark(1, 2); // O
        engine.PlaceMark(1, 0); // X
        engine.PlaceMark(2, 0); // O
        engine.PlaceMark(1, 1); // X
        engine.PlaceMark(2, 2); // O
        var result = engine.PlaceMark(2, 1); // X → draw

        Assert.Equal(MoveResult.Draw, result);
        Assert.True(engine.IsGameOver);
    }

    [Fact]
    public void PlaceMark_DrawGame_BoardIsFull()
    {
        var engine = new GameEngine();
        PlayDrawGame(engine);

        Assert.True(engine.IsBoardFull());
    }

    [Fact]
    public void PlaceMark_DrawGame_NoWinner()
    {
        var engine = new GameEngine();
        PlayDrawGame(engine);

        Assert.False(engine.CheckWinner());
    }

    #endregion

    #region Score Tracking

    [Fact]
    public void Score_XWins_IncrementsXWinsOnly()
    {
        var engine = new GameEngine();
        PlayXWinsTopRow(engine);

        Assert.Equal(1, engine.XWins);
        Assert.Equal(0, engine.OWins);
        Assert.Equal(0, engine.Draws);
    }

    [Fact]
    public void Score_OWins_IncrementsOWinsOnly()
    {
        var engine = new GameEngine();
        PlayOWinsLeftColumn(engine);

        Assert.Equal(0, engine.XWins);
        Assert.Equal(1, engine.OWins);
        Assert.Equal(0, engine.Draws);
    }

    [Fact]
    public void Score_Draw_IncrementsDrawsOnly()
    {
        var engine = new GameEngine();
        PlayDrawGame(engine);

        Assert.Equal(0, engine.XWins);
        Assert.Equal(0, engine.OWins);
        Assert.Equal(1, engine.Draws);
    }

    [Fact]
    public void Score_MultipleGamesAccumulateCorrectly()
    {
        var engine = new GameEngine();

        PlayXWinsTopRow(engine);
        Assert.Equal(1, engine.XWins);

        engine.Reset();
        PlayOWinsLeftColumn(engine);
        Assert.Equal(1, engine.OWins);

        engine.Reset();
        PlayDrawGame(engine);
        Assert.Equal(1, engine.Draws);

        engine.Reset();
        PlayXWinsTopRow(engine);
        Assert.Equal(2, engine.XWins);
        Assert.Equal(1, engine.OWins);
        Assert.Equal(1, engine.Draws);
    }

    #endregion

    #region Reset

    [Fact]
    public void Reset_ClearsAllCells()
    {
        var engine = new GameEngine();
        PlayXWinsTopRow(engine);
        engine.Reset();

        for (int r = 0; r < GameEngine.BoardSize; r++)
            for (int c = 0; c < GameEngine.BoardSize; c++)
                Assert.Equal("", engine.GetCell(r, c));
    }

    [Fact]
    public void Reset_SetsXAsCurrentPlayer()
    {
        var engine = new GameEngine();
        engine.PlaceMark(0, 0); // X
        engine.PlaceMark(1, 0); // O, now X's turn
        engine.PlaceMark(0, 1); // X, now O's turn
        engine.Reset();

        Assert.True(engine.IsXTurn);
        Assert.Equal("X", engine.CurrentPlayerMark);
    }

    [Fact]
    public void Reset_ClearsGameOver()
    {
        var engine = new GameEngine();
        PlayXWinsTopRow(engine);
        Assert.True(engine.IsGameOver);

        engine.Reset();
        Assert.False(engine.IsGameOver);
    }

    [Fact]
    public void Reset_PreservesScores()
    {
        var engine = new GameEngine();
        PlayXWinsTopRow(engine);
        engine.Reset();
        PlayOWinsLeftColumn(engine);
        engine.Reset();

        Assert.Equal(1, engine.XWins);
        Assert.Equal(1, engine.OWins);
    }

    [Fact]
    public void Reset_AllowsNewGameToStart()
    {
        var engine = new GameEngine();
        PlayXWinsTopRow(engine);

        var blockedResult = engine.PlaceMark(2, 2);
        Assert.Equal(MoveResult.GameAlreadyOver, blockedResult);

        engine.Reset();

        var newResult = engine.PlaceMark(0, 0);
        Assert.Equal(MoveResult.Success, newResult);
        Assert.Equal("X", engine.GetCell(0, 0));
    }

    #endregion

    #region GetWinLine

    [Fact]
    public void GetWinLine_RowWin_ReturnsCorrectRowLine()
    {
        var engine = new GameEngine();
        PlayXWinsTopRow(engine);

        var winLine = engine.GetWinLine();
        Assert.NotNull(winLine);
        Assert.Equal(WinLineType.Row, winLine.Type);
        Assert.Equal(0, winLine.Index);
    }

    [Fact]
    public void GetWinLine_ColumnWin_ReturnsCorrectColumnLine()
    {
        var engine = new GameEngine();
        // X wins col 0: X(0,0), O(0,1), X(1,0), O(1,1), X(2,0)
        engine.PlaceMark(0, 0);
        engine.PlaceMark(0, 1);
        engine.PlaceMark(1, 0);
        engine.PlaceMark(1, 1);
        engine.PlaceMark(2, 0);

        var winLine = engine.GetWinLine();
        Assert.NotNull(winLine);
        Assert.Equal(WinLineType.Column, winLine.Type);
        Assert.Equal(0, winLine.Index);
    }

    [Fact]
    public void GetWinLine_MainDiagonalWin_ReturnsCorrectLine()
    {
        var engine = new GameEngine();
        // X: (0,0), (1,1), (2,2) / O: (0,1), (0,2)
        engine.PlaceMark(0, 0);
        engine.PlaceMark(0, 1);
        engine.PlaceMark(1, 1);
        engine.PlaceMark(0, 2);
        engine.PlaceMark(2, 2);

        var winLine = engine.GetWinLine();
        Assert.NotNull(winLine);
        Assert.Equal(WinLineType.DiagonalMain, winLine.Type);
    }

    [Fact]
    public void GetWinLine_AntiDiagonalWin_ReturnsCorrectLine()
    {
        var engine = new GameEngine();
        // X: (0,2), (1,1), (2,0) / O: (0,0), (1,0)
        engine.PlaceMark(0, 2);
        engine.PlaceMark(0, 0);
        engine.PlaceMark(1, 1);
        engine.PlaceMark(1, 0);
        engine.PlaceMark(2, 0);

        var winLine = engine.GetWinLine();
        Assert.NotNull(winLine);
        Assert.Equal(WinLineType.DiagonalAnti, winLine.Type);
    }

    [Fact]
    public void GetWinLine_NoWin_ReturnsNull()
    {
        var engine = new GameEngine();
        engine.PlaceMark(0, 0);

        var winLine = engine.GetWinLine();
        Assert.Null(winLine);
    }

    [Fact]
    public void GetWinLine_DrawGame_ReturnsNull()
    {
        var engine = new GameEngine();
        PlayDrawGame(engine);

        // After draw, the last player's mark won't form a win line
        var winLine = engine.GetWinLine();
        Assert.Null(winLine);
    }

    #endregion

    #region CheckWinner / IsBoardFull Public API

    [Fact]
    public void CheckWinner_EmptyBoard_ReturnsFalse()
    {
        var engine = new GameEngine();
        Assert.False(engine.CheckWinner());
    }

    [Fact]
    public void IsBoardFull_EmptyBoard_ReturnsFalse()
    {
        var engine = new GameEngine();
        Assert.False(engine.IsBoardFull());
    }

    [Fact]
    public void IsBoardFull_PartialBoard_ReturnsFalse()
    {
        var engine = new GameEngine();
        engine.PlaceMark(0, 0);
        engine.PlaceMark(0, 1);
        Assert.False(engine.IsBoardFull());
    }

    #endregion

    #region Helper Methods

    private static void PlayXWinsTopRow(GameEngine engine)
    {
        // X: (0,0), (0,1), (0,2) / O: (1,0), (1,1)
        engine.PlaceMark(0, 0); // X
        engine.PlaceMark(1, 0); // O
        engine.PlaceMark(0, 1); // X
        engine.PlaceMark(1, 1); // O
        engine.PlaceMark(0, 2); // X wins
    }

    private static void PlayOWinsLeftColumn(GameEngine engine)
    {
        // X: (0,1), (1,1), (2,2) / O: (0,0), (1,0), (2,0)
        engine.PlaceMark(0, 1); // X
        engine.PlaceMark(0, 0); // O
        engine.PlaceMark(1, 1); // X
        engine.PlaceMark(1, 0); // O
        engine.PlaceMark(2, 2); // X
        engine.PlaceMark(2, 0); // O wins
    }

    private static void PlayDrawGame(GameEngine engine)
    {
        // Board:
        //   X | O | X
        //   X | X | O
        //   O | X | O
        engine.PlaceMark(0, 0); // X
        engine.PlaceMark(0, 1); // O
        engine.PlaceMark(0, 2); // X
        engine.PlaceMark(1, 2); // O
        engine.PlaceMark(1, 0); // X
        engine.PlaceMark(2, 0); // O
        engine.PlaceMark(1, 1); // X
        engine.PlaceMark(2, 2); // O
        engine.PlaceMark(2, 1); // X → draw
    }

    #endregion
}
