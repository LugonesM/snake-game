using System;

class SnakeGame : IRenderable
{
    public static readonly int Rows = 25;
    public static readonly int Columns = 50;

    private Direction _currentDirection;
    private Direction _nextDirection;
    private Snake _snake;
    private Apple _apple;

    public SnakeGame()
    {
        Console.SetWindowSize(Columns + 2, Rows + 2);
        Console.SetBufferSize(Columns + 2, Rows + 2);
        Console.CursorVisible = false;

        var start = new Position(Rows / 2, Columns / 2);
        _snake = new Snake(start, initialSize: 5);
        _apple = CreateApple();
        _currentDirection = Direction.Right;
        _nextDirection = Direction.Right;
    }

    public bool GameOver => _snake.Dead;

    public void OnKeyPress(ConsoleKey key)
    {
        Direction newDirection = key switch
        {
            ConsoleKey.W => Direction.Up,
            ConsoleKey.S => Direction.Down,
            ConsoleKey.A => Direction.Left,
            ConsoleKey.D => Direction.Right,
            _ => _nextDirection
        };

        if (newDirection != OppositeDirectionTo(_currentDirection))
            _nextDirection = newDirection;
    }

    public void OnGameTick()
    {
        if (GameOver) throw new InvalidOperationException();

        _currentDirection = _nextDirection;
        _snake.Move(_currentDirection);

        if (_snake.Head.Equals(_apple.Position))
        {
            _snake.Grow();
            _apple = CreateApple();
        }
    }

    public void Render()
    {
        Console.Clear();

        // Draw border
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                if (row == 0 || row == Rows - 1 || col == 0 || col == Columns - 1)
                {
                    Console.SetCursorPosition(col, row);
                    Console.Write("#");
                }
            }
        }

        _apple.Render();
        _snake.Render();
        Console.SetCursorPosition(0, Rows);
    }

    private static Direction OppositeDirectionTo(Direction direction) => direction switch
    {
        Direction.Up => Direction.Down,
        Direction.Down => Direction.Up,
        Direction.Left => Direction.Right,
        Direction.Right => Direction.Left,
        _ => throw new ArgumentOutOfRangeException()
    };

    private Apple CreateApple()
    {
        var random = new Random();
        Position pos;

        do
        {
            var top = random.Next(1, Rows - 1);
            var left = random.Next(1, Columns - 1);
            pos = new Position(top, left);
        } while (_snake.Head.Equals(pos) || _snake.BodyContains(pos));

        return new Apple(pos);
    }
}
