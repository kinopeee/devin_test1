using System.Drawing;
using System.Windows.Forms;
using TicTacToe;
using Xunit;

namespace TicTacToe.E2ETests;

public class Form1E2ETests
{
    private MockGameRepository CreateMockRepo() => new MockGameRepository();

    #region Initial State

    [StaFact]
    public void Form_InitialState_AllCellsAreEmpty()
    {
        using var form = new Form1(CreateMockRepo());

        for (int r = 0; r < GameEngine.BoardSize; r++)
            for (int c = 0; c < GameEngine.BoardSize; c++)
                Assert.Equal("", form.GetCell(r, c).Text);
    }

    [StaFact]
    public void Form_InitialState_StatusShowsPlayerTurn()
    {
        using var form = new Form1(CreateMockRepo());
        Assert.Equal("あなたの番です (X)", form.StatusLabel.Text);
    }

    [StaFact]
    public void Form_InitialState_ScoreShowsAllZeros()
    {
        using var form = new Form1(CreateMockRepo());
        Assert.Equal("あなた: 0  PC: 0  引き分け: 0", form.ScoreLabel.Text);
    }

    [StaFact]
    public void Form_InitialState_AllCellsHaveWhiteBackground()
    {
        using var form = new Form1(CreateMockRepo());

        for (int r = 0; r < GameEngine.BoardSize; r++)
            for (int c = 0; c < GameEngine.BoardSize; c++)
                Assert.Equal(Color.White, form.GetCell(r, c).BackColor);
    }

    #endregion

    #region Player Move

    [StaFact]
    public void ClickCell_PlayerClick_ShowsXWithDarkBlueColor()
    {
        using var form = new Form1(CreateMockRepo());

        form.SimulateCellClick(0, 0);

        Assert.Equal("X", form.GetCell(0, 0).Text);
        Assert.Equal(Color.DarkBlue, form.GetCell(0, 0).ForeColor);
    }

    [StaFact]
    public void ClickCell_PlayerClick_ComputerResponds()
    {
        using var form = new Form1(CreateMockRepo());

        form.SimulateCellClick(0, 0); // Player X

        // Computer should have placed O somewhere
        int oCount = 0;
        for (int r = 0; r < GameEngine.BoardSize; r++)
            for (int c = 0; c < GameEngine.BoardSize; c++)
                if (form.GetCell(r, c).Text == "O")
                    oCount++;

        Assert.Equal(1, oCount);
    }

    [StaFact]
    public void ClickCell_PlayerClick_ComputerOHasDarkRedColor()
    {
        using var form = new Form1(CreateMockRepo());

        form.SimulateCellClick(0, 0);

        // Find the O cell and check its color
        for (int r = 0; r < GameEngine.BoardSize; r++)
            for (int c = 0; c < GameEngine.BoardSize; c++)
                if (form.GetCell(r, c).Text == "O")
                    Assert.Equal(Color.DarkRed, form.GetCell(r, c).ForeColor);
    }

    [StaFact]
    public void ClickCell_AfterComputerMove_StatusShowsPlayerTurn()
    {
        using var form = new Form1(CreateMockRepo());

        form.SimulateCellClick(0, 0);

        if (!form.Engine.IsGameOver)
            Assert.Equal("あなたの番です (X)", form.StatusLabel.Text);
    }

    [StaFact]
    public void ClickCell_OccupiedCell_DoesNotChangeAnything()
    {
        using var form = new Form1(CreateMockRepo());

        form.SimulateCellClick(0, 0); // X placed
        string statusAfterFirst = form.StatusLabel.Text;

        form.SimulateCellClick(0, 0); // try same cell

        Assert.Equal("X", form.GetCell(0, 0).Text);
    }

    [StaFact]
    public void ClickCell_DuringComputerTurn_Ignored()
    {
        using var form = new Form1(CreateMockRepo());

        // After player move, it's player's turn again (computer already played)
        form.SimulateCellClick(1, 1);

        if (!form.Engine.IsGameOver)
        {
            Assert.True(form.Engine.IsXTurn);
        }
    }

    #endregion

    #region Game Flow - Computer Never Loses

