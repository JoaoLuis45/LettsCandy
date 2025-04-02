//using MauiLib.Core.Constantes;
//using MauiLib.Core.Servicos;
using SQLite;
namespace MauiLib.Core.Models;

public class Produto{

    //private DatabaseServicos<Anexo> _anexoServico;


    public Produto(){
        this.DataAtualizacao = DateTime.Now;
        this.DataCriacao = DateTime.Now;

        if(this.Foto == null)
            this.Foto = "emptyproduct.png";

        //_anexoServico = new DatabaseServicos<Anexo>(Db.DB_PATH);
    }

    [PrimaryKey, AutoIncrement]
    public int Id {get;set;}
    public int Quantidade {get;set;}
    public string Nome {get;set;}
    public string Descricao {get;set;}
    public double Preco { get; set; }
    public DateTime DataCriacao {get;set;}
    public DateTime DataAtualizacao {get;set;}
    public string Foto { get; set; }

    //[Ignore]
    //public Anexo Anexo
    //{
    //    get
    //    {
    //        if (this.Id == 0) return null;
    //        return _anexoServico.Query().Where(u => u.Id == this.Id).FirstOrDefaultAsync().Result;
    //    }
    //}
    //[Ignore]
    //public string AnexoFilename
    //{
    //    get
    //    {
    //        if ((this.Anexo == null)) return "emptyproduct.png";
    //        return Usuario?.Nome;
    //    }
    //}
    //public Status? Status {get;set;}
}