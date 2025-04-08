using SQLite;
namespace MauiLib.Core.Models;

public class RemessaLigacao{

    public RemessaLigacao()
    {
        this.Items = new List<Item>();
        this.Receitas = new List<Receita>();
        this.Produtos = new List<Produto>();
        this.ProdutoQtd = 0;
    }

    [PrimaryKey, AutoIncrement]
    public int Id {get;set;}
    public int RemessaId {get;set;}
    public int ProdutoId {get;set;}
    public int ProdutoQtd {get;set;}
    public int ReceitaId {get;set;}
    [Ignore]
    public Produto Produto {get;set;}
    [Ignore]
    public Receita Receita {get;set;}
    [Ignore]
    public List<Item> Items { get; set; }
    [Ignore]
    public List<Receita> Receitas { get; set; }
    [Ignore]
    public List<Produto> Produtos { get; set; }
}