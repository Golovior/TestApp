namespace TestApp
{
    partial class Form3
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
            textBox1 = new TextBox();
            button1 = new Button();
            button2 = new Button();
            comboBox1 = new ComboBox();
            label2 = new Label();
            checkBox1 = new CheckBox();
            textBox2 = new TextBox();
            label3 = new Label();
            checkBox2 = new CheckBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 38);
            label1.Name = "label1";
            label1.Size = new Size(40, 15);
            label1.TabIndex = 0;
            label1.Text = "Vraag:";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(75, 35);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(442, 23);
            textBox1.TabIndex = 1;
            // 
            // button1
            // 
            button1.Location = new Point(12, 143);
            button1.Name = "button1";
            button1.Size = new Size(505, 23);
            button1.TabIndex = 2;
            button1.Text = "Toevoegen";
            button1.UseVisualStyleBackColor = true;
            button1.Click += Button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(12, 172);
            button2.Name = "button2";
            button2.Size = new Size(505, 23);
            button2.TabIndex = 3;
            button2.Text = "Terug";
            button2.UseVisualStyleBackColor = true;
            button2.Click += Button2_Click;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(75, 6);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(442, 23);
            comboBox1.TabIndex = 6;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 9);
            label2.Name = "label2";
            label2.Size = new Size(57, 15);
            label2.TabIndex = 5;
            label2.Text = "Opdracht";
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(75, 64);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(152, 19);
            checkBox1.TabIndex = 8;
            checkBox1.Text = "Antwoorden alfabetisch";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(75, 89);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(442, 23);
            textBox2.TabIndex = 10;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 92);
            label3.Name = "label3";
            label3.Size = new Size(63, 15);
            label3.TabIndex = 9;
            label3.Text = "Antwoord:";
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Location = new Point(75, 118);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(103, 19);
            checkBox2.TabIndex = 12;
            checkBox2.Text = "Juist antwoord";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // Form3
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(529, 202);
            Controls.Add(checkBox2);
            Controls.Add(textBox2);
            Controls.Add(label3);
            Controls.Add(checkBox1);
            Controls.Add(comboBox1);
            Controls.Add(label2);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Name = "Form3";
            Text = "Vraag toevoegen";
            FormClosing += CloseApplication;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox textBox1;
        private Button button1;
        private Button button2;
        private ComboBox comboBox1;
        private Label label2;
        private CheckBox checkBox1;
        private TextBox textBox2;
        private Label label3;
        private CheckBox checkBox2;
    }
}