    [StaFact]
    public void PlayFullGame_ComputerNeverLoses()
    {
        using var form = new Form1(CreateMockRepo());

        // Play through a game - player picks first available cells
        while (!form.Engine.IsGameOver)
        {
            bool moved = false;
            for (int r = 0; r < GameEngine.BoardSize && !moved; r++)
            {
                for (int c = 0; c < GameEngine.BoardSize && !moved; c++)
                {
                    if (string.IsNullOrEmpty(form.GetCell(r, c).Text))
                    {
                        form.SimulateCellClick(r, c);
                        moved = true;
                    }
                }
            }
            if (!moved) break;
        }

        // Computer (minimax) should not lose
        Assert.True(form.Engine.OWins > 0 || form.Engine.Draws > 0);
    }

    [StaFact]
    public void PlayFullGame_PlayerCornerStart_ComputerDoesNotLose()
    {
        using var form = new Form1(CreateMockRepo());

        form.SimulateCellClick(0, 0); // corner

        while (!form.Engine.IsGameOver)
        {
            bool moved = false;
            for (int r = 0; r < GameEngine.BoardSize && !moved; r++)
            {
                for (int c = 0; c < GameEngine.BoardSize && !moved; c++)
                {
                    if (string.IsNullOrEmpty(form.GetCell(r, c).Text))
                    {
                        form.SimulateCellClick(r, c);
                        moved = true;
                    }
                }
            }
            if (!moved) break;
        }

        Assert.True(form.Engine.OWins > 0 || form.Engine.Draws > 0);
    }

    [StaFact]
    public void PlayFullGame_PlayerCenterStart_ComputerDoesNotLose()
    {
        using var form = new Form1(CreateMockRepo());

        form.SimulateCellClick(1, 1); // center

        while (!form.Engine.IsGameOver)
        {
            bool moved = false;
            for (int r = 0; r < GameEngine.BoardSize && !moved; r++)
            {
                for (int c = 0; c < GameEngine.BoardSize && !moved; c++)
                {
                    if (string.IsNullOrEmpty(form.GetCell(r, c).Text))
                    {
                        form.SimulateCellClick(r, c);
                        moved = true;
                    }
                }
            }
            if (!moved) break;
        }

        Assert.True(form.Engine.OWins > 0 || form.Engine.Draws > 0);
    }

    #endregion

    #region Score and Database

    [StaFact]
    public void PlayGame_GameEnds_SavesResultToRepository()
    {
        var mockRepo = CreateMockRepo();
        using var form = new Form1(mockRepo);

        // Play through a full game
        while (!form.Engine.IsGameOver)
        {
            bool moved = false;
            for (int r = 0; r < GameEngine.BoardSize && !moved; r++)
            {
                for (int c = 0; c < GameEngine.BoardSize && !moved; c++)
                {
                    if (string.IsNullOrEmpty(form.GetCell(r, c).Text))
                    {
                        form.SimulateCellClick(r, c);
                        moved = true;
                    }
                }
            }
            if (!moved) break;
        }

        Assert.Single(mockRepo.SavedResults);
    }

    [StaFact]
    public void PlayGame_GameEnds_ScoreLabelUpdates()
    {
        var mockRepo = CreateMockRepo();
        using var form = new Form1(mockRepo);

        // Play through a full game
        while (!form.Engine.IsGameOver)
        {
            bool moved = false;
            for (int r = 0; r < GameEngine.BoardSize && !moved; r++)
            {
                for (int c = 0; c < GameEngine.BoardSize && !moved; c++)
                {
                    if (string.IsNullOrEmpty(form.GetCell(r, c).Text))
                    {
                        form.SimulateCellClick(r, c);
                        moved = true;
                    }
                }
            }
            if (!moved) break;
        }

        // Score label should have been updated
        Assert.DoesNotContain("あなた: 0  PC: 0  引き分け: 0", form.ScoreLabel.Text);
    }

    [StaFact]
    public void PlayMultipleGames_ScoreAccumulates()
    {
        var mockRepo = CreateMockRepo();
        using var form = new Form1(mockRepo);

        // Play first game
        while (!form.Engine.IsGameOver)
        {
            bool moved = false;
            for (int r = 0; r < GameEngine.BoardSize && !moved; r++)
                for (int c = 0; c < GameEngine.BoardSize && !moved; c++)
                    if (string.IsNullOrEmpty(form.GetCell(r, c).Text))
                    {
                        form.SimulateCellClick(r, c);
                        moved = true;
                    }
            if (!moved) break;
        }

        form.SimulateResetClick();

        // Play second game
        while (!form.Engine.IsGameOver)
        {
            bool moved = false;
            for (int r = GameEngine.BoardSize - 1; r >= 0 && !moved; r--)
                for (int c = GameEngine.BoardSize - 1; c >= 0 && !moved; c--)
                    if (string.IsNullOrEmpty(form.GetCell(r, c).Text))
                    {
                        form.SimulateCellClick(r, c);
                        moved = true;
                    }
            if (!moved) break;
        }

        Assert.Equal(2, mockRepo.SavedResults.Count);
    }

