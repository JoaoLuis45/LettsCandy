using SQLite;
namespace MauiLib.Core.Models;

public class RemessaTotalizacao{

    public RemessaTotalizacao()
    {
        this.RemessaItems = new List<RemessaItem>();
        this.RemessaQtdTotalItemsEstoque = new List<Item>();
    }

    [PrimaryKey, AutoIncrement]
    public int Id {get;set;}
    public int RemessaId {get;set;}
    [Ignore]
    public List<RemessaItem> RemessaItems { get; set; }
    [Ignore]
    public List<Item> RemessaQtdTotalItemsEstoque { get; set; }

}