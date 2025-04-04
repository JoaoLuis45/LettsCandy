using System;
using System.Windows.Input;
using MauiLib.Core.Constantes;
using MauiLib.Core.Models;
using MauiLib.Core.Servicos;

namespace LettsCandy.Paginas
{
    public partial class ItensPage : ContentPage
    {
        public Item Item { get; set; }

        private DatabaseServicos<Item> _itensServico;
        public ICommand NavigateToItensSalvarCommand { get; private set; }
        
        public ItensPage()
        {
            InitializeComponent();
            _itensServico = new DatabaseServicos<Item>(Db.DB_PATH);
            NavigateToItensSalvarCommand = new Command<Item>(async (item) => await NavigateToItemSalvar(item));
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            CarregarItens();
            CarregarTotalizacao();
        }

        private async Task NavigateToItemSalvar(Item item)
        {
            await Navigation.PushAsync(new ItensSalvarPage(item));
        }

        private async void NovoClicked(object sender, EventArgs e)
        {
            var botao = sender as Button;
            if (sender != null)
            {
                await Navigation.PushAsync(new ItensSalvarPage(new Item{}));
            }

        }
        private async void CarregarItens()
        {
            ItensCollection.ItemsSource = await _itensServico.TodosAsync();
        }

        private async void CarregarTotalizacao()
        {
            var qtdItens = await _itensServico.QuantidadeAsync();
            var itens = await _itensServico.TodosAsync();
            int qtdTotal = 0;
            foreach (var i in itens)
            {
                qtdTotal += i.Qtd;
            }

            LabelQtdItens.Text = qtdItens.ToString();
            LabelTotalItens.Text = qtdTotal.ToString();
        }

      
        private void DiminuirQtd(object sender, EventArgs e)
        {
            var botao = sender as Button;
            if (botao != null)
            {
                var item = botao.BindingContext as Item;
                if (item != null)
                {
                    item.Qtd--;

                    _itensServico.AlterarAsync(item);

                    CarregarItens();
                    CarregarTotalizacao();
                }
            }
        }


        private void AumentarQtd(object sender, EventArgs e)
        {
            var botao = sender as Button;
            if (botao != null)
            {
                var item = botao.BindingContext as Item;
                if (item != null)
                {
                    item.Qtd++;

                    _itensServico.AlterarAsync(item);

                    CarregarItens();
                    CarregarTotalizacao();
                }
            }
        }
    }
}
