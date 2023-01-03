using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace Breakout;

public class Brick : Rectangle
{
    public Brick()
    {
        // Set the brick's appearance
        this.Width = 80;
        this.Height = 30;
        this.Fill = Brushes.Red;
    }

    public void SetPosition(double x, double y)
    {
        Canvas.SetLeft(this, x);
        Canvas.SetTop(this, y);
    }
}