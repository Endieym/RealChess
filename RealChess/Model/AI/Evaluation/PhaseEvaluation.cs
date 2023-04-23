using RealChess.Model.Bitboard;
using RealChess.Model.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealChess.Model.ChessPieces.ChessPiece;

namespace RealChess.Model.AI.Evaluation
{
    /// <summary>
    /// The PhaseEvaluation class provides methods for evaluating the phase of a chess game.
    /// </summary>
    internal static class PhaseEvaluation
    {
        private const int KnightPhase = 1;
        private const int BishopPhase = 1;
        private const int RookPhase = 2;
        private const int QueenPhase = 4;

        /// <summary>
        /// Calculates the weight of the current phase of the game.
        /// Closer to 256 means more in endgame.
        /// </summary>
        /// <returns>The weight of the current phase of the game.</returns>
        public static double GetPhaseWeight()
        {
            double TotalPhase = KnightPhase * 4 + BishopPhase * 4 + RookPhase * 4 + QueenPhase * 2;
            double phase = TotalPhase;

            phase -= CountPieces();

            phase = (phase * 256 + (TotalPhase / 2)) / TotalPhase;

            return phase;
        }

        /// <summary>
        /// Counts the number of pieces on the board and calculates their weight based on their type.
        /// </summary>
        /// <returns>The total weight of the pieces on the board.</returns>
        public static int CountPieces()
        {
            int pieces = 0;

            // Get the pieces for both the white and black players.
            var piecesWhite = BoardLogic.GetPieces(PieceColor.WHITE);
            var piecesBlack = BoardLogic.GetPieces(PieceColor.BLACK);

            // Create a dictionary to store the count of each piece type.
            Dictionary<PieceType, int> pieceCounts = new Dictionary<PieceType, int>();

            // Loop through all of the white pieces and increment the count for each piece type.
            foreach (var piece in piecesWhite)
            {
                if (pieceCounts.ContainsKey(piece.Type))
                {
                    pieceCounts[piece.Type]++;
                }
                else
                {
                    pieceCounts[piece.Type] = 1;
                }
            }

            // Loop through all of the black pieces and increment the count for each piece type.
            foreach (var piece in piecesBlack)
            {
                if (pieceCounts.ContainsKey(piece.Type))
                {
                    pieceCounts[piece.Type]++;
                }
                else
                {
                    pieceCounts[piece.Type] = 1;
                }
            }

            // Calculate the number of pieces for each type and weight them accordingly.
            int numBishops = pieceCounts.ContainsKey(PieceType.BISHOP) ? pieceCounts[PieceType.BISHOP] : 0;
            int numRooks = pieceCounts.ContainsKey(PieceType.ROOK) ? pieceCounts[PieceType.ROOK] : 0;
            int numKnights = pieceCounts.ContainsKey(PieceType.KNIGHT) ? pieceCounts[PieceType.KNIGHT] : 0;
            int numQueens = pieceCounts.ContainsKey(PieceType.QUEEN) ? pieceCounts[PieceType.QUEEN] : 0;

            numBishops *= BishopPhase;
            numRooks *= RookPhase;
            numKnights *= KnightPhase;
            numQueens *= QueenPhase;

            // Add up the total weight of all the pieces and return it.
            pieces += numBishops + numRooks + numKnights + numQueens;
            return pieces;
        }
    }
}
