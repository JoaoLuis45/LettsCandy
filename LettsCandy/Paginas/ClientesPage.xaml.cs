using System;
using System.Drawing;
using System.Windows.Input;
using MauiLib.Core.Constantes;
using MauiLib.Core.Models;
using MauiLib.Core.Servicos;

namespace LettsCandy.Paginas
{
    public partial class ClientesPage : ContentPage
    {
        public Cliente Cliente { get; set; }

        private DatabaseServicos<Cliente> _clientesServico;
        public ICommand NavigateToClientesSalvarCommand { get; private set; }
        
        public ClientesPage()
        {
            InitializeComponent();
            _clientesServico = new DatabaseServicos<Cliente>(Db.DB_PATH);
            NavigateToClientesSalvarCommand = new Command<Cliente>(async (cliente) => await NavigateToClientSalvar(cliente));
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            CarregarClientes();
            CarregarTotalizacao();
        }

        private async Task NavigateToClientSalvar(Cliente cliente)
        {
            await Navigation.PushAsync(new ClientesSalvarPage(cliente));
        }

        private async void NovoClicked(object sender, EventArgs e)
        {
            var botao = sender as Button;
            if (sender != null)
            {
                await Navigation.PushAsync(new ClientesSalvarPage(new Cliente{ SituacaoCliente = MauiLib.Core.Enums.SituacaoCliente.EmDia }));
            }

        }
        private async void CarregarClientes()
        {
            ClientesCollection.ItemsSource = await _clientesServico.TodosAsync();
        }

        private async void CarregarTotalizacao()
        {
            var qtdClientes = await _clientesServico.QuantidadeAsync();
            LabelQtdClientes.Text = qtdClientes.ToString();
        }
    }
}
