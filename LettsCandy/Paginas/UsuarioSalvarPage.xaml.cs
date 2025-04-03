using System.Globalization;
using System.IO;
using MauiLib.Core.Constantes;
using MauiLib.Core.Models;
using MauiLib.Core.Servicos;

namespace LettsCandy.Paginas
{
    public partial class UsuarioSalvarPage : ContentPage
    {
        public Usuario Usuario { get; set; }

        private DatabaseServicos<Usuario> _usuariosServico;

        public UsuarioSalvarPage(Usuario usuario)
        {
            InitializeComponent();
            _usuariosServico = new DatabaseServicos<Usuario>(Db.DB_PATH);
            Usuario = usuario;
            BindingContext = usuario;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await Task.Delay(100);
            NomeEntry.Focus();
        }

        private async void SalvarClicked(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(NomeEntry.Text))
            {
                await DisplayAlert("Erro", "O nome é obrigatório", "Ok");
                NomeEntry.Focus();
                return;
            }


            Usuario.Nome = NomeEntry.Text;
            Usuario.Obs = ObsEditor.Text;

            if (Usuario.Id == 0)
                await _usuariosServico.IncluirAsync(Usuario);
            else
            {
                Usuario.DataAtualizacao = DateTime.Now;
                await _usuariosServico.AlterarAsync(Usuario);
            }
            await Navigation.PopAsync();
        }

        private async void ExcluirClicked(object sender, EventArgs e)
        {
            if (Usuario.Id == 0)
            {
                await DisplayAlert("Erro", "Não é possível excluir um usuário que não existe", "Ok");
                return;
            }
            await _usuariosServico.DeletarAsync(Usuario);
            await Navigation.PopAsync();
        }
    }
}