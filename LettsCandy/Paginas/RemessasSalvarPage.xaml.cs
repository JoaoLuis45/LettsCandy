using System.Globalization;
using System.IO;
using MauiLib.Core.Constantes;
using MauiLib.Core.Enums;
using MauiLib.Core.Models;
using MauiLib.Core.Servicos;

namespace LettsCandy.Paginas
{
    public partial class RemessasSalvarPage : ContentPage
    {
        public Remessa Remessa { get; set; }

        private DatabaseServicos<Item> _itensServico;
        private DatabaseServicos<Produto> _produtosServico;
        private DatabaseServicos<Receita> _receitasServico;
        private DatabaseServicos<ReceitaItem> _receitaItemServico;
        private DatabaseServicos<Remessa> _remessasServico;
        private DatabaseServicos<RemessaLigacao> _remessaLigacaoServico;
        private DatabaseServicos<RemessaItem> _remessaItemServico;

        public RemessasSalvarPage(Remessa remessa)
        {
            InitializeComponent();
            _itensServico = new DatabaseServicos<Item>(Db.DB_PATH);
            _produtosServico = new DatabaseServicos<Produto>(Db.DB_PATH);
            _receitasServico = new DatabaseServicos<Receita>(Db.DB_PATH);
            _receitaItemServico = new DatabaseServicos<ReceitaItem>(Db.DB_PATH);
            _remessasServico = new DatabaseServicos<Remessa>(Db.DB_PATH);
            _remessaLigacaoServico = new DatabaseServicos<RemessaLigacao>(Db.DB_PATH);
            _remessaItemServico = new DatabaseServicos<RemessaItem>(Db.DB_PATH);
            Remessa = remessa;
            BindingContext = remessa;

        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            
        }

        public async Task ExecuteOnAppearing()
        {
            ProdutosPicker.ItemsSource = await _produtosServico.TodosAsync();
            ReceitasPicker.ItemsSource = await _receitasServico.TodosAsync();

            RemessaLigacaoFrame.IsVisible = Remessa.Id != 0;
            FrameTotalizacao.IsVisible = Remessa.Id != 0;
            ProduzirRemessaBtn.IsVisible = Remessa.Id != 0;

            CarregarRemessas();
        }


        private async void SalvarClicked(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(DescricaoEntry.Text))
            {
                await DisplayAlert("Erro", "A descrição é obrigatória", "Ok");
                DescricaoEntry.Focus();
                return;
            }

            Remessa.Descricao = DescricaoEntry.Text;

            if (Remessa.Id == 0)
                await _remessasServico.IncluirAsync(Remessa);
            else
            {
                await _remessasServico.AlterarAsync(Remessa);
            }

            CarregarRemessas();

            await Navigation.PopAsync();
        }

        private async void ExcluirClicked(object sender, EventArgs e)
        {
            if (Remessa.Id == 0)
            {
                await DisplayAlert("Erro", "Não é possível excluir uma remessa que não existe", "Ok");
                return;
            }

            bool confirm = await DisplayAlert("Confirmação", "Você tem certeza que deseja excluir esta remessa?", "Sim", "Não");
            if (!confirm)
            {
                return;
            }

            await _remessasServico.DeletarAsync(Remessa);
            await Navigation.PopAsync();
        }

