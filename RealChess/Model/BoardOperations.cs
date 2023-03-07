using RealChess.Model.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealChess.Model.ChessPieces.ChessPiece;

namespace RealChess.Model
{
    internal static class BoardOperations
    {

        // Returns the bitmask of the possible en passant.
        // if en passant is not available, returns 0
        public static ulong GenerateEnPassant(Pawn pawn, Move lastMove)
        {
            if (lastMove.PieceMoved.Color == pawn.Color)
                return 0;

            // Checks if the last move was a double pawn move 
            if (lastMove.PieceMoved.Type == PieceType.PAWN &&
                Math.Abs(lastMove.EndSquare - lastMove.StartSquare) == 16)
            {
                // Returns the bitmask for the en passant
                return ((Pawn)lastMove.PieceMoved).GoBack() & pawn.GetCaptures();
            }
            return 0;
        }
    }
}
