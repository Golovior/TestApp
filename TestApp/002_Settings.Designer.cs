namespace TestApp
{
    partial class SettingsForm
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
            button3 = new Button();
            button4 = new Button();
            button2 = new Button();
            button5 = new Button();
            fontDialog1 = new FontDialog();
            button6 = new Button();
            button7 = new Button();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(12, 12);
            button1.Name = "button1";
            button1.Size = new Size(153, 23);
            button1.TabIndex = 0;
            button1.Text = "Spel aanmaken";
            button1.UseVisualStyleBackColor = true;
            button1.Click += Button1_Click;
            // 
            // button3
            // 
            button3.Location = new Point(12, 157);
            button3.Name = "button3";
            button3.Size = new Size(153, 23);
            button3.TabIndex = 2;
            button3.Text = "Settings";
            button3.UseVisualStyleBackColor = true;
            button3.Click += Button3_Click;
            // 
            // button4
            // 
            button4.Location = new Point(12, 186);
            button4.Name = "button4";
            button4.Size = new Size(153, 23);
            button4.TabIndex = 3;
            button4.Text = "Terug";
            button4.UseVisualStyleBackColor = true;
            button4.Click += Button4_Click;
            // 
            // button2
            // 
            button2.Location = new Point(12, 99);
            button2.Name = "button2";
            button2.Size = new Size(153, 23);
            button2.TabIndex = 4;
            button2.Text = "Opdracht aanmaken";
            button2.UseVisualStyleBackColor = true;
            button2.Click += Button2_Click;
            // 
            // button5
            // 
            button5.Location = new Point(12, 128);
            button5.Name = "button5";
            button5.Size = new Size(153, 23);
            button5.TabIndex = 5;
            button5.Text = "Informatie syncen";
            button5.UseVisualStyleBackColor = true;
            button5.Click += Button5_Click;
            // 
            // button6
            // 
            button6.Location = new Point(12, 41);
            button6.Name = "button6";
            button6.Size = new Size(153, 23);
            button6.TabIndex = 6;
            button6.Text = "Spelers toevoegen";
            button6.UseVisualStyleBackColor = true;
            button6.Click += Button6_Click;
            // 
            // button7
            // 
            button7.Location = new Point(12, 70);
            button7.Name = "button7";
            button7.Size = new Size(153, 23);
            button7.TabIndex = 7;
            button7.Text = "Spelers status";
            button7.UseVisualStyleBackColor = true;
            button7.Click += Button7_Click;
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(174, 221);
            Controls.Add(button7);
            Controls.Add(button6);
            Controls.Add(button5);
            Controls.Add(button2);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button1);
            Name = "SettingsForm";
            Text = "Form6";
            FormClosing += CloseApplication;
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
        private Button button3;
        private Button button4;
        private Button button2;
        private Button button5;
        private FontDialog fontDialog1;
        private Button button6;
        private Button button7;
    }
}