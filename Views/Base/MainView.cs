using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockShop.Views.Base;

internal class MainView
{
    protected ConsoleColor _backgroundColor;
    protected ConsoleColor _textColor;

    public MainView(ConsoleColor backgroundColor, ConsoleColor textColor)

    {
        this._backgroundColor = backgroundColor;
        this._textColor = textColor;
    }

    public MainView() { }


    public virtual void PrepareMainView(string title, int initialColumn, int initialLine, int finalColumn, int finalLine)
    {
        try
        {
            Console.BackgroundColor = this._backgroundColor;
            Console.ForegroundColor = this._textColor;
            Console.Clear();
        }
        catch (System.IO.IOException) { }
        this.AssembleFrame(initialColumn, initialLine, finalColumn, finalLine);
        this.Centralize(initialColumn, finalColumn, initialLine + 1, title);
    }

    public virtual void Centralize(int initialColumn, int finalColumn, int line, string text)
    {
        int column = initialColumn + ((finalColumn - initialColumn - text.Length) / 2);
        WriteFrame(column, line, text);
    }

    public virtual string ToAsk(string question, int line, int initialColumn, int finalColumn)
    {
        string answer;
        this.ClearArea(initialColumn, line, finalColumn, line);
        WriteFrame(initialColumn, line, question);
        answer = Console.ReadLine();
        return answer.ToLower();
    }
    
    public virtual void ClearArea(int initialColumn, int initialLine, int finalColumn, int finalLine)
    {
        for (int x = initialColumn; x <= finalColumn; x++)
        {
            for (int y = initialLine; y <= finalLine; y++)
            {
                WriteFrame(x, y, " ");
            }
        }
    }

    public virtual void AssembleFrame(int initialColumn, int initialLine, int finalColumn, int finalLine)
    {
        int line, column;

        this.ClearArea(initialColumn, initialLine, finalColumn, finalLine);

        for (column = initialColumn; column <= finalColumn; column++)
        {
            WriteFrame(column, initialLine, "═");
            WriteFrame(column, finalLine, "═");
        }

        for (line = initialLine; line <= finalLine; line++)
        {
            WriteFrame(initialColumn, line, "║");

            WriteFrame(finalColumn, line, "║");
        }

        WriteFrame(initialColumn, initialLine, "╔");

        WriteFrame(finalColumn, initialLine, "╗");

        WriteFrame(initialColumn, finalLine, "╚");
        WriteFrame(finalColumn, finalLine, "╝");
    }
        
    public virtual void WriteFrame(int column, int line, string character)
    {
        try
        {
            Console.SetCursorPosition(column, line);
        }
        catch (System.IO.IOException) { }
        Console.Write(character);
    }

    public virtual string ShowMenu(int initialColumn, int initialLine, List<string> options)
    {
        string option;
        int x;
        int finalColumn = initialColumn + options[0].Length + 1;
        int finalLine = initialLine + options.Count() + 2;

        this.AssembleFrame(initialColumn, initialLine, finalColumn, finalLine);
        for (x = 0; x < options.Count(); x++)
        {
            WriteFrame(initialColumn + 1, initialLine + 1 + x, options[x]);
        }
        WriteFrame(initialColumn + 1, initialLine + 1 + x, "Opção : ");
        option = Console.ReadLine();

        return option;
    }
}
