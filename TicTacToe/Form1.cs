namespace TicTacToe;

public partial class Form1 : Form
{
    private const int BoardSize = GameEngine.BoardSize;
    private readonly Button[,] _cells = new Button[BoardSize, BoardSize];
    private readonly GameEngine _engine = new();

    internal GameEngine Engine => _engine;
    internal Button GetCell(int row, int col) => _cells[row, col];
    internal Label StatusLabel => _statusLabel;
    internal Label ScoreLabel => _scoreLabel;
    internal Button ResetBtn => _resetButton;

    internal void SimulateCellClick(int row, int col) =>
        Cell_Click(_cells[row, col], EventArgs.Empty);

    internal void SimulateResetClick() =>
        ResetButton_Click(_resetButton, EventArgs.Empty);

    public Form1()
    {
        InitializeComponent();
        CreateBoard();
        UpdateStatus();
    }

    private void CreateBoard()
    {
        int cellSize = 100;
        int startX = 20;
        int startY = 50;

        for (int row = 0; row < BoardSize; row++)
        {
            for (int col = 0; col < BoardSize; col++)
            {
                var button = new Button
                {
                    Width = cellSize,
                    Height = cellSize,
                    Left = startX + col * cellSize,
                    Top = startY + row * cellSize,
                    Font = new Font("Arial", 36, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.White,
                    Tag = new Point(row, col)
                };
                button.Click += Cell_Click;
                _cells[row, col] = button;
                Controls.Add(button);
            }
        }
    }

    private void Cell_Click(object? sender, EventArgs e)
    {
        if (sender is not Button button)
            return;

        var pos = (Point)button.Tag!;
        var result = _engine.PlaceMark(pos.X, pos.Y);

        if (result == MoveResult.CellOccupied || result == MoveResult.GameAlreadyOver || result == MoveResult.InvalidPosition)
            return;

        string placedMark = _engine.GetCell(pos.X, pos.Y);
        button.Text = placedMark;
        button.ForeColor = placedMark == "X" ? Color.DarkBlue : Color.DarkRed;

        switch (result)
        {
            case MoveResult.Win:
                UpdateScoreLabel();
                _statusLabel.Text = $"{placedMark} の勝ち！";
                HighlightWinningCells();
                break;
            case MoveResult.Draw:
                UpdateScoreLabel();
                _statusLabel.Text = "引き分け！";
                break;
            case MoveResult.Success:
                UpdateStatus();
                break;
        }
    }

    private void HighlightWinningCells()
    {
        var winLine = _engine.GetWinLine();
        if (winLine == null) return;

        switch (winLine.Type)
        {
            case WinLineType.Row:
                HighlightRow(winLine.Index);
                break;
            case WinLineType.Column:
                HighlightColumn(winLine.Index);
                break;
            case WinLineType.DiagonalMain:
                _cells[0, 0].BackColor = _cells[1, 1].BackColor = _cells[2, 2].BackColor = Color.LightGreen;
                break;
            case WinLineType.DiagonalAnti:
                _cells[0, 2].BackColor = _cells[1, 1].BackColor = _cells[2, 0].BackColor = Color.LightGreen;
                break;
        }
    }

    private void HighlightRow(int row)
    {
        for (int col = 0; col < BoardSize; col++)
            _cells[row, col].BackColor = Color.LightGreen;
    }

    private void HighlightColumn(int col)
    {
        for (int row = 0; row < BoardSize; row++)
            _cells[row, col].BackColor = Color.LightGreen;
    }

    private void UpdateStatus()
    {
        _statusLabel.Text = _engine.IsXTurn ? "X の番です" : "O の番です";
    }

    private void UpdateScoreLabel()
    {
        _scoreLabel.Text = $"X: {_engine.XWins}  O: {_engine.OWins}  引き分け: {_engine.Draws}";
    }

    private void ResetButton_Click(object? sender, EventArgs e)
    {
        _engine.Reset();

        for (int row = 0; row < BoardSize; row++)
        {
            for (int col = 0; col < BoardSize; col++)
            {
                _cells[row, col].Text = "";
                _cells[row, col].BackColor = Color.White;
            }
        }

        UpdateStatus();
    }
}
