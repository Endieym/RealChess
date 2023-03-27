using RealChess.View.Forms;
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

        // Click listener for when a play button is clicked
        private void button_play_Click(object sender, EventArgs e)
        {
            ChessForm frms2 = new ChessForm();
            ChooseGame chooseForm = new ChooseGame
            {
                StartPosition = FormStartPosition.CenterParent
            };
            
            // Opens the dialog to choose which gamemode
            // 1. Normal Chess
            // or
            // 2. Real chess
            chooseForm.ShowDialog();

            if (chooseForm.Clicked)
            {

                if (chooseForm.ChoseReal)
                {
                    frms2.IsReal = true;
                    frms2.Show();
                    this.Hide();

                }
                else
                {
                    frms2.IsReal = false;
                    frms2.Show();
                    this.Hide();
                }
                
            }

        }

        private void MainPage_Load(object sender, EventArgs e)
        {

        }
    }
}
