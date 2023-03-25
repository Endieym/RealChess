using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RealChess
{
    public partial class MainPage : Form
    {
        public MainPage()
        {

            InitializeComponent();


        } 

        private void button_play_Click(object sender, EventArgs e)
        {
            ChessForm frms2 = new ChessForm();
            frms2.Show();
            this.Hide();

        }

        private void MainPage_Load(object sender, EventArgs e)
        {

        }
    }
}
