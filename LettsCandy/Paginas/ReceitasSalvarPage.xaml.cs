using System.Globalization;
using System.IO;
using MauiLib.Core.Constantes;
using MauiLib.Core.Enums;
using MauiLib.Core.Models;
using MauiLib.Core.Servicos;

namespace LettsCandy.Paginas
{
    public partial class ReceitasSalvarPage : ContentPage
    {
        public Receita Receita { get; set; }

        private DatabaseServicos<Receita> _receitasServico;
        private DatabaseServicos<Produto> _produtosServico;
        private DatabaseServicos<Item> _itensServico;
        private DatabaseServicos<ReceitaItem> _receitaItemServico;

        public ReceitasSalvarPage(Receita receita)
        {
            InitializeComponent();
            _receitasServico = new DatabaseServicos<Receita>(Db.DB_PATH);
            _produtosServico = new DatabaseServicos<Produto>(Db.DB_PATH);
            _itensServico = new DatabaseServicos<Item>(Db.DB_PATH);
            _receitaItemServico = new DatabaseServicos<ReceitaItem>(Db.DB_PATH);
            Receita = receita;
            BindingContext = receita;

            
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            ProdutosPicker.ItemsSource = await _produtosServico.TodosAsync();
            ProdutoFrame.IsVisible = Receita.Id != 0;
            if (Receita.Id != 0)
            {
                var produto = _produtosServico.TodosAsync().Result.FirstOrDefault(p => p.Id == Receita.ProdutoId);
                CarregarFrameProduto(produto);
                ProdutosPicker.Title = produto.Nome;
                ProdutosPicker.SelectedItem = produto;
            }
            CarregarReceitasItems();
        }


        private async void SalvarClicked(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(DescricaoEntry.Text))
            {
                await DisplayAlert("Erro", "A descrição é obrigatória", "Ok");
                DescricaoEntry.Focus();
                return;
            }

            if (string.IsNullOrEmpty(QtdProdutosResultantesEntry.Text))
            {
                await DisplayAlert("Erro", "A quantidade de produtos resultantes é obrigatória", "Ok");
                QtdProdutosResultantesEntry.Focus();
                return;
            }

            if (ProdutosPicker.SelectedItem != null)
                Receita.Produto = (Produto)ProdutosPicker.SelectedItem;
            else
            {
                await DisplayAlert("Erro", "O Produto é obrigatório", "Ok");
                ProdutosPicker.Focus();
                return;
            }

            Receita.Descricao = DescricaoEntry.Text;
            Receita.ProdutoNome = Receita.Produto.Nome;
            Receita.ProdutoId = Receita.Produto.Id;
            Receita.QtdProdutosResultantes = int.Parse(QtdProdutosResultantesEntry.Text);

            if (Receita.Id == 0)
                await _receitasServico.IncluirAsync(Receita);
            else
            {
                Receita.DataAtualizacao = DateTime.Now;
                await _receitasServico.AlterarAsync(Receita);
            }
            await Navigation.PopAsync();
        }

        private async void ExcluirClicked(object sender, EventArgs e)
        {
            if (Receita.Id == 0)
            {
                await DisplayAlert("Erro", "Não é possível excluir uma receita que não existe", "Ok");
                return;
            }

            bool confirm = await DisplayAlert("Confirmação", "Você tem certeza que deseja excluir esta receita?", "Sim", "Não");
            if (!confirm)
            {
                return;
            }

            await _receitasServico.DeletarAsync(Receita);
            await Navigation.PopAsync();
        }

        private async void AdicionarItemClicked(object sender, EventArgs e)
        {
            if(Receita.Id == 0)
            {
                await DisplayAlert("Informação", "Salve a receita primeiro para adicionar itens.", "Ok");
                return;
            }

            var todosItens = await _itensServico.TodosAsync();
            var itensNaoAdicionados = todosItens.Where(i => !Receita.Items.Any(ri => ri.Id == i.Id)).ToList();

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
                    Receita.Items.Add(item);
                    await _receitaItemServico.IncluirAsync(new ReceitaItem
                    {
                        ReceitaId = Receita.Id,
                        ItemId = item.Id,
                        NomeItem = item.Nome,
                    });
                    CarregarReceitasItems();   
                }
            }
        }

        private void ProdutosPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            if (picker?.SelectedItem != null)
            {
                var produtoSelecionado = _produtosServico.TodosAsync().Result.FirstOrDefault(p => p.Id == ((Produto)picker.SelectedItem).Id);
                if (produtoSelecionado != null)
                {
                    CarregarFrameProduto(produtoSelecionado);
                    ProdutoFrame.IsVisible = true;
                    return;
                }
            }
            ProdutoFrame.IsVisible = false;
        }

        private void CarregarFrameProduto(Produto produto)
        {
            FotoProduto.Source = produto.Foto;
            NomeProdutoLabel.Text = produto.Nome;
            DescricaoProdutoLabel.Text = produto.Descricao;
        }

        private async void CarregarReceitasItems()
        {
            var receitaItems = await _receitaItemServico.TodosAsync();
            var itensRelacionados = receitaItems
                .Where(ri => ri.ReceitaId == Receita.Id).ToList();
            ItemsCollection.ItemsSource = itensRelacionados;
        }

        private void AumentarQtd(object sender, EventArgs e)
        {
            var botao = sender as Button;
            if (botao != null)
            {
                var receitaItem = botao.BindingContext as ReceitaItem;
                if (receitaItem != null)
                {
                    receitaItem.QtdItem++;
                    _receitaItemServico.AlterarAsync(receitaItem);
                    CarregarReceitasItems();
                }
            }
        }
        private void DiminuirQtd(object sender, EventArgs e)
        {
            var botao = sender as Button;
            if (botao != null)
            {
                var receitaItem = botao.BindingContext as ReceitaItem;
                if (receitaItem != null)
                {
                    receitaItem.QtdItem--;
                    _receitaItemServico.AlterarAsync(receitaItem);
                    CarregarReceitasItems();
                }
            }
        }


        private void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            var entry = sender as Entry;
            if (entry != null)
            {
                var receitaItem = entry.BindingContext as ReceitaItem;
                if (receitaItem != null)
                {
                    receitaItem.QtdItem = int.Parse(entry.Text);
                    _receitaItemServico.AlterarAsync(receitaItem);
                    CarregarReceitasItems();
                }
            }
        }
    }
}