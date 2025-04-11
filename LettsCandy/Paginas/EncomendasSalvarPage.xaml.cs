using System.Globalization;
using System.IO;
using MauiLib.Core.Constantes;
using MauiLib.Core.Enums;
using MauiLib.Core.Models;
using MauiLib.Core.Servicos;

namespace LettsCandy.Paginas
{
    public partial class EncomendasSalvarPage : ContentPage
    {
        public Encomenda Encomenda { get; set; }

        private DatabaseServicos<Produto> _produtosServico;
        private DatabaseServicos<Cliente> _clienteServico;
        private DatabaseServicos<Encomenda> _encomendasServico;
        private DatabaseServicos<EncomendaItem> _encomendaItemsServico;

        public EncomendasSalvarPage(Encomenda encomenda)
        {
            InitializeComponent();
            _produtosServico = new DatabaseServicos<Produto>(Db.DB_PATH);
            _clienteServico = new DatabaseServicos<Cliente>(Db.DB_PATH);
            _encomendasServico = new DatabaseServicos<Encomenda>(Db.DB_PATH);
            _encomendaItemsServico = new DatabaseServicos<EncomendaItem>(Db.DB_PATH);
            Encomenda = encomenda;
            BindingContext = encomenda;


        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
           
        }

        public async Task ExecuteOnAppearing()
        {
            CarregarEncomendaItems();
            if (Encomenda.Id == 0)
            {
                ClientesPicker.ItemsSource = await _produtosServico.TodosAsync();
            }
            else
            {
                var cliente = _clienteServico.TodosAsync().Result.FirstOrDefault(p => p.Id == Encomenda.ClienteId);
                ClientesPicker.Title = cliente.Nome;
                ClientesPicker.SelectedItem = cliente;
            }
            
            StatusBtn.IsVisible = Encomenda.Id != 0;
            switch (Encomenda.SituacaoEncomenda)
            {
                case SituacaoEncomenda.Iniciada:
                    StatusBtn.Text = "Reservar Encomenda";
                    break;
                case SituacaoEncomenda.Reservada:
                    StatusBtn.Text = "Finalizar Encomenda";
                    break;
            }
        }


        private async void SalvarClicked(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(DescricaoEntry.Text))
            {
                await DisplayAlert("Erro", "A descrição é obrigatória", "Ok");
                DescricaoEntry.Focus();
                return;
            }

            if (ClientesPicker.SelectedItem != null)
                Encomenda.Cliente = (Cliente)ClientesPicker.SelectedItem;
            else
            {
                await DisplayAlert("Erro", "O Cliente é obrigatório", "Ok");
                ClientesPicker.Focus();
                return;
            }

            Encomenda.Descricao = DescricaoEntry.Text;

            if (Encomenda.Id == 0)
                await _encomendasServico.IncluirAsync(Encomenda);
            else
            {
                await _encomendasServico.AlterarAsync(Encomenda);
            }

            CarregarEncomendaItems();

            await Navigation.PopAsync();
        }

        private async void ExcluirClicked(object sender, EventArgs e)
        {
            if (Encomenda.Id == 0)
            {
                await DisplayAlert("Erro", "Não é possível excluir uma encomenda que não existe", "Ok");
                return;
            }

            bool confirm = await DisplayAlert("Confirmação", "Você tem certeza que deseja excluir esta encomenda?", "Sim", "Não");
            if (!confirm)
            {
                return;
            }

            await _encomendasServico.DeletarAsync(Encomenda);
            await Navigation.PopAsync();
        }

        private async void StatusBtnClicked(object sender, EventArgs e)
        {
            if (Encomenda.Id == 0)
            {
                await DisplayAlert("Erro", "Não é possível alterar a situação de uma encomenda que não existe", "Ok");
                return;
            }

            var produtos = await _encomendaItemsServico.Query().Where(i => i.EncomendaId == Encomenda.Id).ToListAsync();
            if (produtos.Count == 0)
            {
                await DisplayAlert("Erro", "Não é possível finalizar uma encomenda sem produtos", "Ok");
                return;
            }

            foreach (var item in produtos)
            {
                if (item.QtdProduto <= 0)
                {
                    await DisplayAlert("Erro", "Existem produtos com a quantidade zerada. Exclua-os ou adicione uma quantidade válida!", "Ok");
                    return;
                }
            }
            bool confirm;
            switch (Encomenda.SituacaoEncomenda)
            {
                case SituacaoEncomenda.Iniciada:
                    confirm = await DisplayAlert("Confirmação", "Você tem certeza que deseja reservar esta encomenda?", "Sim", "Não");
                    if (!confirm)
                    {
                        return;
                    }
                    Encomenda.SituacaoEncomenda = SituacaoEncomenda.Reservada;
                    await _encomendasServico.AlterarAsync(Encomenda);
                    break;
                case SituacaoEncomenda.Reservada:
                    confirm = await DisplayAlert("Confirmação", "Você tem certeza que deseja finalizar esta encomenda?", "Sim", "Não");
                    if (!confirm)
                    {
                        return;
                    }
                    var EncomendaItems = await _encomendaItemsServico.TodosAsync();
                    var ProdutosRelacionados = EncomendaItems
                        .Where(ri => ri.EncomendaId == Encomenda.Id).ToList();
                    foreach (var EncomendaItem in ProdutosRelacionados)
                    {
                        var produto = await _produtosServico.Query().Where(i => i.Id == EncomendaItem.ProdutoId).FirstOrDefaultAsync();
                        if (produto != null)
                        {
                            produto.Quantidade -= EncomendaItem.QtdProduto;
                            await _produtosServico.AlterarAsync(produto);
                        }
                    }
                    Encomenda.SituacaoEncomenda = SituacaoEncomenda.Finalizada;
                    Encomenda.DataFinalizacao = DateTime.Now;
                    await _encomendasServico.AlterarAsync(Encomenda);
                    break;
            }
            await Navigation.PopAsync();
        }

