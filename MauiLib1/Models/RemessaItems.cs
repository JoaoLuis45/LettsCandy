using SQLite;
namespace MauiLib.Core.Models;

public class RemessaItem{

    [PrimaryKey, AutoIncrement]
    public int Id {get;set;}
    public int RemessaLigacaoId {get;set;}
    [Ignore]
    public List<ReceitaItem> ReceitaItems { get; set; }
    [Ignore]
    public List<double> ReceitaItemsQtd { get; set; }
}