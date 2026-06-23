using System;
using System.Collections.Generic;
using System.Text;
using StockShop.Models;
using StockShop.Views.Base;
using StockShop.Views;
using StockShop.Controllers.Base;

namespace StockShop.Controllers;

internal class SupplierController : MainController
{
    private const int CodeFieldRowOffset = 2;
    private const int CnpjFieldRowOffset = 3;
    private const int NameFieldRowOffset = 4;
    private const int PhoneFieldRowOffset = 5;

    private static List<SupplierModel> _suppliers = new List<SupplierModel>
    {
        new SupplierModel(1, "52.518.679/0001-09", "Art&Office", "(47)99999-9999", new List<ProductModel>())
    };
    private SupplierModel _model;
    private SupplierView _screen = new SupplierView();

    public SupplierController(int column, int row, SupplierView screen) : base(column, row, screen)
    {
        this._model = new SupplierModel();

        this._fields = new List<string>();
        this._fields.Add("Código               : ");
        this._fields.Add("CNPJ                 : ");
        this._fields.Add("Nome                 : ");
        this._fields.Add("Telefone             : ");

        this._width = this._fields[0].Length + 2 + 30;
        this._heigth = this._fields.Count + 2 + 1;
    }

    public SupplierController()
    {
    }

    public SupplierModel findSupplierByCNPJ(string cnpj)
    {
        SupplierModel supplier = null;
        for (int count = 0; count < _suppliers.Count; count++)
        {
            SupplierModel supplierInCache = _suppliers[count];
            if (supplierInCache != null && supplierInCache.Cnpj.Equals(cnpj))
            {
                supplier = supplierInCache;
            }
        }
        return supplier;
    }

    public SupplierModel findSupplierByCode(int code)
    {
        SupplierModel supplier = null;
        for (int count = 0; count < _suppliers.Count; count++)
        {
            SupplierModel supplierInCache = _suppliers[count];
            if (supplierInCache != null && supplierInCache.Code == code)
            {
                supplier = supplierInCache;
            }
        }
        return supplier;
    }

    public override void EnterData(string which)
    {
        if (which == "PK")
        {
            int column = this._column + 1 + this._fields[0].Length;
            int row = this._row + CodeFieldRowOffset;
            this._model.Code = ReadInt(column, row, "Código inválido! Digite um número inteiro.");
        }
        else
        {
            int column = this._column + 1 + this._fields[0].Length;
            int row = this._row + CnpjFieldRowOffset;

            this._screen.ClearArea(column, row, this._column + this._width - 2, row + this._heigth - 5);

            string inputCnpj;
            while (true)
            {
                try
                {
                    Console.SetCursorPosition(column, row);
                }
                catch (System.IO.IOException) 
                { }
                inputCnpj = Console.ReadLine() ?? "";

                SupplierModel existing = null;
                for (int count = 0; count < _suppliers.Count; count++)
                {
                    if (_suppliers[count].Cnpj.Equals(inputCnpj, StringComparison.OrdinalIgnoreCase) && _suppliers[count].Code != this._model.Code)
                    {
                        existing = _suppliers[count];
                        break;
                    }
                }

                if (existing == null)
                {
                    break;
                }

                int errorLine = this._row + this._heigth + 1;
                this._screen.ClearArea(this._column, errorLine, this._column + this._width + 25, errorLine);
                this._screen.Centralize(this._column, this._column + this._width, errorLine, "CNPJ já cadastrado para outro fornecedor!");
                this._screen.ClearArea(column, row, this._column + this._width - 2, row);
            }
            this._model.Cnpj = inputCnpj;
            int cleanupLine = this._row + this._heigth + 1;
            this._screen.ClearArea(this._column, cleanupLine, this._column + this._width + 25, cleanupLine);

            row++;
            try
            {
                Console.SetCursorPosition(column, row);
            }
            catch (System.IO.IOException) { }
            this._model.Name = Console.ReadLine();

            row++;
            try
            {
                Console.SetCursorPosition(column, row);
            }
            catch (System.IO.IOException) { }
            this._model.PhoneNumber = Console.ReadLine();
        }
    }

    public void ShowData()
    {
        int column = this._column + 1 + this._fields[0].Length;

        this._screen.WriteFrame(column, this._row + CnpjFieldRowOffset, _suppliers[this._position].Cnpj);
        this._screen.WriteFrame(column, this._row + NameFieldRowOffset, _suppliers[this._position].Name);
        this._screen.WriteFrame(column, this._row + PhoneFieldRowOffset, _suppliers[this._position].PhoneNumber);
    }

