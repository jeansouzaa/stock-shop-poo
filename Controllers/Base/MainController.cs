using System;
using System.Collections.Generic;
using StockShop.Views.Base;

namespace StockShop.Controllers.Base;

internal abstract class MainController
{
    protected int _column;
    protected int _row;
    protected int _width;
    protected int _heigth;
    protected int _position;
    protected MainView _screen;
    protected List<string> _fields;

    protected MainController(int column, int row, MainView screen)
    {
        _column = column;
        _row = row;
        _screen = screen;
        _fields = new List<string>();
    }

    protected MainController()
    {
        _screen = new MainView();
        _fields = new List<string>();
    }

    protected virtual void ShowForm(string title)
    {
        this._screen.AssembleFrame(_column, _row, _column + _width, _row + _heigth);

        int row = _row + 1;
        this._screen.Centralize(_column, _column + _width, row, title);

        row++;
        for (int i = 0; i < _fields.Count; i++)
        {
            _screen.WriteFrame(_column + 1, row, _fields[i]);
            row++;
        }
    }

    protected int ReadInt(int column, int row, string errorMessage)
    {
        int value;
        while (true)
        {
            try
            {
                Console.SetCursorPosition(column, row);
            }
            catch (System.IO.IOException) { }
            string input = Console.ReadLine() ?? "";
            if (int.TryParse(input, out value))
            {
                int cleanupLine = this._row + this._heigth + 1;
                this._screen.ClearArea(this._column, cleanupLine, this._column + this._width + 25, cleanupLine);
                break;
            }
            int errorLine = this._row + this._heigth + 1;
            this._screen.ClearArea(this._column, errorLine, this._column + this._width + 25, errorLine);
            this._screen.Centralize(this._column, this._column + this._width, errorLine, errorMessage);
            this._screen.ClearArea(column, row, this._column + this._width - 2, row);
        }
        return value;
    }

    protected double ReadDouble(int column, int row, string errorMessage)
    {
        double value;
        while (true)
        {
            try
            {
                Console.SetCursorPosition(column, row);
            }
            catch (System.IO.IOException) { }
            string input = Console.ReadLine() ?? "";
            if (double.TryParse(input, out value))
            {
                int cleanupLine = this._row + this._heigth + 1;
                this._screen.ClearArea(this._column, cleanupLine, this._column + this._width + 25, cleanupLine);
                break;
            }
            int errorLine = this._row + this._heigth + 1;
            this._screen.ClearArea(this._column, errorLine, this._column + this._width + 25, errorLine);
            this._screen.Centralize(this._column, this._column + this._width, errorLine, errorMessage);
            this._screen.ClearArea(column, row, this._column + this._width - 2, row);
        }
        return value;
    }

    protected DateTime ReadDateTime(int column, int row, string errorMessage)
    {
        DateTime value;
        while (true)
        {
            try
            {
                Console.SetCursorPosition(column, row);
            }
            catch (System.IO.IOException) { }
            string input = Console.ReadLine() ?? "";
            if (DateTime.TryParse(input, out value))
            {
                int cleanupLine = this._row + this._heigth + 1;
                this._screen.ClearArea(this._column, cleanupLine, this._column + this._width + 25, cleanupLine);
                break;
            }
            int errorLine = this._row + this._heigth + 1;
            this._screen.ClearArea(this._column, errorLine, this._column + this._width + 25, errorLine);
            this._screen.Centralize(this._column, this._column + this._width, errorLine, errorMessage);
            this._screen.ClearArea(column, row, this._column + this._width - 2, row);
        }
        return value;
    }

    public abstract void CRUD();

    public abstract void EnterData(string which);
}