        private async void AdicionarProdutoClicked(object sender, EventArgs e)
        {
            if (Encomenda.Id == 0)
            {
                await DisplayAlert("Informação", "Salve a encomenda primeiro para adicionar produtos.", "Ok");
                return;
            }

            var todosProdutos = await _produtosServico.TodosAsync();
            var produtosNaoAdicionados = todosProdutos.Where(i => !Encomenda.Produtos.Any(ri => ri.Id == i.Id)).ToList();

            if (produtosNaoAdicionados.Count == 0)
            {
                await DisplayAlert("Informação", "Não há produtos disponíveis para adicionar", "Ok");
                return;
            }

            string[] ProdutoNomes = produtosNaoAdicionados.Select(i => i.Nome).ToArray();
            string ProdutoSelecionado = await DisplayActionSheet("Selecione um item para adicionar", "Cancelar", null, ProdutoNomes);

            if (ProdutoSelecionado != "Cancelar" && !string.IsNullOrEmpty(ProdutoSelecionado))
            {
                var produto = produtosNaoAdicionados.FirstOrDefault(i => i.Nome == ProdutoSelecionado);
                if (produto != null)
                {
                    Encomenda.Produtos.Add(produto);
                    await _encomendaItemsServico.IncluirAsync(new EncomendaItem
                    {
                        EncomendaId = Encomenda.Id,
                        ProdutoId = produto.Id,
                        NomeProduto = produto.Nome,
                    });
                    CarregarEncomendaItems();
                }
            }
        }

        private async void RemoverProdutoClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Confirmação", "Você tem certeza que deseja excluir este produto?", "Sim", "Não");
            if (!confirm)
            {
                return;
            }

            var botao = sender as Button;
            if (botao != null)
            { 
                var produto = botao.BindingContext as EncomendaItem;
                if(produto != null)
                {
                    await _encomendaItemsServico.DeletarAsync(produto);
                    CarregarEncomendaItems();
                }
            }
        }

        private async void CarregarEncomendaItems()
        {
            var EncomendaItems = await _encomendaItemsServico.TodosAsync();
            var ProdutosRelacionados = EncomendaItems
                .Where(ri => ri.EncomendaId == Encomenda.Id).ToList();

            if (Encomenda.Produtos.Count == 0 && ProdutosRelacionados.Count > 0)
            {
                var produtos = new List<Produto>();
                foreach (var EncomendaItem in ProdutosRelacionados)
                {
                    var produto = await _produtosServico.Query().Where(i => i.Id == EncomendaItem.ProdutoId).FirstOrDefaultAsync();
                    if (produto != null)
                    {
                        produtos.Add(produto);
                    }
                }

                Encomenda.Produtos = produtos;
            }

            var valor = 0.0;
            foreach (var produto in ProdutosRelacionados)
            {
                valor += produto.ValorProduto * produto.QtdProduto;
            }
            Encomenda.Valor = valor;
            await _encomendasServico.AlterarAsync(Encomenda);
            ProdutosCollection.ItemsSource = ProdutosRelacionados;
        }

        private void AumentarQtd(object sender, EventArgs e)
        {
            var botao = sender as Button;
            if (botao != null)
            {
                var encomendaItem = botao.BindingContext as EncomendaItem;
                if (encomendaItem != null)
                {
                    encomendaItem.QtdProduto++;
                    _encomendaItemsServico.AlterarAsync(encomendaItem);
                    CarregarEncomendaItems();
                }
            }
        }
        private void DiminuirQtd(object sender, EventArgs e)
        {
            var botao = sender as Button;
            if (botao != null)
            {
                var encomendaItem = botao.BindingContext as EncomendaItem;
                if (encomendaItem != null)
                {
                    encomendaItem.QtdProduto--;
                    _encomendaItemsServico.AlterarAsync(encomendaItem);
                    CarregarEncomendaItems();
                }
            }
        }


        private void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            var entry = sender as Entry;
            if (entry != null)
            {
                var encomendaItem = entry.BindingContext as EncomendaItem;
                if (encomendaItem != null)
                {
                    encomendaItem.QtdProduto = int.Parse(entry.Text);
                    _encomendaItemsServico.AlterarAsync(encomendaItem);
                    CarregarEncomendaItems();
                }
            }
        }
    }
}