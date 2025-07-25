namespace TestApp
{
    partial class Form14
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
            label1 = new Label();
            comboBox1 = new ComboBox();
            comboBox2 = new ComboBox();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            button1 = new Button();
            button3 = new Button();
            comboBox3 = new ComboBox();
            label5 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 15);
            label1.Name = "label1";
            label1.Size = new Size(27, 15);
            label1.TabIndex = 0;
            label1.Text = "Test";
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(128, 12);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(331, 23);
            comboBox1.TabIndex = 1;
            comboBox1.SelectedIndexChanged += TestChange;
            // 
            // comboBox2
            // 
            comboBox2.FormattingEnabled = true;
            comboBox2.Location = new Point(128, 41);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(331, 23);
            comboBox2.TabIndex = 2;
            comboBox2.SelectedIndexChanged += OpdrachtChange;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 44);
            label2.Name = "label2";
            label2.Size = new Size(57, 15);
            label2.TabIndex = 3;
            label2.Text = "Opdracht";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 96);
            label3.Name = "label3";
            label3.Size = new Size(110, 15);
            label3.TabIndex = 4;
            label3.Text = "Vragen toegevoegd";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(128, 96);
            label4.Name = "label4";
            label4.Size = new Size(13, 15);
            label4.TabIndex = 5;
            label4.Text = "0";
            // 
            // button1
            // 
            button1.Location = new Point(12, 114);
            button1.Name = "button1";
            button1.Size = new Size(447, 23);
            button1.TabIndex = 6;
            button1.Text = "Vraag toevoegen";
            button1.UseVisualStyleBackColor = true;
            button1.Click += Button1_Click;
            // 
            // button3
            // 
            button3.Location = new Point(12, 143);
            button3.Name = "button3";
            button3.Size = new Size(447, 23);
            button3.TabIndex = 8;
            button3.Text = "Terug";
            button3.UseVisualStyleBackColor = true;
            button3.Click += Button3_Click;
            // 
            // comboBox3
            // 
            comboBox3.FormattingEnabled = true;
            comboBox3.Location = new Point(128, 70);
            comboBox3.Name = "comboBox3";
            comboBox3.Size = new Size(331, 23);
            comboBox3.TabIndex = 9;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 73);
            label5.Name = "label5";
            label5.Size = new Size(37, 15);
            label5.TabIndex = 10;
            label5.Text = "Vraag";
            // 
            // Form14
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(468, 179);
            Controls.Add(label5);
            Controls.Add(comboBox3);
            Controls.Add(button3);
            Controls.Add(button1);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(comboBox2);
            Controls.Add(comboBox1);
            Controls.Add(label1);
            Name = "Form14";
            Text = "Form14";
            FormClosing += CloseApplication;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private ComboBox comboBox1;
        private ComboBox comboBox2;
        private Label label2;
        private Label label3;
        private Label label4;
        private Button button1;
        private Button button3;
        private ComboBox comboBox3;
        private Label label5;
    }
}