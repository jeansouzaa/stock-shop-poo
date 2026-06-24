using System;
using System.Collections.Generic;
using StockShop.Controllers.Base;
using StockShop.Models;
using StockShop.Models.Enums;
using StockShop.Views;
using StockShop.Views.Base;

namespace StockShop.Controllers;

internal class ProductMovementController : MainController
{
    private const int CodeFieldRowOffset = 2;
    private const int DateFieldRowOffset = 3;
    private const int QtyFieldRowOffset = 4;
    private const int TypeFieldRowOffset = 5;
    private const int ProductFieldRowOffset = 6;

    private ProductMovementModel _model;
    private static List<ProductMovementModel> _productMovements = new List<ProductMovementModel>();
    private ProductController _productController = new ProductController(10, 5, new ProductView());

    public ProductMovementController(int column, int row, ProductMovementView screen) : base(column, row, screen)
    {
        this._model = new ProductMovementModel();

        this._fields = new List<string>();
        this._fields.Add("Código                 : ");
        this._fields.Add("Data da Movimentação   : ");
        this._fields.Add("Quantidade Movimentada : ");
        this._fields.Add("Tipo de Movimentação   : ");
        this._fields.Add("Código do Produto      : ");

        this._width = this._fields[0].Length + 2 + 30;
        this._heigth = this._fields.Count + 2 + 1;
    }

    public ProductMovementController()
    {
    }

