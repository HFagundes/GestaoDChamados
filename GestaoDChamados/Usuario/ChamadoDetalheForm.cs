using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Npgsql;

public class ChamadoDetalheForm : Form
{
    private readonly string _connStr;
    private readonly int _chamadoId;
    private readonly string _usuarioAtual;

    // UI
    private Label lblTitulo;
    private TextBox txtDescricao;
    private Label lblMeta;
    private Panel pnlAnexo;
    private PictureBox picAnexo;

    // Chat
    private ListBox lstChat;
    private TextBox txtMsg;
    private Button btnEnviar;

    private string _anexoPath = "";

    // pasta fixa onde ficam os uploads (como você pediu)
    private const string PastaUploadsFixa = @"H:\Gestao\GestaoDChamados\GestaoDChamados\bin\Debug\net8.0-windows\uploads";

    public ChamadoDetalheForm(string connectionString, int chamadoId, string usuarioAtual)
    {
        _connStr = connectionString;
        _chamadoId = chamadoId;
        _usuarioAtual = usuarioAtual;

        Text = "Detalhe do Chamado";
        StartPosition = FormStartPosition.CenterParent;
        Width = 1000;
        Height = 680;
        BackColor = Color.White;

        BuildUI();
        CarregarChamado();
        CarregarChat();
    }

