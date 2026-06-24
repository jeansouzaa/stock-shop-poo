using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockShop.Controllers;
using StockShop.Models;
using StockShop.Views.Base;
using StockShop.Views;

namespace StockShop;

internal class Program
{
    private static MainView _screen = null!;
    private static SupplierController _supplierController = null!;
    private static ProductController _productController = null!;
    private static ProductMovementController _productMovementController = null!;

    static void Main(string[] args)
    {
        InitializeSystem();
        RunMainMenuLoop();
    }

    private static void InitializeSystem()
    {
        _screen = new MainView(ConsoleColor.Yellow, ConsoleColor.DarkGreen);
        _supplierController = new SupplierController(10, 5, new SupplierView());
        _productController = new ProductController(10, 5, new ProductView());
        _productMovementController = new ProductMovementController(10, 3, new ProductMovementView());
    }

    private static void RunMainMenuLoop()
    {
        List<string> menuOptions = BuildMenuOptions();

        while (true)
        {
            _screen.PrepareMainView("Sistema StockShop", 0, 0, 79, 24);
            string option = _screen.ShowMenu(2, 2, menuOptions);

            if (option == "0")
            {
                ExitSystem();
                break;
            }

            ProcessMenuOption(option);
        }
    }

    private static List<string> BuildMenuOptions()
    {
        return new List<string>
        {
            "1 - Cadastrar Produto                       ",
            "2 - Cadastrar Fornecedor                    ",
            "3 - Movimentações de Produto                ",
            "4 - Relatórios de Produtos Movimentados     ",
            "5 - Relatórios de Produtos Cadastrados      ",
            "6 - Relatórios de Fornecedores Cadastrados  ",
            "7 - Relatório de Produtos com Estoque Baixo ",
            "0 - Sair                                    "
        };
    }

    private static void ProcessMenuOption(string option)
    {
        switch (option)
        {
            case "1":
                _productController.CRUD();
                break;
            case "2":
                _supplierController.CRUD();
                break;
            case "3":
                _productMovementController.CRUD();
                break;
            case "4":
                _productMovementController.ReportMovementedProduct();
                break;
            case "5":
                _productController.ReportRegisteredProducts();
                break;
            case "6":
                _supplierController.ReportRegisteredSuppliers();
                break;
            case "7":
                _productController.ReportLowStockProducts();
                break;
            default:
                ShowInvalidOptionWarning();
                break;
        }
    }

    private static void ShowInvalidOptionWarning()
    {
        _screen.ClearArea(2, 2, 78, 24);
        _screen.Centralize(2, 12, 12, "Opção inválida");
        try
        {
            Console.ReadKey();
        }
        catch (InvalidOperationException)
        {
            Console.ReadLine();
        }
    }

    private static void ExitSystem()
    {
        try
        {
            Console.Clear();
        }
        catch (System.IO.IOException) { }
        Console.WriteLine("Sistema encerrado, até logo!");
    }
}


