using Avalonia.Controls;

namespace Breakout;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
            
        // Set the window size and title
        this.Width = 800;
        this.Height = 600;
        this.Title = "Atari Breakout";

        // Create the game board and add it to the window
        var gameBoard = new GameBoard();
        this.Content = gameBoard;
    }
}