    private void BuildUI()
    {
        // esquerda: detalhes + anexo
        var left = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16) };
        // direita: chat
        var right = new Panel { Dock = DockStyle.Right, Width = 360, Padding = new Padding(12), BackColor = Color.White };

        Controls.Add(left);
        Controls.Add(right);

        lblTitulo = new Label
        {
            Text = "Assunto do Chamado",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            AutoSize = true,
            Left = 0,
            Top = 0
        };
        left.Controls.Add(lblTitulo);

        lblMeta = new Label
        {
            Text = "Urgência • Situação • Criado em",
            AutoSize = true,
            Left = 0,
            Top = 36
        };
        left.Controls.Add(lblMeta);

        txtDescricao = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Left = 0,
            Top = 68,
            Width = 560,
            Height = 220
        };
        left.Controls.Add(txtDescricao);

        // descrição não selecionável
        txtDescricao.Cursor = Cursors.Arrow;
        txtDescricao.ShortcutsEnabled = false;
        txtDescricao.TabStop = false;
        txtDescricao.GotFocus += (s, e) => this.ActiveControl = null;
        txtDescricao.MouseDown += (s, e) => txtDescricao.SelectionLength = 0;
        txtDescricao.MouseMove += (s, e) => txtDescricao.SelectionLength = 0;
        txtDescricao.KeyDown += (s, e) => e.SuppressKeyPress = true;

        // Anexo (apenas imagem logo abaixo)
        var grpAnexo = new GroupBox
        {
            Text = "Anexo",
            Left = 0,
            Top = 300,
            Width = 560,
            Height = 280
        };
        left.Controls.Add(grpAnexo);

        pnlAnexo = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(8)
        };
        grpAnexo.Controls.Add(pnlAnexo);

        picAnexo = new PictureBox
        {
            Dock = DockStyle.Fill,
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.White
        };
        pnlAnexo.Controls.Add(picAnexo);

        // Chat (direita)
        var lblChat = new Label
        {
            Text = "Chat com Funcionário",
            AutoSize = true,
            Left = 6,
            Top = 6,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        right.Controls.Add(lblChat);

        lstChat = new ListBox
        {
            Left = 6,
            Top = 30,
            Width = right.Width - 24,
            Height = 480,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };
        right.Controls.Add(lstChat);

        txtMsg = new TextBox
        {
            Left = 6,
            Top = 520,
            Width = right.Width - 98,
            Height = 28,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };
        btnEnviar = new Button
        {
            Text = "Enviar",
            Left = right.Width - 86,
            Top = 520,
            Width = 80,
            Height = 28,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right
        };
        btnEnviar.Click += (_, __) => EnviarMensagem();

        right.Controls.Add(txtMsg);
        right.Controls.Add(btnEnviar);
    }

    private void CarregarChamado()
    {
        using var conn = new NpgsqlConnection(_connStr);
        conn.Open();

        const string sql = @"
            SELECT id, nome, email, urgencia, assunto, descricao, datacriacao, situacao, anexo_caminho
            FROM chamados
            WHERE id = @id
            LIMIT 1;";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", _chamadoId);
        using var rd = cmd.ExecuteReader();
        if (!rd.Read())
        {
            MessageBox.Show("Chamado não encontrado.");
            Close();
            return;
        }

        var urg = rd["urgencia"]?.ToString() ?? "";
        var assunto = rd["assunto"]?.ToString() ?? "";
        var descr = rd["descricao"]?.ToString() ?? "";
        var situacao = rd["situacao"]?.ToString() ?? "";
        var dt = Convert.ToDateTime(rd["datacriacao"]);
        _anexoPath = rd["anexo_caminho"]?.ToString() ?? "";

        lblTitulo.Text = assunto;
        lblMeta.Text = $"{urg} • {situacao} • {dt:dd/MM/yyyy HH:mm}";
        txtDescricao.Text = descr;

        CarregarAnexoImagem();
    }

    private void CarregarAnexoImagem()
    {
        picAnexo.Image = null;
        picAnexo.ImageLocation = null;

        if (string.IsNullOrWhiteSpace(_anexoPath))
            return;

        // pega só o nome do arquivo, independente se veio "/uploads/arquivo.png" ou "uploads/arquivo.png"
        var fileName = Path.GetFileName(_anexoPath);
        if (string.IsNullOrWhiteSpace(fileName))
            return;

        var caminhoCompleto = Path.Combine(PastaUploadsFixa, fileName);

        // Debug opcional pra você ver o caminho:
        // MessageBox.Show($"anexo_caminho: {_anexoPath}\nfileName: {fileName}\ncaminhoCompleto: {caminhoCompleto}\nExiste: {File.Exists(caminhoCompleto)}");

        if (File.Exists(caminhoCompleto))
        {
            try
            {
                picAnexo.ImageLocation = caminhoCompleto;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar anexo: " + ex.Message);
            }
        }
    }

    private void CarregarChat()
    {
        lstChat.Items.Clear();

        using var conn = new NpgsqlConnection(_connStr);
        conn.Open();
        const string sql = @"
            SELECT remetente_id, mensagem, criado_em
            FROM chat_mensagens
            WHERE chamado_id = @id
            ORDER BY criado_em ASC;";
        using var da = new NpgsqlDataAdapter(sql, conn);
        da.SelectCommand.Parameters.AddWithValue("@id", _chamadoId);
        var dt = new DataTable();
        da.Fill(dt);

        foreach (DataRow r in dt.Rows)
        {
            var autor = Convert.ToString(r["remetente_id"]) ?? "";
            var msg = Convert.ToString(r["mensagem"]) ?? "";
            var ts = Convert.ToDateTime(r["criado_em"]);
            lstChat.Items.Add($"[{ts:dd/MM HH:mm}] {autor}: {msg}");
        }

        if (lstChat.Items.Count > 0)
            lstChat.TopIndex = lstChat.Items.Count - 1;
    }

    private void EnviarMensagem()
    {
        var texto = (txtMsg.Text ?? "").Trim();
        if (texto.Length == 0) return;

        using var conn = new NpgsqlConnection(_connStr);
        conn.Open();

        const string sql = @"
        INSERT INTO chat_mensagens 
            (chamado_id, remetente_tipo, remetente_id, mensagem, criado_em)
        VALUES 
            (@id, @tipo, @remetenteId, @mensagem, NOW());";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", _chamadoId);
        cmd.Parameters.AddWithValue("@tipo", "usuario");
        cmd.Parameters.AddWithValue("@remetenteId", _usuarioAtual);
        cmd.Parameters.AddWithValue("@mensagem", texto);
        cmd.ExecuteNonQuery();

        txtMsg.Clear();
        CarregarChat();
    }
}
