using SQLite;
namespace MauiLib.Core.Models;

public class EncomendaItem{

    public EncomendaItem()
    {
        this.QtdProduto = 0;
    }

    [PrimaryKey, AutoIncrement]
    public int Id {get;set;}
    public int ProdutoId {get;set;}
    public int EncomendaId {get;set;}
    public double ValorProduto {get;set;}
    public string NomeProduto {get;set;}
    public int QtdProduto {get;set;}
    [Ignore]
    public Produto Produto {get;set;}
}