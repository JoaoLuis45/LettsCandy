using SQLite;
using MauiLib.Core.Models;
namespace MauiLib.Core.Servicos;

public class DatabaseServicos<T> where T : new(){
    private SQLiteAsyncConnection _database;

    public DatabaseServicos(string dbpath)
    {
        _database = new SQLiteAsyncConnection(dbpath);
        // Adiciona a criação das tabelas dependentes antes de criar a tabela principal
        if (typeof(T) == typeof(Receita))
        {
            _database.CreateTableAsync<Produto>().Wait();
            _database.CreateTableAsync<Item>().Wait();
        }
        _database.CreateTableAsync<T>().Wait();
    }

    public Task<int> IncluirAsync(T item)=> _database.InsertAsync(item);
    

    public Task<int> AlterarAsync(T item) => _database.UpdateAsync(item);

    public Task<int> DeletarAsync(T item) => _database.DeleteAsync(item);


    public Task<List<T>> TodosAsync() => _database.Table<T>().ToListAsync();
    public AsyncTableQuery<T> Query() => _database.Table<T>();
    

    public Task<int> QuantidadeAsync() => _database.Table<T>().CountAsync();
    
}