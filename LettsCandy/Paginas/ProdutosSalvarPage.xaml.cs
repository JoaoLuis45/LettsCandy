using System.Globalization;
using System.IO;
using MauiLib.Core.Constantes;
using MauiLib.Core.Models;
using MauiLib.Core.Servicos;

namespace LettsCandy.Paginas
{
    public partial class ProdutosSalvarPage : ContentPage
    {
        public Produto Produto { get; set; }

        private DatabaseServicos<Produto> _produtoServico;
        private DatabaseServicos<Anexo> _anexoServico;

        public ProdutosSalvarPage(Produto produto)
        {
            InitializeComponent();
            _produtoServico = new DatabaseServicos<Produto>(Db.DB_PATH);
            _anexoServico = new DatabaseServicos<Anexo>(Db.DB_PATH);
            Produto = produto;
            BindingContext = produto;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (Produto.Id == 0)
            {
                return;
            }
            else CarregarImagens();
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

            if (string.IsNullOrEmpty(QtdEntry.Text))
            {
                await DisplayAlert("Erro", "A quantidade é obrigatória", "Ok");
                QtdEntry.Focus();
                return;
            }

            if (string.IsNullOrEmpty(PrecoEntry.Text))
            {
                await DisplayAlert("Erro", "O preço é obrigatório", "Ok");
                PrecoEntry.Focus();
                return;
            }


            Produto.Nome = NomeEntry.Text;
            Produto.Descricao = DescricaoEditor.Text;
            Produto.Quantidade = int.Parse(QtdEntry.Text);
            Produto.Preco = double.Parse(PrecoEntry.Text);

            if (Produto.Id == 0)
                await _produtoServico.IncluirAsync(Produto);
            else
            {
                Produto.DataAtualizacao = DateTime.Now;
                await _produtoServico.AlterarAsync(Produto);
            }
            await Navigation.PopAsync();


        }

        private async void ExcluirClicked(object sender, EventArgs e)
        {
            if (Produto.Id == 0)
            {
                await DisplayAlert("Erro", "Não é possível excluir um produto que não existe", "Ok");
                return;
            }
            await _produtoServico.DeletarAsync(Produto);
            await Navigation.PopAsync();
        }

        private async void CarregarImagens()
        {
            if(Produto.Foto != "emptyproduct.png")
                SelectedImage.Source = Produto.Foto;

        }

        private async void AnexarClicked(object sender, EventArgs e)
        {
            try
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    // Abre a galeria para selecionar uma foto
                    var photo = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                    {
                        Title = "Selecione uma foto"
                    });
                    if (photo != null)
                    {
                        using Stream stream = await photo.OpenReadAsync();

                        //define o diretorio e o nome do arquivo onde a foto será salva
                        string directory = FileSystem.AppDataDirectory;
                        string filename = Path.Combine(directory, $"{DateTime.Now.ToString("ddMMyyyy_hhmmss")}.jpg");


                        //salva a foto no diretorio definido
                        using FileStream fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write);
                        await stream.CopyToAsync(fileStream);


                        Produto.Foto = filename;
                        await _produtoServico.AlterarAsync(Produto);


                        CarregarImagens();

                    }
                }
                else
                {
                    await DisplayAlert("Erro", "A captura de fotos não é suportada neste dispositivo!", "Fechar");
                }
            }
            catch (FeatureNotEnabledException fnsEx)
            {
                await DisplayAlert("Erro", "A captura de fotos não é suportada neste dispositivo. - " + fnsEx.Message, "Fechar");
            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Erro", "permissão para acessar a câmera não concedida. - " + pEx.Message, "Fechar");
            }
            catch (Exception ex)
            {
           
                await DisplayAlert("Erro", $"Ocorreu um erro: {ex.Message}", "OK");
            }
        }

    }
}