using System.ComponentModel;
using System.Windows.Input;
using MauiLib.Core.Constantes;
using MauiLib.Core.Models;
using MauiLib.Core.Servicos;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace LettsCandy.Paginas
{
    public partial class EncomendasPage : ContentPage
    {
        public Encomenda Encomenda { get; set; }

        private DatabaseServicos<Encomenda> _encomendasServico;

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

        public ICommand NavigateToEncomendasSalvarCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        public EncomendasPage()
        {
            InitializeComponent();
            _encomendasServico = new DatabaseServicos<Encomenda>(Db.DB_PATH);
            NavigateToEncomendasSalvarCommand = new Command<Encomenda>(async (encomenda) => await NavigateToOrderSalvar(encomenda));
            RefreshCommand = new Command(async () => await OnRefreshCommand());
            BindingContext = this;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            CarregarEncomendas();
            CarregarTotalizacao();
        }

        private async Task OnRefreshCommand()
        {
            IsBusy = true;
            try
            {
                CarregarEncomendas();
                CarregarTotalizacao();
            }
            finally
            {
                IsBusy = false;
            }
        }


        private async Task NavigateToOrderSalvar(Encomenda encomenda)
        {
            loadingIndicator.IsVisible = true;
            loadingIndicator.IsRunning = true;

            var paginasEncomendasSalvar = new EncomendasSalvarPage(encomenda);

            await paginasEncomendasSalvar.ExecuteOnAppearing();

            await Navigation.PushAsync(paginasEncomendasSalvar);

            loadingIndicator.IsVisible = false;
            loadingIndicator.IsRunning = false;
        }

        private async void NovoClicked(object sender, EventArgs e)
        {
            var botao = sender as Button;
            if (sender != null)
            {
                var paginasEncomendasSalvar = new EncomendasSalvarPage(new Encomenda { });

                await paginasEncomendasSalvar.ExecuteOnAppearing();

                await Navigation.PushAsync(paginasEncomendasSalvar);
            }

        }
        private async void CarregarEncomendas()
        {
            EncomendasCollection.ItemsSource = await _encomendasServico.TodosAsync();
        }

        private async void CarregarTotalizacao()
        {
            var qtdEncomendas = await _encomendasServico.QuantidadeAsync();
            LabelQtdEncomendas.Text = qtdEncomendas.ToString();

            var encomendas = await _encomendasServico.TodosAsync();
            var valorTotalEncomendas = 0.0;
            foreach (var item in encomendas)
            {
                valorTotalEncomendas += item.Valor;
            }
            LabelValorTotalEncomendas.Text = "R$: " + valorTotalEncomendas.ToString("F2");
        }
    }
}
