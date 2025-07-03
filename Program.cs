using MonoSnake;

internal class Program
{
    private static void Main(string[] args)
    {
        using var game = new Game1();
        game.Run();
    }
}