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
        internal static bool IsLegalMove(ChessPieceControl pieceSource, Point targetLocation)
        {
            return true;
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
