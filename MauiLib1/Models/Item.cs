//using LettsCandy.Enums;
using SQLite;

namespace MauiLib.Core.Models;

public class Item{

    public Item()
    {
        this.DataAtualizacao = DateTime.Now;
        this.DataCriacao = DateTime.Now;
    }

    [PrimaryKey, AutoIncrement]
    public int Id {get;set;}
    public double Qtd {get;set;}
    public string Nome {get;set;}
    public DateTime DataCriacao { get; set; }
    public DateTime DataAtualizacao { get; set; }
}