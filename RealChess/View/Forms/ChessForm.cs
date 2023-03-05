using RealChess.Model.ChessPieces;
using RealChess.View;
using static RealChess.Controller.BoardController;
using static RealChess.Controller.GameController;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RealChess.Model;
using RealChess.Controller;

namespace RealChess
{
    public partial class ChessForm : Form
    {
        // class member array of Panels to track chessboard tiles
        private Panel[,] _chessBoardPanels;

        // current chess piece clicked
        private static ChessPieceControl _currentPieceClicked = null;

        
        private const int tileSize = 65;
        private const int gridSize = 8;
        public ChessForm()
        {
            InitializeComponent();
            this.Load += new EventHandler(ChessForm_Load);

        }

        // Go back to main menu
        private void home_Click(object sender, EventArgs e)
        {

            MainPage home = new MainPage();
            home.Show();
            this.Hide();

        }
        

        // event handler of Form Load... init things here
        private void ChessForm_Load(object sender, EventArgs e)
        {
            
            var clr1 = Color.Green;
            var clr2 = Color.White;
            
            // initialize the chess board panels
            _chessBoardPanels = new Panel[gridSize, gridSize];

            // initialize the board data structure

            Board chessBoard = new Board();
            Dictionary<int, ChessPiece> dict1 = chessBoard.GetPlayer1().Pieces;
            Dictionary<int, ChessPiece> dict2 = chessBoard.GetPlayer2().Pieces;

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
                        if (row > 5)
                            pieceControl.SetPiece(dict1[row * gridSize + col]);
                        else
                            pieceControl.SetPiece(dict2[ row * gridSize + col]);

                        // Add an event listener to the UserControl
                        pieceControl.Controls["piecePic"].Click += new EventHandler(PieceControl_Click);

                        // Adds the ChessPieceControl to the panel
                        newPanel.Controls.Add(pieceControl);
                    }
                }
            }

            SetBoard(_chessBoardPanels, tileSize,gridSize);
            BoardController.SetBoard(chessBoard);
        }

        // Resets the current piece clicked to null
        public static void ResetPieceClicked()
        {
            // Resets the piece control's color to transparent
            if(_currentPieceClicked != null)
                _currentPieceClicked.BackColor = Color.Transparent;
            _currentPieceClicked = null;

        }

        

        // Event handler for when the piece control is clicked
        private void PieceControl_Click(object sender, EventArgs e)
        {
            MouseEventArgs mouseArgs = e as MouseEventArgs;

            if (mouseArgs.Button != MouseButtons.Left)
                return;

            // Get the clicked piece
            PictureBox piecePic = sender as PictureBox;

            ChessPieceControl myPiece = (ChessPieceControl)piecePic.Parent;


            // Check if a piece was already clicked, if not, set currentPiece to it
            if (_currentPieceClicked is null)
            {
                //MessageBox.Show("Source");
                _currentPieceClicked = myPiece;
                myPiece.BackColor = Color.Yellow;
                ShowLegalMoves(myPiece);

            }

            else
            {

                // If a piece is already selected, move the current piece to the clicked panel
                Panel targetPanel = (Panel)myPiece.Parent;

                
                // Trigger the Click event of the target panel
                Panel_Click(targetPanel, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
            }
        }

        //private void ClearColor()
        //{
        //    for(int row = 0; row < gridSize; row++) 
        //    {
        //        for(int col = 0; col < gridSize; col++)
        //        {
        //            foreach (ChessPieceControl c in _chessBoardPanels[row, col].Controls)
        //            {
        //                c.BackColor = Color.Transparent;
                        

        //            }
        //        }
        //    }
        //}
        
        // Event handler when a panel is clicked
        private void Panel_Click(object sender, EventArgs e)
        {   
            if(_currentPieceClicked != null)
                GameController.ClearLegalMoves(_currentPieceClicked);
            ResetPieceClicked();
            //MouseEventArgs mouseArgs = e as MouseEventArgs;

            //if (mouseArgs.Button != MouseButtons.Left)
            //    return;

            //if (_currentPieceClicked is null)
            //    return;

            
            ////MessageBox.Show("Target");

            //// Get the clicked panel
            //Panel myPanel = sender as Panel;           

            //if (!IsLegalMove(_currentPieceClicked, myPanel))
            //{
            //    ClearColor();
            //    _currentPieceClicked = null;
            //    return;
            //}
            //MovePiece(_currentPieceClicked, myPanel);

            //// Reset the selected ChessPieceControl
            //_currentPieceClicked = null;
            
        }



    }

}
