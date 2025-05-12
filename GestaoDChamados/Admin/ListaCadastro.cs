using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Npgsql;

public class ListarCadastro : Form
{
    private DataGridView dgv;
    private string connStr = "Host=localhost;Port=5432;Database=GestaoChamados;Username=postgres;Password=123;";

    public ListarCadastro()
    {
        this.FormBorderStyle = FormBorderStyle.None;
        this.ControlBox = false;
        this.Text = "Lista de Usuários";
        this.Width = 900;
        this.Height = 500;

        dgv = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AllowUserToAddRows = false
        };

        dgv.CellClick += Dgv_CellClick;

        Controls.Add(dgv);
        Load += (s, e) => CarregarUsuarios();
    }

    private void CarregarUsuarios()
    {
        using var conn = new NpgsqlConnection(connStr);
        conn.Open();

        var query = "SELECT id, nome, usuario, senha, tipo, cargo FROM usuarios";
        var da = new NpgsqlDataAdapter(query, conn);
        var dt = new DataTable();
        da.Fill(dt);

        dgv.Columns.Clear();
        dgv.DataSource = dt;
        dgv.Columns["id"].Visible = false;

        // Botões
        var editarBtn = new DataGridViewButtonColumn
        {
            HeaderText = "Editar",
            Text = "Editar",
            UseColumnTextForButtonValue = true,
            Width = 70
        };

        var excluirBtn = new DataGridViewButtonColumn
        {
            HeaderText = "Excluir",
            Text = "Excluir",
            UseColumnTextForButtonValue = true,
            Width = 70
        };

        dgv.Columns.Add(editarBtn);
        dgv.Columns.Add(excluirBtn);
    }

    private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;

        var id = dgv.Rows[e.RowIndex].Cells["id"].Value.ToString();

        if (dgv.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
        {
            var acao = dgv.Columns[e.ColumnIndex].HeaderText;

            if (acao == "Editar")
            {
                EditarUsuario(id);
            }
            else if (acao == "Excluir")
            {
                ExcluirUsuario(id);
                CarregarUsuarios();
            }
        }
    }

    private void EditarUsuario(string id)
    {
        var row = dgv.Rows[dgv.CurrentCell.RowIndex];
        var nome = row.Cells["nome"].Value.ToString();
        var usuario = row.Cells["usuario"].Value.ToString();
        var senha = row.Cells["senha"].Value.ToString();
        var tipo = row.Cells["tipo"].Value.ToString();
        var cargo = row.Cells["cargo"].Value.ToString();

        var form = new Form
        {
            Text = "Editar Usuário",
            Width = 400,
            Height = 350,
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false
        };

        int lblWidth = 70;
        int txtWidth = 250;
        int marginTop = 20;
        int space = 30;

        var lblNome = new Label { Text = "Nome", Top = marginTop, Left = 20, Width = lblWidth };
        var txtNome = new TextBox { Text = nome, Top = marginTop, Left = 100, Width = txtWidth };

        var lblUsuario = new Label { Text = "Usuário", Top = marginTop + space, Left = 20, Width = lblWidth };
        var txtUsuario = new TextBox { Text = usuario, Top = marginTop + space, Left = 100, Width = txtWidth };

        var lblSenha = new Label { Text = "Senha", Top = marginTop + space * 2, Left = 20, Width = lblWidth };
        var txtSenha = new TextBox { Text = senha, Top = marginTop + space * 2, Left = 100, Width = txtWidth };

        var lblTipo = new Label { Text = "Tipo", Top = marginTop + space * 3, Left = 20, Width = lblWidth };
        var txtTipo = new TextBox { Text = tipo, Top = marginTop + space * 3, Left = 100, Width = txtWidth };

        var lblCargo = new Label { Text = "Cargo", Top = marginTop + space * 4, Left = 20, Width = lblWidth };
        var txtCargo = new TextBox { Text = cargo, Top = marginTop + space * 4, Left = 100, Width = txtWidth };

        var btnSalvar = new Button
        {
            Text = "Salvar",
            Top = marginTop + space * 5 + 10,
            Left = 100,
            Width = txtWidth
        };

        btnSalvar.Click += (s, e) =>
        {
            using var conn = new NpgsqlConnection(connStr);
            conn.Open();
            var cmd = new NpgsqlCommand(@"UPDATE usuarios SET nome=@nome, usuario=@usuario, senha=@senha, tipo=@tipo, cargo=@cargo WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("id", int.Parse(id));
            cmd.Parameters.AddWithValue("nome", txtNome.Text);
            cmd.Parameters.AddWithValue("usuario", txtUsuario.Text);
            cmd.Parameters.AddWithValue("senha", txtSenha.Text);
            cmd.Parameters.AddWithValue("tipo", txtTipo.Text);
            cmd.Parameters.AddWithValue("cargo", txtCargo.Text);
            cmd.ExecuteNonQuery();
            CarregarUsuarios(); // atualiza a grade ao salvar
            form.Close();       // fecha só o form de edição
        };

        form.Controls.AddRange(new Control[]
        {
            lblNome, txtNome,
            lblUsuario, txtUsuario,
            lblSenha, txtSenha,
            lblTipo, txtTipo,
            lblCargo, txtCargo,
            btnSalvar
        });

        form.Show(this); // Torna a janela de edição não modal, e não fecha a principal
    }

    private void ExcluirUsuario(string id)
    {
        var confirm = MessageBox.Show("Tem certeza que deseja excluir este usuário?", "Confirmar Exclusão", MessageBoxButtons.YesNo);
        if (confirm == DialogResult.Yes)
        {
            using var conn = new NpgsqlConnection(connStr);
            conn.Open();
            var cmd = new NpgsqlCommand("DELETE FROM usuarios WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("id", int.Parse(id));
            cmd.ExecuteNonQuery();
        }
    }
}
