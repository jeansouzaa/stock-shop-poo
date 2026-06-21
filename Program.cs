using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockShop
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Screen minhaTela = new Screen(ConsoleColor.Yellow, ConsoleColor.DarkGreen);
            SupplierController supplierController = new SupplierController(10, 5, minhaTela);
            ProductController productController = new ProductController(10, 5, minhaTela);
            ProductMovementController productMovementController = new ProductMovementController(10, 3, minhaTela);

            string opcao = "";
            List<string> opcoesMenu = new List<string>();
            opcoesMenu.Add("1 - Cadastrar Produto                     ");
            opcoesMenu.Add("2 - Cadastrar Fornecedor                  ");
            opcoesMenu.Add("3 - Movimentações de Produto              ");
            opcoesMenu.Add("4 - Relatórios de Produtos Movimentados   ");
            opcoesMenu.Add("5 - Relatórios de Produtos Cadastrados    ");
            opcoesMenu.Add("6 - Relatórios de Fornecedores Cadastrados");
            opcoesMenu.Add("0 - Sair                                  ");

            while (true)
            {
                minhaTela.PrepareScreen("Sistema StockShop", 0, 0, 79, 24);
                opcao = minhaTela.ShowMenu(2, 2, opcoesMenu);

                if (opcao == "0")
                {
                    Console.Clear();
                    Console.WriteLine("Adeus, e obrigado pelos peixes");
                    break;
                }

                switch(opcao)
                {
                    case "1":
                        productController.CRUD();
                        break;
                    case "2":
                        supplierController.CRUD();
                        break;
                    case "3":
                        productMovementController.CRUD();
                        break;
                    case "4":
                        productMovementController.ReportMovementedProduct();
                        break;
                    case "5":
                        productController.ReportRegisteredProducts();
                        break;
                    case "6":
                        supplierController.ReportRegisteredSuppliers();
                        break;
                    default:
                        minhaTela.ClearArea(2, 2, 78, 24);
                        minhaTela.Centralize(2, 12, 12, "Opção inválida");
                        Console.ReadKey();
                        continue;
                }
            }
        }
    }
}
