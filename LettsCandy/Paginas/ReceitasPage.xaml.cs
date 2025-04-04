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
            await Navigation.PushAsync(new ReceitasSalvarPage(receita));
        }

        private async void NovoClicked(object sender, EventArgs e)
        {
            var botao = sender as Button;
            if (sender != null)
            {
                await Navigation.PushAsync(new ReceitasSalvarPage(new Receita { }));
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
