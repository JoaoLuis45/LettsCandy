using System.Globalization;
using System.IO;
using MauiLib.Core.Constantes;
using MauiLib.Core.Enums;
using MauiLib.Core.Models;
using MauiLib.Core.Servicos;

namespace LettsCandy.Paginas
{
    public partial class ComprasSalvarPage : ContentPage
    {
        public Compra Compra { get; set; }

        private DatabaseServicos<Item> _itensServico;
        private DatabaseServicos<Compra> _comprasServico;
        private DatabaseServicos<CompraItem> _compraItemsServico;

        public ComprasSalvarPage(Compra compra)
        {
            InitializeComponent();
            _itensServico = new DatabaseServicos<Item>(Db.DB_PATH);
            _comprasServico = new DatabaseServicos<Compra>(Db.DB_PATH);
            _compraItemsServico = new DatabaseServicos<CompraItem>(Db.DB_PATH);
            Compra = compra;
            BindingContext = compra;


        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            CarregarCompraItems();
            FinalizarCompraBtn.IsVisible = Compra.Id != 0;
        }


        private async void SalvarClicked(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(DescricaoEntry.Text))
            {
                await DisplayAlert("Erro", "A descrição é obrigatória", "Ok");
                DescricaoEntry.Focus();
                return;
            }

            Compra.Descricao = DescricaoEntry.Text;

            if (Compra.Id == 0)
                await _comprasServico.IncluirAsync(Compra);
            else
            {
                await _comprasServico.AlterarAsync(Compra);
            }

            CarregarCompraItems();

            await Navigation.PopAsync();
        }

        private async void ExcluirClicked(object sender, EventArgs e)
        {
            if (Compra.Id == 0)
            {
                await DisplayAlert("Erro", "Não é possível excluir uma compra que não existe", "Ok");
                return;
            }

            bool confirm = await DisplayAlert("Confirmação", "Você tem certeza que deseja excluir esta compra?", "Sim", "Não");
            if (!confirm)
            {
                return;
            }

            await _comprasServico.DeletarAsync(Compra);
            await Navigation.PopAsync();
        }
        private async void FinalizarCompraClicked(object sender, EventArgs e)
        {
            if (Compra.Id == 0)
            {
                await DisplayAlert("Erro", "Não é possível finalizar uma compra que não existe", "Ok");
                return;
            }

            var itens = await _compraItemsServico.Query().Where(i => i.CompraId == Compra.Id).ToListAsync();
            if (itens.Count == 0)
            {
                await DisplayAlert("Erro", "Não é possível finalizar uma compra sem itens", "Ok");
                return;
            }

            foreach (var item in itens)
            {
                if(item.QtdItem <= 0)
                {
                    await DisplayAlert("Erro", "Existem itens com a quantidade zerada. Exclua-os ou adicione uma quantidade válida!", "Ok");
                    return;
                }
            }

            bool confirm = await DisplayAlert("Confirmação", "Você tem certeza que deseja excluir esta compra?", "Sim", "Não");
            if (!confirm)
            {
                return;
            }

            foreach (var item in itens)
            {
                var itemRelacionado = await _itensServico.Query().Where(i => i.Id == item.ItemId).FirstOrDefaultAsync();
                if (itemRelacionado != null)
                {
                    itemRelacionado.Qtd += item.QtdItem;
                    await _itensServico.AlterarAsync(itemRelacionado);
                }
            }

            Compra.SituacaoCompra = SituacaoCompra.Finalizada;
            await _comprasServico.AlterarAsync(Compra);

            await Navigation.PopAsync();
        }

