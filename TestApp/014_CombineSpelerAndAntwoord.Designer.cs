namespace TestApp
{
    partial class CombineSpelerAndAntwoordForm
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

        private void AddPlayers(string name, int order, bool connected)
        {
            CheckBox currentCheckbox = new System.Windows.Forms.CheckBox();
            Label currentLabel = new System.Windows.Forms.Label();

            int width = 15;
            int heightOrder = order;

            if (order > 8)
            {
                width += 300;
                heightOrder -= 8;
            }

            int height = 27 * heightOrder;
            //
            // checkbox
            //
            currentCheckbox.Location = new System.Drawing.Point(width, height);
            currentCheckbox.Name = "connected~" + order;
            currentCheckbox.Size = new System.Drawing.Size(20, 20);
            currentCheckbox.TabIndex = 1;
            currentCheckbox.UseVisualStyleBackColor = true;
            if (connected)
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
            currentLabel.Text = name;

            this.panel1.Controls.Add(currentCheckbox);
            this.panel1.Controls.Add(currentLabel);

            this.checkboxes.Add(currentCheckbox);
            this.labels.Add(currentLabel);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new Panel();
            button2 = new Button();
            button1 = new Button();
            comboBox2 = new ComboBox();
            comboBox1 = new ComboBox();
            label2 = new Label();
            label1 = new Label();
            comboBox3 = new ComboBox();
            label3 = new Label();
            SuspendLayout();
            //
            // panel1
            //
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.Location = new Point(12, 99);
            panel1.Name = "panel1";
            panel1.Size = new Size(491, 241);
            panel1.TabIndex = 13;
            //
            // button2
            //
            button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            button2.Location = new Point(12, 375);
            button2.Name = "button2";
            button2.Size = new Size(491, 23);
            button2.TabIndex = 12;
            button2.Text = "Terug";
            button2.UseVisualStyleBackColor = true;
            button2.Click += Button2_Click;
            //
            // button1
            //
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            button1.Location = new Point(12, 346);
            button1.Name = "button1";
            button1.Size = new Size(491, 23);
            button1.TabIndex = 11;
            button1.Text = "Opslaan";
            button1.UseVisualStyleBackColor = true;
            button1.Click += Button1_Click;
            //
            // comboBox2
            //
            comboBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            comboBox2.FormattingEnabled = true;
            comboBox2.Location = new Point(83, 41);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(420, 23);
            comboBox2.TabIndex = 10;
            comboBox2.SelectedIndexChanged += SelectedQuestion;
            //
            // comboBox1
            //
            comboBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(83, 12);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(420, 23);
            comboBox1.TabIndex = 9;
            comboBox1.SelectedIndexChanged += SelectedOpdracht;
            //
            // label2
            //
            label2.AutoSize = true;
            label2.Location = new Point(12, 44);
            label2.Name = "label2";
            label2.Size = new Size(37, 15);
            label2.TabIndex = 8;
            label2.Text = "Vraag";
            //
            // label1
            //
            label1.AutoSize = true;
            label1.Location = new Point(12, 15);
            label1.Name = "label1";
            label1.Size = new Size(57, 15);
            label1.TabIndex = 7;
            label1.Text = "Opdracht";
            //
            // comboBox3
            //
            comboBox3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            comboBox3.FormattingEnabled = true;
            comboBox3.Location = new Point(83, 70);
            comboBox3.Name = "comboBox3";
            comboBox3.Size = new Size(420, 23);
            comboBox3.TabIndex = 15;
            //
            // label3
            //
            label3.AutoSize = true;
            label3.Location = new Point(12, 73);
            label3.Name = "label3";
            label3.Size = new Size(60, 15);
            label3.TabIndex = 14;
            label3.Text = "Antwoord";
            //
            // CombineSpelerAndAntwoordForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(520, 412);
            Controls.Add(comboBox3);
            Controls.Add(label3);
            Controls.Add(panel1);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(comboBox2);
            Controls.Add(comboBox1);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "CombineSpelerAndAntwoordForm";
            Text = "Optie selecteren bij Speler";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private List<Label> labels = new();
        private List<CheckBox> checkboxes = new();
        private Panel panel1;
        private Button button2;
        private Button button1;
        private ComboBox comboBox2;
        private ComboBox comboBox1;
        private Label label2;
        private Label label1;
        private ComboBox comboBox3;
        private Label label3;
    }
}