    public bool FindProduct()
    {
        bool found = false;
        for (int count = 0; count < _suppliers.Count; count++)
        {
            if (_suppliers[count].Code == this._model.Code)
            {
                this._position = count;
                found = true;
            }
        }
        return found;
    }

    public override void CRUD()
    {
        bool found;
        string answer;
        int initialColumn = this._column + 1;
        int finalColumn = this._column + this._width - 1;
        int line = this._row + this._heigth - 1;

        this.ShowForm("Cadastro de Fornecedores");
        this.EnterData("PK");
        found = this.FindProduct();
        if (found)
        {
            this.ShowData();
            answer = this._screen.ToAsk("Deseja alterar/excluir/voltar (A/E/V): ", line, initialColumn, finalColumn);

            if (answer == "a" || answer == "A")
            {
                this.EnterData("DT");
                answer = this._screen.ToAsk("Confirma alteração (S/N): ", line, initialColumn, finalColumn);
                if (answer == "s" || answer == "S")
                {
                    _suppliers[this._position].Cnpj = this._model.Cnpj;
                    _suppliers[this._position].Name = this._model.Name;
                    _suppliers[this._position].PhoneNumber = this._model.PhoneNumber;
                }
            }
            if (answer == "e" || answer == "E")
            {
                answer = this._screen.ToAsk("Confirma exclusão (S/N): ", line, initialColumn, finalColumn);
                if (answer == "s" || answer == "S")
                {
                    _suppliers.RemoveAt(this._position);
                }
            }
        }
        else
        {
            answer = this._screen.ToAsk("Fornecedor não encontrado. Deseja cadastrar (S/N): ",
                line, initialColumn, finalColumn);

            if (answer == "s" || answer == "S")
            {
                this.EnterData("DT");
                answer = this._screen.ToAsk("Confirma cadastro (S/N): ", line, initialColumn, finalColumn);
                if (answer == "s" || answer == "S")
                {
                    _suppliers.Add(
                        new SupplierModel(this._model.Code, this._model.Cnpj, this._model.Name, this._model.PhoneNumber)
                    );
                }
            }
        }
    }

    public SupplierModel findSupplierByName(string name)
    {
        SupplierModel supplier = null;
        for (int count = 0; count < _suppliers.Count; count++)
        {
            SupplierModel supplierInCache = _suppliers[count];
            if (supplierInCache != null && supplierInCache.Name.Equals(name))
            {
                supplier = supplierInCache;
            }
        }
        return supplier;
    }

    internal void ReportRegisteredSuppliers()
    {
        this._screen.PrepareMainView("Relatório de Fornecedores Cadastrados", 0, 0, 79, 24);
        
        int row = 3;
        this._screen.WriteFrame(2, row, "Código | CNPJ                 | Nome                | Telefone");
        this._screen.WriteFrame(2, row + 1, "------------------------------------------------------------------------------");
        row += 2;

        if (_suppliers.Count == 0)
        {
            this._screen.WriteFrame(2, row, "Nenhum fornecedor cadastrado.");
            row++;
        }
        else
        {
            foreach (var supplier in _suppliers)
            {
                if (row >= 22)
                {
                    this._screen.WriteFrame(2, row, "Pressione qualquer tecla para mostrar mais...");
                    try
                    {
                        Console.ReadKey();
                    }
                    catch (InvalidOperationException)
                    {
                        Console.ReadLine();
                    }
                    this._screen.PrepareMainView("Relatório de Fornecedores Cadastrados", 0, 0, 79, 24);
                    row = 3;
                    this._screen.WriteFrame(2, row, "Código | CNPJ                 | Nome                | Telefone");
                    this._screen.WriteFrame(2, row + 1, "------------------------------------------------------------------------------");
                    row += 2;
                }

                string line = $"{supplier.Code,-6} | {supplier.Cnpj,-20} | {supplier.Name,-19} | {supplier.PhoneNumber}";
                this._screen.WriteFrame(2, row, line);
                row++;
            }
        }
        
        this._screen.WriteFrame(2, row + 1, "Pressione qualquer tecla para voltar ao menu...");
        try
        {
            Console.ReadKey();
        }
        catch (InvalidOperationException)
        {
            Console.ReadLine();
        }
    }
}
