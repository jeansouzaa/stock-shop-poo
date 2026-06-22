namespace StockShop.Models;

internal class SupplierModel
{
    private int _code;
    private string _cnpj;
    private string _name;
    private string _phoneNumber;
    private List<ProductModel> _products = new List<ProductModel>();

    public SupplierModel(int code, string cnpj, string name, string phoneNumber, List<ProductModel> products)
    {
        this._code = code;
        this._name = name;
        this._cnpj = cnpj;
        this._phoneNumber = phoneNumber;
        this._products = products;
    }

    public SupplierModel() { }

    public SupplierModel(int code, string cnpj, string name, string phoneNumber)
    {
        Code = code;
        Cnpj = cnpj;
        Name = name;
        PhoneNumber = phoneNumber;
    }

    public int Code
    {
        get { return _code; }
        set { _code = value; }
    }

    public string Cnpj
    {
        get { return _cnpj; }
        set { _cnpj = value; }
    }

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    public string PhoneNumber
    {
        get{ return _phoneNumber; }
        set{ _phoneNumber = value; }   
    }

    public List<ProductModel> Products
    {
        get { return _products;}
        set { _products = value; }
    }
}
