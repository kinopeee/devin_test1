namespace TicTacToe;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;
    private Label _statusLabel = null!;
    private Label _scoreLabel = null!;
    private Button _resetButton = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(360, 440);
        this.Text = "\u4e09\u76ee\u4e26\u3079 - PC\u5bfe\u6226";
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.FromArgb(240, 240, 240);

        _statusLabel = new Label
        {
            Text = "",
            Font = new Font("Yu Gothic UI", 14, FontStyle.Bold),
            AutoSize = false,
            Width = 320,
            Height = 35,
            Left = 20,
            Top = 10,
            TextAlign = ContentAlignment.MiddleCenter
        };
        Controls.Add(_statusLabel);

        _resetButton = new Button
        {
            Text = "\u30ea\u30bb\u30c3\u30c8",
            Font = new Font("Yu Gothic UI", 11),
            Width = 120,
            Height = 35,
            Left = 20,
            Top = 370,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.LightSteelBlue
        };
        _resetButton.Click += ResetButton_Click;
        Controls.Add(_resetButton);

        _scoreLabel = new Label
        {
            Text = "X: 0  O: 0  \u5f15\u304d\u5206\u3051: 0",
            Font = new Font("Yu Gothic UI", 10),
            AutoSize = false,
            Width = 200,
            Height = 35,
            Left = 150,
            Top = 370,
            TextAlign = ContentAlignment.MiddleRight
        };
        Controls.Add(_scoreLabel);
    }

    #endregion
}