    public static List<ProductMovementModel> FindProductMovementsByProduct(ProductModel product)
    {
        List<ProductMovementModel> productMovements = new List<ProductMovementModel>();
        if (_productMovements != null && product != null)
        {
            for (int count = 0; count < _productMovements.Count; count++)
            {
                ProductMovementModel productmovementInCache = _productMovements[count];
                if (productmovementInCache != null && productmovementInCache.Product.Code == product.Code)
                {
                    productMovements.Add(productmovementInCache);
                }
            }
        }
        return productMovements;
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
            int row = this._row + DateFieldRowOffset;

            this._screen.ClearArea(column, row, this._column + this._width - 2, row + this._heigth - 5);

            this._model.MovementDate = ReadDateTime(column, row, "Data inválida! Use o formato AAAA-MM-DD.");

            int inputQty;
            ProductModel product = null;
            TypeMovement typeMovement;

            while (true)
            {
                row = this._row + QtyFieldRowOffset;
                inputQty = ReadInt(column, row, "Quantidade inválida! Digite um número inteiro.");
                row = this._row + TypeFieldRowOffset;
                Console.SetCursorPosition(column, row);
                while (true)
                {
                    string input = Console.ReadLine() ?? "";
                    int catIndex;
                    if (int.TryParse(input, out catIndex) && Enum.IsDefined(typeof(TypeMovement), catIndex))
                    {
                        typeMovement = (TypeMovement)catIndex;
                        break;
                    }
                    int errorLine = this._row + this._heigth + 1;
                    this._screen.ClearArea(this._column, errorLine, this._column + this._width, errorLine);
                    this._screen.Centralize(this._column, this._column + this._width, errorLine, "Tipo inválido! Digite 0 para Entrada e 1 para Saída.");
                    this._screen.ClearArea(column, row, this._column + this._width - 2, row);
                    Console.SetCursorPosition(column, row);
                }

                int finalErrorLine = this._row + this._heigth + 1;
                this._screen.ClearArea(this._column, finalErrorLine, this._column + this._width, finalErrorLine);

                row = this._row + ProductFieldRowOffset;
                while (true)
                {
                    Console.SetCursorPosition(column, row);
                    string input = Console.ReadLine() ?? "";

                    product = ProductController.FindProductByCode(input);
                    if (product != null)
                    {
                        break;
                    }

                    int errorLine = this._row + this._heigth + 1;
                    this._screen.ClearArea(this._column, errorLine, this._column + this._width + 25, errorLine + 2);
                    this._screen.Centralize(this._column, this._column + this._width, errorLine, "Produto não cadastrado!");
                    this._screen.Centralize(this._column, this._column + this._width, errorLine + 1, "1-Cadastrar Novo | 2-Listar Produtos | 3-Cancelar Operação: ");

                    string option = Console.ReadLine() ?? "";
                    this._screen.ClearArea(this._column, errorLine, this._column + this._width + 25, errorLine + 2);

                    switch (option)
                    {
                        case "1":
                            this._productController.CRUD();
                            this.RegisterMovement();
                            break;
                        case "2":
                            this._productController.ReportRegisteredProducts();
                            this.RegisterMovement();
                            break;
                        case "3":
                            throw new OperationCanceledException();
                        default:
                            break;
                    }

                    this._screen.ClearArea(column, row, this._column + this._width - 2, row);
                    Console.SetCursorPosition(column, row);
                }

                if (product.QtyStock >= inputQty)
                {
                    break;
                }

                int stockErrorLine = this._row + this._heigth + 1;
                this._screen.ClearArea(this._column, stockErrorLine, this._column + this._width + 25, stockErrorLine);
                this._screen.Centralize(this._column, this._column + this._width, stockErrorLine, $"Estoque insuficiente! Estoque atual: {product.QtyStock}");

                this._screen.ClearArea(column, this._row + QtyFieldRowOffset, this._column + this._width - 2, this._row + QtyFieldRowOffset);
                this._screen.ClearArea(column, this._row + ProductFieldRowOffset, this._column + this._width - 2, this._row + ProductFieldRowOffset);
            }
            this._model.TypeMovement = typeMovement;
            this._model.QtyMovemented = inputQty;
            this._model.Product = product;
        }
    }

    private void RegisterMovement()
    {
        this.ShowForm("Cadastro de Movimentações");
        int column = this._column + 1 + this._fields[0].Length;
        this._screen.WriteFrame(column, this._row + CodeFieldRowOffset, this._model.Code.ToString());
        this._screen.WriteFrame(column, this._row + DateFieldRowOffset, this._model.MovementDate.ToString("yyyy-MM-dd"));
        this._screen.WriteFrame(column, this._row + QtyFieldRowOffset, this._model.QtyMovemented.ToString());
        this._screen.WriteFrame(column, this._row + TypeFieldRowOffset, this._model.TypeMovement.ToString());
        this._screen.WriteFrame(column, this._row + ProductFieldRowOffset, this._model.Product.Code.ToString());
    }

    public void ShowData()
    {
        int column = this._column + 1 + this._fields[0].Length;

        this._screen.WriteFrame(column, this._row + DateFieldRowOffset, _productMovements[this._position].MovementDate.ToString("yyyy-MM-dd"));
        this._screen.WriteFrame(column, this._row + QtyFieldRowOffset, _productMovements[this._position].QtyMovemented.ToString());
        this._screen.WriteFrame(column, this._row + TypeFieldRowOffset, _productMovements[this._position].TypeMovement.ToString());
        string productCodeStr = _productMovements[this._position].Product != null ? _productMovements[this._position].Product.Code.ToString() : "N/A";
        this._screen.WriteFrame(column, this._row + ProductFieldRowOffset, productCodeStr);
    }

    public bool FindProduct()
    {
        bool found = false;
        for (int count = 0; count < _productMovements.Count; count++)
        {
            if (_productMovements[count].Code == this._model.Code)
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

        this.ShowForm("Cadastro de Movimentações");
        this.EnterData("PK");
        found = this.FindProduct();
        if (found)
        {
            this.ShowData();
            response = this._screen.ToAsk("Deseja alterar/excluir/voltar (A/E/V): ", line, startColumn, endColumn);

            if (response == "a" || response == "A")
            {
                try
                {
                    ProductModel originalProduct = _productMovements[this._position].Product;
                    int originalQty = _productMovements[this._position].QtyMovemented;

                    originalProduct.QtyStock += originalQty;

                    this.EnterData("DT");
                    response = this._screen.ToAsk("Confirma alteração (S/N): ", line, startColumn, endColumn);
                    if (response == "s" || response == "S")
                    {
                        if (this._model.TypeMovement == TypeMovement.Saida)
                        {
                            this._model.Product.QtyStock -= this._model.QtyMovemented;
                        }
                        else
                        {
                            this._model.Product.QtyStock += this._model.QtyMovemented;
                        }

                        _productMovements[this._position].MovementDate = this._model.MovementDate;
                        _productMovements[this._position].QtyMovemented = this._model.QtyMovemented;
                        _productMovements[this._position].TypeMovement = this._model.TypeMovement;
                        _productMovements[this._position].Product = this._model.Product;
                    }
                    else
                    {
                        originalProduct.QtyStock -= originalQty;
                    }
                }
                catch (OperationCanceledException)
                {
                    _productMovements[this._position].Product.QtyStock -= _productMovements[this._position].QtyMovemented;
                }
            }
            if (response == "e" || response == "E")
            {
                response = this._screen.ToAsk("Confirma exclusão (S/N): ", line, startColumn, endColumn);
                if (response == "s" || response == "S")
                {
                    _productMovements[this._position].Product.QtyStock += _productMovements[this._position].QtyMovemented;
                    _productMovements.RemoveAt(this._position);
                }
            }
        }
        else
        {
            response = this._screen.ToAsk("Movimentação não encontrada. Deseja cadastrar (S/N): ",
                line, startColumn, endColumn);

            if (response == "s" || response == "S")
            {
                try
                {
                    this.EnterData("DT");
                    response = this._screen.ToAsk("Confirma cadastro (S/N): ", line, startColumn, endColumn);
                    if (response == "s" || response == "S")
                    {
                        this._model.Product.QtyStock -= this._model.QtyMovemented;

                        _productMovements.Add(
                            new ProductMovementModel(this._model.Code, this._model.MovementDate,
                                this._model.QtyMovemented, this._model.TypeMovement, this._model.Product)
                        );
                    }
                }
                catch (OperationCanceledException)
                {
                }
            }
        }
    }

    internal void ReportMovementedProduct()
    {
        this._screen.PrepareMainView("Relatórios de Produtos Movimentados", 0, 0, 79, 24);

        int row = 3;
        this._screen.WriteFrame(2, row, "Código | Data       | Qtd | Descrição do Produto");
        this._screen.WriteFrame(2, row + 1, "------------------------------------------------------------------------------");
        row += 2;

        if (_productMovements.Count == 0)
        {
            this._screen.WriteFrame(2, row, "Nenhuma movimentação registrada.");
            row++;
        }
        else
        {
            for (int count = 0; count < _productMovements.Count; count++)
            {
                ProductMovementModel movement = _productMovements[count];
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
                    this._screen.PrepareMainView("Relatórios de Produtos Movimentados", 0, 0, 79, 24);
                    row = 3;
                    this._screen.WriteFrame(2, row, "Código | Data       | Qtd | Descrição do Produto");
                    this._screen.WriteFrame(2, row + 1, "------------------------------------------------------------------------------");
                    row += 2;
                }

                string productDesc = movement.Product != null ? movement.Product.Description : "N/A";
                string dateStr = movement.MovementDate.ToString("yyyy-MM-dd");
                string line = $"{movement.Code,-4} | {dateStr,-10} | {movement.QtyMovemented,-3} | {movement.TypeMovement,-10} | {productDesc}";
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