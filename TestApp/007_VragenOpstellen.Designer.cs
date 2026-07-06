namespace TestApp
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button1 = new Button();
            button2 = new Button();
            button4 = new Button();
            button3 = new Button();
            button5 = new Button();
            contentPanel = new Panel();
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            //
            // button1
            //
            button1.Location = new Point(12, 12);
            button1.Name = "button1";
            button1.Size = new Size(230, 23);
            button1.TabIndex = 0;
            button1.Text = "Nieuwe vragen maken";
            button1.UseVisualStyleBackColor = true;
            button1.Click += Button1_Click;
            //
            // button2
            //
            button2.Location = new Point(12, 41);
            button2.Name = "button2";
            button2.Size = new Size(230, 23);
            button2.TabIndex = 1;
            button2.Text = "Antwoordmogelijkheden toevoegen";
            button2.UseVisualStyleBackColor = true;
            button2.Click += Button2_Click;
            //
            // button4
            //
            button4.Location = new Point(12, 128);
            button4.Name = "button4";
            button4.Size = new Size(230, 23);
            button4.TabIndex = 3;
            button4.Text = "Terug";
            button4.UseVisualStyleBackColor = true;
            button4.Click += Button4_Click;
            //
            // button3
            //
            button3.Location = new Point(12, 99);
            button3.Name = "button3";
            button3.Size = new Size(230, 23);
            button3.TabIndex = 4;
            button3.Text = "Juist antwoord selecteren";
            button3.UseVisualStyleBackColor = true;
            button3.Click += Button3_Click;
            //
            // button5
            //
            button5.Location = new Point(12, 70);
            button5.Name = "button5";
            button5.Size = new Size(230, 23);
            button5.TabIndex = 5;
            button5.Text = "Antwoorden koppelen";
            button5.UseVisualStyleBackColor = true;
            button5.Click += Button5_Click;
            //
            // contentPanel
            //
            contentPanel.Controls.Add(button5);
            contentPanel.Controls.Add(button3);
            contentPanel.Controls.Add(button4);
            contentPanel.Controls.Add(button2);
            contentPanel.Controls.Add(button1);
            contentPanel.Location = new Point(0, 0);
            contentPanel.Margin = new Padding(0);
            contentPanel.Name = "contentPanel";
            contentPanel.Size = new Size(252, 160);
            contentPanel.TabIndex = 0;
            //
            // tableLayoutPanel1
            //
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(contentPanel, 1, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Size = new Size(1024, 720);
            tableLayoutPanel1.TabIndex = 5;
            //
            // Form2
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1024, 720);
            Controls.Add(tableLayoutPanel1);
            Name = "Form2";
            Text = "Vragen opstellen";
            FormClosing += CloseApplication;
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
        private Button button2;
        private Button button4;
        private Button button3;
        private Button button5;
        private Panel contentPanel;
        private TableLayoutPanel tableLayoutPanel1;
    }
}