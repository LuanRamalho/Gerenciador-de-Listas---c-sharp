using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using Microsoft.Data.Sqlite;

public class MainForm : Form
{
    private DatabaseContext db = new DatabaseContext();
    private TextBox txtNome = new TextBox { Width = 200, Location = new Point(20, 20) };
    private Button btnAdd = new Button { Text = "Adicionar", Location = new Point(230, 18), BackColor = Color.LightGreen, FlatStyle = FlatStyle.Flat };
    private TextBox txtBusca = new TextBox { Width = 150, Location = new Point(20, 60), PlaceholderText = "Buscar lista..." };
    private FlowLayoutPanel panelCategorias = new FlowLayoutPanel { Location = new Point(20, 100), Size = new Size(440, 340), AutoScroll = true, BackColor = Color.FromArgb(30, 30, 30) };

    public MainForm()
    {
        this.Text = "Gerenciador de Listas";
        this.Size = new Size(500, 500);
        this.BackColor = Color.FromArgb(45, 45, 48);
        this.StartPosition = FormStartPosition.CenterScreen;

        btnAdd.Click += (s, e) => { 
            if (!string.IsNullOrWhiteSpace(txtNome.Text)) { 
                db.AdicionarCategoria(txtNome.Text); 
                txtNome.Clear(); 
                CarregarCategorias(); 
            } 
        };
        txtBusca.TextChanged += (s, e) => CarregarCategorias(txtBusca.Text);

        this.Controls.AddRange(new Control[] { txtNome, btnAdd, txtBusca, panelCategorias });
        CarregarCategorias();
    }

    private void CarregarCategorias(string filtro = "")
    {
        panelCategorias.Controls.Clear();
        DataTable dt = db.BuscarCategorias(filtro);
        foreach (DataRow row in dt.Rows)
        {
            int id = Convert.ToInt32(row["Id"]);
            string nome = row["Nome"]?.ToString() ?? "Sem Nome";

            Panel rowPanel = new Panel { Size = new Size(400, 45), Margin = new Padding(5), BackColor = Color.MediumPurple };
            LinkLabel lbl = new LinkLabel { Text = nome, Location = new Point(10, 12), Font = new Font("Segoe UI", 11, FontStyle.Bold), LinkColor = Color.White, AutoSize = true, LinkBehavior = LinkBehavior.HoverUnderline };
            
            lbl.Click += (s, e) => new ItemsForm(id, nome).Show();

            // Botão Editar
            Button btnEdit = new Button { Text = "Editar", Size = new Size(60, 25), Location = new Point(265, 10), BackColor = Color.DodgerBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnEdit.Click += (s, e) => {
                string novoNome = Microsoft.VisualBasic.Interaction.InputBox("Novo nome da lista:", "Editar", nome);
                if (!string.IsNullOrWhiteSpace(novoNome)) {
                    db.ExecutarNonQuery("UPDATE Categorias SET Nome = @n WHERE Id = @id", new SqliteParameter("@n", novoNome), new SqliteParameter("@id", id));
                    CarregarCategorias();
                }
            };

            Button btnDel = new Button { Text = "Excluir", Size = new Size(60, 25), Location = new Point(330, 10), BackColor = Color.Crimson, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnDel.Click += (s, e) => { db.RemoverCategoria(id); CarregarCategorias(); };

            rowPanel.Controls.AddRange(new Control[] { lbl, btnEdit, btnDel });
            panelCategorias.Controls.Add(rowPanel);
        }
    }
}