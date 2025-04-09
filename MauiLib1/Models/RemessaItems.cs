using SQLite;
namespace MauiLib.Core.Models;

public class RemessaItem{

    [PrimaryKey, AutoIncrement]
    public int Id {get;set;}
    public int RemessaLigacaoId {get;set;}
    public int ReceitaItemId { get; set; }
    public string ReceitaNomeItem { get; set; }
    public double ReceitaItemQtd { get; set; }
    [Ignore]
    public Item Item { get; set; }
}