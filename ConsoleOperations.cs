namespace SharpTxt;

class ConsoleOperations
{
    public static void ClearScreen()
    {
        Console.Clear();
    }

    public static void MoveCursor(int row, int col)
    {
        Console.CursorTop = row;
        Console.CursorLeft = col;
    }
}