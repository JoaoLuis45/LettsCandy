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
    }
}
