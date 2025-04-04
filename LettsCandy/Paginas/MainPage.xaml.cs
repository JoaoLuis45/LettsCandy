using System.Windows.Input;
using MauiLib.Core.Constantes;
using MauiLib.Core.Models;
using MauiLib.Core.Servicos;

namespace LettsCandy.Paginas
{
    public partial class MainPage : ContentPage
    {
        DatabaseServicos<Produto> _produtoServico;

        public MainPage()
        {
            InitializeComponent();
            _produtoServico = new DatabaseServicos<Produto>(Db.DB_PATH);

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
            await Navigation.PushAsync(new ReceitasPage());
        }
    }
}
