using SQLite;
namespace MauiLib.Core.Models;

public class RemessaLigacao{

    public RemessaLigacao()
    {
        this.RemessaItems = new List<RemessaItem>();
        this.ProdutoQtd = 0;
    }

    [PrimaryKey, AutoIncrement]
    public int Id {get;set;}
    public int RemessaId {get;set;}
    public int ProdutoId {get;set;}
    public int ProdutoQtd {get;set;}
    public int ReceitaId {get;set;}
    public double ReceitaQtd {get;set;}
    [Ignore]
    public Produto Produto {get;set;}
    [Ignore]
    public Receita Receita {get;set;}
    [Ignore]
    public List<RemessaItem> RemessaItems { get; set; }
}