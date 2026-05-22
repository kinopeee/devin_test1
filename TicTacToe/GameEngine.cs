namespace TicTacToe;

public enum MoveResult
{
    Success,
    Win,
    Draw,
    CellOccupied,
    GameAlreadyOver,
    InvalidPosition
}

public enum WinLineType
{
    Row,
    Column,
    DiagonalMain,
    DiagonalAnti
}

public record WinLine(WinLineType Type, int Index);

public class GameEngine
{
    public const int BoardSize = 3;
    private readonly string[,] _board = new string[BoardSize, BoardSize];

    public bool IsXTurn { get; private set; } = true;
    public bool IsGameOver { get; private set; }
    public int XWins { get; private set; }
    public int OWins { get; private set; }
    public int Draws { get; private set; }
    public int MoveCount { get; private set; }
    public string CurrentPlayerMark => IsXTurn ? "X" : "O";

    public GameEngine()
    {
        ClearBoard();
    }

    public string GetCell(int row, int col)
    {
        if (row < 0 || row >= BoardSize || col < 0 || col >= BoardSize)
            throw new ArgumentOutOfRangeException();
        return _board[row, col];
    }

    public MoveResult PlaceMark(int row, int col)
    {
        if (IsGameOver)
            return MoveResult.GameAlreadyOver;
        if (row < 0 || row >= BoardSize || col < 0 || col >= BoardSize)
            return MoveResult.InvalidPosition;
        if (!string.IsNullOrEmpty(_board[row, col]))
            return MoveResult.CellOccupied;

        _board[row, col] = CurrentPlayerMark;
        MoveCount++;

        if (CheckWinner())
        {
            IsGameOver = true;
            if (IsXTurn) XWins++; else OWins++;
            return MoveResult.Win;
        }

        if (IsBoardFull())
        {
            IsGameOver = true;
            Draws++;
            return MoveResult.Draw;
        }

        IsXTurn = !IsXTurn;
        return MoveResult.Success;
    }

    public bool CheckWinner()
    {
        string mark = CurrentPlayerMark;

        for (int i = 0; i < BoardSize; i++)
        {
            if (_board[i, 0] == mark && _board[i, 1] == mark && _board[i, 2] == mark)
                return true;
            if (_board[0, i] == mark && _board[1, i] == mark && _board[2, i] == mark)
                return true;
        }

        if (_board[0, 0] == mark && _board[1, 1] == mark && _board[2, 2] == mark)
            return true;
        if (_board[0, 2] == mark && _board[1, 1] == mark && _board[2, 0] == mark)
            return true;

        return false;
    }

    public bool IsBoardFull()
    {
        for (int row = 0; row < BoardSize; row++)
            for (int col = 0; col < BoardSize; col++)
                if (string.IsNullOrEmpty(_board[row, col]))
                    return false;
        return true;
    }

    public WinLine? GetWinLine()
    {
        string mark = CurrentPlayerMark;

        for (int i = 0; i < BoardSize; i++)
        {
            if (_board[i, 0] == mark && _board[i, 1] == mark && _board[i, 2] == mark)
                return new WinLine(WinLineType.Row, i);
            if (_board[0, i] == mark && _board[1, i] == mark && _board[2, i] == mark)
                return new WinLine(WinLineType.Column, i);
        }

        if (_board[0, 0] == mark && _board[1, 1] == mark && _board[2, 2] == mark)
            return new WinLine(WinLineType.DiagonalMain, 0);
        if (_board[0, 2] == mark && _board[1, 1] == mark && _board[2, 0] == mark)
            return new WinLine(WinLineType.DiagonalAnti, 0);

        return null;
    }

    public void Reset()
    {
        IsGameOver = false;
        IsXTurn = true;
        MoveCount = 0;
        ClearBoard();
    }

    private void ClearBoard()
    {
        for (int row = 0; row < BoardSize; row++)
            for (int col = 0; col < BoardSize; col++)
                _board[row, col] = "";
    }
}
