using RealChess.Model.AI.Evaluation;
using RealChess.Model.Bitboard;
using RealChess.Model.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using static RealChess.Model.Bitboard.BoardLogic;
using static RealChess.Model.ChessPieces.ChessPiece;

namespace RealChess.Model.AI
{
    internal static class MoveChecker
    {

        public static double MoveBonus(Move move)
        {
            double buff = 0;

            if (move.IsCapture)
            {
                if (IsGoodCapture(move))
                {
                    buff += move.CapturedPiece.Value * 100;
                }
            }

            if(move.IsKingSideCastle || move.IsQueenSideCastle)
            {
                buff += EvaluationConstants.movePenalty;
            }

            if(IsTrade(move))
                buff += TradeBuff(move);
            
            return buff;
        }

        public static int MovePenalty(Move move)
        {
            int debuff = 0;

            debuff += FalseThreat(move);

            debuff += HangingPenalty(move);

            return debuff;
        }

        public static int FalseThreat(Move move)
        {
            int antiThreatValue = 0;
            var threatenedPieces = ThreatenedPieces(move);

            if(EvaluatePieceSafety(move.PieceMoved) < 0)
            {
                foreach (var piece in threatenedPieces)
                {
                    antiThreatValue += GetThreatValue(piece);
                    
                }
            }

            return antiThreatValue;
        }

        public static int GetThreatValue(ChessPiece piece)
        {
            var threat = 0;

           
            var safety = EvaluatePieceSafety(piece);

            if (safety < 0)
                  threat = -safety;
            

            return threat;
        }

        public static int HangingPenalty(Move move)
        {
            var hangingPiece = HangingPiece(move.PieceMoved.Color);

            int debuff = 0;  

            if (hangingPiece == null)
                return 0;

            debuff += hangingPiece.Value * 100;
            
            debuff += ThreateningValue(move, hangingPiece);
           
            if (move.IsCapture && hangingPiece.Value <= move.CapturedPiece.Value)
                debuff = 0;

            else if (hangingPiece.Equals(move.PieceMoved))
            {
                if (move.IsPositiveCapture || IsTrade(move))
                    debuff = 0;
                else
                    debuff -= GetPotentialThreatValue(move);
            }

            else if (ThreateningValue(move, hangingPiece) > hangingPiece.Value)
                debuff = 0;
            
            return debuff;
        }

        public static bool NextMoveAllowsDraw(Board board, PieceColor color)
        {
            var enemyMoves = board.GetAllNonCaptureMoves(BoardOperations.GetOppositeColor(color));

            foreach(var move in enemyMoves)
            {

                if (IsThreefoldRepetition(BoardOperations.GetBoardStateAfterMove(move, board),
                    board.GetAllStates()))
                    return true;
            }

            return false;
        }
        
        public static ChessPiece HangingPiece(PieceColor color)
        {
            var hangingList = BoardLogic.GetHangingPieces(color);
            return hangingList.Max();

        }

        /// <summary>
        /// Calculates the value of the threat on the pieces threatened by recent move
        /// </summary>
        /// <param name="move">Move made</param>
        /// <param name="hangingPiece">Most valuable piece left hanging by move</param>
        /// <returns>The value of the threats as a whole number </returns>
        public static int ThreateningValue(Move move, ChessPiece hangingPiece)
        {
            int value = 0;
            var threatened = BoardLogic.ThreatenedPieces(move);

            foreach(var piece in threatened)
            {
                value += GetThreatValue(piece, hangingPiece);
            }

            return value *-1;
        }

        /// <summary>
        /// Method the gets the real threat value on a piece
        /// </summary>
        /// <param name="threatened">Threatened piece</param>
        /// <param name="hangingPiece">Piece left hanging by move</param>
        /// <returns>The value of the threat</returns>
        public static int GetThreatValue(ChessPiece threatened,ChessPiece hangingPiece)
        {
            int value = 0;
            
            var threatenedValue = EvaluatePieceSafety(threatened);
           
            // If the piece threatened is not safe
            if (threatenedValue < 0)
            {
                value += threatenedValue;

                // If the piece threatened can capture the hanging piece, then 
                // the threat value is zero.
                if (BoardLogic.IsThreateningPiece(threatened, hangingPiece))
                    value -= threatenedValue;
            }

            return value;
        }

        /// <summary>
        /// Calculates the potential threat of a move made
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        public static int GetPotentialThreatValue(Move move)
        {
            var threatened = BoardLogic.ThreatenedPieces(move);

            int value = 0;
            foreach(var pieceThreatened in threatened)
            {
                value += Math.Min(EvaluatePieceSafety(pieceThreatened),0);
            }

            return value;
        }

        /// <summary>
        /// Checks if a capture is a good one
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        public static bool IsGoodCapture(Move capture)
        {
            if (capture.IsPositiveCapture)
                return true;

            return false;
        }

        /// <summary>
        /// Calculates the buff a trade has. If the player is more forward in material, 
        /// they should prioritise trades.
        /// </summary>
        /// <param name="move">Trade move</param>
        /// <returns></returns>
        public static double TradeBuff(Move move)
        {
            double evaluation = BoardEvaluation.EvaluateMaterial();

            double tradeBuff = 0;

            evaluation *= move.PieceMoved.Color == PieceColor.WHITE ? 1 : -1;

            if(evaluation > 100)
                tradeBuff += EvaluationConstants.tradeBonus * (evaluation / 100);

            if(evaluation < 100)
                tradeBuff -= EvaluationConstants.tradeBonus * (evaluation / 100);

            return Math.Min(move.PieceMoved.Value * 100 - 20, tradeBuff);
                
        }

        public static bool IsTrade(Move move)
        {
            if (move.IsCapture)
            {
                if (move.PieceMoved.Value == move.CapturedPiece.Value)
                    return true;
            }

            return false;
        }

        public static bool IsBadForPhase(Move move, GamePhase phase) 
        { 
            if(phase == GamePhase.Opening)
            {
                if (BoardLogic.RevokesCastlingRights(move))
                    return true;
                

            }
            else if(phase == GamePhase.Middlegame)
            {

            }
            else
            {

            }

            return false;
        
        
        
        }
    }
}
