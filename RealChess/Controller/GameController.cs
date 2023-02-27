using RealChess.View;
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

        // Checks if a move is legal
        internal static bool IsLegalMove(ChessPieceControl pieceSource, Panel targetPanel)
        {
            foreach (ChessPieceControl c in targetPanel.Controls)
                if (c.Equals(pieceSource) || c.Piece.Color == pieceSource.Piece.Color)
                    return false;
            return true;
        }

        internal static void ShowLegalMoves(ChessPieceControl pieceSource, Panel[,] panelBoard)
        {
            List<int> movesList = pieceSource.Piece.GetMovesList();
            foreach(int move in movesList)
            {
                panelBoard[move / 8, move % 8].BackColor = Color.Yellow;
            }
            
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
        }
    }
}
