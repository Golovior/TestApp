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
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.button1.Location = new System.Drawing.Point(277, 348);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(186, 56);
            this.button1.TabIndex = 0;
            this.button1.Text = "Test starten";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(43, 71);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(340, 212);
            this.panel1.TabIndex = 1;
            this.panel1.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label1.Location = new System.Drawing.Point(348, 264);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 30);
            this.label1.TabIndex = 2;
            this.label1.Text = "Naam";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.label2.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label2.Location = new System.Drawing.Point(43, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 25);
            this.label2.TabIndex = 3;
            this.label2.Text = "label2";
            this.label2.Visible = false;
            // 
            // panel2
            // 
            this.panel2.Location = new System.Drawing.Point(405, 71);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(340, 212);
            this.panel2.TabIndex = 4;
            this.panel2.Visible = false;
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.button2.Location = new System.Drawing.Point(54, 330);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(185, 56);
            this.button2.TabIndex = 5;
            this.button2.Text = "Antwoord";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label3.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label3.Location = new System.Drawing.Point(128, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(585, 73);
            this.label3.TabIndex = 6;
            this.label3.Text = "Dit is een check om te zien of de tekst op meerdere regels komt te staan.";
            this.label3.Visible = false;
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textBox1.Location = new System.Drawing.Point(278, 306);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(186, 36);
            this.textBox1.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.Image = global::TestApp.Properties.Resources.mol_logo;
            this.label4.Location = new System.Drawing.Point(370, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(388, 455);
            this.label4.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.Image = global::TestApp.Properties.Resources.mol_logo_small;
            this.label5.Location = new System.Drawing.Point(91, 213);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(194, 228);
            this.label5.TabIndex = 8;
            this.label5.Visible = false;
            // 
            // Form20
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button1);
            this.Name = "Form20";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Text = "Form20";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CloseApplication);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        public void prepareQuestion(List<Options> options)
        {
            int optionId = 0;
            if (radioButtons == null)
                radioButtons = new List<RadioButton>();
            else
                radioButtons.Clear();

            foreach(Options o in options)
            {
                RadioButton r = new System.Windows.Forms.RadioButton();

                int height = optionId * 38 + 10;

                if(optionId > 4)
                {
                    height = height - (5 * 38);
                }

                r.AutoSize = true;
                r.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
                r.ForeColor = System.Drawing.SystemColors.ControlLight;
                r.Location = new System.Drawing.Point(20, height);
                r.Name = "radioButton~" + Convert.ToString(optionId);
                r.Size = new System.Drawing.Size(157, 34);
                r.TabIndex = 3;
                r.TabStop = true;
                r.Text = o.GetValue();
                r.UseVisualStyleBackColor = true;
                r.Visible = true;

                if(optionId > 4)
                {
                    panel2.Controls.Add(r);
                }
                else
                {
                    panel1.Controls.Add(r);
                }

                radioButtons.Add(r);

                optionId++;
            }
            
        }

        public void repositionElements()
        {
            int centerWidthPixel = this.Width / 2;
            int centerHeightPixel = this.Height / 2;

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

            this.panel1.Size = new System.Drawing.Size(centerWidthPixel - 244, 5 * 38);
            this.panel2.Size = new System.Drawing.Size(centerWidthPixel - 244, 5 * 38);

            this.button2.Location = new System.Drawing.Point(224, 5 * 38 + 245);
        }

        #endregion

        private Button button1;
        private Panel panel1;
        private Label label1;
        private Label label2;
        private Panel panel2;
        private List<RadioButton> radioButtons;
        private Button button2;
        private Label label3;
        private TextBox textBox1;
        private Label label4;
        private Label label5;
    }
}