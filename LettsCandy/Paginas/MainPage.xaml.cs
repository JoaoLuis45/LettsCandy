using System.Windows.Input;
using MauiLib.Core.Constantes;
using MauiLib.Core.Models;
using MauiLib.Core.Servicos;

namespace LettsCandy.Paginas
{
    public partial class MainPage : ContentPage
    {
        DatabaseServicos<Produto> _produtoServico;
        DatabaseServicos<Item> _itensServico;

        public MainPage()
        {
            InitializeComponent();
            _produtoServico = new DatabaseServicos<Produto>(Db.DB_PATH);
            _itensServico = new DatabaseServicos<Item>(Db.DB_PATH);

        }

        private async Task<bool> VerificaQtdItems()
        {
            var qtdItens = await _itensServico.QuantidadeAsync();
            if (qtdItens == 0) 
            {
                await DisplayAlert("Informação", "Não Existem itens cadastrados. Cadastre para poder acessar a tela de receitas.", "Ok");
                return true;
            }
            return false;
        }

        private async Task<bool> VerificaQtdProdutos()
        {
            var qtdProdutos = await _produtoServico.QuantidadeAsync();
            if (qtdProdutos == 0)
            {
                await DisplayAlert("Informação", "Não Existem produtos cadastrados. Cadastre para poder acessar a tela de receitas.", "Ok");
                return true;
            }
            return false;
        }

        private async void NavigateToProduct(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new ProdutosPage());
        }

        private async void NavigateToUsers(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new UsuariosPage());
        }
        private async void NavigateToClients(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new ClientesPage());
        }
        private async void NavigateToItems(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new ItensPage());
        }
        private async void NavigateToReceipts(object sender, TappedEventArgs e)
        {
            if (await VerificaQtdProdutos()) return;
            if (await VerificaQtdItems()) return;
            await Navigation.PushAsync(new ReceitasPage());
        }
    }
}
