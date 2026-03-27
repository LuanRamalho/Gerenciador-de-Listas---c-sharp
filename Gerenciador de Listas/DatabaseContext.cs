using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Encodings.Web;

public class Item
{
    public string Texto { get; set; }
    public bool Concluido { get; set; }
    public string Nota { get; set; }
}

public class Categoria
{
    public string Nome { get; set; }
    public List<Item> Itens { get; set; } = new List<Item>();
}

public class DatabaseContext
{
    private string filePath = "listas.json";
    private List<Categoria> categorias;

    public DatabaseContext()
    {
        CarregarDados();
    }

    private void CarregarDados()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            categorias = JsonSerializer.Deserialize<List<Categoria>>(json) ?? new List<Categoria>();
        }
        else
        {
            categorias = new List<Categoria>();
        }
    }

    private void SalvarDados()
    {
        var options = new JsonSerializerOptions 
        { 
            WriteIndented = true,
            // Esta linha permite que acentos e cedilha apareçam normalmente no arquivo
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping 
        };

        string json = JsonSerializer.Serialize(categorias, options);
        File.WriteAllText(filePath, json); // Corrigido de ReadAllText para WriteAllText
    }

    // --- Métodos de Categorias ---

    public void AdicionarCategoria(string nome)
    {
        if (!categorias.Any(c => c.Nome == nome))
        {
            categorias.Add(new Categoria { Nome = nome });
            SalvarDados();
        }
    }

    public void RemoverCategoria(string nome)
    {
        var cat = categorias.FirstOrDefault(c => c.Nome == nome);
        if (cat != null)
        {
            categorias.Remove(cat);
            SalvarDados();
        }
    }

    public void AtualizarNomeCategoria(string nomeAntigo, string novoNome)
    {
        var cat = categorias.FirstOrDefault(c => c.Nome == nomeAntigo);
        if (cat != null)
        {
            cat.Nome = novoNome;
            SalvarDados();
        }
    }

    public List<Categoria> BuscarCategorias(string filtro = "")
    {
        if (string.IsNullOrEmpty(filtro)) return categorias;
        return categorias.Where(c => c.Nome.Contains(filtro, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    // --- Métodos de Itens ---

    public List<Item> BuscarItens(string catNome, string filtro = "")
    {
        var cat = categorias.FirstOrDefault(c => c.Nome == catNome);
        if (cat == null) return new List<Item>();

        if (string.IsNullOrEmpty(filtro)) return cat.Itens;
        return cat.Itens.Where(i => i.Texto.Contains(filtro, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public void AdicionarItem(string catNome, string texto)
    {
        var cat = categorias.FirstOrDefault(c => c.Nome == catNome);
        if (cat != null)
        {
            cat.Itens.Add(new Item { Texto = texto, Concluido = false, Nota = "" });
            SalvarDados();
        }
    }

    public void AtualizarItem(string catNome, string textoOriginal, string novoTexto, bool concluido, string nota)
    {
        var cat = categorias.FirstOrDefault(c => c.Nome == catNome);
        var item = cat?.Itens.FirstOrDefault(i => i.Texto == textoOriginal);
        if (item != null)
        {
            item.Texto = novoTexto;
            item.Concluido = concluido;
            item.Nota = nota;
            SalvarDados();
        }
    }

    public void RemoverItem(string catNome, string texto)
    {
        var cat = categorias.FirstOrDefault(c => c.Nome == catNome);
        var item = cat?.Itens.FirstOrDefault(i => i.Texto == texto);
        if (item != null)
        {
            cat.Itens.Remove(item);
            SalvarDados();
        }
    }

    public string BuscarNotaPrivada(string catNome, string texto)
    {
        var item = categorias.FirstOrDefault(c => c.Nome == catNome)?.Itens.FirstOrDefault(i => i.Texto == texto);
        return item?.Nota ?? "";
    }

    public void SalvarNota(string catNome, string texto, string nota)
    {
        var item = categorias.FirstOrDefault(c => c.Nome == catNome)?.Itens.FirstOrDefault(i => i.Texto == texto);
        if (item != null)
        {
            item.Nota = nota;
            SalvarDados();
        }
    }
}
