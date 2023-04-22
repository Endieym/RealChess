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
                    countControl += SubEvaluations.EvaluatePieceMobility(piece);
            }

            return countControl;
        }

        public static int EvaluatePlayerDanger(PieceColor color)
        {
            var safety = EvaluatePlayerSafety(color);

            return Math.Min(safety, 0);

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

            var pieces = (color == PieceColor.WHITE ? _gameBoard.WhitePlayer :
                _gameBoard.BlackPlayer).Pieces;

            if (AreRooksConnected(pieces, color, _gameBoard.BitBoard))
                MaterialEvaluation += RooksConnectedBuff;

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
                if (piece.Type != PieceType.KING)
                {
                    safety += BoardLogic.EvaluatePieceSafety(piece);
                }
            }
            return safety;

        }

        public static int EvaluateKingActivity(PieceColor color)
        {
            int activity = 0;

            King king = _gameBoard.GetKing(color);

            activity += SubEvaluations.EvaluatePieceMobility(king);

            activity += SubEvaluations.EvaluatePosition(king, GamePhase.Endgame);

            activity += SubEvaluations.ForceKingToCornerEndgameEval(color);
            
            return activity;

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
                kingFront |= kingFront >> 8;
            else
                kingFront |= kingFront << 8;
            
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

                development += SubEvaluations.EvaluatePosition(piece, currentPhase);                               
            }
            return development;
        }

        public static int EvaluatePlayerPawnStructure(PieceColor color)
        {
            int structure = 0;

            var pawns = BoardOperations.GetAllPawns(_gameBoard, color);
            var enemyPawns = BoardOperations.GetAllPawns(_gameBoard, BoardOperations.GetOppositeColor(color));
            
            ulong pawnBoard = BoardOperations.GetPiecesPositions(pawns.Cast<ChessPiece>().ToList());
            ulong enemyPawnBoard = BoardOperations.GetPiecesPositions(enemyPawns.Cast<ChessPiece>().ToList());


            int defendedPawnsCount = SubEvaluations.EvaluatePawnChain(pawns);
            
            structure += defendedPawnsCount * pawnChainBuff;

            int backwardPawnsCount = pawns.Count - defendedPawnsCount;

            structure -= backwardPawnsCount * backwardPawnPenalty;



            return structure;
        }
    }
}
