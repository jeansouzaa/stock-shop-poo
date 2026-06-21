using System;
using System.Collections.Generic;
using System.Text;

namespace StockShop
{
    internal class SupplierController
    {
        int _column, _row, _width, _heigth, _position;
        private Screen _screen;
        private List<string> _fields;
        private List<SupplierModel> _suppliers = new List<SupplierModel>();
        private SupplierModel _model;

        public SupplierController(int column, int row, Screen screen)
        {
            this._column = column;
            this._row = row;
            this._screen = screen;

            this._model = new SupplierModel();
            this._suppliers.Add(new SupplierModel("52.518.679/0001-09", "Art&Office", "(47)99999-9999", new List<ProductModel>()));

            this._fields = new List<string>();
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

        public void EnterData(string which)
        {
            if (which == "PK")
            {
                int column, row;
                column = this._column + 1 + this._fields[0].Length;
                row = this._row + 2;
                Console.SetCursorPosition(column, row);
                this._model.Cnpj = Console.ReadLine();
            }
            else
            {
                int column, row;
                column = this._column + 1 + this._fields[0].Length;
                row = this._row + 3;

                this._screen.ClearArea(column, row, this._column + this._width - 2, row + this._heigth - 5);

                Console.SetCursorPosition(column, row);
                this._model.Name = Console.ReadLine();

                row++;
                Console.SetCursorPosition(column, row);
                this._model.PhoneNumber = Console.ReadLine();
            }
        }


        public void ShowData()
        {
            int column, row;
            column = this._column + 1 + this._fields[0].Length;

            row = this._row + 3;
            Screen.WriteFrame(column, row, this._suppliers[this._position].Name);

            row++;
            Screen.WriteFrame(column, row, this._suppliers[this._position].PhoneNumber);
        }

        public bool FindProduct()
        {
            bool found = false;
            for (int count = 0; count < this._suppliers.Count; count++)
            {
                if (this._suppliers[count].Cnpj.Equals(this._model.Cnpj))
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
            string answer;
            int initialColumn = this._column + 1;
            int finalColumn = this._column + this._width - 1;
            int line = this._row + this._heigth - 1;

            this.ShowForm();
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
                        this._suppliers[this._position].Name = this._model.Name;
                        this._suppliers[this._position].PhoneNumber = this._model.PhoneNumber;
                    }
                }
                if (answer == "e" || answer == "E")
                {
                    answer = this._screen.ToAsk("Confirma exclusão (S/N): ", line, initialColumn, finalColumn);
                    if (answer == "s" || answer == "S")
                    {
                        this._suppliers.RemoveAt(this._position);
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
                        this._suppliers.Add(
                            new SupplierModel(this._model.Cnpj, this._model.Name, this._model.PhoneNumber)
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
                row, "Cadastro de Fornecedores");

            row++;
            for (int i = 0; i < this._fields.Count; i++)
            {
                Screen.WriteFrame(this._column + 1, row, this._fields[i]);
                row++;
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
            throw new NotImplementedException();
        }
    }
}
