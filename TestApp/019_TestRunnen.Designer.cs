using System.Reflection.Metadata.Ecma335;

namespace TestApp
{
    partial class Form20
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
            antwoordLabels = new List<Label>();
            antwoordRBs = new List<RadioButton>();
            button1 = new Button();
            panel1 = new Panel();
            label1 = new Label();
            label2 = new Label();
            panel2 = new Panel();
            button2 = new Button();
            label3 = new Label();
            textBox1 = new TextBox();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point);
            button1.Location = new Point(277, 348);
            button1.Name = "button1";
            button1.Size = new Size(186, 56);
            button1.TabIndex = 0;
            button1.Text = "Test starten";
            button1.UseVisualStyleBackColor = true;
            button1.Click += Button1_Click;
            // 
            // panel1
            // 
            panel1.Location = new Point(43, 71);
            panel1.Name = "panel1";
            panel1.Size = new Size(340, 212);
            panel1.TabIndex = 1;
            panel1.Visible = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            label1.ForeColor = SystemColors.ControlLight;
            label1.Location = new Point(348, 264);
            label1.Name = "label1";
            label1.Size = new Size(70, 30);
            label1.TabIndex = 2;
            label1.Text = "Naam";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 14.25F, FontStyle.Italic, GraphicsUnit.Point);
            label2.ForeColor = SystemColors.ControlLight;
            label2.Location = new Point(43, 9);
            label2.Name = "label2";
            label2.Size = new Size(61, 25);
            label2.TabIndex = 3;
            label2.Text = "label2";
            label2.Visible = false;
            // 
            // panel2
            // 
            panel2.Location = new Point(405, 71);
            panel2.Name = "panel2";
            panel2.Size = new Size(340, 212);
            panel2.TabIndex = 4;
            panel2.Visible = false;
            // 
            // button2
            // 
            button2.Font = new Font("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point);
            button2.Location = new Point(54, 330);
            button2.Name = "button2";
            button2.Size = new Size(185, 56);
            button2.TabIndex = 5;
            button2.Text = "Antwoord";
            button2.UseVisualStyleBackColor = true;
            button2.Visible = false;
            button2.Click += Button2_Click;
            // 
            // label3
            // 
            label3.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point);
            label3.ForeColor = SystemColors.ControlLight;
            label3.Location = new Point(128, 9);
            label3.Name = "label3";
            label3.Size = new Size(585, 73);
            label3.TabIndex = 6;
            label3.Text = "Dit is een check om te zien of de tekst op meerdere regels komt te staan.";
            label3.Visible = false;
            // 
            // textBox1
            // 
            textBox1.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            textBox1.Location = new Point(278, 306);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(186, 36);
            textBox1.TabIndex = 3;
            // 
            // label4
            // 
            label4.Image = Properties.Resources.mol_logo;
            label4.Location = new Point(370, 30);
            label4.Name = "label4";
            label4.Size = new Size(388, 455);
            label4.TabIndex = 7;
            // 
            // label5
            // 
            label5.Image = Properties.Resources.mol_logo_small;
            label5.Location = new Point(91, 213);
            label5.Name = "label5";
            label5.Size = new Size(194, 228);
            label5.TabIndex = 8;
            label5.Visible = false;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 426);
            label6.Name = "label6";
            label6.Size = new Size(38, 15);
            label6.TabIndex = 9;
            label6.Text = "label6";
            label6.Visible = false;
            // 
            // Form20
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaptionText;
            ClientSize = new Size(800, 450);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label1);
            Controls.Add(textBox1);
            Controls.Add(label3);
            Controls.Add(button2);
            Controls.Add(panel2);
            Controls.Add(label2);
            Controls.Add(panel1);
            Controls.Add(button1);
            Name = "Form20";
            RightToLeft = RightToLeft.No;
            Text = "Form20";
            FormClosing += CloseApplication;
            ResumeLayout(false);
            PerformLayout();
        }

        public void repositionElements()
        {
            int currentWidth = Screen.GetWorkingArea(this).Width;
            int currentHeight = Screen.GetWorkingArea(this).Height;

            int centerWidthPixel = currentWidth / 2;
            int centerHeightPixel = currentHeight / 2;

            int bottomEmpty = 10;
            int betweenEmpty = 10;

            if (centerHeightPixel > 150)
            {
                bottomEmpty = 50;
                betweenEmpty = 15;
            }

            if (centerHeightPixel > 250)
            {
                bottomEmpty = 100;
                betweenEmpty = 30;
            }

            if (centerHeightPixel > 350)
            {
                bottomEmpty = 150;
                betweenEmpty = 45;
            }

            if (centerHeightPixel > 450)
            {
                bottomEmpty = 200;
                betweenEmpty = 60;
            }

            this.label1.Location = new System.Drawing.Point(centerWidthPixel - 35, this.Height - bottomEmpty - (betweenEmpty * 2) - 66);
            this.textBox1.Location = new System.Drawing.Point(centerWidthPixel - 93, this.Height - bottomEmpty - 36 - betweenEmpty);
            this.button1.Location = new System.Drawing.Point(centerWidthPixel - 93, this.Height - bottomEmpty - 30);

            this.label4.Location = new System.Drawing.Point(centerWidthPixel - 194, centerHeightPixel - 455);

            this.label5.Location = new System.Drawing.Point(20, 20);
            this.label2.Location = new System.Drawing.Point(224, 134);
            this.label3.Location = new System.Drawing.Point(254, 134);
            this.label3.Size = new System.Drawing.Size(this.Width - 400, 150);

            this.panel1.Location = new System.Drawing.Point(224, 170);
            this.panel2.Location = new System.Drawing.Point(centerWidthPixel, 170);

            this.panel1.Size = new System.Drawing.Size(centerWidthPixel - 244, 7 * 38 + 15);
            this.panel2.Size = new System.Drawing.Size(centerWidthPixel - 244, 7 * 38 + 15);

            this.button2.Location = new System.Drawing.Point(224, 7 * 38 + 245);
        }

        public void ShowNextQuestion(int vraagNummer, string question, List<List<string>> antwoordenOpties, string alfabetisch = "1")
        {
            label2.Text = Convert.ToString(vraagNummer);
            label3.Text = question;

            this.panel1.Controls.Clear();
            this.panel2.Controls.Clear();
            antwoordLabels.Clear();
            antwoordRBs.Clear();

            List<string> opties = new();

            foreach(List<string> antwoord in antwoordenOpties)
            {
                opties.Add(antwoord[2]);
            }

            if(alfabetisch == "1")
                opties.Sort();

            int optieId = 0;
            bool twoColumns = false;
            int splitId = 0;
            Panel toPutIn = panel1;

            if (opties.Count > 5) {
                twoColumns = true;
                splitId = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(opties.Count) / 2.0));
            }

            foreach(string optie in opties)
            {
                optieId++;
                int calNumber = optieId;

                if (twoColumns)
                {
                    if (splitId < optieId)
                    {
                        calNumber = optieId - splitId;
                        toPutIn = panel2;
                    }
                }

                RadioButton optieRB = new RadioButton();
                optieRB.Name = "Keuze~" + Convert.ToString(optieId);
                optieRB.Location = new System.Drawing.Point(10, 17 + (30 * calNumber));
                optieRB.Size = new System.Drawing.Size(20, 20);
                optieRB.TabIndex = 8 + optieId;

                antwoordRBs.Add(optieRB);

                toPutIn.Controls.Add(optieRB);

                Label optieLbl = new Label();
                optieLbl.Location = new System.Drawing.Point(30, 10 + (30 * calNumber));
                optieLbl.AutoSize = true;
                optieLbl.Name = "label~" + Convert.ToString(optieId);
                optieLbl.Size = new System.Drawing.Size(toPutIn.Width - 40, 20);
                optieLbl.TabIndex = 8 + opties.Count + optieId;
                optieLbl.Text = optie;
                optieLbl.ForeColor = System.Drawing.Color.White;
                optieLbl.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.GraphicsUnit.Point);

                antwoordLabels.Add(optieLbl);

                toPutIn.Controls.Add(optieLbl);
            }
        }

        #endregion

        private List<Label> antwoordLabels;
        private List<RadioButton> antwoordRBs;
        private Button button1;
        private Panel panel1;
        private Label label1;
        private Label label2;
        private Panel panel2;
        private Button button2;
        private Label label3;
        private TextBox textBox1;
        private Label label4;
        private Label label5;
        private Label label6;
    }
}