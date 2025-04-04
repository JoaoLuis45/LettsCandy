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

        public ReceitasSalvarPage(Receita receita)
        {
            InitializeComponent();
            _receitasServico = new DatabaseServicos<Receita>(Db.DB_PATH);
            _produtosServico = new DatabaseServicos<Produto>(Db.DB_PATH);
            Receita = receita;
            BindingContext = receita;


            
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            var produtos = await _produtosServico.TodosAsync();
            ProdutosPicker.ItemsSource = produtos.Select(p => p.Nome).ToList();
            ProdutosPicker.SelectedItem = Receita.Produto.Nome;
        }

        private async void SalvarClicked(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(DescricaoEntry.Text))
            {
                await DisplayAlert("Erro", "A descrição é obrigatória", "Ok");
                DescricaoEntry.Focus();
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
    }
}