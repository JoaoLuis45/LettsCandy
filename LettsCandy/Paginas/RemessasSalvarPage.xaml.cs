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
        private DatabaseServicos<Remessa> _remessasServico;
        private DatabaseServicos<RemessaLigacao> _remessaLigacaoServico;
        private DatabaseServicos<RemessaItem> _remessaItemServico;

        public RemessasSalvarPage(Remessa remessa)
        {
            InitializeComponent();
            _itensServico = new DatabaseServicos<Item>(Db.DB_PATH);
            _produtosServico = new DatabaseServicos<Produto>(Db.DB_PATH);
            _receitasServico = new DatabaseServicos<Receita>(Db.DB_PATH);
            _remessasServico = new DatabaseServicos<Remessa>(Db.DB_PATH);
            _remessaLigacaoServico = new DatabaseServicos<RemessaLigacao>(Db.DB_PATH);
            _remessaItemServico = new DatabaseServicos<RemessaItem>(Db.DB_PATH);
            Remessa = remessa;
            BindingContext = remessa;

            SituacaoRemessaPicker.ItemsSource = Enum.GetValues(typeof(SituacaoRemessa)).Cast<SituacaoRemessa>().ToList();
            SituacaoRemessaPicker.SelectedItem = remessa.SituacaoRemessa;


        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
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

            if (SituacaoRemessaPicker.SelectedItem != null)
                Remessa.SituacaoRemessa = (SituacaoRemessa)SituacaoRemessaPicker.SelectedItem;
            else
                Remessa.SituacaoRemessa = SituacaoRemessa.NaoProduzida;

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

            var remessaLigacao = new RemessaLigacao {
                RemessaId = Remessa.Id,
                Produtos = _produtosServico.TodosAsync().Result,
                Receitas = _receitasServico.TodosAsync().Result,
            };
            await _remessaLigacaoServico.IncluirAsync(remessaLigacao);
            Remessa.RemessaLigacoes.Add(remessaLigacao);

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
                if(item != null)
                {
                    await _remessaLigacaoServico.DeletarAsync(item);
                    item.Items.Clear();
                    CarregarRemessas();
                }
            }
        }

        private async void CarregarRemessas()
        {

            var remessaLigacao = await _remessaLigacaoServico.TodosAsync();
            var remessasLigacaoRelacionados = remessaLigacao
                .Where(ri => ri.Id == Remessa.Id).ToList();

            //var remessaItems = await _remessaItemServico.TodosAsync();
            //var itensRelacionados = remessaItems
            //    .Where(ri => ri.RemessaLigacaoId == RemessaLigacao.Id).ToList();

            //if (RemessaLigacao.Items.Count == 0 && itensRelacionados.Count > 0)
            //{
            //    var itens = new List<Item>();
            //    foreach (var remessaItem in itensRelacionados)
            //    {
            //        var item = await _itensServico.Query().Where(i => i.Id == remessaItem.ItemId).FirstOrDefaultAsync();
            //        if (item != null)
            //        {
            //            itens.Add(item);
            //        }
            //    }

            //    RemessaLigacao.Items = itens;
            //}

            //var valor = 0.0;
            //foreach (var item in itensRelacionados)
            //{
            //    valor += item.ValorItem * item.QtdItem;
            //}
            //Compra.Valor = valor;
            ProdutosCollection.ItemsSource = remessasLigacaoRelacionados;
        }

        private void AumentarQtd(object sender, EventArgs e)
        {
            var botao = sender as Button;
            if (botao != null)
            {
                var remessaItem = botao.BindingContext as RemessaItem;
                if (remessaItem != null)
                {
                    remessaItem.QtdItem++;
                    _remessaItemServico.AlterarAsync(remessaItem);
                    CarregarRemessas();
                }
            }
        }
        private void DiminuirQtd(object sender, EventArgs e)
        {
            var botao = sender as Button;
            if (botao != null)
            {
                var remessaItem = botao.BindingContext as RemessaItem;
                if (remessaItem != null)
                {
                    remessaItem.QtdItem--;
                    _remessaItemServico.AlterarAsync(remessaItem);
                    CarregarRemessas();
                }
            }
        }


        private void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            var entry = sender as Entry;
            if (entry != null)
            {
                var remessaItem = entry.BindingContext as RemessaItem;
                if (remessaItem != null)
                {
                    remessaItem.QtdItem = int.Parse(entry.Text);
                    _remessaItemServico.AlterarAsync(remessaItem);
                    CarregarRemessas();
                }
            }
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