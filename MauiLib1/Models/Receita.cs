using MauiLib.Core.Enums;
using SQLite;
namespace MauiLib.Core.Models;

public class Receita{
    public Receita(){
        this.DataAtualizacao = DateTime.Now;
        this.DataCriacao = DateTime.Now;
        this.Items = new List<Item>();
    }

    [PrimaryKey, AutoIncrement]
    public int Id {get;set;}
    public int ProdutoId {get;set;}
    public List<int> ItemId {get;set;}
    public string Descricao {get;set;}
    public DateTime DataCriacao {get;set;}
    public DateTime DataAtualizacao {get;set;}
    [Ignore]
    public Produto Produto { get; set; }
    [Ignore]
    public List<Item> Items { get; set; }
}