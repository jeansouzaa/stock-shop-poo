using System;
using System.Collections.Generic;
using System.Text;
using StockShop.Models.Enums;

namespace StockShop.Models;

internal class ProductModel
{
    private int _code;
    private string _description;
    private ProductCategory _category;
    private int _qtyStock;
    private double _unitaryPrice;
    private SupplierModel _supplier;

    public ProductModel(int code, string description, ProductCategory category, int qtyStock, double unitaryPrice, SupplierModel supplier)
    {
        _code = code;
        _description = description;
        _category = category;
        _qtyStock = qtyStock;
        _unitaryPrice = unitaryPrice;
        _supplier = supplier;
    }

    public ProductModel() { }

    public int Code
    {
        get { return _code; }
        set { _code = value; }
    }

    public string Description
    {
        get { return _description;}
        set { _description = value; }
    }

    public ProductCategory Category
    {
        get { return _category;}
        set { _category = value; }
    }

    public int QtyStock
    {
        get { return _qtyStock;}
        set { _qtyStock = value; }
    }

    public double UnitaryPrice
    {
        get { return _unitaryPrice;}
        set { _unitaryPrice = value; }
    }

    public SupplierModel Supplier
    {
        get { return _supplier;}
        set { _supplier = value; }
    }
}