        private async void AdicionarItemClicked(object sender, EventArgs e)
        {
            if (Remessa.Id == 0)
            {
                await DisplayAlert("Informação", "Salve a remessa primeiro para adicionar itens.", "Ok");
                return;
            }

            if (ProdutosPicker.SelectedItem == null)
            {
                await DisplayAlert("Erro", "O Produto é obrigatório", "Ok");
                ProdutosPicker.Focus();
                return;
            }

            if (ReceitasPicker.SelectedItem == null)
            {
                await DisplayAlert("Erro", "A receita é obrigatório", "Ok");
                ReceitasPicker.Focus();
                return;
            }

            if (string.IsNullOrEmpty(ProdutoQtdEntry.Text))
            {
                await DisplayAlert("Erro", "A quantidade é obrigatória", "Ok");
                ProdutoQtdEntry.Focus();
                return;
            }


            var remessaLigacao = new RemessaLigacao
            {
                RemessaId = Remessa.Id,
                ProdutoId = ((Produto)ProdutosPicker.SelectedItem).Id,
                ProdutoQtd = int.Parse(ProdutoQtdEntry.Text),
                ReceitaId = ((Receita)ReceitasPicker.SelectedItem).Id,
                ReceitaQtd = Math.Ceiling((double)int.Parse(ProdutoQtdEntry.Text) / ((Receita)ReceitasPicker.SelectedItem).QtdProdutosResultantes * 10) / 10,
            };

            await _remessaLigacaoServico.IncluirAsync(remessaLigacao);
            Remessa.RemessaLigacoes.Add(remessaLigacao);

            var ReceitaItems = await _receitaItemServico.Query().Where(ri => ri.ReceitaId == remessaLigacao.ReceitaId).ToListAsync();

            foreach (var item in ReceitaItems)
            {
                var remessaItem = new RemessaItem
                {
                    RemessaLigacaoId = remessaLigacao.Id,
                    ReceitaItemId = item.Id,
                    ReceitaNomeItem = item.NomeItem,
                    ReceitaItemQtd = Math.Round(remessaLigacao.ReceitaQtd * item.QtdItem, 1)
                };
                await _remessaItemServico.IncluirAsync(remessaItem);
            }

            CarregarRemessas();
        }

        private async void RemoverRemessaLigacaoClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Confirmação", "Você tem certeza que deseja excluir este produto?", "Sim", "Não");
            if (!confirm)
            {
                return;
            }

