using System.Windows.Input;
using MauiLib.Core.Constantes;
using MauiLib.Core.Models;
using MauiLib.Core.Servicos;

namespace LettsCandy.Paginas
{
    public partial class ReceitasPage : ContentPage
    {
        public Receita Receita { get; set; }

        private DatabaseServicos<Receita> _receitasServico;
        private DatabaseServicos<Produto> _produtosServico;
        public ICommand NavigateToReceitasSalvarCommand { get; private set; }
        
        public ReceitasPage()
        {
            InitializeComponent();
            _receitasServico = new DatabaseServicos<Receita>(Db.DB_PATH);
            _produtosServico = new DatabaseServicos<Produto>(Db.DB_PATH);
            NavigateToReceitasSalvarCommand = new Command<Receita>(async (receita) => await NavigateToReceiptSalvar(receita));
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            CarregarReceitas();
            CarregarTotalizacao();
        }

        private async Task NavigateToReceiptSalvar(Receita receita)
        {
            loadingIndicator.IsVisible = true;
            loadingIndicator.IsRunning = true;
            var paginaReceitasSalvar = new ReceitasSalvarPage(receita);

            await paginaReceitasSalvar.ExecuteOnAppearing();

            await Navigation.PushAsync(paginaReceitasSalvar);
            loadingIndicator.IsVisible = false;
            loadingIndicator.IsRunning = false;

        }

        private async void NovoClicked(object sender, EventArgs e)
        {
            var botao = sender as Button;
            if (sender != null)
            {
                var paginaReceitasSalvar = new ReceitasSalvarPage(new Receita { });

                await paginaReceitasSalvar.ExecuteOnAppearing();

                await Navigation.PushAsync(paginaReceitasSalvar);
            }

        }
        private async void CarregarReceitas()
        {
            ReceitasCollection.ItemsSource = await _receitasServico.TodosAsync();
        }

        private async void CarregarTotalizacao()
        {
            var qtdReceitas = await _receitasServico.QuantidadeAsync();
            LabelQtdReceitas.Text = qtdReceitas.ToString();
        }
    }
}
