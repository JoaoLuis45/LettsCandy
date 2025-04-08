using MauiLib.Core.Enums;
using SQLite;
namespace MauiLib.Core.Models;

public class Remessa{
    public Remessa()
    {
        this.DataRemessa = DateTime.Now;
        this.RemessaLigacoes = new List<RemessaLigacao>();
    }

    [PrimaryKey, AutoIncrement]
    public int Id {get;set;}
    public string Descricao {get;set;}
    public DateTime DataRemessa { get; set; }
    [Ignore]
    public List<RemessaLigacao> RemessaLigacoes { get; set; }
    [Ignore]
    public SituacaoRemessa SituacaoRemessa { get; set; }
}