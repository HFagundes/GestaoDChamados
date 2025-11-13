    using System;
    using System.Data;
    using System.Drawing;
    using System.Windows.Forms;
    using Npgsql;

    public class MeusChamados : Form
    {
        private readonly string _usuarioAutenticado;
        private readonly string _connectionString =
            "Host=localhost;Port=5432;Database=GestaoChamados;Username=postgres;Password=123;";

        private DataGridView dgvChamados;
        private FlowLayoutPanel pnlFiltros;
        private Button btnTodos, btnAbertos, btnEmAndamento, btnEncerrados;

        // painel central onde aparece a lista OU o detalhe
        private Panel _contentPanel;

        // container do grid para conseguir centralizar
        private Panel _gridContainer;

        public MeusChamados(string usuario)
        {
            _usuarioAutenticado = usuario;

            Text = "Meus Chamados";
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.White;
            Dock = DockStyle.Fill;

            BuildUI();
            CarregarChamados(); 
        }

        private void BuildUI()
        {
            // painel de filtros (topo) - alinhado à direita
            pnlFiltros = new FlowLayoutPanel();
            pnlFiltros.Dock = DockStyle.Top;
            pnlFiltros.Height = 64;
            pnlFiltros.FlowDirection = FlowDirection.RightToLeft; // joga os botões pro lado direito
            pnlFiltros.Padding = new Padding(0, 12, 12, 12);     // padding direito
            pnlFiltros.AutoSize = false;
            pnlFiltros.WrapContents = false;
            pnlFiltros.BackColor = Color.White;


            btnEncerrados = MakeFilterButton("Encerrados", (s, e) => CarregarChamados("Encerrado"));
            btnEmAndamento = MakeFilterButton("Em Andamento", (s, e) => CarregarChamados("Em Andamento"));
            btnAbertos = MakeFilterButton("Abertos", (s, e) => CarregarChamados("Aberto"));
            btnTodos = MakeFilterButton("Todos", (s, e) => CarregarChamados(null));

            // Como o FlowDirection é RightToLeft, a ordem de Add define a ordem da direita pra esquerda.
            pnlFiltros.Controls.Add(btnEncerrados);
            pnlFiltros.Controls.Add(btnEmAndamento);
            pnlFiltros.Controls.Add(btnAbertos);
            pnlFiltros.Controls.Add(btnTodos);

            Controls.Add(pnlFiltros);

        // grid
        dgvChamados = new DataGridView();


        // ✅ define a largura manualmente
        dgvChamados.Width = 1100;              
        dgvChamados.Height = 400;             
        dgvChamados.Location = new Point(20, 20); 

    
        dgvChamados.Anchor = AnchorStyles.Top | AnchorStyles.Left;

        dgvChamados.AutoGenerateColumns = false;

        dgvChamados.ReadOnly = true;
        dgvChamados.AllowUserToAddRows = false;
        dgvChamados.AllowUserToResizeRows = false;
        dgvChamados.RowHeadersVisible = false;
        dgvChamados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvChamados.BackgroundColor = Color.White;
        dgvChamados.BorderStyle = BorderStyle.None;
        dgvChamados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        // tamanho da fonte e linhas maiores
        dgvChamados.DefaultCellStyle.Font = new Font("Segoe UI", 10f);
            dgvChamados.RowTemplate.Height = 36;

            dgvChamados.EnableHeadersVisualStyles = false;
            dgvChamados.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvChamados.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvChamados.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            dgvChamados.ColumnHeadersHeight = 42;
            dgvChamados.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 240, 255);
            dgvChamados.DefaultCellStyle.SelectionForeColor = Color.Black;

            // colunas
            dgvChamados.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                DataPropertyName = "id",
                Visible = false
            });
            dgvChamados.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Nome",
                HeaderText = "Nome",
                DataPropertyName = "nome",
                FillWeight = 120
            });
            dgvChamados.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Email",
                HeaderText = "Email",
                DataPropertyName = "email",
                FillWeight = 140
            });
            dgvChamados.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Urgencia",
                HeaderText = "Urgência",
                DataPropertyName = "urgencia",
                FillWeight = 80
            });
            dgvChamados.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Assunto",
                HeaderText = "Assunto",
                DataPropertyName = "assunto",
                FillWeight = 140
            });
            dgvChamados.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Descricao",
                HeaderText = "Descrição",
                DataPropertyName = "descricao",
                FillWeight = 220
            });
            dgvChamados.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DataCriacao",
                HeaderText = "Criado em",
                DataPropertyName = "datacriacao",
                FillWeight = 90,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" }
            });
            dgvChamados.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Situacao",
                HeaderText = "Situação",
                DataPropertyName = "situacao",
                FillWeight = 90
            });
            dgvChamados.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "AnexoPath",
                DataPropertyName = "anexo_caminho",
                Visible = false
            });

            var colVer = new DataGridViewButtonColumn
            {
                Name = "Ver",
                HeaderText = " ",
                Text = "Ver",
                UseColumnTextForButtonValue = true,
                Width = 70
            };
            var colExcluir = new DataGridViewButtonColumn
            {
                Name = "Excluir",
                HeaderText = " ",
                Text = "Excluir",
                UseColumnTextForButtonValue = true,
                Width = 80
            };

            dgvChamados.Columns.Add(colVer);
            dgvChamados.Columns.Add(colExcluir);
            dgvChamados.CellClick += DgvChamados_CellClick;

            // painel central (lista ou detalhe)
            _contentPanel = new Panel();
            _contentPanel.Dock = DockStyle.Fill;
            _contentPanel.Padding = new Padding(12);
            _contentPanel.Resize += ContentPanel_Resize; // pra centralizar o grid

            // container do grid para controlar tamanho e centralização
            _gridContainer = new Panel();
            _gridContainer.Width = 1200;   // tabela maior
            _gridContainer.Height = 420;  // altura maior
            _gridContainer.BackColor = Color.Transparent;

            _gridContainer.Controls.Add(dgvChamados);

            _contentPanel.Controls.Add(_gridContainer);

            Controls.Add(_contentPanel);
            _contentPanel.BringToFront();

            // faz a primeira centralização
            CentralizarGrid();
        }

        private void ContentPanel_Resize(object sender, EventArgs e)
        {
            CentralizarGrid();
        }

        private void CentralizarGrid()
        {
            if (_gridContainer == null || _contentPanel == null) return;

            // centraliza horizontalmente e deixa "em cima da tela" (topo com margem)
            int left = (_contentPanel.ClientSize.Width - _gridContainer.Width) / 2;
            if (left < 0) left = 0;

            _gridContainer.Left = left;
            _gridContainer.Top = 30; // um pouquinho abaixo dos filtros
        }

        private Button MakeFilterButton(string text, EventHandler onClick)
        {
            var btn = new Button();
            btn.Text = text;
            btn.Height = 36;
            btn.Width = 140;
            btn.Margin = new Padding(6);
            btn.FlatStyle = FlatStyle.Flat;
            btn.BackColor = Color.Black;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += onClick;
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(40, 40, 40);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.Black;
            return btn;
        }

        private void CarregarChamados(string situacaoFiltro = null)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();

                    string sql = @"
                        SELECT id, nome, email, urgencia, assunto, descricao, datacriacao, situacao, anexo_caminho
                        FROM chamados
                        WHERE id_usuario = @usuario";

                    if (!string.IsNullOrEmpty(situacaoFiltro))
                        sql += " AND situacao = @situacao";

                    sql += " ORDER BY datacriacao DESC;";

                    using (var da = new NpgsqlDataAdapter(sql, conn))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@usuario", _usuarioAutenticado);
                        if (!string.IsNullOrEmpty(situacaoFiltro))
                            da.SelectCommand.Parameters.AddWithValue("@situacao", situacaoFiltro);

                        var dt = new DataTable();
                        da.Fill(dt);
                        dgvChamados.DataSource = dt;

                        // destaque visual por situação
                        foreach (DataGridViewRow row in dgvChamados.Rows)
                        {
                            string sit = Convert.ToString(row.Cells["Situacao"].Value) ?? "";
                            if (sit.Equals("Aberto", StringComparison.OrdinalIgnoreCase) ||
                                sit.Equals("Abertos", StringComparison.OrdinalIgnoreCase))
                            {
                                row.DefaultCellStyle.BackColor = Color.FromArgb(250, 255, 250);
                            }
                            else if (sit.Equals("Em Andamento", StringComparison.OrdinalIgnoreCase))
                            {
                                row.DefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);
                            }
                            else if (sit.Equals("Encerrado", StringComparison.OrdinalIgnoreCase) ||
                                     sit.Equals("Encerrados", StringComparison.OrdinalIgnoreCase))
                            {
                                row.DefaultCellStyle.BackColor = Color.FromArgb(252, 248, 248);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao carregar os chamados: " + ex.Message,
                        "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DgvChamados_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var grid = dgvChamados;
            var colName = grid.Columns[e.ColumnIndex].Name;

            if (colName == "Ver")
            {
                int id = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["Id"].Value);
                MostrarDetalheChamado(id);
            }
            else if (colName == "Excluir")
            {
                int id = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["Id"].Value);

                if (MessageBox.Show("Excluir este chamado permanentemente?",
                        "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    ExcluirChamado(id);
                    CarregarChamados();
                }
            }
        }

        private void ExcluirChamado(int id)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();

                    // apaga chat
                    using (var cmdChat = new NpgsqlCommand(
                               "DELETE FROM chat_mensagens WHERE chamado_id = @id;", conn))
                    {
                        cmdChat.Parameters.AddWithValue("@id", id);
                        cmdChat.ExecuteNonQuery();
                    }

                    // apaga chamado
                    using (var cmd = new NpgsqlCommand(
                               "DELETE FROM chamados WHERE id = @id AND id_usuario = @usuario;", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@usuario", _usuarioAutenticado);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao excluir chamado: " + ex.Message,
                        "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void MostrarDetalheChamado(int id)
        {
            // esconde os filtros quando estiver no detalhe
            pnlFiltros.Visible = false;

            _contentPanel.Controls.Clear();

            var detalhe = new ChamadoDetalheForm(_connectionString, id, _usuarioAutenticado);
            detalhe.TopLevel = false;
            detalhe.FormBorderStyle = FormBorderStyle.None;
            detalhe.Dock = DockStyle.Fill;

            _contentPanel.Controls.Add(detalhe);
            detalhe.Show();
        }
    }
