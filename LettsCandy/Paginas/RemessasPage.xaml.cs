using System.ComponentModel;
using System.Windows.Input;
using MauiLib.Core.Constantes;
using MauiLib.Core.Models;
using MauiLib.Core.Servicos;

namespace LettsCandy.Paginas
{
    public partial class RemessasPage : ContentPage
    {
        public Remessa Remessa { get; set; }

        private DatabaseServicos<Remessa> _remessasServico;

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

        public ICommand NavigateToRemessasSalvarCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        public RemessasPage()
        {
            InitializeComponent();
            _remessasServico = new DatabaseServicos<Remessa>(Db.DB_PATH);
            NavigateToRemessasSalvarCommand = new Command<Remessa>(async (remessa) => await NavigateToShipmentSalvar(remessa));
            RefreshCommand = new Command(async () => await OnRefreshCommand());
            BindingContext = this;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            CarregarRemessas();
            CarregarTotalizacao();
        }

        private async Task OnRefreshCommand()
        {
            IsBusy = true;
            try
            {
                CarregarRemessas();
                CarregarTotalizacao();
            }
            finally
            {
                IsBusy = false;
            }
        }


        private async Task NavigateToShipmentSalvar(Remessa remessa)
        {
            loadingIndicator.IsVisible = true;
            loadingIndicator.IsRunning = true;

            var paginaSalvarRemessas = new RemessasSalvarPage(remessa);

            await paginaSalvarRemessas.ExecuteOnAppearing();

            await Navigation.PushAsync(paginaSalvarRemessas);

            loadingIndicator.IsVisible = false;
            loadingIndicator.IsRunning = false;
        }

        private async void NovoClicked(object sender, EventArgs e)
        {
            var botao = sender as Button;
            if (sender != null)
            {
                var paginaSalvarRemessas = new RemessasSalvarPage(new Remessa { SituacaoRemessa = MauiLib.Core.Enums.SituacaoRemessa.NaoProduzida });

                await paginaSalvarRemessas.ExecuteOnAppearing();

                await Navigation.PushAsync(paginaSalvarRemessas);
            }

        }
        private async void CarregarRemessas()
        {
            RemessasCollection.ItemsSource = await _remessasServico.TodosAsync();
        }

        private async void CarregarTotalizacao()
        {
            var qtdRemessas = await _remessasServico.QuantidadeAsync();
            LabelQtdRemessas.Text = qtdRemessas.ToString();
        }
    }
}