        private async void AdicionarItemClicked(object sender, EventArgs e)
        {
            if (Compra.Id == 0)
            {
                await DisplayAlert("Informação", "Salve a compra primeiro para adicionar itens.", "Ok");
                return;
            }

            var todosItens = await _itensServico.TodosAsync();
            var itensNaoAdicionados = todosItens.Where(i => !Compra.Items.Any(ri => ri.Id == i.Id)).ToList();

            if (itensNaoAdicionados.Count == 0)
            {
                await DisplayAlert("Informação", "Não há itens disponíveis para adicionar", "Ok");
                return;
            }

            string[] itensNomes = itensNaoAdicionados.Select(i => i.Nome).ToArray();
            string itemSelecionado = await DisplayActionSheet("Selecione um item para adicionar", "Cancelar", null, itensNomes);

            if (itemSelecionado != "Cancelar" && !string.IsNullOrEmpty(itemSelecionado))
            {
                var item = itensNaoAdicionados.FirstOrDefault(i => i.Nome == itemSelecionado);
                if (item != null)
                {
                    Compra.Items.Add(item);
                    await _compraItemsServico.IncluirAsync(new CompraItem
                    {
                        CompraId = Compra.Id,
                        ItemId = item.Id,
                        NomeItem = item.Nome,
                    });
                    CarregarCompraItems();
                }
            }
        }

        private async void RemoverItemClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Confirmação", "Você tem certeza que deseja excluir este item?", "Sim", "Não");
            if (!confirm)
            {
                return;
            }

            var botao = sender as Button;
            if (botao != null)
            { 
                var item = botao.BindingContext as CompraItem;
                if(item != null)
                {
                    await _compraItemsServico.DeletarAsync(item);
                    Compra.Items.Clear();
                    CarregarCompraItems();
                }
            }
        }

        private async void CarregarCompraItems()
        {
            var compraItems = await _compraItemsServico.TodosAsync();
            var itensRelacionados = compraItems
                .Where(ri => ri.CompraId == Compra.Id).ToList();

            if (Compra.Items.Count == 0 && itensRelacionados.Count > 0)
            {
                var itens = new List<Item>();
                foreach (var compraItem in itensRelacionados)
                {
                    var item = await _itensServico.Query().Where(i => i.Id == compraItem.ItemId).FirstOrDefaultAsync();
                    if (item != null)
                    {
                        itens.Add(item);
                    }
                }

                Compra.Items = itens;
            }

            var valor = 0.0;
            foreach (var item in itensRelacionados)
            {
                valor += item.ValorItem * item.QtdItem;
            }
            Compra.Valor = valor;
            await _comprasServico.AlterarAsync(Compra);
            ItemsCollection.ItemsSource = itensRelacionados;
        }

        private void AumentarQtd(object sender, EventArgs e)
        {
            var botao = sender as Button;
            if (botao != null)
            {
                var compraItem = botao.BindingContext as CompraItem;
                if (compraItem != null)
                {
                    compraItem.QtdItem++;
                    _compraItemsServico.AlterarAsync(compraItem);
                    CarregarCompraItems();
                }
            }
        }
        private void DiminuirQtd(object sender, EventArgs e)
        {
            var botao = sender as Button;
            if (botao != null)
            {
                var compraItem = botao.BindingContext as CompraItem;
                if (compraItem != null)
                {
                    compraItem.QtdItem--;
                    _compraItemsServico.AlterarAsync(compraItem);
                    CarregarCompraItems();
                }
            }
        }


        private void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            var entry = sender as Entry;
            if (entry != null)
            {
                var compraItem = entry.BindingContext as CompraItem;
                if (compraItem != null)
                {
                    compraItem.QtdItem = int.Parse(entry.Text);
                    _compraItemsServico.AlterarAsync(compraItem);
                    CarregarCompraItems();
                }
            }
        }

        private async void ValorItemEntry_Unfocused(object sender, FocusEventArgs e)
        {
            var entry = sender as Entry;
            if (entry != null)
            {
                entry.Text = entry.Text.Replace(',', '.');
                var compraItem = entry.BindingContext as CompraItem;
                if (compraItem != null)
                {
                    if (double.TryParse(entry.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double valor))
                    {
                        compraItem.ValorItem = valor;
                        await _compraItemsServico.AlterarAsync(compraItem);
                        CarregarCompraItems();
                    }
                }
            }
        }
    }
}