using System;
using System.Collections.Generic;
using StockShop.Controllers.Base;
using StockShop.Models;
using StockShop.Models.Enums;
using StockShop.Views;
using StockShop.Views.Base;

namespace StockShop.Controllers;

internal class ProductController : MainController
{
    private const int CodeFieldRowOffset = 2;
    private const int DescriptionFieldRowOffset = 3;
    private const int CategoryFieldRowOffset = 6;
    private const int QtyStockFieldRowOffset = 7;
    private const int PriceFieldRowOffset = 8;
    private const int SupplierFieldRowOffset = 9;

    private ProductModel _model;
    private SupplierController _supplierController = new SupplierController(10, 5, new SupplierView());
    private static List<ProductModel> _products = new List<ProductModel>();

    public ProductController(int column, int row, ProductView screen) : base(column, row, screen)
    {
        this._model = new ProductModel();

        this._fields.Add("Código                                : ");
        this._fields.Add("Descrição                             : ");
        this._fields.Add("0-Escolar, 1-Escritório, 2-Presente,   ");
        this._fields.Add("3-Brinquedo, 4-Artesanato, 5-Papelaria ");
        this._fields.Add("Categoria                             : ");
        this._fields.Add("Quantidade em Estoque                 : ");
        this._fields.Add("Preço                                 : ");
        this._fields.Add("Código do Fornecedor                  : ");

        this._width = this._fields[0].Length + 2 + 30;
        this._heigth = this._fields.Count + 2 + 4;
    }

    public ProductController()
    {
    }

    protected override void ShowForm(string title)
    {
        _screen.AssembleFrame(_column, _row, _column + _width, _row + _heigth);

        int row = _row + 1;
        _screen.Centralize(_column, _column + _width, row, title);

        row++;
        for (int i = 0; i < _fields.Count; i++)
        {
            _screen.WriteFrame(_column + 1, row, _fields[i]);
            row++;
        }
    }

