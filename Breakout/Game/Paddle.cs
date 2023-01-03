using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace Breakout;

public class Paddle : Rectangle
{
    public Paddle()
    {
        // Set the paddle's appearance
        this.Width = 100;
        this.Height = 20;
        this.Fill = Brushes.White;

        // Set the paddle's starting position
        Canvas.SetLeft(this, 350);
        Canvas.SetTop(this, 560);
    }

    public void Move(double dx)
    {
        // Get the paddle's current position
        double x = Canvas.GetLeft(this);

        // Update the paddle's position
        x += dx;

        // Make sure the paddle doesn't go off the screen
        if (x < 0)
        {
            x = 0;
        }
        if (x + 100 > 800)
        {
            x = 700;
        }

        // Set the paddle's new position
        Canvas.SetLeft(this, x);
    }
}