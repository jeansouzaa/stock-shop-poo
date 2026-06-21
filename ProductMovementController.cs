using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace StockShop
{
    internal class ProductMovementController
    {
        int _column, _row, _width, _heigth, _position;
        private Screen _screen;
        private List<string> _fields;
        private ProductMovementModel _model;
        private List<ProductMovementModel> _productMovements = new List<ProductMovementModel>();

        public ProductMovementController(int column, int row, Screen screen)
        {
            this._column = column;
            this._row = row;
            this._screen = screen;

            this._model = new ProductMovementModel();

            this._fields = new List<string>();
            this._fields.Add("Data da Movimentação                   : ");
            this._fields.Add("Quantidade Movimentada                 : ");
            this._fields.Add("Nome do Produto Movimentado            : ");

            this._width = this._fields[0].Length + 2 + 30;
            this._heigth = this._fields.Count + 2 + 1;
        }
        public List<ProductMovementModel> findProductMovementsByProduct(ProductModel product)
        {
            List<ProductMovementModel> productMovements = new List<ProductMovementModel>();
            if(this._productMovements != null && product != null)
            {
                for (int count = 0; count < this._productMovements.Count; count++)
                {
                    ProductMovementModel productmovementInCache = this._productMovements[count];
                    if (productmovementInCache != null && productmovementInCache.Product.Code.Equals(product.Code))
                    {
                        productMovements.Add(productmovementInCache);
                    }
                }
            }
            return productMovements;
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
                this._model.MovementDate = DateTime.Parse(Console.ReadLine());

                row++;
                Console.SetCursorPosition(column, row);
                this._model.QtyMovemented = int.Parse(Console.ReadLine());

                row++;
                Console.SetCursorPosition(column, row);
                this._model.Product = new ProductController().findProductByCode(Console.ReadLine());
            }
        }


        public void ShowData()
        {
            int column, row;
            column = this._column + 1 + this._fields[0].Length;

            row = this._row + 3;
            Screen.WriteFrame(column, row, this._productMovements[this._position].MovementDate.ToString());

            row++;
            Screen.WriteFrame(column, row, this._productMovements[this._position].QtyMovemented.ToString());

            row++;
            Screen.WriteFrame(column, row, this._productMovements[this._position].Product.Description);
        }

        public bool FindProduct()
        {
            bool found = false;
            for (int count = 0; count < this._productMovements.Count; count++)
            {
                if (this._productMovements[count].Code.Equals(this._model.Code))
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
                        this._productMovements[this._position].MovementDate = this._model.MovementDate;
                        this._productMovements[this._position].QtyMovemented = this._model.QtyMovemented;
                        this._productMovements[this._position].Product = this._model.Product;
                    }
                }
                if (resp == "e" || resp == "E")
                {
                    resp = this._screen.ToAsk("Confirma exclusão (S/N): ", linha, colini, colfin);
                    if (resp == "s" || resp == "S")
                    {
                        this._productMovements.RemoveAt(this._position);
                    }
                }
            }
            else
            {
                resp = this._screen.ToAsk("Movimentação não encontrada. Deseja cadastrar (S/N): ",
                    linha, colini, colfin);

                if (resp == "s" || resp == "S")
                {
                    this.EnterData("DT");
                    resp = this._screen.ToAsk("Confirma cadastro (S/N): ", linha, colini, colfin);
                    if (resp == "s" || resp == "S")
                    {
                        this._productMovements.Add(
                            new ProductMovementModel(this._model.Code, this._model.MovementDate,
                                this._model.QtyMovemented, this._model.Product)
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
                row, "Cadastro de Movimentações");

            row++;
            for (int i = 0; i < this._fields.Count; i++)
            {
                Screen.WriteFrame(this._column + 1, row, this._fields[i]);
                row++;
            }
        }

        internal void ReportMovementedProduct()
        {
            throw new NotImplementedException();
        }
    }
}
