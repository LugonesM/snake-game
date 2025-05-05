using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    public static async Task Main(string[] args)
    {
        Console.Clear();
        Console.CursorVisible = false;

        Console.WriteLine("Welcome to Snake!");
        Console.WriteLine("Use W A S D to move.");
        Console.WriteLine("Press any key to start...");
        Console.ReadKey(true);

        var tickRate = TimeSpan.FromMilliseconds(100);
        var snakeGame = new SnakeGame();

        using (var cts = new CancellationTokenSource())
        {
            async Task MonitorKeyPresses()
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(intercept: true).Key;
                        snakeGame.OnKeyPress(key);
                    }
                    await Task.Delay(10);
                }
            }

            var monitorKeyPresses = MonitorKeyPresses();

            do
            {
                snakeGame.OnGameTick();
                snakeGame.Render();
                await Task.Delay(tickRate);
            } while (!snakeGame.GameOver);

            cts.Cancel();
            await monitorKeyPresses;

            Console.Clear();
            Console.SetCursorPosition(SnakeGame.Columns / 2 - 5, SnakeGame.Rows / 2);
            Console.Write("GAME OVER");
            Console.SetCursorPosition(SnakeGame.Columns / 2 - 9, SnakeGame.Rows / 2 + 1);
            Console.Write("Press any key to exit...");
            Console.ReadKey(true);
        }
    }
}
