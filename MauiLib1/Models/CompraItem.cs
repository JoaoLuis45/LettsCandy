using SQLite;
namespace MauiLib.Core.Models;

public class CompraItem{

    public CompraItem()
    {
        this.QtdItem = 0;
    }

    [PrimaryKey, AutoIncrement]
    public int Id {get;set;}
    public int ItemId {get;set;}
    public int CompraId {get;set;}
    public string NomeItem {get;set;}
    public int QtdItem {get;set;}
}