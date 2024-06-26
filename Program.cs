using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SharpTxt;
class Program
{
    static void Main(string[] args)
    {
        Func<string> GetFilePath = () =>
        {
            Console.WriteLine("Path to txt file: ");
            string? path = Console.ReadLine();
            return string.IsNullOrEmpty(path) ? "untitled.txt" : path;
        };

        string filePath = args.Length > 0 ? args[0] : GetFilePath();

        if (!filePath.Split('.')[^1].Equals("txt", StringComparison.OrdinalIgnoreCase))
        {
            ConsoleOperations.ClearScreen();
            throw new ArgumentException("Provided Path to txt file is not valid");
        }
        new Editor(filePath).Run();
    }
}
