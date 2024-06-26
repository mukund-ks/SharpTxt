namespace SharpTxt;

class Editor
{
    Buffer _buffer;
    Cursor _cursor;
    Stack<object> _history;
    string _filePath;

    public Editor(string filePath)
    {
        _filePath = filePath;
        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "New File, by SharpTxt.");
        }
        var lines = File.ReadAllLines(_filePath)
                        .Where(x => x != Environment.NewLine);

        _buffer = new Buffer(lines);
        _cursor = new Cursor();
        _history = new Stack<object>();
    }

    public void Run()
    {
        Console.Title = "SharpTxt";
        Console.TreatControlCAsInput = true;
        while (true)
        {
            Render();
            HandleInput();
        }
    }

    private void HandleInput()
    {
        var character = Console.ReadKey();

        try
        {
            if (character.Modifiers == ConsoleModifiers.Control &&
                  character.Key == ConsoleKey.Q)
            {
                ExitEditor();
            }
            else if (character.Modifiers == ConsoleModifiers.Control &&
                  character.Key == ConsoleKey.X)
            {
                SaveToFile();
            }
            else if (character.Key == ConsoleKey.UpArrow)
            {
                _cursor = _cursor.Up(_buffer);
            }
            else if (character.Key == ConsoleKey.DownArrow)
            {
                _cursor = _cursor.Down(_buffer);
            }
            else if (character.Key == ConsoleKey.LeftArrow)
            {
                _cursor = _cursor.Left(_buffer);
            }
            else if (character.Key == ConsoleKey.RightArrow)
            {
                _cursor = _cursor.Right(_buffer);
            }
            else if (character.Modifiers == ConsoleModifiers.Control &&
                character.Key == ConsoleKey.U)
            {
                RestoreSnapshot();
            }
            else if (character.Key == ConsoleKey.Backspace)
            {
                if (_cursor.Col > 0)
                {
                    SaveSnapshot();
                    _buffer = _buffer.Delete(_cursor.Row, _cursor.Col - 1);
                    _cursor = _cursor.Left(_buffer);
                }
            }
            else if (character.Key == ConsoleKey.Enter)
            {
                SaveSnapshot();
                _buffer = _buffer.SplitLine(_cursor.Row, _cursor.Col);
                _cursor = _cursor.Down(_buffer).MoveToCol(0);
            }
            else if (IsTextChar(character))
            {
                SaveSnapshot();
                _buffer = _buffer.Insert(character.KeyChar.ToString(), _cursor.Row, _cursor.Col);
                _cursor = _cursor.Right(_buffer);
            }
        }
        catch (Exception ex)
        {
            Console.Clear();
            Console.WriteLine("An error occurred: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
    }

    private static bool IsTextChar(ConsoleKeyInfo character)
    {
        return !char.IsControl(character.KeyChar);
    }

    private void Render()
    {
        ConsoleOperations.ClearScreen();
        ConsoleOperations.MoveCursor(0, 0);
        _buffer.Render();
        ConsoleOperations.MoveCursor(_cursor.Row, _cursor.Col);
    }

    private void SaveToFile()
    {
        File.WriteAllLines(_filePath, _buffer.GetLines());
        ConsoleOperations.ClearScreen();
        Console.WriteLine("File saved successfully.");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(true);
    }

    private static void ExitEditor()
    {
        ConsoleOperations.ClearScreen();
        Console.WriteLine("SharpTxt Exited!");
        Environment.Exit(0);
    }

    private void SaveSnapshot()
    {
        _history.Push(_cursor);
        _history.Push(_buffer);
    }

    private void RestoreSnapshot()
    {
        if (_history.Count > 0)
        {
            _buffer = (Buffer)_history.Pop();
            _cursor = (Cursor)_history.Pop();
        }
    }
}