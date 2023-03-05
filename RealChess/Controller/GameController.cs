using RealChess.View;
using RealChess.View.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RealChess.Controller
{
    internal static class GameController
    {
        // Current chess piece clicked
        private static ChessPieceControl _currentPieceClicked = null;

        // The panel board
        private static Panel[,] _panelBoard;

        private static int _tileSize;

        private static int _gridSize;

        // Sets the panel board which represents all the squares on the chessboard
        public static void SetBoard(Panel[,] panelBoard, int tileSize, int gridSize)
        {
            _gridSize = gridSize;
            _tileSize = tileSize;
            _panelBoard = panelBoard;
        }

        // Checks if a move is legal
        internal static bool IsLegalMove(ChessPieceControl pieceSource, Panel targetPanel)
        {
            foreach (ChessPieceControl c in targetPanel.Controls)
                if (c.Equals(pieceSource) || c.Piece.Color == pieceSource.Piece.Color)
                    return false;
            return true;
        }

        // Highlights the legal squares the current piece can traverse to
        internal static void ShowLegalMoves(ChessPieceControl pieceSource)
        {
            // Gets both the legal moves and captures of the clicked piece control
            List<int> movesList = BoardController.GetMovesList(pieceSource.Piece);
            List<int> capturesList = BoardController.GetCapturesList(pieceSource.Piece);
            
            // Shows the legal moves
            foreach(int move in movesList)
            {
                LegalMoveControl legalMoveControl = new LegalMoveControl();
                legalMoveControl.Transfer += LegalMoveControl_Move;
                _currentPieceClicked = pieceSource;
                _panelBoard[move / 8, move % 8].Controls.Add(legalMoveControl);
            }

            // Shows the legal captures 
            foreach (int capture in capturesList)
            {
                LegalMoveControl legalMoveControl = new LegalMoveControl();
                legalMoveControl.SetCapture();
                legalMoveControl.Transfer += LegalMoveControl_Move;
                _currentPieceClicked = pieceSource;
                
                _panelBoard[capture / 8, capture % 8].Controls.Add(legalMoveControl);
                legalMoveControl.BringToFront();
                legalMoveControl.BackColor = Color.Transparent;
            }
            
        }

        internal static void ClearLegalMoves(ChessPieceControl pieceSource)
        {
            // Gets both the legal moves and captures of the clicked piece control
            List<int> movesList = BoardController.GetMovesList(pieceSource.Piece);
            List<int> capturesList = BoardController.GetCapturesList(pieceSource.Piece);
            
            // Hides the legal moves
            foreach (int move in movesList)
            {
                Panel currentPanel = _panelBoard[move / 8, move % 8];
                foreach (Control c in currentPanel.Controls)
                {
                    if (c is LegalMoveControl)
                        currentPanel.Controls.Remove(c);
                }
            }

            // Hides the legal captures
            foreach (int capture in capturesList)
            {
                Panel currentPanel = _panelBoard[capture / 8, capture % 8];
                foreach (Control c in currentPanel.Controls)
                {
                    if (c is LegalMoveControl)
                        currentPanel.Controls.Remove(c);
                }
            }

            

        }



        private static void LegalMoveControl_Move(object sender, TransferEventArgs e)
        {
            // Transfer the selected piece to the clicked panel
            MovePiece(_currentPieceClicked, e.TargetSquare);
            
        }

        internal static void MovePiece(ChessPieceControl pieceSource, Panel targetPanel)
        {

            bool isCapture = false;
            foreach (Control c in targetPanel.Controls)
            {
                if(c is ChessPieceControl)
                    isCapture = true;
            }

            // Clear controls of selected panel
            targetPanel.Controls.Clear();

            

            // Get pieces' current location on the board
            var oldRow = (pieceSource.Parent.Location.Y - 30) / _tileSize;
            var oldCol = (pieceSource.Parent.Location.X - 10) / _tileSize;
            int oldKey = oldRow * _gridSize + oldCol;  //tileSize * col + 10, tileSize * row + 30

            // Remove control from previous panel
            pieceSource.Parent.Controls.Remove(pieceSource);

            pieceSource.BackColor = Color.Transparent;
            // Move the selected ChessPieceControl to the target panel
            targetPanel.Controls.Add(pieceSource);

            // Remove the dots indicating which squares are legal to move to.
            ClearLegalMoves(pieceSource);

            // Finds the new location of the piece
            var row = (targetPanel.Location.Y - 30) / _tileSize;
            var col = (targetPanel.Location.X - 10) / _tileSize;
            int newKey = row *_gridSize +col;  //tileSize * col + 10, tileSize * row + 30

            
            // Updates the data structure
            BoardController.UpdateBoard(pieceSource.Piece, newKey, oldKey, isCapture);
            ChessForm.ResetPieceClicked();

        }

        
        
    }
}
