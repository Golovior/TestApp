namespace TestApp
{
    partial class JuistAntwoordSelecteren
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
            labels = new List<Label>();
            checkboxes = new List<CheckBox>();
            label1 = new Label();
            label2 = new Label();
            comboBox1 = new ComboBox();
            comboBox2 = new ComboBox();
            button1 = new Button();
            button2 = new Button();
            panel1 = new Panel();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 15);
            label1.Name = "label1";
            label1.Size = new Size(57, 15);
            label1.TabIndex = 0;
            label1.Text = "Opdracht";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 44);
            label2.Name = "label2";
            label2.Size = new Size(37, 15);
            label2.TabIndex = 1;
            label2.Text = "Vraag";
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(84, 12);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(419, 23);
            comboBox1.TabIndex = 2;
            comboBox1.SelectedIndexChanged += SelectedOpdracht;
            // 
            // comboBox2
            // 
            comboBox2.FormattingEnabled = true;
            comboBox2.Location = new Point(84, 41);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(419, 23);
            comboBox2.TabIndex = 3;
            comboBox2.SelectedIndexChanged += ChangeQuestion;
            // 
            // button1
            // 
            button1.Location = new Point(12, 317);
            button1.Name = "button1";
            button1.Size = new Size(491, 23);
            button1.TabIndex = 4;
            button1.Text = "Opslaan";
            button1.UseVisualStyleBackColor = true;
            button1.Click += Button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(12, 346);
            button2.Name = "button2";
            button2.Size = new Size(491, 23);
            button2.TabIndex = 5;
            button2.Text = "Terug";
            button2.UseVisualStyleBackColor = true;
            button2.Click += Button2_Click;
            // 
            // panel1
            // 
            panel1.Location = new Point(12, 70);
            panel1.Name = "panel1";
            panel1.Size = new Size(491, 241);
            panel1.TabIndex = 6;
            // 
            // JuistAntwoordSelecteren
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(517, 380);
            Controls.Add(panel1);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(comboBox2);
            Controls.Add(comboBox1);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "JuistAntwoordSelecteren";
            Text = "Juist antwoord selecteren";
            ResumeLayout(false);
            PerformLayout();
        }

        private void updateElements()
        {
            this.panel1.Controls.Clear();

            this.labels.Clear();
            this.checkboxes.Clear();
        }

        private void MakeAnswerRow(List<string> antwoord, int order, bool correct = false)
        {
            CheckBox currentCheckbox = new System.Windows.Forms.CheckBox();
            Label currentLabel = new System.Windows.Forms.Label();

            order++;

            int width = 15;
            if(order > 8)
            {
                width += 300;
                order -= 8;
            }

            int height = 27 * order;
            // 
            // checkbox
            // 
            currentCheckbox.Location = new System.Drawing.Point(width, height);
            currentCheckbox.Name = "correct~" + order;
            currentCheckbox.Size = new System.Drawing.Size(20, 20);
            currentCheckbox.TabIndex = 1;
            currentCheckbox.UseVisualStyleBackColor = true;
            if (correct)
            {
                currentCheckbox.Checked = true;
            }
            // 
            // label1
            // 
            currentLabel.Location = new System.Drawing.Point(width + 20, height + 4);
            currentLabel.Name = "label~" + order;
            currentLabel.Size = new System.Drawing.Size(250, 18);
            currentLabel.TabIndex = 2;
            currentLabel.Text = antwoord[2];

            this.panel1.Controls.Add(currentCheckbox);
            this.panel1.Controls.Add(currentLabel);

            this.checkboxes.Add(currentCheckbox);
            this.labels.Add(currentLabel);
        }

        #endregion

        private List<Label> labels = new();
        private List<CheckBox> checkboxes = new();
        private Label label1;
        private Label label2;
        private ComboBox comboBox1;
        private ComboBox comboBox2;
        private Button button1;
        private Button button2;
        private Panel panel1;
    }
}