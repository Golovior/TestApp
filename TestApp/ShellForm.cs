using System;
using System.Drawing;
using System.Windows.Forms;

namespace TestApp
{
    public enum Section
    {
        SpelSpelers,
        Vragen,
        Tests,
        UitvoerenResultaten
    }

    public class ShellForm : BaseForm
    {
        private readonly Panel contentPanel;

        public ShellForm()
        {
            Text = "Test Applicatie";
            ClientSize = new Size(1280, 800);

            contentPanel = new Panel
            {
                Dock = DockStyle.Fill
            };

            Panel sidebarPanel = new()
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = Color.WhiteSmoke
            };

            sidebarPanel.Controls.Add(CreateSidebarButton("Afsluiten", 700, (s, e) => Application.Exit()));
            sidebarPanel.Controls.Add(CreateSidebarButton("Uitvoeren && Resultaten", 202, (s, e) => ShowSection(Section.UitvoerenResultaten)));
            sidebarPanel.Controls.Add(CreateSidebarButton("Tests", 164, (s, e) => ShowSection(Section.Tests)));
            sidebarPanel.Controls.Add(CreateSidebarButton("Vragen", 126, (s, e) => ShowSection(Section.Vragen)));
            sidebarPanel.Controls.Add(CreateSidebarButton("Spel && Spelers", 88, (s, e) => ShowSection(Section.SpelSpelers)));

            Label title = new()
            {
                Text = "Test Applicatie",
                Location = new Point(12, 20),
                Size = new Size(196, 40),
                Font = new Font("Segoe UI", 14F, FontStyle.Bold)
            };
            sidebarPanel.Controls.Add(title);

            Controls.Add(contentPanel);
            Controls.Add(sidebarPanel);

            FormClosing += (s, e) => Application.Exit();

            ShowSection(Section.SpelSpelers);
        }

        private static Button CreateSidebarButton(string text, int y, EventHandler onClick)
        {
            Button button = new()
            {
                Text = text,
                Location = new Point(12, y),
                Size = new Size(196, 32),
                UseVisualStyleBackColor = true
            };
            button.Click += onClick;
            return button;
        }

        public void ShowSection(Section section)
        {
            contentPanel.SuspendLayout();

            Control? outgoing = contentPanel.Controls.Count > 0 ? contentPanel.Controls[0] : null;

            UserControl next = section switch
            {
                Section.SpelSpelers => new SpelSpelersControl(this),
                Section.Vragen => new VragenControl(this),
                Section.Tests => new TestsControl(this),
                Section.UitvoerenResultaten => new UitvoerenResultatenControl(this),
                _ => throw new ArgumentOutOfRangeException(nameof(section))
            };
            next.Dock = DockStyle.Fill;

            contentPanel.Controls.Clear();
            outgoing?.Dispose();

            contentPanel.Controls.Add(next);
            contentPanel.ResumeLayout();
            next.Focus();
        }
    }
}
