﻿using System;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using GestaoDChamados.Usuario.ChatBots.chatgpt;
using AtendeAI;
using GestaoDChamados.Admin;


namespace ChamadosApp
{
    public class AdminForm : Form
    {
        private Panel sidebar, header, mainContent;
        private Label lblTitulo;

        public AdminForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "atende.AI";
            Size = new Size(1980, 1080);
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            BackColor = Color.DarkGray;

            sidebar = CriarSidebar();
            header = CriarHeader();

            mainContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.DarkGray,
            };

            Controls.Add(mainContent);
            Controls.Add(header);
            Controls.Add(sidebar);

            CriarMainContent();
        }
        private Panel CriarSidebar()
        {
            var panel = new Panel
            {
                Width = 250,
                BackColor = Color.Black,
                Dock = DockStyle.Left,
            };

            string[] botoes = { "MENU", "CADASTRO DE USUÁRIO", "CADASTRO DE FUNCIONÁRIO", "LISTA DE CADASTROS" };

            // Adiciona os botões de baixo para cima
            for (int i = botoes.Length - 1; i >= 0; i--)
            {
                string nome = botoes[i];
                var button = new Button
                {
                    Text = nome,
                    Height = 45,
                    Dock = DockStyle.Top,
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.White,
                    BackColor = Color.Black,
                    Font = new Font("Segoe UI", 10, FontStyle.Regular),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Padding = new Padding(0)
                };
                button.FlatAppearance.BorderSize = 0;

                button.MouseEnter += (s, e) =>
                {
                    button.BackColor = Color.White;
                    button.ForeColor = Color.Black;
                };
                button.MouseLeave += (s, e) =>
                {
                    button.BackColor = Color.Black;
                    button.ForeColor = Color.White;
                };
                if (nome == "MENU")
                {
                    button.Click += (s, e) =>
                    {
                        mainContent.Controls.Clear();
                        CriarMainContent();
                    };
                }
                else if (nome == "CADASTRO DE FUNCIONÁRIO")
                {
                    button.Click += (s, e) =>
                    {
                        mainContent.Controls.Clear();
                        var chatForm = new CadastroFuncionario
                        {
                            TopLevel = false,
                            Dock = DockStyle.Fill
                        };
                        mainContent.Controls.Add(chatForm);
                        chatForm.Show();
                    };
                }
                else if (nome == "CADASTRO DE USUÁRIO")
                {
                    button.Click += (s, e) =>
                    {
                        mainContent.Controls.Clear();
                        var chatForm = new CadastroFuncionario
                        {
                            TopLevel = false,
                            Dock = DockStyle.Fill
                        };
                        mainContent.Controls.Add(chatForm);
                        chatForm.Show();
                    };
                }
                //else if (nome == "LISTA DE CADASTROS")
                //{
                //    button.Click += (s, e) =>
                //    {
                //        mainContent.Controls.Clear();
                //        var chatForm = new ListarCadastro
                //        {
                //            TopLevel = false,
                //            Dock = DockStyle.Fill
                //        };
                //        mainContent.Controls.Add(chatForm);
                //        chatForm.Show();
                //    };
                //}

                panel.Controls.Add(button); // Adiciona o botão à lista
            }

            // Espaço entre logo e botões
            var spacer = new Panel
            {
                Height = 50,
                Dock = DockStyle.Top
            };
            panel.Controls.Add(spacer);

            // Logo no topo
            var logo = new PictureBox
            {
                Image = Image.FromFile("resources/atendeai.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                Height = 90,
                Dock = DockStyle.Top,
                BackColor = Color.Transparent
            };
            panel.Controls.Add(logo);

            return panel;
        }

        private Panel CriarHeader()
        {
            var panel = new Panel
            {
                Height = 60,
                BackColor = Color.DarkGray,
                Dock = DockStyle.Top
            };

            lblTitulo = new Label
            {
                Text = "Painel do Administrador",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 60),
                AutoSize = true,
                Location = new Point(10, 15)
            };

            panel.Controls.Add(lblTitulo);
            return panel;
        }

        private void CriarMainContent()
        {
            mainContent.Controls.Clear();

            // Painel centralizado
            var painelCentral = new Panel
            {
                Size = new Size(600, 300),
                Location = new Point((ClientSize.Width - 600) / 2, 50),
                Anchor = AnchorStyles.Top
            };

            // Logo da empresa


        }
    }
}