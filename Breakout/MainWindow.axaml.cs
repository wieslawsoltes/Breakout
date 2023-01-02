using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;

namespace Breakout
{
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

    public class GameBoard : Canvas
    {
        // The ball and the paddle
        private Ball ball;
        private Paddle paddle;
        
        // An array to store references to the bricks
        private Brick[,] bricks;

        // The timer that controls the game loop
        private DispatcherTimer mainTimer;

        // The popup control
        private Popup popup;
        
        // The speed at which the paddle moves (in pixels per second)
        private const double PaddleSpeed = 300;
        
        private DispatcherTimer timer;
        private bool isLeftKeyDown;
        private bool isRightKeyDown;
        
        public int NumRows { get; set; }
        public int NumColumns { get; set; }
        public int GapHeight { get; set; }
        
        public GameBoard()
        {
            NumRows = 5;
            NumColumns = 10;
            GapHeight = 2 * 30;
                
            // Initialize the ball and the paddle
            ball = new Ball();
            paddle = new Paddle();

            // Add the ball and the paddle to the game board
            this.Children.Add(ball);
            this.Children.Add(paddle);

            // Initialize the bricks array using the NumRows and NumColumns properties
            bricks = new Brick[NumRows, NumColumns];
            
            AddBricks();

            // Set the timer interval and start the timer
            mainTimer = new DispatcherTimer();
            mainTimer.Interval = TimeSpan.FromMilliseconds(16);
            mainTimer.Tick += OnMainTimerTick;
            mainTimer.Start();
            
            // Subscribe to the PointerMoved event
            // this.PointerMoved += OnPointerMoved;
            
            // Create a DispatcherTimer to update the position of the paddle continuously
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(16);
            timer.Tick += OnTimerTick;
            
            // Register the OnKeyDown and OnKeyUp methods as handlers for the KeyDown and KeyUp events
            this.KeyDown += OnKeyDown;
            this.KeyUp += OnKeyUp;
            
            // Initialize the popup control
            popup = new Popup
            {
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                IsOpen = false,
                Child = new TextBlock
                {
                    Text = "Game Over!",
                    FontSize = 20,
                    Foreground = Brushes.White
                }
            };
            this.Children.Add(popup);
        }
        private void AddBricks()
        {
            for (int i = 0; i < NumRows; i++)
            {
                for (int j = 0; j < NumColumns; j++)
                {
                    // Create a new brick
                    var brick = new Brick();

                    // Set the position of the brick
                    brick.SetPosition(j * brick.Width, i * brick.Height + GapHeight);

                    // Add the brick to the game board
                    this.Children.Add(brick);

                    // Store a reference to the brick in the bricks array
                    bricks[i, j] = brick;
                }
            }
        }
        private void OnTimerTick(object sender, EventArgs e)
        {
            // Get the current position of the paddle
            double x = Canvas.GetLeft(paddle);

            // Update the position of the paddle based on the state of the keys
            if (isLeftKeyDown)
            {
                x -= PaddleSpeed * 16 / 1000;
            }
            if (isRightKeyDown)
            {
                x += PaddleSpeed * 16 / 1000;
            }

            // Clamp the position of the paddle to the game board
            x = Math.Max(0, x);
            x = Math.Min(800 - paddle.Width, x);

            // Set the new position of the paddle
            Canvas.SetLeft(paddle, x);

            // Stop the DispatcherTimer if both keys are released
            if (!isLeftKeyDown && !isRightKeyDown)
            {
                timer.Stop();
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // Check if the left or right arrow key is being held down
            if (e.Key == Key.Left)
            {
                isLeftKeyDown = true;
            }
            else if (e.Key == Key.Right)
            {
                isRightKeyDown = true;
            }

            // Start the DispatcherTimer if it is not already running
            if (!timer.IsEnabled)
            {
                timer.Start();
            }
        }
        
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            // Check if the left or right arrow key is being released
            if (e.Key == Key.Left)
            {
                isLeftKeyDown = false;
            }
            else if (e.Key == Key.Right)
            {
                isRightKeyDown = false;
            }

            // Stop the DispatcherTimer if both keys are released
            if (!isLeftKeyDown && !isRightKeyDown)
            {
                timer.Stop();
            }
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            // Make the game board focusable
            this.Focusable = true;
            // Give keyboard focus to the game board
            this.Focus();
        }

        // The game loop
        private void OnMainTimerTick(object sender, EventArgs e)
        {
            // Update the ball position
            ball.Update();

            // Check for collisions with the paddle and the bricks
            CheckPaddleCollision();
            CheckBrickCollision();

            // Check if the game is over
            CheckGameOver();
        }

        // Check for collisions with the paddle
        private void CheckPaddleCollision()
        {
            // Get the bounding boxes of the ball and the paddle
            var ballRect = new Rect(Canvas.GetLeft(ball), Canvas.GetTop(ball), ball.Width, ball.Height);
            var paddleRect = new Rect(Canvas.GetLeft(paddle), Canvas.GetTop(paddle), paddle.Width, paddle.Height);

            // Check if the bounding boxes intersect
            if (ballRect.Intersects(paddleRect))
            {
                // Calculate the new velocity of the ball based on where it hits the paddle
                double ballCenter = Canvas.GetLeft(ball) + ball.Width / 2;
                double paddleCenter = Canvas.GetLeft(paddle) + paddle.Width / 2;
                double dx = ballCenter - paddleCenter;
                ball.VelocityX = dx / (paddle.Width / 2) * 200;
                ball.VelocityY = -ball.VelocityY;
            }
        }
        
        // Check for collisions with the bricks
        private void CheckBrickCollision()
        {
            // Get the bounding box for the ball
            var ballRect = new Rect(Canvas.GetLeft(ball), Canvas.GetTop(ball), ball.Width, ball.Height);

            // Check for collisions with each brick
            for (int i = 0; i < NumRows; i++)
            {
                for (int j = 0; j < NumColumns; j++)
                {
                    // Skip bricks that have already been removed
                    if (bricks[i, j] == null)
                    {
                        continue;
                    }

                    // Calculate the bounding box for the brick, taking the gap into account
                    // TODO: var brickRect = new Rect(Canvas.GetLeft(bricks[i, j]), Canvas.GetTop(bricks[i, j]) + GapHeight, bricks[i, j].Width, bricks[i, j].Height);
                    var brickRect = new Rect(Canvas.GetLeft(bricks[i, j]), Canvas.GetTop(bricks[i, j]), bricks[i, j].Width, bricks[i, j].Height);

                    // Check if the bounding boxes intersect
                    if (brickRect.Intersects(ballRect))
                    {
                        // Remove the brick from the game board
                        this.Children.Remove(bricks[i, j]);
                        bricks[i, j] = null;

                        // Update the velocity of the ball
                        ball.VelocityY = -ball.VelocityY;
                    }
                }
            }
        }

        // Check if the game is over
        private void CheckGameOver()
        {
            // Check if the ball has fallen off the bottom of the screen
            if (Canvas.GetTop(ball) > 600)
            {
                // Stop the game loop
                mainTimer.Stop();

                // Show the popup
                popup.IsOpen = true;
            }
        }
        
        // Handle the user's input
        private void OnPointerMoved(object sender, PointerEventArgs e)
        {
            // Get the pointer position relative to the game board
            var point = e.GetPosition(this);

            // Move the paddle to the pointer position
            paddle.Move(point.X - 50);
        }
        
    }
    
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
}