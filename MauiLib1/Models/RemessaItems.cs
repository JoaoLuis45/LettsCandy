using SQLite;
namespace MauiLib.Core.Models;

public class RemessaItem{

    public RemessaItem()
    {
        this.QtdItem = 0;
    }

    [PrimaryKey, AutoIncrement]
    public int Id {get;set;}
    public int RemessaLigacaoId {get;set;}
    public int ItemId {get;set;}
    public string NomeItem {get;set;}
    public int QtdItem {get;set;}
}