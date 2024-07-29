namespace TestApp
{
    partial class Form21
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

        protected void showResultaten(List<string[]> spelerInfo)
        {
            if (spelerInfo == null)
                return;

            this.panel1.Controls.Clear();
            labelsTijd = new List<Label>();
            labelsAntwoorden = new List<Label>();
            labelsSpelers = new List<Label>();

            int key = 0;

            foreach (string[] info in spelerInfo)
            {
                Label spelerNaamLabel = new System.Windows.Forms.Label();
                Label antwoordenLabel = new System.Windows.Forms.Label();
                Label TijdLabel = new System.Windows.Forms.Label();

                spelerNaamLabel.AutoSize = true;
                spelerNaamLabel.Location = new System.Drawing.Point(0, 10 + (key * 15));
                spelerNaamLabel.Name = "labelNaam" + Convert.ToString(key);
                spelerNaamLabel.Size = new System.Drawing.Size(150, 15);
                spelerNaamLabel.TabIndex = 0;
                spelerNaamLabel.Text = info[0];

                antwoordenLabel.AutoSize = true;
                antwoordenLabel.Location = new System.Drawing.Point(199, 10 + (key * 15));
                antwoordenLabel.Name = "labelAntwoorden" + Convert.ToString(key);
                antwoordenLabel.Size = new System.Drawing.Size(150, 15);
                antwoordenLabel.TabIndex = 1;
                antwoordenLabel.Text = info[1];

                TijdLabel.AutoSize = true;
                TijdLabel.Location = new System.Drawing.Point(384, 10 + (key * 15));
                TijdLabel.Name = "labelTijd" + Convert.ToString(key);
                TijdLabel.Size = new System.Drawing.Size(150, 15);
                TijdLabel.TabIndex = 2;
                TijdLabel.Text = info[2];

                this.labelsTijd.Add(TijdLabel);
                this.labelsAntwoorden.Add(antwoordenLabel);
                this.labelsSpelers.Add(spelerNaamLabel);

                this.panel1.Controls.Add(TijdLabel);
                this.panel1.Controls.Add(antwoordenLabel);
                this.panel1.Controls.Add(spelerNaamLabel);

                key++;
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Test";
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Location = new System.Drawing.Point(12, 63);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(450, 217);
            this.panel1.TabIndex = 1;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(54, 12);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(408, 23);
            this.comboBox1.TabIndex = 2;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.ChangeTest);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Speler";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(211, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "Juiste antwoorden";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(396, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 15);
            this.label4.TabIndex = 5;
            this.label4.Text = "Tijd";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 286);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(450, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Terug";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // Form21
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 324);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Name = "Form21";
            this.Text = "Form21";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CloseApplication);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private Panel panel1;
        private ComboBox comboBox1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Button button1;
        private List<Label> labelsTijd = new List<Label>();
        private List<Label> labelsAntwoorden = new List<Label>();
        private List<Label> labelsSpelers = new List<Label>();
    }
}