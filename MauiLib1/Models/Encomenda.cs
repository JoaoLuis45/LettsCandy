using MauiLib.Core.Enums;
using SQLite;
namespace MauiLib.Core.Models;

public class Encomenda{
    public Encomenda()
    {
        this.DataInicio = DateTime.Now;
        this.EncomendaItems = new List<EncomendaItem>();
        this.Produtos = new List<Produto>();
        SituacaoEncomenda = MauiLib.Core.Enums.SituacaoEncomenda.Iniciada;
    }

    [PrimaryKey, AutoIncrement]
    public int Id {get;set;}
    public string Descricao {get;set;}
    public int ClienteId {get;set;}
    public double Valor {get;set;}
    public DateTime DataInicio { get; set; }
    public DateTime DataFinalizacao { get; set; }
    public SituacaoEncomenda SituacaoEncomenda { get; set; }
    [Ignore]
    public List<EncomendaItem> EncomendaItems { get; set; }
    [Ignore]
    public List<Produto> Produtos { get; set; }
    [Ignore]
    public Cliente Cliente { get; set; }
}