    public static ProductModel FindProductByCode(string code)
    {
        ProductModel product = null;
        int parsedCode;
        if (int.TryParse(code, out parsedCode))
        {
            for (int count = 0; count < _products.Count; count++)
            {
                ProductModel productInCache = _products[count];
                if (productInCache != null && productInCache.Code == parsedCode)
                {
                    product = productInCache;
                }
            }
        }
        return product;
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
            int row = this._row + DescriptionFieldRowOffset;

            this._screen.ClearArea(column, row, this._column + this._width - 2, row + this._heigth - 5);

            Console.SetCursorPosition(column, row);
            this._model.Description = Console.ReadLine();

            row = this._row + CategoryFieldRowOffset;
            Console.SetCursorPosition(column, row);

            ProductCategory category;
            while (true)
            {
                string input = Console.ReadLine() ?? "";
                int catIndex;
                if (int.TryParse(input, out catIndex) && Enum.IsDefined(typeof(ProductCategory), catIndex))
                {
                    category = (ProductCategory)catIndex;
                    break;
                }
                int errorLine = this._row + this._heigth + 1;
                this._screen.ClearArea(this._column, errorLine, this._column + this._width, errorLine);
                this._screen.Centralize(this._column, this._column + this._width, errorLine, "Categoria inválida! Digite de 0 a 5.");
                this._screen.ClearArea(column, row, this._column + this._width - 2, row);
                Console.SetCursorPosition(column, row);
            }

            int finalErrorLine = this._row + this._heigth + 1;
            this._screen.ClearArea(this._column, finalErrorLine, this._column + this._width, finalErrorLine);

            this._model.Category = category;

            row = this._row + QtyStockFieldRowOffset;
            this._model.QtyStock = ReadInt(column, row, "Quantidade inválida! Digite um número inteiro.");

            row = this._row + PriceFieldRowOffset;
            this._model.UnitaryPrice = ReadDouble(column, row, "Preço inválido! Digite um valor numérico decimal.");

            row = this._row + SupplierFieldRowOffset;
            Console.SetCursorPosition(column, row);

            SupplierModel supplier = null;
            while (true)
            {
                string input = Console.ReadLine() ?? "";
                int supplierCode;
                if (int.TryParse(input, out supplierCode))
                {
                    supplier = SupplierController.FindSupplierByCode(supplierCode);
                    if (supplier != null)
                    {
                        break;
                    }
                }

                int errorLine = this._row + this._heigth + 1;
                this._screen.ClearArea(this._column, errorLine, this._column + this._width + 25, errorLine + 2);
                this._screen.Centralize(this._column, this._column + this._width, errorLine, "Fornecedor não cadastrado!");
                this._screen.Centralize(this._column, this._column + this._width, errorLine + 1, "1-Cadastrar Novo | 2-Tentar Novamente | 3-Listar Fornecedores: ");

                string option = Console.ReadLine() ?? "";
                this._screen.ClearArea(this._column - 3, errorLine, this._column + this._width + 25, errorLine + 2);
                switch (option)
                {
                    case "1":
                        this._supplierController.CRUD();
                        this.RegisterProduct();
                        break;
                    case "3":
                        this._supplierController.ReportRegisteredSuppliers();
                        this.RegisterProduct();
                        break;
                    default:
                        break;
                }

                this._screen.ClearArea(this._column, errorLine, this._column + this._width + 25, errorLine + 2);
                this._screen.ClearArea(column, row, this._column + this._width - 2, row);
                Console.SetCursorPosition(column, row);
            }
            this._model.Supplier = supplier;
        }
    }

    private void RegisterProduct()
    {
        this.ShowForm("Cadastro de Produtos");
        int column = this._column + 1 + this._fields[0].Length;
        this._screen.WriteFrame(column, this._row + CodeFieldRowOffset, this._model.Code.ToString());
        this._screen.WriteFrame(column, this._row + DescriptionFieldRowOffset, this._model.Description);
        this._screen.WriteFrame(column, this._row + CategoryFieldRowOffset, ((int)this._model.Category).ToString());
        this._screen.WriteFrame(column, this._row + QtyStockFieldRowOffset, this._model.QtyStock.ToString());
        this._screen.WriteFrame(column, this._row + PriceFieldRowOffset, "R$" + this._model.UnitaryPrice.ToString());
    }

    public void ShowData()
    {
        int column = this._column + 1 + this._fields[0].Length;

        this._screen.WriteFrame(column, this._row + DescriptionFieldRowOffset, _products[this._position].Description);
        this._screen.WriteFrame(column, this._row + CategoryFieldRowOffset, _products[this._position].Category.ToString());
        this._screen.WriteFrame(column, this._row + QtyStockFieldRowOffset, _products[this._position].QtyStock.ToString());
        this._screen.WriteFrame(column, this._row + PriceFieldRowOffset, "R$" + _products[this._position].UnitaryPrice.ToString("F2"));

        string supplierCodeStr = _products[this._position].Supplier != null ? _products[this._position].Supplier.Code.ToString() : "N/A";
        this._screen.WriteFrame(column, this._row + SupplierFieldRowOffset, supplierCodeStr);
    }

    public bool FindProduct()
    {
        bool found = false;
        for (int count = 0; count < _products.Count; count++)
        {
            if (_products[count].Code == this._model.Code)
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
        string response;
        int startColumn = this._column + 1;
        int endColumn = this._column + this._width - 1;
        int line = this._row + this._heigth - 1;

        this.ShowForm("Cadastro de Produtos");
        this.EnterData("PK");
        found = this.FindProduct();
        if (found)
        {
            this.ShowData();
            response = this._screen.ToAsk("Deseja alterar/excluir/voltar (A/E/V): ", line, startColumn, endColumn);

            if (response == "a" || response == "A")
            {
                this.EnterData("DT");
                response = this._screen.ToAsk("Confirma alteração (S/N): ", line, startColumn, endColumn);
                if (response == "s" || response == "S")
                {
                    _products[this._position].Description = this._model.Description;
                    _products[this._position].Category = this._model.Category;
                    _products[this._position].UnitaryPrice = this._model.UnitaryPrice;
                    _products[this._position].Supplier = this._model.Supplier;
                    _products[this._position].QtyStock = this._model.QtyStock;
                }
            }
            if (response == "e" || response == "E")
            {
                response = this._screen.ToAsk("Confirma exclusão (S/N): ", line, startColumn, endColumn);
                if (response == "s" || response == "S")
                {
                    _products.RemoveAt(this._position);
                }
            }
        }
        else
        {
            response = this._screen.ToAsk("Produto não encontrado. Deseja cadastrar (S/N): ",
                line, startColumn, endColumn);

            if (response == "s" || response == "S")
            {
                this.EnterData("DT");
                response = this._screen.ToAsk("Confirma cadastro (S/N): ", line, startColumn, endColumn);
                if (response == "s" || response == "S")
                {
                    _products.Add(new ProductModel(this._model.Code, this._model.Description, this._model.Category, this._model.QtyStock, this._model.UnitaryPrice, this._model.Supplier));
                }
            }
        }
    }

    internal void ReportRegisteredProducts()
    {
        this._screen.PrepareMainView("Relatório de Produtos Cadastrados", 0, 0, 79, 24);

        int row = 3;
        this._screen.WriteFrame(2, row, "Código | Descrição           | Categoria     | Preço    | Estoque | Fornecedor");
        this._screen.WriteFrame(2, row + 1, "------------------------------------------------------------------------------");
        row += 2;

        if (_products.Count == 0)
        {
            this._screen.WriteFrame(2, row, "Nenhum produto cadastrado.");
            row++;
        }
        else
        {
            foreach (var product in _products)
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
                    this._screen.PrepareMainView("Relatório de Produtos Cadastrados", 0, 0, 79, 24);
                    row = 3;
                    this._screen.WriteFrame(2, row, "Código | Descrição           | Categoria     | Preço    | Estoque | Fornecedor");
                    this._screen.WriteFrame(2, row + 1, "------------------------------------------------------------------------------");
                    row += 2;
                }

                string supplierName = product.Supplier != null ? product.Supplier.Name : "N/A";
                string line = $"{product.Code,-4} | {product.Description,-19} | {product.Category,-12} | {product.UnitaryPrice,-8:F2} | {product.QtyStock,-5} | {supplierName}";
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

    internal void ReportLowStockProducts()
    {
        this._screen.PrepareMainView("Relatório de Produtos com Estoque Baixo", 0, 0, 79, 24);

        int row = 3;
        this._screen.WriteFrame(2, row, "Código | Descrição           | Categoria     | Preço    | Estoque | Fornecedor");
        this._screen.WriteFrame(2, row + 1, "------------------------------------------------------------------------------");
        row += 2;

        bool foundAny = false;

        foreach (var product in _products)
        {
            if (product.QtyStock < 5)
            {
                foundAny = true;
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
                    this._screen.PrepareMainView("Relatório de Produtos com Estoque Baixo", 0, 0, 79, 24);
                    row = 3;
                    this._screen.WriteFrame(2, row, "Código | Descrição           | Categoria     | Preço    | Estoque | Fornecedor");
                    this._screen.WriteFrame(2, row + 1, "------------------------------------------------------------------------------");
                    row += 2;
                }

                string supplierName = product.Supplier != null ? product.Supplier.Name : "N/A";
                string line = $"{product.Code,-4} | {product.Description,-19} | {product.Category,-12} | {product.UnitaryPrice,-8:F2} | {product.QtyStock,-5} | {supplierName}";
                this._screen.WriteFrame(2, row, line);
                row++;
            }
        }

        if (!foundAny)
        {
            this._screen.WriteFrame(2, row, "Nenhum produto com estoque baixo.");
            row++;
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
