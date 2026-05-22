using System.Drawing;
using System.Windows.Forms;
using TicTacToe;
using Xunit;

namespace TicTacToe.E2ETests;

public class Form1E2ETests
{
    #region Initial State

    [StaFact]
    public void Form_InitialState_AllCellsAreEmpty()
    {
        using var form = new Form1();

        for (int r = 0; r < GameEngine.BoardSize; r++)
            for (int c = 0; c < GameEngine.BoardSize; c++)
                Assert.Equal("", form.GetCell(r, c).Text);
    }

    [StaFact]
    public void Form_InitialState_StatusShowsXTurn()
    {
        using var form = new Form1();
        Assert.Equal("X の番です", form.StatusLabel.Text);
    }

    [StaFact]
    public void Form_InitialState_ScoreShowsAllZeros()
    {
        using var form = new Form1();
        Assert.Equal("X: 0  O: 0  引き分け: 0", form.ScoreLabel.Text);
    }

    [StaFact]
    public void Form_InitialState_AllCellsHaveWhiteBackground()
    {
        using var form = new Form1();

        for (int r = 0; r < GameEngine.BoardSize; r++)
            for (int c = 0; c < GameEngine.BoardSize; c++)
                Assert.Equal(Color.White, form.GetCell(r, c).BackColor);
    }

    #endregion

    #region Cell Click - Basic

    [StaFact]
    public void ClickCell_FirstClick_ShowsXWithDarkBlueColor()
    {
        using var form = new Form1();

        form.SimulateCellClick(0, 0);

        Assert.Equal("X", form.GetCell(0, 0).Text);
        Assert.Equal(Color.DarkBlue, form.GetCell(0, 0).ForeColor);
    }

    [StaFact]
    public void ClickCell_SecondClick_ShowsOWithDarkRedColor()
    {
        using var form = new Form1();

        form.SimulateCellClick(0, 0); // X
        form.SimulateCellClick(1, 1); // O

        Assert.Equal("O", form.GetCell(1, 1).Text);
        Assert.Equal(Color.DarkRed, form.GetCell(1, 1).ForeColor);
    }

    [StaFact]
    public void ClickCell_AfterXMove_StatusShowsOTurn()
    {
        using var form = new Form1();

        form.SimulateCellClick(0, 0);

        Assert.Equal("O の番です", form.StatusLabel.Text);
    }

    [StaFact]
    public void ClickCell_OccupiedCell_DoesNotChangeText()
    {
        using var form = new Form1();

        form.SimulateCellClick(0, 0); // X
        form.SimulateCellClick(0, 0); // try again (O's turn)

        Assert.Equal("X", form.GetCell(0, 0).Text);
    }

    [StaFact]
    public void ClickCell_OccupiedCell_DoesNotSwitchTurn()
    {
        using var form = new Form1();

        form.SimulateCellClick(0, 0); // X → O's turn
        Assert.Equal("O の番です", form.StatusLabel.Text);

        form.SimulateCellClick(0, 0); // rejected
        Assert.Equal("O の番です", form.StatusLabel.Text); // still O's turn
    }

    #endregion

    #region Win Scenario

    [StaFact]
    public void PlayGame_XWinsTopRow_ShowsWinMessage()
    {
        using var form = new Form1();

        form.SimulateCellClick(0, 0); // X
        form.SimulateCellClick(1, 0); // O
        form.SimulateCellClick(0, 1); // X
        form.SimulateCellClick(1, 1); // O
        form.SimulateCellClick(0, 2); // X wins

        Assert.Equal("X の勝ち！", form.StatusLabel.Text);
    }

    [StaFact]
    public void PlayGame_XWinsTopRow_HighlightsWinningCells()
    {
        using var form = new Form1();

        form.SimulateCellClick(0, 0); // X
        form.SimulateCellClick(1, 0); // O
        form.SimulateCellClick(0, 1); // X
        form.SimulateCellClick(1, 1); // O
        form.SimulateCellClick(0, 2); // X wins

        Assert.Equal(Color.LightGreen, form.GetCell(0, 0).BackColor);
        Assert.Equal(Color.LightGreen, form.GetCell(0, 1).BackColor);
        Assert.Equal(Color.LightGreen, form.GetCell(0, 2).BackColor);
    }

