using SQLite;
namespace MauiLib.Core.Models;

public class Anexo
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Arquivo { get; set; }
    public int ProdutoId { get; set; }

    public static explicit operator Anexo(int v)
    {
        throw new NotImplementedException();
    }
}