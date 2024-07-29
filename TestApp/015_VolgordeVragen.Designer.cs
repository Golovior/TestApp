namespace TestApp
{
    partial class Form16
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
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 70);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(579, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Terug";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Test";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(45, 12);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(546, 23);
            this.comboBox1.TabIndex = 2;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.ChangeTest);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Location = new System.Drawing.Point(12, 96);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(579, 443);
            this.panel1.TabIndex = 3;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 41);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(579, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Opslaan";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // Form16
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(603, 551);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Name = "Form16";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CloseApplication);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void updateElements(Tests test)
        {
            this.buttonsUp.Clear();
            this.buttonsDown.Clear();
            this.labels.Clear();

            List<Questions> questions = test.GetQuestions();
            this.panel1.Controls.Clear();

            if (questions == null)
                return;

            int id = 1;

            foreach (Questions q in questions)
            {
                if (q.GetQuestion() == "")
                    continue;

                this.makeQuestionRow(id, q);
                id++;
            }
        }

        private void makeQuestionRow(int id, Questions q)
        {
            Button currentButtonUp = new System.Windows.Forms.Button();
            Button currentButtonDown = new System.Windows.Forms.Button();
            Button currentButtonRemove = new System.Windows.Forms.Button();
            Label currentLabel = new System.Windows.Forms.Label();

            int height = 27 * id;
            // 
            // button1
            // 
            currentButtonUp.Location = new System.Drawing.Point(440, height);
            currentButtonUp.Name = "Up~" + Convert.ToString(id);
            currentButtonUp.Size = new System.Drawing.Size(31, 23);
            currentButtonUp.TabIndex = 0;
            currentButtonUp.Text = "^";
            currentButtonUp.UseVisualStyleBackColor = true;
            currentButtonUp.Click += new System.EventHandler(this.ButtonUp_click);
            // 
            // button2
            // 
            currentButtonDown.Location = new System.Drawing.Point(475, height);
            currentButtonDown.Name = "Down~" + Convert.ToString(id);
            currentButtonDown.Size = new System.Drawing.Size(29, 23);
            currentButtonDown.TabIndex = 1;
            currentButtonDown.Text = "v";
            currentButtonDown.UseVisualStyleBackColor = true;
            currentButtonDown.Click += new System.EventHandler(this.ButtonDown_click);
            // 
            // button2
            // 
            currentButtonRemove.Location = new System.Drawing.Point(510, height);
            currentButtonRemove.Name = "Remove~" + Convert.ToString(id);
            currentButtonRemove.Size = new System.Drawing.Size(29, 23);
            currentButtonRemove.TabIndex = 1;
            currentButtonRemove.Text = "X";
            currentButtonRemove.UseVisualStyleBackColor = true;
            currentButtonRemove.Click += new System.EventHandler(this.ButtonRemove_click);
            // 
            // label1
            // 
            currentLabel.Location = new System.Drawing.Point(12, height + 4);
            currentLabel.Name = "label~" + Convert.ToString(id);
            currentLabel.Size = new System.Drawing.Size(438, 18);
            currentLabel.TabIndex = 2;
            currentLabel.Text = q.GetQuestion(); 

            this.panel1.Controls.Add(currentButtonUp);
            this.panel1.Controls.Add(currentButtonDown);
            this.panel1.Controls.Add(currentButtonRemove);
            this.panel1.Controls.Add(currentLabel);

            this.buttonsUp.Add(currentButtonUp);
            this.buttonsDown.Add(currentButtonDown);
            this.labels.Add(currentLabel);
        }

        #endregion

        private List<Button> buttonsUp = new List<Button>();
        private List<Button> buttonsDown = new List<Button>();
        private List<Button> buttonsRemove = new List<Button>();
        private List<Label> labels = new List<Label>();
        private Button button1;
        private Label label1;
        private ComboBox comboBox1;
        private Panel panel1;
        private Button button2;
    }
}