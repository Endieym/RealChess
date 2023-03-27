using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private void normalChess_Click(object sender, EventArgs e)
        {
            ChoseReal = false;
            Clicked = true;
            this.Close();
        }

        private void realChess_Click(object sender, EventArgs e)
        {
            ChoseReal = true;
            Clicked = true;
            this.Close();

        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Clicked = false;
            this.Close();
        }
    }
}
