using System;
using System.Windows.Forms;

namespace RealChess.View.Forms
{
    public partial class ChooseGame : Form
    {
        public bool ChoseReal { get; set; }
        public bool Clicked { get; set; }
        public ChooseGame()
        {
            InitializeComponent();
            this.ControlBox = false;
            NormalLabel.Click += normalChess_Click;
            RealLabel.Click += realChess_Click;


        }

        // Event handler when normal chess picture was clicked
        private void normalChess_Click(object sender, EventArgs e)
        {
            ChoseReal = false;
            Clicked = true;
            this.Close();
        }

        // Event handler when real chess picture was clicked
        private void realChess_Click(object sender, EventArgs e)
        {
            ChoseReal = true;
            Clicked = true;
            this.Close();

        }

        // Event handler when close button was clicked
        private void CloseButton_Click(object sender, EventArgs e)
        {
            Clicked = false;
            this.Close();
        }
    }
}