    [StaFact]
    public void PlayGame_XWinsTopRow_NonWinningCellsNotHighlighted()
    {
        using var form = new Form1();

        form.SimulateCellClick(0, 0); // X
        form.SimulateCellClick(1, 0); // O
        form.SimulateCellClick(0, 1); // X
        form.SimulateCellClick(1, 1); // O
        form.SimulateCellClick(0, 2); // X wins

        Assert.Equal(Color.White, form.GetCell(1, 0).BackColor);
        Assert.Equal(Color.White, form.GetCell(1, 1).BackColor);
        Assert.Equal(Color.White, form.GetCell(2, 0).BackColor);
    }

    [StaFact]
    public void PlayGame_XWinsTopRow_UpdatesScore()
    {
        using var form = new Form1();

        form.SimulateCellClick(0, 0); // X
        form.SimulateCellClick(1, 0); // O
        form.SimulateCellClick(0, 1); // X
        form.SimulateCellClick(1, 1); // O
        form.SimulateCellClick(0, 2); // X wins

        Assert.Equal("X: 1  O: 0  引き分け: 0", form.ScoreLabel.Text);
    }

    [StaFact]
    public void PlayGame_OWinsLeftColumn_ShowsWinMessage()
    {
        using var form = new Form1();

        form.SimulateCellClick(0, 1); // X
        form.SimulateCellClick(0, 0); // O
        form.SimulateCellClick(1, 1); // X
        form.SimulateCellClick(1, 0); // O
        form.SimulateCellClick(2, 2); // X
        form.SimulateCellClick(2, 0); // O wins

        Assert.Equal("O の勝ち！", form.StatusLabel.Text);
    }

    [StaFact]
    public void PlayGame_OWinsLeftColumn_HighlightsColumn()
    {
        using var form = new Form1();

        form.SimulateCellClick(0, 1); // X
        form.SimulateCellClick(0, 0); // O
        form.SimulateCellClick(1, 1); // X
        form.SimulateCellClick(1, 0); // O
        form.SimulateCellClick(2, 2); // X
        form.SimulateCellClick(2, 0); // O wins

        Assert.Equal(Color.LightGreen, form.GetCell(0, 0).BackColor);
        Assert.Equal(Color.LightGreen, form.GetCell(1, 0).BackColor);
        Assert.Equal(Color.LightGreen, form.GetCell(2, 0).BackColor);
    }

    [StaFact]
    public void PlayGame_XWinsMainDiagonal_HighlightsDiagonal()
    {
        using var form = new Form1();

        form.SimulateCellClick(0, 0); // X
        form.SimulateCellClick(0, 1); // O
        form.SimulateCellClick(1, 1); // X
        form.SimulateCellClick(0, 2); // O
        form.SimulateCellClick(2, 2); // X wins

        Assert.Equal(Color.LightGreen, form.GetCell(0, 0).BackColor);
        Assert.Equal(Color.LightGreen, form.GetCell(1, 1).BackColor);
        Assert.Equal(Color.LightGreen, form.GetCell(2, 2).BackColor);
    }

    [StaFact]
    public void PlayGame_XWinsAntiDiagonal_HighlightsDiagonal()
    {
        using var form = new Form1();

        form.SimulateCellClick(0, 2); // X
        form.SimulateCellClick(0, 0); // O
        form.SimulateCellClick(1, 1); // X
        form.SimulateCellClick(1, 0); // O
        form.SimulateCellClick(2, 0); // X wins

        Assert.Equal(Color.LightGreen, form.GetCell(0, 2).BackColor);
        Assert.Equal(Color.LightGreen, form.GetCell(1, 1).BackColor);
        Assert.Equal(Color.LightGreen, form.GetCell(2, 0).BackColor);
    }

    [StaFact]
    public void PlayGame_AfterWin_ClicksAreIgnored()
    {
        using var form = new Form1();

        // X wins top row
        form.SimulateCellClick(0, 0); // X
        form.SimulateCellClick(1, 0); // O
        form.SimulateCellClick(0, 1); // X
        form.SimulateCellClick(1, 1); // O
        form.SimulateCellClick(0, 2); // X wins

        // Try clicking remaining cells
        form.SimulateCellClick(2, 0);
        form.SimulateCellClick(2, 1);
        form.SimulateCellClick(2, 2);

        Assert.Equal("", form.GetCell(2, 0).Text);
        Assert.Equal("", form.GetCell(2, 1).Text);
        Assert.Equal("", form.GetCell(2, 2).Text);
    }

