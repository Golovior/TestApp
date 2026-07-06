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
            labelSpel = new Label();
            comboBox1 = new ComboBox();
            comboBox2 = new ComboBox();
            button3 = new Button();
            SuspendLayout();
            //
            // labelSpel
            //
            labelSpel.AutoSize = true;
            labelSpel.Location = new Point(12, 9);
            labelSpel.Name = "labelSpel";
            labelSpel.Size = new Size(31, 15);
            labelSpel.TabIndex = 12;
            labelSpel.Text = "Spel";
            //
            // comboBox1
            //
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(54, 6);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(300, 23);
            comboBox1.TabIndex = 13;
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            //
            // comboBox2
            //
            comboBox2.FormattingEnabled = true;
            comboBox2.Location = new Point(12, 35);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(300, 23);
            comboBox2.TabIndex = 14;
            //
            // button3
            //
            button3.Location = new Point(318, 35);
            button3.Name = "button3";
            button3.Size = new Size(185, 23);
            button3.TabIndex = 15;
            button3.Text = "Toevoegen aan spel";
            button3.UseVisualStyleBackColor = true;
            button3.Click += Button3_Click;
            //
            // panel1
            //
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.Location = new Point(12, 82);
            panel1.Name = "panel1";
            panel1.Size = new Size(491, 393);
            panel1.TabIndex = 9;
            //
            // button2
            //
            button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
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
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
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
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new Point(853, 64);
            label2.Name = "label2";
            label2.Size = new Size(38, 15);
            label2.TabIndex = 10;
            label2.Text = "Actief";
            //
            // label1
            //
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(925, 64);
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
            Controls.Add(button3);
            Controls.Add(comboBox2);
            Controls.Add(comboBox1);
            Controls.Add(labelSpel);
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
            int playerPanelWidth = this.panel1.ClientSize.Width - 40;
            int activeX = playerPanelWidth - 127;
            int inactiveX = playerPanelWidth - 43;

            //
            // playerPanel
            //
            playerPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            playerPanel.Location = new System.Drawing.Point(20, height);
            playerPanel.Size = new System.Drawing.Size(playerPanelWidth, 27);
            playerPanel.Name = "playerPanel~" + Convert.ToString(order);

            //
            // activeRadiobutton
            //
            active.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            active.Location = new System.Drawing.Point(activeX, 0);
            active.Size = new System.Drawing.Size(22, 22);
            active.TabIndex = 0;
            active.Name = "active~" + Convert.ToString(order);

            if(activePlayer)
                active.Checked = true;

            //
            // inactiveRadiobutton
            //
            inactive.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            inactive.Location = new System.Drawing.Point(inactiveX, 0);
            inactive.Size = new System.Drawing.Size(22, 22);
            inactive.TabIndex = 0;
            inactive.Name = "inactive~" + Convert.ToString(order);

            if(!activePlayer)
                inactive.Checked = true;

            //
            // label1
            //
            currentLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            currentLabel.Location = new System.Drawing.Point(0, 4);
            currentLabel.Name = "player~" + order;
            currentLabel.Size = new System.Drawing.Size(activeX - 10, 18);
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
        private Label labelSpel;
        private ComboBox comboBox1;
        private ComboBox comboBox2;
        private Button button3;
    }
}
