using System;
using System.Collections.Generic;
using System.Text;
using StockShop.Models.Enums;

namespace StockShop.Models;

internal class ProductMovementModel
{
    private long _code;
    private DateTime _movementDate;
    private int _qtyMovemented;
    private TypeMovement _typeMovement;
    private ProductModel _product;

    public ProductMovementModel(long code, DateTime movementDate, int qtyMovemented, TypeMovement typeMovement, ProductModel product)
    {
        this._code = code;
        this._movementDate = movementDate;
        this._qtyMovemented = qtyMovemented;
        this._typeMovement = typeMovement;
        this._product = product;
    }

    public ProductMovementModel() { }

    public long Code
    {
        get { return _code; }
        set { _code = value; }
    }
    
    public DateTime MovementDate
    {
        get { return _movementDate; }
        set { _movementDate = value; }
    }

    public int QtyMovemented
    {
        get { return _qtyMovemented; }
        set { _qtyMovemented = value; }
    }

    public ProductModel Product
    {
        get { return _product; }
        set { _product = value; }
    }

    public TypeMovement TypeMovement
    {
        get { return _typeMovement; }
        set { _typeMovement = value; }
    }
}