    #endregion

    #region Draw Scenario

    [StaFact]
    public void PlayGame_Draw_ShowsDrawMessage()
    {
        using var form = new Form1();

        // Board: X O X / X X O / O X O
        form.SimulateCellClick(0, 0); // X
        form.SimulateCellClick(0, 1); // O
        form.SimulateCellClick(0, 2); // X
        form.SimulateCellClick(1, 2); // O
        form.SimulateCellClick(1, 0); // X
        form.SimulateCellClick(2, 0); // O
        form.SimulateCellClick(1, 1); // X
        form.SimulateCellClick(2, 2); // O
        form.SimulateCellClick(2, 1); // X → draw

        Assert.Equal("引き分け！", form.StatusLabel.Text);
    }

    [StaFact]
    public void PlayGame_Draw_UpdatesScore()
    {
        using var form = new Form1();

        form.SimulateCellClick(0, 0); // X
        form.SimulateCellClick(0, 1); // O
        form.SimulateCellClick(0, 2); // X
        form.SimulateCellClick(1, 2); // O
        form.SimulateCellClick(1, 0); // X
        form.SimulateCellClick(2, 0); // O
        form.SimulateCellClick(1, 1); // X
        form.SimulateCellClick(2, 2); // O
        form.SimulateCellClick(2, 1); // X → draw

        Assert.Equal("X: 0  O: 0  引き分け: 1", form.ScoreLabel.Text);
    }

    [StaFact]
    public void PlayGame_Draw_NoCellsHighlighted()
    {
        using var form = new Form1();

        form.SimulateCellClick(0, 0); // X
        form.SimulateCellClick(0, 1); // O
        form.SimulateCellClick(0, 2); // X
        form.SimulateCellClick(1, 2); // O
        form.SimulateCellClick(1, 0); // X
        form.SimulateCellClick(2, 0); // O
        form.SimulateCellClick(1, 1); // X
        form.SimulateCellClick(2, 2); // O
        form.SimulateCellClick(2, 1); // X → draw

        for (int r = 0; r < GameEngine.BoardSize; r++)
            for (int c = 0; c < GameEngine.BoardSize; c++)
                Assert.Equal(Color.White, form.GetCell(r, c).BackColor);
    }

    #endregion

    #region Reset

    [StaFact]
    public void Reset_AfterWin_ClearsAllCellTexts()
    {
        using var form = new Form1();

        // X wins top row
        form.SimulateCellClick(0, 0);
        form.SimulateCellClick(1, 0);
        form.SimulateCellClick(0, 1);
        form.SimulateCellClick(1, 1);
        form.SimulateCellClick(0, 2);

        form.SimulateResetClick();

        for (int r = 0; r < GameEngine.BoardSize; r++)
            for (int c = 0; c < GameEngine.BoardSize; c++)
                Assert.Equal("", form.GetCell(r, c).Text);
    }

    [StaFact]
    public void Reset_AfterWin_RestoresWhiteBackground()
    {
        using var form = new Form1();

        // X wins top row
        form.SimulateCellClick(0, 0);
        form.SimulateCellClick(1, 0);
        form.SimulateCellClick(0, 1);
        form.SimulateCellClick(1, 1);
        form.SimulateCellClick(0, 2);

        form.SimulateResetClick();

        for (int r = 0; r < GameEngine.BoardSize; r++)
            for (int c = 0; c < GameEngine.BoardSize; c++)
                Assert.Equal(Color.White, form.GetCell(r, c).BackColor);
    }

    [StaFact]
    public void Reset_AfterWin_StatusShowsXTurn()
    {
        using var form = new Form1();

        form.SimulateCellClick(0, 0);
        form.SimulateCellClick(1, 0);
        form.SimulateCellClick(0, 1);
        form.SimulateCellClick(1, 1);
        form.SimulateCellClick(0, 2);

        form.SimulateResetClick();

        Assert.Equal("X の番です", form.StatusLabel.Text);
    }

