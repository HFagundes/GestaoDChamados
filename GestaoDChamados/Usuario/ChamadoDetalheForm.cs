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

    // Título + meta
    private Label lblTitulo;
    private Label lblMeta;

    // Bloco "Solicitante"
    private GroupBox grpSolicitante;
    private Label lblSolicNome;
    private Label lblSolicEmail;
    private Label lblSolicUsuario;
    private Label lblSolicUrgencia;

    // Descrição
    private TextBox txtDescricao;

    // Anexo
    private Panel pnlAnexo;
    private PictureBox picAnexo;

    // Chat (agora com TextBox para log)
    private TextBox txtChatLog;
    private TextBox txtMsg;
    private Button btnEnviar;

    private string _anexoPath = "";

    // pasta fixa dos uploads
    private const string PastaUploadsFixa =
        @"H:\Gestao\GestaoDChamados\GestaoDChamados\bin\Debug\net8.0-windows\uploads";

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
        // Esquerda: detalhes + anexo
        var left = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16) };
        // Direita: chat
        var right = new Panel { Dock = DockStyle.Right, Width = 380, Padding = new Padding(12), BackColor = Color.White };

        Controls.Add(left);
        Controls.Add(right);

        // Título e meta (fontes maiores)
        lblTitulo = new Label
        {
            Text = "Assunto do Chamado",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
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
            Top = 34,
            Font = new Font("Segoe UI", 10, FontStyle.Regular)
        };
        left.Controls.Add(lblMeta);

        // ==== BLOCO SOLICITANTE ====
        grpSolicitante = new GroupBox
        {
            Text = "Solicitante",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Left = 0,
            Top = 64,
            Width = 560,
            Height = 120
        };
        left.Controls.Add(grpSolicitante);

        lblSolicNome = new Label
        {
            Text = "Nome:",
            AutoSize = true,
            Left = 10,
            Top = 24,
            Font = new Font("Segoe UI", 9.5f)
        };
        grpSolicitante.Controls.Add(lblSolicNome);

        lblSolicEmail = new Label
        {
            Text = "E-mail:",
            AutoSize = true,
            Left = 10,
            Top = 44,
            Font = new Font("Segoe UI", 9.5f)
        };
        grpSolicitante.Controls.Add(lblSolicEmail);

        lblSolicUsuario = new Label
        {
            Text = "Usuário:",
            AutoSize = true,
            Left = 10,
            Top = 64,
            Font = new Font("Segoe UI", 9.5f)
        };
        grpSolicitante.Controls.Add(lblSolicUsuario);

        lblSolicUrgencia = new Label
        {
            Text = "Urgência:",
            AutoSize = true,
            Left = 10,
            Top = 84,
            Font = new Font("Segoe UI", 9.5f)
        };
        grpSolicitante.Controls.Add(lblSolicUrgencia);

        // ==== DESCRIÇÃO (maior, fonte maior) ====
        txtDescricao = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Left = 0,
            Top = grpSolicitante.Bottom + 10,
            Width = 560,
            Height = 220,                     // área maior
            Font = new Font("Segoe UI", 11f)  // fonte maior
        };
        left.Controls.Add(txtDescricao);

        // Descrição não selecionável
        txtDescricao.Cursor = Cursors.Arrow;
        txtDescricao.ShortcutsEnabled = false;
        txtDescricao.TabStop = false;
        txtDescricao.GotFocus += (s, e) => this.ActiveControl = null;
        txtDescricao.MouseDown += (s, e) => txtDescricao.SelectionLength = 0;
        txtDescricao.MouseMove += (s, e) => txtDescricao.SelectionLength = 0;
        txtDescricao.KeyDown += (s, e) => e.SuppressKeyPress = true;

        // ==== ANEXO (área maior) ====
        var grpAnexo = new GroupBox
        {
            Text = "Anexo",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Left = 0,
            Top = txtDescricao.Bottom + 10,
            Width = 560,
            Height = 240           // altura maior pra foto ficar grandona
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

        // ==== CHAT (direita) ====
        var lblChat = new Label
        {
            Text = "Chat com Funcionário",
            AutoSize = true,
            Left = 6,
            Top = 6,
            Font = new Font("Segoe UI", 11, FontStyle.Bold)
        };
        right.Controls.Add(lblChat);

        // Log do chat em TextBox multilinha (com quebra de linha)
        txtChatLog = new TextBox
        {
            Left = 6,
            Top = 32,
            Width = right.Width - 24,
            Height = 460,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            WordWrap = true,
            Font = new Font("Segoe UI", 9.5f),
            BorderStyle = BorderStyle.FixedSingle
        };
        right.Controls.Add(txtChatLog);

        txtMsg = new TextBox
        {
            Left = 6,
            Top = 500,
            Width = right.Width - 98,
            Height = 30,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            Font = new Font("Segoe UI", 9.5f)
        };
        btnEnviar = new Button
        {
            Text = "Enviar",
            Left = right.Width - 86,
            Top = 500,
            Width = 80,
            Height = 30,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
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
            SELECT id, nome, email, id_usuario, urgencia, assunto, descricao, datacriacao, situacao, anexo_caminho
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

        var nome = rd["nome"]?.ToString() ?? "";
        var email = rd["email"]?.ToString() ?? "";
        var usuario = rd["id_usuario"]?.ToString() ?? "";
        var urg = rd["urgencia"]?.ToString() ?? "";
        var assunto = rd["assunto"]?.ToString() ?? "";
        var descr = rd["descricao"]?.ToString() ?? "";
        var situacao = rd["situacao"]?.ToString() ?? "";
        var dt = Convert.ToDateTime(rd["datacriacao"]);
        _anexoPath = rd["anexo_caminho"]?.ToString() ?? "";

        // Título + meta
        lblTitulo.Text = assunto;
        lblMeta.Text = $"{urg} • {situacao} • {dt:dd/MM/yyyy HH:mm}";

        // Solicitante
        lblSolicNome.Text = $"Nome: {nome}";
        lblSolicEmail.Text = $"E-mail: {email}";
        lblSolicUsuario.Text = $"Usuário: {usuario}";
        lblSolicUrgencia.Text = $"Urgência: {urg}";

        // Descrição
        txtDescricao.Text = descr;

        CarregarAnexoImagem();
    }

    private void CarregarAnexoImagem()
    {
        picAnexo.Image = null;
        picAnexo.ImageLocation = null;

        if (string.IsNullOrWhiteSpace(_anexoPath))
            return;

        var fileName = Path.GetFileName(_anexoPath);
        if (string.IsNullOrWhiteSpace(fileName))
            return;

        var caminhoCompleto = Path.Combine(PastaUploadsFixa, fileName);

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
        txtChatLog.Clear();

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

            txtChatLog.AppendText($"[{ts:dd/MM HH:mm}] {autor}: {msg}{Environment.NewLine}");
        }

        // rola automaticamente para o final
        txtChatLog.SelectionStart = txtChatLog.TextLength;
        txtChatLog.ScrollToCaret();
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
