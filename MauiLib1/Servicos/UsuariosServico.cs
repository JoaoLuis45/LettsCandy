using SQLite;
using MauiLib.Core.Models;
namespace MauiLib.Core.Servicos;

public class UsuariosServico
{
    private static UsuariosServico _usuariosServico = new UsuariosServico();
    private List<Usuario> _usuarios = new List<Usuario>();

    public UsuariosServico()
    {
        _usuarios.Add(new Usuario { Id = 1, Nome = "João" });
        _usuarios.Add(new Usuario { Id = 2, Nome = "Letícia" });
    }

    public static UsuariosServico Instancia()
        => _usuariosServico;

    public List<Usuario> Todos()
        => _usuarios;

}