    [StaFact]
    public void Reset_PreservesScoreFromPreviousGames()
    {
        using var form = new Form1();

        // Game 1: X wins
        form.SimulateCellClick(0, 0);
        form.SimulateCellClick(1, 0);
        form.SimulateCellClick(0, 1);
        form.SimulateCellClick(1, 1);
        form.SimulateCellClick(0, 2);

        form.SimulateResetClick();

        Assert.Equal("X: 1  O: 0  引き分け: 0", form.ScoreLabel.Text);
    }

    [StaFact]
    public void Reset_AllowsNewGameToStart()
    {
        using var form = new Form1();

        // Game 1: X wins
        form.SimulateCellClick(0, 0);
        form.SimulateCellClick(1, 0);
        form.SimulateCellClick(0, 1);
        form.SimulateCellClick(1, 1);
        form.SimulateCellClick(0, 2);

        form.SimulateResetClick();

        // New game: click should work
        form.SimulateCellClick(1, 1);
        Assert.Equal("X", form.GetCell(1, 1).Text);
    }

    #endregion

    #region Multi-Game Scenario

    [StaFact]
    public void MultipleGames_ScoreAccumulatesCorrectly()
    {
        using var form = new Form1();

        // Game 1: X wins top row
        form.SimulateCellClick(0, 0); // X
        form.SimulateCellClick(1, 0); // O
        form.SimulateCellClick(0, 1); // X
        form.SimulateCellClick(1, 1); // O
        form.SimulateCellClick(0, 2); // X wins
        Assert.Equal("X: 1  O: 0  引き分け: 0", form.ScoreLabel.Text);

        form.SimulateResetClick();

        // Game 2: O wins left column
        form.SimulateCellClick(0, 1); // X
        form.SimulateCellClick(0, 0); // O
        form.SimulateCellClick(1, 1); // X
        form.SimulateCellClick(1, 0); // O
        form.SimulateCellClick(2, 2); // X
        form.SimulateCellClick(2, 0); // O wins
        Assert.Equal("X: 1  O: 1  引き分け: 0", form.ScoreLabel.Text);

        form.SimulateResetClick();

        // Game 3: Draw
        form.SimulateCellClick(0, 0); // X
        form.SimulateCellClick(0, 1); // O
        form.SimulateCellClick(0, 2); // X
        form.SimulateCellClick(1, 2); // O
        form.SimulateCellClick(1, 0); // X
        form.SimulateCellClick(2, 0); // O
        form.SimulateCellClick(1, 1); // X
        form.SimulateCellClick(2, 2); // O
        form.SimulateCellClick(2, 1); // draw
        Assert.Equal("X: 1  O: 1  引き分け: 1", form.ScoreLabel.Text);
    }

    [StaFact]
    public void MultipleGames_EachGameStartsFresh()
    {
        using var form = new Form1();

        // Game 1
        form.SimulateCellClick(0, 0);
        form.SimulateCellClick(1, 0);
        form.SimulateCellClick(0, 1);
        form.SimulateCellClick(1, 1);
        form.SimulateCellClick(0, 2);

        form.SimulateResetClick();

        // Game 2: verify first click is X (not O)
        form.SimulateCellClick(2, 2);
        Assert.Equal("X", form.GetCell(2, 2).Text);
        Assert.Equal(Color.DarkBlue, form.GetCell(2, 2).ForeColor);
    }

    #endregion

    #region Engine-UI Consistency

    [StaFact]
    public void ClickCell_EngineBoardMatchesUIBoard()
    {
        using var form = new Form1();

        form.SimulateCellClick(0, 0); // X
        form.SimulateCellClick(1, 1); // O
        form.SimulateCellClick(2, 0); // X

        for (int r = 0; r < GameEngine.BoardSize; r++)
        {
            for (int c = 0; c < GameEngine.BoardSize; c++)
            {
                Assert.Equal(form.Engine.GetCell(r, c), form.GetCell(r, c).Text);
            }
        }
    }

    [StaFact]
    public void WinGame_EngineStateMatchesUIState()
    {
        using var form = new Form1();

        form.SimulateCellClick(0, 0); // X
        form.SimulateCellClick(1, 0); // O
        form.SimulateCellClick(0, 1); // X
        form.SimulateCellClick(1, 1); // O
        form.SimulateCellClick(0, 2); // X wins

        Assert.True(form.Engine.IsGameOver);
        Assert.Equal(1, form.Engine.XWins);
        Assert.Contains("X の勝ち", form.StatusLabel.Text);
    }

    #endregion
}
