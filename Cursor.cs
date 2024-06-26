namespace SharpTxt;

class Cursor(int row = 0, int col = 0)
{
    public int Row { get; private set; } = row;
    public int Col { get; private set; } = col;

    internal Cursor Up(Buffer buffer)
    {
        return new Cursor(Row - 1, Col).SetCursorBound(buffer);
    }

    internal Cursor Down(Buffer buffer)
    {
        return new Cursor(Row + 1, Col).SetCursorBound(buffer);
    }


    internal Cursor Left(Buffer buffer)
    {
        return new Cursor(Row, Col - 1).SetCursorBound(buffer);
    }

    internal Cursor Right(Buffer buffer)
    {
        return new Cursor(Row, Col + 1).SetCursorBound(buffer);
    }

    private Cursor SetCursorBound(Buffer buffer)
    {
        Row = Math.Min(buffer.LineCount() - 1, Math.Max(Row, 0));
        Col = Math.Min(buffer.LineLength(Row), Math.Max(Col, 0));
        return new Cursor(Row, Col);
    }

    internal Cursor MoveToCol(int col)
    {
        return new Cursor(Row, col);
    }
}