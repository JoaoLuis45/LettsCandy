using System;
using System.Windows.Input;
using MauiLib.Core.Constantes;
using MauiLib.Core.Models;
using MauiLib.Core.Servicos;

namespace LettsCandy.Paginas
{
    public partial class ProdutosPage : ContentPage
    {
        public Produto Produto { get; set; }

        private DatabaseServicos<Produto> _produtoServico;
        public ICommand NavigateToProdutosSalvarCommand { get; private set; }
        
        public ProdutosPage()
        {
            InitializeComponent();
            _produtoServico = new DatabaseServicos<Produto>(Db.DB_PATH);
            NavigateToProdutosSalvarCommand = new Command<Produto>(async (produto) => await NavigateToProductSalvar(produto));
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            CarregarProdutos();
            CarregarTotalizacao();
        }

        private async Task NavigateToProductSalvar(Produto produto)
        {
            await Navigation.PushAsync(new ProdutosSalvarPage(produto));
        }

        private async void NovoClicked(object sender, EventArgs e)
        {
            var botao = sender as Button;
            if (sender != null)
            {
                await Navigation.PushAsync(new ProdutosSalvarPage(new Produto{}));
            }

        }
        private async void CarregarProdutos()
        {
            ProdutosCollection.ItemsSource = await _produtoServico.TodosAsync();
        }

        private async void CarregarTotalizacao()
        {
            var qtdProdutos = await _produtoServico.QuantidadeAsync();
            var produtos = await _produtoServico.TodosAsync();
            int estoque = 0;
            foreach (var item in produtos)
            {
                estoque += item.Quantidade;
            }

            LabelQtdProdutos.Text = qtdProdutos.ToString();
            LabelEstoque.Text = estoque.ToString();
        }

      
        private void DiminuirQtd(object sender, EventArgs e)
        {
            var botao = sender as Button;
            if (botao != null)
            {
                var produto = botao.BindingContext as Produto;
                if (produto != null)
                {
                    produto.Quantidade--;

                    _produtoServico.AlterarAsync(produto);

                    CarregarProdutos();
                    CarregarTotalizacao();
                }
            }
        }


        private void AumentarQtd(object sender, EventArgs e)
        {
            var botao = sender as Button;
            if (botao != null)
            {
                var produto = botao.BindingContext as Produto;
                if (produto != null)
                {
                    produto.Quantidade++;

                    _produtoServico.AlterarAsync(produto);

                    CarregarProdutos();
                    CarregarTotalizacao();
                }
            }
        }
    }
}
