namespace TicTacToe;

public class ComputerPlayer
{
    private readonly GameEngine _engine;

    public ComputerPlayer(GameEngine engine)
    {
        _engine = engine;
    }

    public (int Row, int Col) GetBestMove()
    {
        int bestScore = int.MinValue;
        int bestRow = -1;
        int bestCol = -1;

        string[,] board = GetBoardSnapshot();

        for (int row = 0; row < GameEngine.BoardSize; row++)
        {
            for (int col = 0; col < GameEngine.BoardSize; col++)
            {
                if (string.IsNullOrEmpty(board[row, col]))
                {
                    board[row, col] = "O";
                    int score = Minimax(board, 0, false);
                    board[row, col] = "";

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestRow = row;
                        bestCol = col;
                    }
                }
            }
        }

        return (bestRow, bestCol);
    }

    private int Minimax(string[,] board, int depth, bool isMaximizing)
    {
        if (CheckWin(board, "O")) return 10 - depth;
        if (CheckWin(board, "X")) return depth - 10;
        if (IsFull(board)) return 0;

        if (isMaximizing)
        {
            int best = int.MinValue;
            for (int row = 0; row < GameEngine.BoardSize; row++)
            {
                for (int col = 0; col < GameEngine.BoardSize; col++)
                {
                    if (string.IsNullOrEmpty(board[row, col]))
                    {
                        board[row, col] = "O";
                        best = Math.Max(best, Minimax(board, depth + 1, false));
                        board[row, col] = "";
                    }
                }
            }
            return best;
        }
        else
        {
            int best = int.MaxValue;
            for (int row = 0; row < GameEngine.BoardSize; row++)
            {
                for (int col = 0; col < GameEngine.BoardSize; col++)
                {
                    if (string.IsNullOrEmpty(board[row, col]))
                    {
                        board[row, col] = "X";
                        best = Math.Min(best, Minimax(board, depth + 1, true));
                        board[row, col] = "";
                    }
                }
            }
            return best;
        }
    }

    private string[,] GetBoardSnapshot()
    {
        var board = new string[GameEngine.BoardSize, GameEngine.BoardSize];
        for (int row = 0; row < GameEngine.BoardSize; row++)
            for (int col = 0; col < GameEngine.BoardSize; col++)
                board[row, col] = _engine.GetCell(row, col);
        return board;
    }

    private static bool CheckWin(string[,] board, string mark)
    {
        for (int i = 0; i < GameEngine.BoardSize; i++)
        {
            if (board[i, 0] == mark && board[i, 1] == mark && board[i, 2] == mark)
                return true;
            if (board[0, i] == mark && board[1, i] == mark && board[2, i] == mark)
                return true;
        }

        if (board[0, 0] == mark && board[1, 1] == mark && board[2, 2] == mark)
            return true;
        if (board[0, 2] == mark && board[1, 1] == mark && board[2, 0] == mark)
            return true;

        return false;
    }

    private static bool IsFull(string[,] board)
    {
        for (int row = 0; row < GameEngine.BoardSize; row++)
            for (int col = 0; col < GameEngine.BoardSize; col++)
                if (string.IsNullOrEmpty(board[row, col]))
                    return false;
        return true;
    }
}
