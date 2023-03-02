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

        // Sets the panel board which represents all the squares on the chessboard
        public static void SetBoard(Panel[,] panelBoard)
        {
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
            List<int> movesList = pieceSource.Piece.GetMovesList();
            foreach(int move in movesList)
            {
                LegalMoveControl legalMoveControl = new LegalMoveControl();
                legalMoveControl.Transfer += LegalMoveControl_Move;
                _currentPieceClicked = pieceSource;
                _panelBoard[move / 8, move % 8].Controls.Add(legalMoveControl);
            }
            
        }

        private static void LegalMoveControl_Move(object sender, TransferEventArgs e)
        {
            // Transfer the selected piece to the clicked panel
            MovePiece(_currentPieceClicked, e.TargetSquare);
            
        }

        internal static void MovePiece(ChessPieceControl pieceSource, Panel targetPanel)
        {
            // Clear controls of selected panel
            targetPanel.Controls.Clear();

            // Remove control from previous panel
            pieceSource.Parent.Controls.Remove(pieceSource);

            pieceSource.BackColor = Color.Transparent;
            // Move the selected ChessPieceControl to the target panel
            targetPanel.Controls.Add(pieceSource);

            // Remove the dots indicating which squares are legal to move to.
            ClearLegalMoves(pieceSource);

            ChessForm.ResetPieceClicked();

        }

        internal static void ClearLegalMoves(ChessPieceControl pieceSource)
        {
            List<int> movesList = pieceSource.Piece.GetMovesList();
            foreach (int move in movesList)
            {
                LegalMoveControl legalMoveControl = new LegalMoveControl();
                legalMoveControl.Transfer += LegalMoveControl_Move;
                _currentPieceClicked = pieceSource;

                Panel currentPanel = _panelBoard[move / 8, move % 8];
                foreach (Control c in currentPanel.Controls)
                {
                    if(c is LegalMoveControl)
                        currentPanel.Controls.Remove(c);
                }
            }
        }
    }
}
