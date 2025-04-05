using MauiLib.Core.Enums;
using SQLite;
namespace MauiLib.Core.Models;

public class Compra{
    public Compra()
    {
        this.DataCompra = DateTime.Now;
        this.Items = new List<Item>();
        this.Valor = 0.0;
    }

    [PrimaryKey, AutoIncrement]
    public int Id {get;set;}
    public string Descricao {get;set;}
    public double Valor {get;set;}
    public DateTime DataCompra { get; set; }
    public SituacaoCompra SituacaoCompra { get; set; }
    [Ignore]
    public List<Item> Items { get; set; }
}