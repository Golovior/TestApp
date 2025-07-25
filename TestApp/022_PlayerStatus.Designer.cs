namespace TestApp
{
    partial class PlayerStatus
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
            playersList = new List<Label>();
            inactiveList = new List<RadioButton>();
            activeList = new List<RadioButton>();
            panels = new List<Panel>();
            panel1 = new Panel();
            button2 = new Button();
            button1 = new Button();
            label2 = new Label();
            label1 = new Label();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Location = new Point(12, 27);
            panel1.Name = "panel1";
            panel1.Size = new Size(491, 448);
            panel1.TabIndex = 9;
            // 
            // button2
            // 
            button2.Location = new Point(12, 510);
            button2.Name = "button2";
            button2.Size = new Size(491, 23);
            button2.TabIndex = 8;
            button2.Text = "Terug";
            button2.UseVisualStyleBackColor = true;
            button2.Click += Button2_Click;
            // 
            // button1
            // 
            button1.Location = new Point(12, 481);
            button1.Name = "button1";
            button1.Size = new Size(491, 23);
            button1.TabIndex = 7;
            button1.Text = "Opslaan";
            button1.UseVisualStyleBackColor = true;
            button1.Click += Button1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(343, 9);
            label2.Name = "label2";
            label2.Size = new Size(38, 15);
            label2.TabIndex = 10;
            label2.Text = "Actief";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(419, 9);
            label1.Name = "label1";
            label1.Size = new Size(63, 15);
            label1.TabIndex = 11;
            label1.Text = "Afgevallen";
            // 
            // PlayerStatus
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(519, 545);
            Controls.Add(label1);
            Controls.Add(label2);
            Controls.Add(panel1);
            Controls.Add(button2);
            Controls.Add(button1);
            Name = "PlayerStatus";
            Text = "PlayerStatus";
            ResumeLayout(false);
            PerformLayout();
        }

        private void MakePlayerRow(string player, int order, bool activePlayer = true)
        {
            Panel playerPanel = new();
            Label currentLabel = new System.Windows.Forms.Label();
            RadioButton active = new();
            RadioButton inactive = new();

            int height = 27 * order;

            //
            // playerPanel
            //
            playerPanel.Location = new System.Drawing.Point(20, height);
            playerPanel.Size = new System.Drawing.Size(450, 27);
            playerPanel.Name = "playerPanel~" + Convert.ToString(order);

            //
            // activeRadiobutton
            //
            active.Location = new System.Drawing.Point(323, 0);
            active.Size = new System.Drawing.Size(22, 22);
            active.TabIndex = 0;
            active.Name = "active~" + Convert.ToString(order);

            if(activePlayer)
                active.Checked = true;

            //
            // inactiveRadiobutton
            //
            inactive.Location = new System.Drawing.Point(407, 0);
            inactive.Size = new System.Drawing.Size(22, 22);
            inactive.TabIndex = 0;
            inactive.Name = "inactive~" + Convert.ToString(order);

            if(!activePlayer)
                inactive.Checked = true;

            // 
            // label1
            // 
            currentLabel.Location = new System.Drawing.Point(0, 4);
            currentLabel.Name = "player~" + order;
            currentLabel.Size = new System.Drawing.Size(250, 18);
            currentLabel.TabIndex = 2;
            currentLabel.Text = player;

            playersList.Add(currentLabel);
            activeList.Add(active);
            inactiveList.Add(inactive);

            playerPanel.Controls.Add(currentLabel);
            playerPanel.Controls.Add(active);
            playerPanel.Controls.Add(inactive);

            this.panels.Add(playerPanel);

            this.panel1.Controls.Add(playerPanel);
        }

        #endregion

        private List<Label> playersList;
        private List<RadioButton> activeList;
        private List<RadioButton> inactiveList;
        private List<Panel> panels;
        private Panel panel1;
        private Button button2;
        private Button button1;
        private Label label2;
        private Label label1;
    }
}