namespace TicTacToe;

public partial class Form1 : Form
{
    private const int BoardSize = 3;
    private readonly Button[,] _cells = new Button[BoardSize, BoardSize];
    private bool _isXTurn = true;
    private bool _gameOver;
    private int _xWins;
    private int _oWins;
    private int _draws;

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
        if (_gameOver || sender is not Button button || !string.IsNullOrEmpty(button.Text))
            return;

        button.Text = _isXTurn ? "X" : "O";
        button.ForeColor = _isXTurn ? Color.DarkBlue : Color.DarkRed;

        if (CheckWinner())
        {
            _gameOver = true;
            string winner = _isXTurn ? "X" : "O";
            if (_isXTurn) _xWins++; else _oWins++;
            UpdateScoreLabel();
            _statusLabel.Text = $"{winner} \u306e\u52dd\u3061\uff01";
            HighlightWinningCells();
        }
        else if (IsBoardFull())
        {
            _gameOver = true;
            _draws++;
            UpdateScoreLabel();
            _statusLabel.Text = "\u5f15\u304d\u5206\u3051\uff01";
        }
        else
        {
            _isXTurn = !_isXTurn;
            UpdateStatus();
        }
    }

    private bool CheckWinner()
    {
        string mark = _isXTurn ? "X" : "O";

        for (int i = 0; i < BoardSize; i++)
        {
            if (_cells[i, 0].Text == mark && _cells[i, 1].Text == mark && _cells[i, 2].Text == mark)
                return true;
            if (_cells[0, i].Text == mark && _cells[1, i].Text == mark && _cells[2, i].Text == mark)
                return true;
        }

        if (_cells[0, 0].Text == mark && _cells[1, 1].Text == mark && _cells[2, 2].Text == mark)
            return true;
        if (_cells[0, 2].Text == mark && _cells[1, 1].Text == mark && _cells[2, 0].Text == mark)
            return true;

        return false;
    }

    private bool IsBoardFull()
    {
        for (int row = 0; row < BoardSize; row++)
        {
            for (int col = 0; col < BoardSize; col++)
            {
                if (string.IsNullOrEmpty(_cells[row, col].Text))
                    return false;
            }
        }
        return true;
    }

    private void HighlightWinningCells()
    {
        string mark = _isXTurn ? "X" : "O";

        for (int i = 0; i < BoardSize; i++)
        {
            if (_cells[i, 0].Text == mark && _cells[i, 1].Text == mark && _cells[i, 2].Text == mark)
            {
                HighlightRow(i);
                return;
            }
            if (_cells[0, i].Text == mark && _cells[1, i].Text == mark && _cells[2, i].Text == mark)
            {
                HighlightColumn(i);
                return;
            }
        }

        if (_cells[0, 0].Text == mark && _cells[1, 1].Text == mark && _cells[2, 2].Text == mark)
        {
            _cells[0, 0].BackColor = _cells[1, 1].BackColor = _cells[2, 2].BackColor = Color.LightGreen;
            return;
        }
        if (_cells[0, 2].Text == mark && _cells[1, 1].Text == mark && _cells[2, 0].Text == mark)
        {
            _cells[0, 2].BackColor = _cells[1, 1].BackColor = _cells[2, 0].BackColor = Color.LightGreen;
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
        _statusLabel.Text = _isXTurn ? "X \u306e\u756a\u3067\u3059" : "O \u306e\u756a\u3067\u3059";
    }

    private void UpdateScoreLabel()
    {
        _scoreLabel.Text = $"X: {_xWins}  O: {_oWins}  \u5f15\u304d\u5206\u3051: {_draws}";
    }

    private void ResetButton_Click(object? sender, EventArgs e)
    {
        _gameOver = false;
        _isXTurn = true;

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
