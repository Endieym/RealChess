using RealChess.Model.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealChess.Model.ChessPieces.ChessPiece;
using static RealChess.Model.AI.Evaluation.EvaluationConstants;
using static RealChess.Model.Bitboard.BoardLogic;
using RealChess.Model.Bitboard;

namespace RealChess.Model.AI.Evaluation
{
    internal static class MajorEvaluations
    {
        private static Board _gameBoard;

        public static void SetBoard(Board board)
        {
            _gameBoard = board;
        }

        /// <summary>
        /// Evaluates control of squares for a specific player
        /// </summary>
        /// <param name="color">Player color</param>
        /// <returns>The evaluation as a number</returns>
        public static int EvaluatePlayerControl(PieceColor color)
        {
            var pieces = color == PieceColor.WHITE ? _gameBoard.GetPlayer1().Pieces :
                _gameBoard.GetPlayer2().Pieces;

            int countControl = 0;
            foreach (var piece in pieces.Values)
            {
                if (!(piece.Type == PieceType.KING))// king shouldn't be active
                    countControl += SubEvaluations.EvaluatePieceMobility(piece, _gameBoard.BitBoard);
            }

            return countControl;
        }

        

        /// <summary>
        /// Counts pieces and their values of a specific coloured player
        /// </summary>
        /// <param name="color">player color</param>
        /// <returns>Count (value) of pieces</returns>
        public static int EvaluatePlayerMaterial(PieceColor color)
        {
            int MaterialEvaluation = 0;

            MaterialEvaluation += SubEvaluations.CountMaterial(color) * 100;

            if (SubEvaluations.CountBishopPair(color) >= 2)
                MaterialEvaluation += BishopPairBuff;

            return MaterialEvaluation;
        }

        /// <summary>
        /// Evaluates the safety of all pieces of a player
        /// </summary>
        /// <param name="color">Player color</param>
        /// <returns>The number evaluation</returns>
        public static int EvaluatePlayerSafety(PieceColor color)
        {
            int safety = 0;

            var pieces = color == PieceColor.WHITE ? _gameBoard.GetPlayer1().Pieces :
                _gameBoard.GetPlayer2().Pieces;

            // Evaluates safety of every piece
            foreach (var piece in pieces.Values)
            {
                if (piece.Type != PieceType.PAWN && piece.Type != PieceType.KING)
                {
                    safety += BoardLogic.EvaluatePieceSafety(piece);
                }
            }
            return safety;

        }

        /// <summary>
        /// Evaluates the safety of the king of a player
        /// </summary>
        /// <param name="color">Player color</param>
        /// <returns>The evaluation as a number</returns>
        public static int EvaluatePlayerKingSafety(PieceColor color)
        {
            ulong kingPerimeter = GetKingPerimeter(color);

            int kingSafety = SubEvaluations.EvaluateKingPerimeter(color, kingPerimeter);

            ulong safeSides = KingSidePawns | QueenSidePawns;

            ulong kingFront = _gameBoard.GetKing(color).GetPosition();

            if (color == PieceColor.WHITE)
                kingFront |= kingFront >> 7 | kingFront >> 8 | kingFront >> 9;
            else
                kingFront |= kingFront << 7 | kingFront << 8 | kingFront << 9;


            if ((kingFront & safeSides) > 0)
                kingSafety += SubEvaluations.PawnShield(color, kingPerimeter);

            return kingSafety;

        }




        /// <summary>
        /// Evaluates the development a player has with their pieces
        /// </summary>
        /// <param name="color">Player color</param>
        /// <returns>The evaluation as a number</returns>
        public static int EvaluatePlayerDevelopment(PieceColor color)
        {
            var pieces = color == PieceColor.WHITE ? _gameBoard.GetPlayer1().Pieces :
                _gameBoard.GetPlayer2().Pieces;

            int development = 0;
            GamePhase currentPhase = _gameBoard.CurrentPhase;

            foreach (var piece in pieces.Values)
            {
                if (!(currentPhase == GamePhase.Opening &&
                     piece.Type != PieceType.PAWN &&
                         piece.Type != PieceType.KNIGHT &&
                         piece.Type != PieceType.BISHOP
                         ))
                {
                    int index = (int)Math.Log(piece.GetPosition(), 2);

                    if (piece.Color == PieceColor.BLACK)
                        index = 63 - index;

                    development += PreprocessedTables.PieceSquareTable(piece.Type, currentPhase)[index];
                }

            }
            return development;
        }

    }
}
