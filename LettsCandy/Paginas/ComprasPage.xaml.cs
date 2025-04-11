using System.ComponentModel;
using System.Windows.Input;
using MauiLib.Core.Constantes;
using MauiLib.Core.Models;
using MauiLib.Core.Servicos;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace LettsCandy.Paginas
{
    public partial class ComprasPage : ContentPage
    {
        public Compra Compra { get; set; }

        private DatabaseServicos<Compra> _comprasServico;

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        public ICommand NavigateToComprasSalvarCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        public ComprasPage()
        {
            InitializeComponent();
            _comprasServico = new DatabaseServicos<Compra>(Db.DB_PATH);
            NavigateToComprasSalvarCommand = new Command<Compra>(async (compra) => await NavigateToBoughtSalvar(compra));
            RefreshCommand = new Command(async () => await OnRefreshCommand());
            BindingContext = this;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            CarregarCompras();
            CarregarTotalizacao();
        }

        private async Task OnRefreshCommand()
        {
            IsBusy = true;
            try
            {
                CarregarCompras();
                CarregarTotalizacao();
            }
            finally
            {
                IsBusy = false;
            }
        }


        private async Task NavigateToBoughtSalvar(Compra compra)
        {
            loadingIndicator.IsVisible = true;
            loadingIndicator.IsRunning = true;

            var paginasComprasSalvar = new ComprasSalvarPage(compra);

            await paginasComprasSalvar.ExecuteOnAppearing();

            await Navigation.PushAsync(paginasComprasSalvar);

            loadingIndicator.IsVisible = false;
            loadingIndicator.IsRunning = false;
        }

        private async void NovoClicked(object sender, EventArgs e)
        {
            var botao = sender as Button;
            if (sender != null)
            {
                var paginasComprasSalvar = new ComprasSalvarPage(new Compra { });

                await paginasComprasSalvar.ExecuteOnAppearing();

                await Navigation.PushAsync(paginasComprasSalvar);
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

            var compras = await _comprasServico.TodosAsync();
            var valorTotalCompras = 0.0;
            foreach (var item in compras)
            {
                valorTotalCompras = valorTotalCompras + item.Valor;
            }
            LabelValorTotalCompras.Text = "R$: " + valorTotalCompras.ToString("F2");
        }
    }
}
