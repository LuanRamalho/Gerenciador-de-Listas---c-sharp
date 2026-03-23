using System;
using System.Data;
using Microsoft.Data.Sqlite;

public class DatabaseContext
{
    private string connectionString = "Data Source=listas.db";

    public DatabaseContext()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            string sql = @"
                CREATE TABLE IF NOT EXISTS Categorias (Id INTEGER PRIMARY KEY AUTOINCREMENT, Nome TEXT);
                CREATE TABLE IF NOT EXISTS Itens (Id INTEGER PRIMARY KEY AUTOINCREMENT, CategoriaId INTEGER, Texto TEXT, Concluido BIT, Nota TEXT);";
            using (var command = new SqliteCommand(sql, connection))
                command.ExecuteNonQuery();
        }
    }

    // --- Métodos de Categorias (Janela 1) ---

    public void AdicionarCategoria(string nome) => 
        ExecutarNonQuery("INSERT INTO Categorias (Nome) VALUES (@nome)", new SqliteParameter("@nome", nome));
    
    public void RemoverCategoria(int id) => 
        ExecutarNonQuery("DELETE FROM Categorias WHERE Id = @id; DELETE FROM Itens WHERE CategoriaId = @id;", new SqliteParameter("@id", id));
    
    public DataTable BuscarCategorias(string filtro = "")
    {
        string sql = string.IsNullOrEmpty(filtro) ? "SELECT * FROM Categorias" : "SELECT * FROM Categorias WHERE Nome LIKE @filtro";
        return ExecutarQuery(sql, new SqliteParameter("@filtro", $"%{filtro}%"));
    }

    // --- Métodos de Itens (Janela 2) ---

    public void AdicionarItem(int catId, string texto) => 
        ExecutarNonQuery("INSERT INTO Itens (CategoriaId, Texto, Concluido, Nota) VALUES (@catId, @texto, 0, '')", new SqliteParameter("@catId", catId), new SqliteParameter("@texto", texto));
    
    public void AtualizarStatusItem(int id, bool concluido) => 
        ExecutarNonQuery("UPDATE Itens SET Concluido = @c WHERE Id = @id", new SqliteParameter("@c", concluido ? 1 : 0), new SqliteParameter("@id", id));
    
    // NOVO MÉTODO DE EXCLUSÃO DE ITEM
    public void RemoverItem(int itemId) =>
        ExecutarNonQuery("DELETE FROM Itens WHERE Id = @id", new SqliteParameter("@id", itemId));

    public DataTable BuscarItens(int catId, string filtro = "")
    {
        string sql = "SELECT * FROM Itens WHERE CategoriaId = @catId";
        if (!string.IsNullOrEmpty(filtro)) sql += " AND Texto LIKE @filtro";
        return ExecutarQuery(sql, new SqliteParameter("@catId", catId), new SqliteParameter("@filtro", $"%{filtro}%"));
    }

    // --- Métodos de Nota (Janela 3) ---

    public void SalvarNota(int id, string nota) => 
        ExecutarNonQuery("UPDATE Itens SET Nota = @n WHERE Id = @id", new SqliteParameter("@n", nota), new SqliteParameter("@id", id));

    public string BuscarNotaPrivada(int itemId)
    {
        DataTable dt = ExecutarQuery("SELECT Nota FROM Itens WHERE Id = @id", new SqliteParameter("@id", itemId));
        if (dt.Rows.Count > 0) return dt.Rows[0]["Nota"]?.ToString() ?? "";
        return "";
    }

    // --- Motores de Execução ---

    public void ExecutarNonQuery(string sql, params SqliteParameter[] parameters)
    {
        using var conn = new SqliteConnection(connectionString);
        conn.Open();
        using var cmd = new SqliteCommand(sql, conn);
        cmd.Parameters.AddRange(parameters);
        cmd.ExecuteNonQuery();
    }

    private DataTable ExecutarQuery(string sql, params SqliteParameter[] parameters)
    {
        DataTable dt = new DataTable();
        using var conn = new SqliteConnection(connectionString);
        conn.Open();
        using var cmd = new SqliteCommand(sql, conn);
        cmd.Parameters.AddRange(parameters);
        using (var reader = cmd.ExecuteReader())
        {
            dt.Load(reader);
        }
        return dt;
    }
}