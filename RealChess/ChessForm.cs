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
    public partial class ChessForm : Form
    {
        // class member array of Panels to track chessboard tiles
        private Panel[,] _chessBoardPanels;

        public ChessForm()
        {
            InitializeComponent();
            this.Load += new EventHandler(ChessForm_Load);

        }

        private void home_Click(object sender, EventArgs e)
        {
            MainPage home = new MainPage();
            home.Show();
            this.Hide();
        }
        

        // event handler of Form Load... init things here
        private void ChessForm_Load(object sender, EventArgs e)
        {
            const int tileSize = 65;
            const int gridSize = 8;
            var clr1 = Color.DarkGray;
            var clr2 = Color.White;

            // initialize the "chess board"
            _chessBoardPanels = new Panel[gridSize, gridSize];

            // double for loop to handle all rows and columns
            for (var n = 0; n < gridSize; n++)
            {
                for (var m = 0; m < gridSize; m++)
                {
                    // create new Panel control which will be one 
                    // chess board tile
                    var newPanel = new Panel
                    {
                        Size = new Size(tileSize, tileSize),
                        Location = new Point(tileSize * n +10, tileSize * m + 30)
                    };

                    // add to Form's Controls so that they show up
                    Controls.Add(newPanel);

                    // add to our 2d array of panels for future use
                    _chessBoardPanels[n, m] = newPanel;

                    // color the backgrounds
                    if (n % 2 == 0)
                        newPanel.BackColor = m % 2 != 0 ? clr1 : clr2;
                    else
                        newPanel.BackColor = m % 2 != 0 ? clr2 : clr1;
                }
            }
        }
    }

}
