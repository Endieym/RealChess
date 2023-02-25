﻿using RealChess.Model.ChessPieces;
using RealChess.View;
using static RealChess.Controller.BoardController;
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

        // current chess piece clicked
        private ChessPieceControl _currentPieceClicked = null;

        // current panel in the panel board clicked
        private Panel _currentPanelClicked = null;

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
            for (var row = 0; row < gridSize; row++)
            {
                for (var col = 0; col < gridSize; col++)
                {
                   
                    // create new Panel control which will be one 
                    // chess board tile
                    var newPanel = new Panel
                    {
                        Size = new Size(tileSize, tileSize),
                        Location = new Point(tileSize * col + 10, tileSize * row + 30)
                    };
                    // add to Form's Controls so that they show up
                    Controls.Add(newPanel);

                    // Add the click event handler to the panel
                    newPanel.Click += new EventHandler(Panel_Click);

                    // add to our 2d array of panels for future use
                    _chessBoardPanels[row, col] = newPanel;

                    // color the backgrounds
                    if (row % 2 == 0)
                        newPanel.BackColor = col % 2 != 0 ? clr1 : clr2;
                    else
                        newPanel.BackColor = col % 2 != 0 ? clr2 : clr1;

                    if (row < 2 || row > 5)
                    {
                        // Create a new ChessPieceControl
                        ChessPieceControl pieceControl = new ChessPieceControl();

                        // Set the piece on the control
                        ChessPiece piece = GetPieceForSquare(row, col);
                        pieceControl.SetPiece(piece);

                        // Add an event listener to the UserControl
                        pieceControl.Controls["piecePic"].Click += new EventHandler(PieceControl_Click);

                        // Adds the ChessPieceControl to the panel
                        newPanel.Controls.Add(pieceControl);
                    }
                }
            }
        }

        // Event handler for when the piece control is clicked
        private void PieceControl_Click(object sender, EventArgs e)
        {


            // Get the clicked piece
            PictureBox piecePic = sender as PictureBox;

            ChessPieceControl myPiece = (ChessPieceControl)piecePic.Parent;


            // Check if a piece was already clicked, if not, set currentPiece to it
            if (_currentPieceClicked is null)
            {
                //MessageBox.Show("Source");
                _currentPieceClicked = myPiece;
                myPiece.BackColor = Color.Yellow;


            }

            else
            {

                // If a piece is already selected, move the current piece to the clicked panel
                Panel targetPanel = (Panel)myPiece.Parent;

                // Trigger the Click event of the target panel
                Panel_Click(targetPanel, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
            }
        }

        // Event handler for a panel is clicked
        private void Panel_Click(object sender, EventArgs e)
        {

            if (_currentPieceClicked is null)
                return;

            
            //MessageBox.Show("Target");

            // Get the clicked panel
            Panel myPanel = sender as Panel;

            if (_currentPieceClicked.Parent.Equals(myPanel))
                return;

            if (!IsLegalMove(_currentPieceClicked, myPanel.Location))
            {
                return;
            }
            // Clear controls of selected panel
            myPanel.Controls.Clear();

            // Remove control from previous panel
            _currentPieceClicked.Parent.Controls.Remove(_currentPieceClicked);

            _currentPieceClicked.BackColor = Color.Transparent;
            // Move the selected ChessPieceControl to the target panel
            myPanel.Controls.Add(_currentPieceClicked);

            // Reset the selected ChessPieceControl
            _currentPieceClicked = null;
            
        }



    }

}
