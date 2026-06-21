using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace StockShop
{
    internal class ProductController
    {

        private Screen _screen;
        private ProductModel _model;
        private SupplierController _supplierController = new SupplierController();
        private int _column, _row, _width, _heigth, _position;
        private string _title;
        private List<string> _fields;

        private List<ProductModel> _products = new List<ProductModel>();

        public ProductController(int column, int row, Screen screen)
        {
            this._column = column;
            this._row = row;
            this._screen = screen;

            this._model = new ProductModel();


            this._fields = new List<string>();
            this._fields.Add("Código                : ");
            this._fields.Add("Descrição             : ");
            this._fields.Add("Categoria             : ");
            this._fields.Add("Quantidade em Estoque : ");
            this._fields.Add("Preço                 : ");
            this._fields.Add("Nome do Fornecedor    : ");

            this._width = this._fields[0].Length + 2 + 30;
            this._heigth = this._fields.Count + 2 + 1;
        }

        public ProductController()
        {
        }

        public ProductModel findProductByCode(string code)
        {
            ProductModel product = null;
            for (int count = 0; count < this._products.Count; count++)
            {
                ProductModel productInCache = this._products[count];
                if (productInCache != null && productInCache.Code.Equals(code))
                {
                    product = productInCache;
                }
            }
            return product;
        }

        public void EnterData(string which)
        {
            if (which == "PK")
            {
                int column, row;
                column = this._column + 1 + this._fields[0].Length;
                row = this._row + 2;
                Console.SetCursorPosition(column, row);
                this._model.Code = int.Parse(Console.ReadLine());
            }
            else
            {
                int column, row;
                column = this._column + 1 + this._fields[0].Length;
                row = this._row + 3;

                this._screen.ClearArea(column, row, this._column + this._width - 2, row + this._heigth - 5);

                Console.SetCursorPosition(column, row);
                this._model.Description = Console.ReadLine();

                row++;
                Console.SetCursorPosition(column, row);
                this._model.Category = (ProductCategory)Enum.Parse(typeof(ProductCategory), Console.ReadLine());

                row++;
                Console.SetCursorPosition(column, row);
                this._model.QtyStock = int.Parse(Console.ReadLine());

                row++;
                Console.SetCursorPosition(column, row);
                this._model.UnitaryPrice = double.Parse(Console.ReadLine());

                row++;
                Console.SetCursorPosition(column, row);
                this._model.Supplier = this._supplierController.findSupplierByName(Console.ReadLine());
            }
        }


        public void ShowData()
        {
            int column, row;
            column = this._column + 1 + this._fields[0].Length;

            row = this._row + 3;
            Screen.WriteFrame(column, row, _products[this._position].Description);

            row++;
            Screen.WriteFrame(column, row, _products[this._position].Category.ToString());

            row++;
            Screen.WriteFrame(column, row, "R$" + _products[this._position].UnitaryPrice);
        }

        public bool FindProduct()
        {
            bool found = false;
            for (int count = 0; count < this._products.Count; count++)
            {
                if (this._products[count].Code.Equals(this._model.Code))
                {
                    this._position = count;
                    found = true;
                }
            }
            return found;
        }

        public void CRUD()
        {
            bool found;
            string resp;
            int colini = this._column + 1;
            int colfin = this._column + this._width - 1;
            int linha = this._row + this._heigth - 1;

            this.ShowForm();
            this.EnterData("PK");
            found = this.FindProduct();
            if (found)
            {
                this.ShowData();
                resp = this._screen.ToAsk("Deseja alterar/excluir/voltar (A/E/V): ", linha, colini, colfin);

                if (resp == "a" || resp == "A")
                {
                    this.EnterData("DT");
                    resp = this._screen.ToAsk("Confirma alteração (S/N): ", linha, colini, colfin);
                    if (resp == "s" || resp == "S")
                    {
                        this._products[this._position].Description = this._model.Description;
                        this._products[this._position].Category = this._model.Category;
                        this._products[this._position].UnitaryPrice = this._model.UnitaryPrice;
                        this._products[this._position].Supplier = this._model.Supplier;
                        this._products[this._position].QtyStock = this._model.QtyStock;
                    }
                }
                if (resp == "e" || resp == "E")
                {
                    resp = this._screen.ToAsk("Confirma exclusão (S/N): ", linha, colini, colfin);
                    if (resp == "s" || resp == "S")
                    {
                        this._products.RemoveAt(this._position);
                    }
                }
            }
            else
            {
                resp = this._screen.ToAsk("Produto não encontrado. Deseja cadastrar (S/N): ",
                    linha, colini, colfin);

                if (resp == "s" || resp == "S")
                {
                    this.EnterData("DT");
                    resp = this._screen.ToAsk("Confirma cadastro (S/N): ", linha, colini, colfin);
                    if (resp == "s" || resp == "S")
                    {
                        this._products.Add(
                            new ProductModel(this._model.Code, this._model.Description,
                                this._model.Category,this._model.QtyStock, this._model.UnitaryPrice, this._model.Supplier)
                        );
                    }
                }
            }
        }


        private void ShowForm()
        {
            this._screen.AssembleFrame(this._column, this._row,
                this._column + this._width, this._row + this._heigth);

            int row = this._row + 1;
            this._screen.Centralize(this._column, this._column + this._width,
                row, "Cadastro de Produtos");

            row++;
            for (int i = 0; i < this._fields.Count; i++)
            {
                Screen.WriteFrame(this._column + 1, row, this._fields[i]);
                row++;
            }
        }

        internal void ReportRegisteredProducts()
        {
            throw new NotImplementedException();
        }
    }

}
