using System.ComponentModel;
using System.Windows.Input;
using MauiLib.Core.Constantes;
using MauiLib.Core.Models;
using MauiLib.Core.Servicos;

namespace LettsCandy.Paginas
{
    public partial class ComprasPage : ContentPage
    {
        public Compra Compra { get; set; }

        private DatabaseServicos<Compra> _comprasServico;

        public ICommand NavigateToComprasSalvarCommand { get; private set; }

        public ComprasPage()
        {
            InitializeComponent();
            _comprasServico = new DatabaseServicos<Compra>(Db.DB_PATH);
            NavigateToComprasSalvarCommand = new Command<Compra>(async (compra) => await NavigateToBoughtSalvar(compra));
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            CarregarCompras();
            CarregarTotalizacao();
        }


        private async Task NavigateToBoughtSalvar(Compra compra)
        {
            await Navigation.PushAsync(new ComprasSalvarPage(compra));
        }

        private async void NovoClicked(object sender, EventArgs e)
        {
            var botao = sender as Button;
            if (sender != null)
            {
                await Navigation.PushAsync(new ComprasSalvarPage(new Compra { SituacaoCompra = MauiLib.Core.Enums.SituacaoCompra.NaoUtilizada }));
            }

        }
        private async void CarregarCompras()
        {
            ComprasCollection.ItemsSource = await _comprasServico.TodosAsync();
        }

        private async void CarregarTotalizacao()
        {
            var qtdCompras = await _comprasServico.QuantidadeAsync();
            LabelQtdCompras.Text = qtdCompras.ToString();
        }
    }
}
