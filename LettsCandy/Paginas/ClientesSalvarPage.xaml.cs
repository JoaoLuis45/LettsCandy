using System.Globalization;
using System.IO;
using MauiLib.Core.Constantes;
using MauiLib.Core.Enums;
using MauiLib.Core.Models;
using MauiLib.Core.Servicos;

namespace LettsCandy.Paginas
{
    public partial class ClientesSalvarPage : ContentPage
    {
        public Cliente Cliente { get; set; }

        private DatabaseServicos<Cliente> _clientesServico;

        public ClientesSalvarPage(Cliente cliente)
        {
            InitializeComponent();
            _clientesServico = new DatabaseServicos<Cliente>(Db.DB_PATH);
            Cliente = cliente;
            BindingContext = cliente;

            SituacaoClientePicker.ItemsSource = Enum.GetValues(typeof(SituacaoCliente)).Cast<SituacaoCliente>().ToList();
            SituacaoClientePicker.SelectedItem = cliente.SituacaoCliente;
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
                await DisplayAlert("Erro", "O nome é obrigatório", "Ok");
                NomeEntry.Focus();
                return;
            }

            if (string.IsNullOrEmpty(TelefoneEntry.Text))
            {
                await DisplayAlert("Erro", "O telefone é obrigatório", "Ok");
                TelefoneEntry.Focus();
                return;
            }

            if (string.IsNullOrEmpty(EnderecoEntry.Text))
            {
                await DisplayAlert("Erro", "O endereço é obrigatório", "Ok");
                EnderecoEntry.Focus();
                return;
            }

            if (string.IsNullOrEmpty(CidadeEntry.Text))
            {
                await DisplayAlert("Erro", "A cidade é obrigatória", "Ok");
                CidadeEntry.Focus();
                return;
            }

            if (string.IsNullOrEmpty(NumeroCasaEntry.Text))
            {
                await DisplayAlert("Erro", "O número da casa é obrigatório", "Ok");
                NumeroCasaEntry.Focus();
                return;
            }

            if (SituacaoClientePicker.SelectedItem != null)
                Cliente.SituacaoCliente = (SituacaoCliente)SituacaoClientePicker.SelectedItem;
            else
                Cliente.SituacaoCliente = SituacaoCliente.EmDia;

            Cliente.Nome = NomeEntry.Text;
            Cliente.Telefone = TelefoneEntry.Text;
            Cliente.Cidade = CidadeEntry.Text;
            Cliente.Endereco = EnderecoEntry.Text;
            Cliente.NumeroCasa = NumeroCasaEntry.Text;
            Cliente.Obs = ObsEditor.Text;

            if (Cliente.Id == 0)
                await _clientesServico.IncluirAsync(Cliente);
            else
            {
                Cliente.DataAtualizacao = DateTime.Now;
                await _clientesServico.AlterarAsync(Cliente);
            }
            await Navigation.PopAsync();
        }

        private async void ExcluirClicked(object sender, EventArgs e)
        {
            if (Cliente.Id == 0)
            {
                await DisplayAlert("Erro", "Não é possível excluir um cliente que não existe", "Ok");
                return;
            }

            bool confirm = await DisplayAlert("Confirmação", "Você tem certeza que deseja excluir este cliente?", "Sim", "Não");
            if (!confirm)
            {
                return;
            }

            await _clientesServico.DeletarAsync(Cliente);
            await Navigation.PopAsync();
        }

        private void TelefoneEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;
            string text = entry.Text;

            if (text.Length > 16)
            {
                text = text.Remove(text.Length - 1);
            }

            entry.Text = text;
        }

        private void TelefoneEntry_Unfocused(object sender, FocusEventArgs e)
        {
            var entry = sender as Entry;
            string text = entry.Text;

           if (text.Length == 11)
            {
                text = string.Format(CultureInfo.CurrentCulture, "({0}) {1} {2}-{3}",
                    text.Substring(0, 2),
                    text.Substring(2, 1),
                    text.Substring(3, 4),
                    text.Substring(7, 4));
            }
            else if (text.Length == 10)
            {
                text = string.Format(CultureInfo.CurrentCulture, "({0}) {1}-{2}",
                    text.Substring(0, 2),
                    text.Substring(2, 4),
                    text.Substring(6, 4));
            }

            entry.Text = text;
        }
    }
}