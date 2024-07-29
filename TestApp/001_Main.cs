namespace TestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Form2 f2 = new(this);

            this.Hide();
            f2.Show();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            Form6 form = new(this);

            this.Hide();
            form.Show();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Form11 form = new(this);

            this.Hide();
            form.Show();
        }

        private void CloseApplication(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Form18 form = new(this);

            this.Hide();
            form.Show();
        }
    }
}