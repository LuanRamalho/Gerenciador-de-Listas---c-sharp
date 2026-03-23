using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using Microsoft.Data.Sqlite;

public class ItemsForm : Form
{
    private int catId;
    private DatabaseContext db = new DatabaseContext();
    private FlowLayoutPanel panelItens = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Color.FromArgb(245, 245, 245) };

    public ItemsForm(int id, string titulo)
    {
        this.catId = id;
        this.Text = "Itens de: " + titulo;
        this.Size = new Size(450, 600);
        this.StartPosition = FormStartPosition.CenterScreen;

        Panel topPanel = new Panel { Dock = DockStyle.Top, Height = 90, BackColor = Color.DeepSkyBlue };
        TextBox txtItem = new TextBox { Location = new Point(10, 15), Width = 250 };
        Button btnAdd = new Button { Text = "Adicionar Item", Location = new Point(270, 12), Width = 100, BackColor = Color.White, FlatStyle = FlatStyle.Flat };
        TextBox txtBusca = new TextBox { Location = new Point(10, 50), Width = 250, PlaceholderText = "Filtrar itens..." };

        btnAdd.Click += (s, e) => { 
            if(!string.IsNullOrWhiteSpace(txtItem.Text)) {
                db.AdicionarItem(catId, txtItem.Text); 
                txtItem.Clear(); 
                CarregarItens(); 
            }
        };
        txtBusca.TextChanged += (s, e) => CarregarItens(txtBusca.Text);

        topPanel.Controls.AddRange(new Control[] { txtItem, btnAdd, txtBusca });
        this.Controls.Add(panelItens);
        this.Controls.Add(topPanel);
        CarregarItens();
    }

    private void CarregarItens(string filtro = "")
    {
        panelItens.Controls.Clear();
        DataTable dt = db.BuscarItens(catId, filtro);
        foreach (DataRow row in dt.Rows)
        {
            int itemId = Convert.ToInt32(row["Id"]);
            bool concluido = Convert.ToBoolean(row["Concluido"]);
            string texto = row["Texto"]?.ToString() ?? "";

            Panel itemPanel = new Panel { Size = new Size(420, 45), BackColor = Color.White, Margin = new Padding(3), BorderStyle = BorderStyle.FixedSingle };
            CheckBox chk = new CheckBox { Checked = concluido, Location = new Point(5, 12), AutoSize = true };
            Label lbl = new Label { Text = texto, Location = new Point(30, 12), AutoSize = true, Font = new Font("Segoe UI", 10, concluido ? FontStyle.Strikeout : FontStyle.Regular) };
            
            Button btnEdit = new Button { Text = "Ed.", Location = new Point(255, 10), Width = 50, BackColor = Color.LightBlue, FlatStyle = FlatStyle.Flat };
            
            // NOVO BOTÃO DE EXCLUIR
            Button btnDel = new Button { Text = "Exc.", Location = new Point(310, 10), Width = 50, BackColor = Color.Crimson, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            
            // BOTÃO NOTA (MOVido ligeiramente para a direita para acomodar o Exc.)
            Button btnNota = new Button { Text = "Nota", Location = new Point(365, 10), Width = 45, BackColor = Color.LightYellow, FlatStyle = FlatStyle.Flat };

            // Lógica de Edição (InputBox)
            btnEdit.Click += (s, e) => {
                // Necessário adicionar referência ao Microsoft.VisualBasic no .csproj se não compilar
                string novoTexto = Microsoft.VisualBasic.Interaction.InputBox("Editar item:", "Editar", texto);
                if (!string.IsNullOrWhiteSpace(novoTexto)) {
                    db.ExecutarNonQuery("UPDATE Itens SET Texto = @t WHERE Id = @id", new SqliteParameter("@t", novoTexto), new SqliteParameter("@id", itemId));
                    CarregarItens();
                }
            };

            // LÓGICA DE EXCLUSÃO DO ITEM
            btnDel.Click += (s, e) => {
                if (MessageBox.Show($"Deseja realmente excluir '{texto}'?", "Confirmar Exclusão", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    db.RemoverItem(itemId);
                    CarregarItens(); // Recarrega a lista para remover visualmente o item
                }
            };

            // Lógica da Nota (abre NotesForm)
            btnNota.Click += (s, e) => new NotesForm(itemId, texto).ShowDialog();

            // Lógica do Checkbox (riscado)
            chk.CheckedChanged += (s, e) => {
                db.AtualizarStatusItem(itemId, chk.Checked);
                lbl.Font = new Font("Segoe UI", 10, chk.Checked ? FontStyle.Strikeout : FontStyle.Regular);
            };

            // Adiciona todos os controles ao painel do item
            itemPanel.Controls.AddRange(new Control[] { chk, lbl, btnEdit, btnDel, btnNota });
            panelItens.Controls.Add(itemPanel);
        }
    }
}