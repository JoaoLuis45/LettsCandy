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
