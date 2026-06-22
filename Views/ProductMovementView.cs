using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockShop.Views.Base;

namespace StockShop.Views;

internal class ProductMovementView : MainView
{

    public ProductMovementView() : base(ConsoleColor.DarkGreen, ConsoleColor.White)
    {
    }

    public override void Centralize(int initialColumn, int finalColumn, int line, string text)
    {
        int column = initialColumn + ((finalColumn - initialColumn - text.Length) / 2);
        base.WriteFrame(column, line, text);
    }

    public override string ToAsk(string question, int line, int initialColumn, int finalColumn)
    {
        string answer;
        this.ClearArea(initialColumn, line, finalColumn, line);
        base.WriteFrame(initialColumn, line, question);
        answer = Console.ReadLine();
        return answer.ToLower();
    }
}