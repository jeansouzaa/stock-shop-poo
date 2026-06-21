using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockShop
{
    internal class Screen
    {
        private ConsoleColor _backgroundColor;
        private ConsoleColor _textColor;

        public Screen(ConsoleColor backgroundColor, ConsoleColor textColor)
        {
            this._backgroundColor = backgroundColor;
            this._textColor = textColor;
        }

        public Screen() { }

        public void PrepareScreen(string title, int initialColumn, int initialLine, int finalColumn, int finalLine)
        {
            Console.BackgroundColor = this._backgroundColor;
            Console.ForegroundColor = this._textColor;
            Console.Clear();
            this.AssembleFrame(initialColumn, initialLine, finalColumn, finalLine);
            this.Centralize(initialColumn, finalColumn, initialLine + 1, title);
        }

        public void Centralize(int initialColumn, int finalColumn, int line, string text)
        {
            int column = initialColumn + ((finalColumn - initialColumn - text.Length) / 2);
            WriteFrame(column, line, text);
        }

        public string ToAsk(string question, int line, int initialColumn, int finalColumn)
        {
            string answer;
            this.ClearArea(initialColumn, line, finalColumn, line);
            WriteFrame(initialColumn, line, question);
            answer = Console.ReadLine();
            return answer.ToLower();
        }
        
        public void ClearArea(int initialColumn, int initialLine, int finalColumn, int finalLine)
        {
            for (int x = initialColumn; x <= finalColumn; x++)
            {
                for (int y = initialLine; y <= finalLine; y++)
                {
                    WriteFrame(x, y, " ");
                }
            }
        }

        public void AssembleFrame(int initialColumn, int initialLine, int finalColumn, int finalLine)
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
            
        public static void WriteFrame(int column, int line, string character)
        {
            Console.SetCursorPosition(column, line);
            Console.Write(character);
        }

        public string ShowMenu(int initialColumn, int initialLine, List<string> options)
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
}
