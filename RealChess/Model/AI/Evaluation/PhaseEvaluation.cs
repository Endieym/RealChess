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
    internal static class PhaseEvaluation
    {
        private const int PawnPhase = 0;
        private const int KnightPhase = 1;
        private const int BishopPhase = 1;
        private const int RookPhase = 2;
        private const int QueenPhase = 4;

        public static double GetPhaseWeight()
        {
            double TotalPhase = KnightPhase * 4 + BishopPhase * 4 + RookPhase * 4 + QueenPhase * 2;
            double phase = TotalPhase;

            phase -= CountPieces();

            phase = (phase * 256 + (TotalPhase / 2)) / TotalPhase;

            return phase;
        }

        public static int CountPieces()
        {
            int pieces = 0;
            var piecesWhite = BoardLogic.GetPieces(PieceColor.WHITE);
            var piecesBlack = BoardLogic.GetPieces(PieceColor.BLACK);

            Dictionary<PieceType, int> pieceCounts = new Dictionary<PieceType, int>();

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

            foreach(var piece in piecesBlack)
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

            int numBishops = pieceCounts.ContainsKey(PieceType.BISHOP) ? pieceCounts[PieceType.BISHOP] : 0;
            int numRooks = pieceCounts.ContainsKey(PieceType.ROOK) ? pieceCounts[PieceType.ROOK] : 0;
            int numKnights = pieceCounts.ContainsKey(PieceType.KNIGHT) ? pieceCounts[PieceType.KNIGHT] : 0;
            int numQueens = pieceCounts.ContainsKey(PieceType.QUEEN) ? pieceCounts[PieceType.QUEEN] : 0;

            numBishops *= BishopPhase;
            numRooks *= RookPhase;
            numKnights *= KnightPhase;
            numQueens *= QueenPhase;

            pieces += numBishops + numRooks + numKnights + numQueens;
            return pieces;
        }
    }
}
