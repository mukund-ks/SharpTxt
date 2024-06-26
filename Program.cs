using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp;
class Program {
    static void Main(string[] args) {
        Func<string> GetFilePath = () => {
            Console.WriteLine("Path to txt file: ");
            string? path = Console.ReadLine();
            return String.IsNullOrEmpty(path) ? "untitled.txt" : path;
        };

        string filePath = args.Length > 0 ? args[0] : GetFilePath();

        if (!filePath.Split('.')[^1].Equals("txt", StringComparison.OrdinalIgnoreCase)) {
            ConsoleOperations.ClearScreen();
            throw new ArgumentException("Provided Path to txt file is not valid");
        }
        new Editor(filePath).Run();
    }
}

class Editor {
    Buffer _buffer;
    Cursor _cursor;
    Stack<object> _history;
    string _filePath;

    public Editor(string filePath) {
        _filePath = filePath;
        if (!File.Exists(_filePath)) {
            File.WriteAllText(_filePath, "New File, by SharpTxt.");
        }
        var lines = File.ReadAllLines(_filePath)
                        .Where(x => x != Environment.NewLine);

        _buffer = new Buffer(lines);
        _cursor = new Cursor();
        _history = new Stack<object>();
    }

    public void Run() {
        Console.Title = "SharpTxt";
        Console.TreatControlCAsInput = true;
        while (true) {
            Render();
            HandleInput();
        }
    }

    private void HandleInput() {
        var character = Console.ReadKey();

        try {
            if (character.Modifiers == ConsoleModifiers.Control &&
                  character.Key == ConsoleKey.Q) {
                ExitEditor();
            } else if (character.Modifiers == ConsoleModifiers.Control &&
                  character.Key == ConsoleKey.X) {
                SaveToFile();
            } else if (character.Key == ConsoleKey.UpArrow) {
                _cursor = _cursor.Up(_buffer);
            } else if (character.Key == ConsoleKey.DownArrow) {
                _cursor = _cursor.Down(_buffer);
            } else if (character.Key == ConsoleKey.LeftArrow) {
                _cursor = _cursor.Left(_buffer);
            } else if (character.Key == ConsoleKey.RightArrow) {
                _cursor = _cursor.Right(_buffer);
            } else if (character.Modifiers == ConsoleModifiers.Control &&
                character.Key == ConsoleKey.U) {
                RestoreSnapshot();
            } else if (character.Key == ConsoleKey.Backspace) {
                if (_cursor.Col > 0) {
                    SaveSnapshot();
                    _buffer = _buffer.Delete(_cursor.Row, _cursor.Col - 1);
                    _cursor = _cursor.Left(_buffer);
                }
            } else if (character.Key == ConsoleKey.Enter) {
                SaveSnapshot();
                _buffer = _buffer.SplitLine(_cursor.Row, _cursor.Col);
                _cursor = _cursor.Down(_buffer).MoveToCol(0);
            } else if (IsTextChar(character)) {
                SaveSnapshot();
                _buffer = _buffer.Insert(character.KeyChar.ToString(), _cursor.Row, _cursor.Col);
                _cursor = _cursor.Right(_buffer);
            }
        } catch (Exception ex) {
            Console.Clear();
            Console.WriteLine("An error occurred: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
    }

    private static bool IsTextChar(ConsoleKeyInfo character) {
        return !Char.IsControl(character.KeyChar);
    }

    private void Render() {
        ConsoleOperations.ClearScreen();
        ConsoleOperations.MoveCursor(0, 0);
        _buffer.Render();
        ConsoleOperations.MoveCursor(_cursor.Row, _cursor.Col);
    }

    private void SaveToFile() {
        File.WriteAllLines(_filePath, _buffer.GetLines());
        ConsoleOperations.ClearScreen();
        Console.WriteLine("File saved successfully.");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(true);
    }

    private static void ExitEditor() {
        ConsoleOperations.ClearScreen();
        Console.WriteLine("SharpTxt Exited!");
        Environment.Exit(0);
    }

    private void SaveSnapshot() {
        _history.Push(_cursor);
        _history.Push(_buffer);
    }

    private void RestoreSnapshot() {
        if (_history.Count > 0) {
            _buffer = (Buffer)_history.Pop();
            _cursor = (Cursor)_history.Pop();
        }
    }
}

class Buffer(IEnumerable<string> lines) {
    string[] _lines = lines.ToArray();

    public void Render() {
        foreach (var line in _lines) {
            Console.WriteLine(line);
        }
        Console.WriteLine($"{new String('-', 5)}SharpTxt{new String('-', 5)}\n" +
              "C-x: Save\tC-q: Exit\t" +
              "C-u: Undo\t");
    }

    public int LineCount() {
        return _lines.Count();
    }

    public int LineLength(int row) {
        return _lines[row].Length;
    }

    public string[] GetLines() { return _lines; }

    internal Buffer Insert(string character, int row, int col) {
        var linesDeepCopy = _lines.Select(x => x).ToArray();
        linesDeepCopy[row] = linesDeepCopy[row].Insert(col, character);
        return new Buffer(linesDeepCopy);
    }

    internal Buffer Delete(int row, int col) {
        var linesDeepCopy = _lines.Select(x => x).ToArray();
        linesDeepCopy[row] = linesDeepCopy[row].Remove(col, 1);
        return new Buffer(linesDeepCopy);
    }

    internal Buffer SplitLine(int row, int col) {
        var linesDeepCopy = _lines.Select(x => x).ToList();

        var line = linesDeepCopy[row];

        var newLines = new[] { line.Substring(0, col), line.Substring(col, line.Length - line.Substring(0, col).Length) };

        linesDeepCopy[row] = newLines[0];
        linesDeepCopy.Insert(row + 1, newLines[1]);

        return new Buffer(linesDeepCopy);
    }
}

class Cursor(int row = 0, int col = 0) {
    public int Row { get; private set; } = row;
    public int Col { get; private set; } = col;

    internal Cursor Up(Buffer buffer) {
        return new Cursor(Row - 1, Col).SetCursorBound(buffer);
    }

    internal Cursor Down(Buffer buffer) {
        return new Cursor(Row + 1, Col).SetCursorBound(buffer);
    }


    internal Cursor Left(Buffer buffer) {
        return new Cursor(Row, Col - 1).SetCursorBound(buffer);
    }

    internal Cursor Right(Buffer buffer) {
        return new Cursor(Row, Col + 1).SetCursorBound(buffer);
    }

    private Cursor SetCursorBound(Buffer buffer) {
        Row = Math.Min(buffer.LineCount() - 1, Math.Max(Row, 0));
        Col = Math.Min(buffer.LineLength(Row), Math.Max(Col, 0));
        return new Cursor(Row, Col);
    }

    internal Cursor MoveToCol(int col) {
        return new Cursor(Row, col);
    }
}

class ConsoleOperations {
    public static void ClearScreen() {
        Console.Clear();
    }

    public static void MoveCursor(int row, int col) {
        Console.CursorTop = row;
        Console.CursorLeft = col;
    }
}
