using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace Breakout;

public class Ball : Ellipse
{
    // The ball's velocity
    public double VelocityX { get; set; }
    public double VelocityY { get; set; }

    public Ball()
    {
        // Set the ball's appearance
        this.Width = 20;
        this.Height = 20;
        this.Fill = Brushes.White;

        // Set the ball's starting position
        Canvas.SetLeft(this, 380);
        Canvas.SetTop(this, 280);
            
        // Set the ball's velocity
        VelocityX = 200;
        VelocityY = -200;
        // TODO: Askk ChatGPT for better VelocityY so it does not sometimes hit a brick when game starts.
    }

    public void Update()
    {
        // Get the ball's current position
        double x = Canvas.GetLeft(this);
        double y = Canvas.GetTop(this);

        // Update the ball's position based on its velocity
        x += VelocityX * 16 / 1000;
        y += VelocityY * 16 / 1000;

        // Check if the ball has collided with a wall
        if (x < 0 || x + 20 > 800)
        {
            VelocityX = -VelocityX;
        }
        if (y < 0)
        {
            VelocityY = -VelocityY;
        }

        // Set the ball's new position
        Canvas.SetLeft(this, x);
        Canvas.SetTop(this, y);
    }
}