    #endregion

    #region Reset

    [StaFact]
    public void Reset_AfterGame_ClearsAllCells()
    {
        using var form = new Form1(CreateMockRepo());

        form.SimulateCellClick(0, 0);

        form.SimulateResetClick();

        for (int r = 0; r < GameEngine.BoardSize; r++)
            for (int c = 0; c < GameEngine.BoardSize; c++)
                Assert.Equal("", form.GetCell(r, c).Text);
    }

    [StaFact]
    public void Reset_AfterGame_StatusShowsPlayerTurn()
    {
        using var form = new Form1(CreateMockRepo());

        form.SimulateCellClick(0, 0);
        form.SimulateResetClick();

        Assert.Equal("あなたの番です (X)", form.StatusLabel.Text);
    }

    [StaFact]
    public void Reset_AfterGame_CellsHaveWhiteBackground()
    {
        using var form = new Form1(CreateMockRepo());

        form.SimulateCellClick(0, 0);
        form.SimulateResetClick();

        for (int r = 0; r < GameEngine.BoardSize; r++)
            for (int c = 0; c < GameEngine.BoardSize; c++)
                Assert.Equal(Color.White, form.GetCell(r, c).BackColor);
    }

    [StaFact]
    public void Reset_AllowsNewGame()
    {
        using var form = new Form1(CreateMockRepo());

        form.SimulateCellClick(1, 1);
        form.SimulateResetClick();
        form.SimulateCellClick(0, 0);

        Assert.Equal("X", form.GetCell(0, 0).Text);
    }

    #endregion

    #region Win Highlighting

    [StaFact]
    public void GameOver_WinHighlighting_ShowsGreenCells()
    {
        var mockRepo = CreateMockRepo();
        using var form = new Form1(mockRepo);

        // Play a full game
        while (!form.Engine.IsGameOver)
        {
            bool moved = false;
            for (int r = 0; r < GameEngine.BoardSize && !moved; r++)
                for (int c = 0; c < GameEngine.BoardSize && !moved; c++)
                    if (string.IsNullOrEmpty(form.GetCell(r, c).Text))
                    {
                        form.SimulateCellClick(r, c);
                        moved = true;
                    }
            if (!moved) break;
        }

        if (form.Engine.XWins > 0 || form.Engine.OWins > 0)
        {
            // At least one cell should be highlighted
            bool hasHighlight = false;
            for (int r = 0; r < GameEngine.BoardSize; r++)
                for (int c = 0; c < GameEngine.BoardSize; c++)
                    if (form.GetCell(r, c).BackColor == Color.LightGreen)
                        hasHighlight = true;
            Assert.True(hasHighlight);
        }
    }

    #endregion

    #region Status Messages

    [StaFact]
    public void GameOver_ShowsAppropriateMessage()
    {
        using var form = new Form1(CreateMockRepo());

        while (!form.Engine.IsGameOver)
        {
            bool moved = false;
            for (int r = 0; r < GameEngine.BoardSize && !moved; r++)
                for (int c = 0; c < GameEngine.BoardSize && !moved; c++)
                    if (string.IsNullOrEmpty(form.GetCell(r, c).Text))
                    {
                        form.SimulateCellClick(r, c);
                        moved = true;
                    }
            if (!moved) break;
        }

        string status = form.StatusLabel.Text;
        Assert.True(
            status == "あなたの勝ち！" || status == "PCの勝ち！" || status == "引き分け！",
            $"Unexpected status: {status}");
    }

    [StaFact]
    public void GameOver_ClicksAreIgnored()
    {
        using var form = new Form1(CreateMockRepo());

        while (!form.Engine.IsGameOver)
        {
            bool moved = false;
            for (int r = 0; r < GameEngine.BoardSize && !moved; r++)
                for (int c = 0; c < GameEngine.BoardSize && !moved; c++)
                    if (string.IsNullOrEmpty(form.GetCell(r, c).Text))
                    {
                        form.SimulateCellClick(r, c);
                        moved = true;
                    }
            if (!moved) break;
        }

        string statusAfterGame = form.StatusLabel.Text;

        // Try clicking after game over
        form.SimulateCellClick(0, 0);

        Assert.Equal(statusAfterGame, form.StatusLabel.Text);
    }

    #endregion
}
