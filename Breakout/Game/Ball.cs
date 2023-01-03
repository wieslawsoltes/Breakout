using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace Breakout;

public class Ball : Ellipse
{
    private static Random random = new Random();

    public double VelocityX { get; set; }
    public double VelocityY { get; set; }

    public Ball()
    {
        this.Width = 20;
        this.Height = 20;
        this.Fill = Brushes.White;
    }

    public void Start()
    {
        // Set the ball's starting position
        Canvas.SetLeft(this, 400 - Width / 2);
        Canvas.SetTop(this, 300 - Height / 2);
        
        // Set the ball's initial velocity
        VelocityX = 200;
        VelocityY = -200;

        // Choose a random initial horizontal direction
        if (random.NextDouble() < 0.5)
        {
            VelocityX = -VelocityX;
        }
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