            var botao = sender as Button;
            if (botao != null)
            {
                var item = botao.BindingContext as RemessaLigacao;
                if (item != null)
                {
                    await _remessaLigacaoServico.DeletarAsync(item);
                    CarregarRemessas();
                }
            }
        }

        private async void CarregarRemessas()
        {
            var remessaLigacao = await _remessaLigacaoServico.TodosAsync();
            var remessasLigacaoRelacionados = remessaLigacao
                .Where(ri => ri.RemessaId == Remessa.Id).ToList();

            FrameTotalizacao.IsVisible = remessasLigacaoRelacionados.Count != 0;

            foreach (var i in remessasLigacaoRelacionados)
            {
                i.Produto = await _produtosServico.Query().Where(p => p.Id == i.ProdutoId).FirstOrDefaultAsync();
                i.Receita = await _receitasServico.Query().Where(p => p.Id == i.ReceitaId).FirstOrDefaultAsync();
                i.RemessaItems = await _remessaItemServico.Query().Where(p => p.RemessaLigacaoId == i.Id).ToListAsync();
            }

            ProdutosCollection.ItemsSource = remessasLigacaoRelacionados;
            CarregarTotalizacao(remessasLigacaoRelacionados);
        }

        private async void CarregarTotalizacao(List<RemessaLigacao> remessasRelacionadas)
        {
            var remessaItensDistintos = remessasRelacionadas
                .SelectMany(rl => rl.RemessaItems)
                .GroupBy(ri => ri.ReceitaNomeItem)
                .Select(g => new RemessaItem
                {
                    ReceitaNomeItem = g.Key,
                    ReceitaItemQtd = Math.Round(g.Sum(ri => ri.ReceitaItemQtd), 1)
                })
                .ToList();

            foreach (var item in remessaItensDistintos)
            {
                item.Item = await _itensServico.Query().Where(i => i.Nome == item.ReceitaNomeItem).FirstOrDefaultAsync();
            }

            TotRemessaItensCollection.ItemsSource = remessaItensDistintos;
        }

        private async void ProduzirRemessaClicked(object sender, EventArgs e)
        {

            if (Remessa.Id == 0)
            {
                await DisplayAlert("Informação", "Salve a remessa primeiro para produzir.", "Ok");
                return;
            }
            if (Remessa.SituacaoRemessa == SituacaoRemessa.Produzida)
            {
                await DisplayAlert("Informação", "Remessa já produzida!.", "Ok");
                return;
            }
            if (RemessaLigacaoFrame.IsVisible == false)
            {
                await DisplayAlert("Informação", "Adicione produtos e receitas para produzir.", "Ok");
                return;
            }
            

            var remessaLigacao = await _remessaLigacaoServico.TodosAsync();
            var remessasLigacaoRelacionados = remessaLigacao
                .Where(ri => ri.RemessaId == Remessa.Id).ToList();

            foreach (var i in remessasLigacaoRelacionados)
            {
                i.Produto = await _produtosServico.Query().Where(p => p.Id == i.ProdutoId).FirstOrDefaultAsync();
                i.Receita = await _receitasServico.Query().Where(p => p.Id == i.ReceitaId).FirstOrDefaultAsync();
                i.RemessaItems = await _remessaItemServico.Query().Where(p => p.RemessaLigacaoId == i.Id).ToListAsync();
            }


            var remessaItensDistintos = remessasLigacaoRelacionados
                .SelectMany(rl => rl.RemessaItems)
                .GroupBy(ri => ri.ReceitaNomeItem)
                .Select(g => new RemessaItem
                {
                    ReceitaNomeItem = g.Key,
                    ReceitaItemQtd = Math.Round(g.Sum(ri => ri.ReceitaItemQtd), 1)
                })
                .ToList();

            foreach (var item in remessaItensDistintos)
            {
                item.Item = await _itensServico.Query().Where(i => i.Nome == item.ReceitaNomeItem).FirstOrDefaultAsync();
            }

            foreach (var item in remessaItensDistintos)
            {
                if(item.ReceitaItemQtd > item.Item.Qtd)
                {
                    await DisplayAlert("Informação", $"Não é possível produzir a remessa pois a quamntidade do item {item.Item.Nome} é maior do que a quantidade em estoque.", "Ok");
                    return;
                }
            }

            if (RemessaLigacaoFrame.IsVisible == true)
            {
                var confirm = await DisplayAlert("Confirmação", "Você tem certeza que deseja produzir esta remessa?", "Sim", "Não");
                if (!confirm)
                {
                    return;
                }
            }

            foreach (var item in remessaItensDistintos)
            {
                item.Item = await _itensServico.Query().Where(i => i.Nome == item.ReceitaNomeItem).FirstOrDefaultAsync();
                item.Item.Qtd -= item.ReceitaItemQtd;
                DarBaixaDosInsumos(item.Item);
            }


            foreach (var item in remessasLigacaoRelacionados)
            {
                item.Produto.Quantidade += item.ProdutoQtd;
                DarEntradaDosProdutos(item.Produto);
            }

           Remessa.SituacaoRemessa = SituacaoRemessa.Produzida;
            await _remessasServico.AlterarAsync(Remessa);
            await Navigation.PopAsync();
        }


        private async void DarBaixaDosInsumos(Item item)
        {
            await _itensServico.AlterarAsync(item);
        }

        private async void DarEntradaDosProdutos(Produto produto)
        {
            await _produtosServico.AlterarAsync(produto);
        }


        private async void ProdutoQtdEntry_Unfocused(object sender, FocusEventArgs e)
        {
            var entry = sender as Entry;
            if (entry != null)
            {
                var remessaLigacao = entry.BindingContext as RemessaLigacao;
                if (remessaLigacao != null)
                {
                    var qtd = int.Parse(entry.Text);
                    remessaLigacao.ProdutoQtd = qtd;
                    await _remessaLigacaoServico.AlterarAsync(remessaLigacao);
                    CarregarRemessas();
                }
            }
        }

    }
}