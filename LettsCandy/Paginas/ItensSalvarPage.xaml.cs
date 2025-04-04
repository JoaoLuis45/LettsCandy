using MauiLib.Core.Constantes;
using MauiLib.Core.Models;
using MauiLib.Core.Servicos;

namespace LettsCandy.Paginas
{
    public partial class ItensSalvarPage : ContentPage
    {
        public Item Item { get; set; }

        private DatabaseServicos<Item> _itensServico;

        public ItensSalvarPage(Item item)
        {
            InitializeComponent();
            _itensServico = new DatabaseServicos<Item>(Db.DB_PATH);
            Item = item;
            BindingContext = item;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await Task.Delay(100);
            NomeEntry.Focus();
        }

        private async void SalvarClicked(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(NomeEntry.Text))
            {
                await DisplayAlert("Erro", "O nome do item é obrigatório", "Ok");
                NomeEntry.Focus();
                return;
            }
            if (string.IsNullOrEmpty(QtdEntry.Text))
            {
                await DisplayAlert("Erro", "A quantidade do item é obrigatória", "Ok");
                QtdEntry.Focus();
                return;
            }


            Item.Nome = NomeEntry.Text;
            Item.Qtd = int.Parse(QtdEntry.Text);

            if (Item.Id == 0)
                await _itensServico.IncluirAsync(Item);
            else
            {
                Item.DataAtualizacao = DateTime.Now;
                await _itensServico.AlterarAsync(Item);
            }
            await Navigation.PopAsync();
        }

        private async void ExcluirClicked(object sender, EventArgs e)
        {
            if (Item.Id == 0)
            {
                await DisplayAlert("Erro", "Não é possível excluir um item que não existe", "Ok");
                return;
            }

            bool confirm = await DisplayAlert("Confirmação", "Você tem certeza que deseja excluir este item?", "Sim", "Não");
            if (!confirm)
            {
                return;
            }

            await _itensServico.DeletarAsync(Item);
            await Navigation.PopAsync();
        }
    }
}