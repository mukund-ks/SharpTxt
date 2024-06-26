namespace SharpTxt;

class Buffer(IEnumerable<string> lines)
{
    string[] _lines = lines.ToArray();

    public void Render()
    {
        foreach (var line in _lines)
        {
            Console.WriteLine(line);
        }
        Console.WriteLine($"{new string('-', 5)}SharpTxt{new string('-', 5)}\n" +
              "C-x: Save\tC-q: Exit\t" +
              "C-u: Undo\t");
    }

    public int LineCount()
    {
        return _lines.Count();
    }

    public int LineLength(int row)
    {
        return _lines[row].Length;
    }

    public string[] GetLines() { return _lines; }

    internal Buffer Insert(string character, int row, int col)
    {
        var linesDeepCopy = _lines.Select(x => x).ToArray();
        linesDeepCopy[row] = linesDeepCopy[row].Insert(col, character);
        return new Buffer(linesDeepCopy);
    }

    internal Buffer Delete(int row, int col)
    {
        var linesDeepCopy = _lines.Select(x => x).ToArray();
        linesDeepCopy[row] = linesDeepCopy[row].Remove(col, 1);
        return new Buffer(linesDeepCopy);
    }

    internal Buffer SplitLine(int row, int col)
    {
        var linesDeepCopy = _lines.Select(x => x).ToList();

        var line = linesDeepCopy[row];

        var newLines = new[] { line.Substring(0, col), line.Substring(col, line.Length - line.Substring(0, col).Length) };

        linesDeepCopy[row] = newLines[0];
        linesDeepCopy.Insert(row + 1, newLines[1]);

        return new Buffer(linesDeepCopy);
    }
}