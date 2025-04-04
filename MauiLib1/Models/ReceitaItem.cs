using SQLite;
namespace MauiLib.Core.Models;

public class ReceitaItem{

    public ReceitaItem()
    {
        this.QtdItem = 0;
    }

    [PrimaryKey, AutoIncrement]
    public int Id {get;set;}
    public int ItemId {get;set;}
    public int ReceitaId {get;set;}
    public string NomeItem {get;set;}
    public int QtdItem {get;set;}
}