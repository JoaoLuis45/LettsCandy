using System;
using System.Windows.Input;
using MauiLib.Core.Constantes;
using MauiLib.Core.Models;
using MauiLib.Core.Servicos;

namespace LettsCandy.Paginas
{
    public partial class UsuariosPage : ContentPage
    {
        public Usuario Usuario { get; set; }

        private DatabaseServicos<Usuario> _usuariosServico;
        public ICommand NavigateToUsuariosSalvarCommand { get; private set; }
        
        public UsuariosPage()
        {
            InitializeComponent();
            _usuariosServico = new DatabaseServicos<Usuario>(Db.DB_PATH);
            NavigateToUsuariosSalvarCommand = new Command<Usuario>(async (usuario) => await NavigateToUsuarioSalvar(usuario));
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            CarregarUsuarios();
            CarregarTotalizacao();
        }

        private async Task NavigateToUsuarioSalvar(Usuario usuario)
        {
            await Navigation.PushAsync(new UsuarioSalvarPage(usuario));
        }

        private async void NovoClicked(object sender, EventArgs e)
        {
            var botao = sender as Button;
            if (sender != null)
            {
                await Navigation.PushAsync(new UsuarioSalvarPage(new Usuario { }));
            }

        }

        private async void CarregarUsuarios()
        {
            UsuariosCollection.ItemsSource = await _usuariosServico.TodosAsync();
        }

        private async void CarregarTotalizacao()
        {
            var qtdUsuarios = await _usuariosServico.QuantidadeAsync();
            LabelQtdUsuarios.Text = qtdUsuarios.ToString();
        }
    }
}
