using System;
using System.Drawing;
using System.Windows.Forms;

public class NotesForm : Form
{
    private DatabaseContext db = new DatabaseContext();
    private int itemId;
    private TextBox txtNota = new TextBox { Multiline = true, Dock = DockStyle.Fill, BackColor = Color.LightCyan, Font = new Font("Consolas", 11) };

    public NotesForm(int id, string nomeItem)
    {
        this.itemId = id;
        this.Text = "Anotação: " + nomeItem;
        this.Size = new Size(350, 400);
        this.StartPosition = FormStartPosition.CenterParent;

        // Carrega a nota já existente no banco
        txtNota.Text = db.BuscarNotaPrivada(itemId);

        Button btnSalvar = new Button { 
            Text = "SALVAR ANOTAÇÃO", 
            Dock = DockStyle.Bottom, 
            Height = 50, 
            BackColor = Color.LimeGreen, 
            ForeColor = Color.White, 
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat 
        };

        btnSalvar.Click += (s, e) => { 
            db.SalvarNota(itemId, txtNota.Text); 
            MessageBox.Show("Nota salva com sucesso!");
            this.Close(); 
        };

        this.Controls.Add(txtNota);
        this.Controls.Add(btnSalvar);
    }
}