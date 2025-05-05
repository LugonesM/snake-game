using System;
using System.Collections.Generic;
using System.Linq;

enum Direction
{
    Up,
    Down,
    Left,
    Right
}

interface IRenderable
{
    void Render();
}

readonly struct Position
{
    public Position(int top, int left)
    {
        Top = top;
        Left = left;
    }

    public int Top { get; }
    public int Left { get; }

    public Position RightBy(int n) => new Position(Top, Left + n);
    public Position DownBy(int n) => new Position(Top + n, Left);
}

class Apple : IRenderable
{
    public Apple(Position position)
    {
        Position = position;
    }

    public Position Position { get; }

    public void Render()
    {
        Console.SetCursorPosition(Position.Left, Position.Top);
        Console.Write("🍏");
    }
}

class Snake : IRenderable
{
    private List<Position> _body;
    private int _growthSpurtsRemaining;

    public Snake(Position spawnLocation, int initialSize = 1)
    {
        _body = new List<Position> { spawnLocation };
        _growthSpurtsRemaining = Math.Max(0, initialSize - 1);
        Dead = false;
    }

    public bool Dead { get; private set; }
    public Position Head => _body.First();
    public IEnumerable<Position> Body => _body.Skip(1);

    public void Move(Direction direction)
    {
        if (Dead) throw new InvalidOperationException();

        Position newHead = direction switch
        {
            Direction.Up => Head.DownBy(-1),
            Direction.Down => Head.DownBy(1),
            Direction.Left => Head.RightBy(-1),
            Direction.Right => Head.RightBy(1),
            _ => throw new ArgumentOutOfRangeException()
        };

        if (_body.Contains(newHead) || !PositionIsValid(newHead))
        {
            Dead = true;
            return;
        }

        _body.Insert(0, newHead);

        if (_growthSpurtsRemaining > 0)
            _growthSpurtsRemaining--;
        else
            _body.RemoveAt(_body.Count - 1);
    }

    public void Grow()
    {
        if (Dead) throw new InvalidOperationException();
        _growthSpurtsRemaining++;
    }

    public void Render()
    {
        Console.SetCursorPosition(Head.Left, Head.Top);
        Console.Write("◉");

        foreach (var pos in Body)
        {
            Console.SetCursorPosition(pos.Left, pos.Top);
            Console.Write("■");
        }
    }

    public bool BodyContains(Position pos) => _body.Contains(pos);

    private static bool PositionIsValid(Position position) =>
        position.Top > 0 && position.Top < SnakeGame.Rows - 1 &&
        position.Left > 0 && position.Left < SnakeGame.Columns - 1;
}
