using MauiLib.Core.Enums;
using SQLite;
namespace MauiLib.Core.Models;

public class Cliente{
    public Cliente(){
        this.DataAtualizacao = DateTime.Now;
        this.DataCriacao = DateTime.Now;
    }

    [PrimaryKey, AutoIncrement]
    public int Id {get;set;}
    public string Nome {get;set;}
    public string Obs {get;set;}
    public DateTime DataCriacao {get;set;}
    public DateTime DataAtualizacao {get;set;}
    public string Endereco { get; set; }
    public string Cidade { get; set; }
    public string NumeroCasa { get; set; }
    public string Telefone { get; set; }
    public SituacaoCliente SituacaoCliente { get; set; }
}