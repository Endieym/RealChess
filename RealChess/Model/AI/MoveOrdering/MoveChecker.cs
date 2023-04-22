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
    /// <summary>
    /// Provides methods for checking various properties of a chess move, such as whether it is a good or bad move,
    /// whether it allows a draw, and whether it results in a piece being left hanging.
    /// and can guess the score of the move
    /// </summary>
    internal static class MoveChecker
    {
        /// <summary>
        /// Calculates a bonus for a given move based on various factors.
        /// </summary>
        /// <param name="move">The move to evaluate.</param>
        /// <returns>The bonus value for the move.</returns>
        public static double MoveBonus(Move move)
        {
            double buff = 0;

            // Adds a bonus for a good capture
            if (move.IsCapture)
            {
                if (IsGoodCapture(move))
                {
                    buff += move.CapturedPiece.Value * 100;
                }
            }

            // Adds a bonus for castling
            if(move.IsKingSideCastle || move.IsQueenSideCastle)
            {
                buff += EvaluationConstants.movePenalty;
            }

            // Adds a bonus for favorable trades
            if(IsTrade(move))
                buff += TradeBuff(move);
            
            return buff;
        }

        /// <summary>
        /// Calculates the penalty for a given move.
        /// </summary>
        /// <param name="move">The move to calculate the penalty for.</param>
        /// <returns>The penalty for the move.</returns>

        public static int MovePenalty(Move move)
        {
            int debuff = 0;

            debuff += FalseThreat(move);

            debuff += HangingPenalty(move);

            return debuff;
        }

        /// <summary>
        /// Calculates the penalty for a move that creates a false threat, i.e., a move that threatens an opponent's piece
        /// but does not actually pose a serious threat and instead allows the opponent to launch a counterattack.
        /// </summary>
        /// <param name="move">The move to evaluate.</param>
        /// <returns>The penalty for the move, in terms of the value of the pieces that are threatened.</returns>
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


        /// <summary>
        /// Returns the value of a threat to a chess piece.
        /// </summary>
        /// <param name="piece">The chess piece to evaluate.</param>
        /// <returns>The value of the threat.</returns>
        public static int GetThreatValue(ChessPiece piece)
        {
            var threat = 0;

           
            var safety = EvaluatePieceSafety(piece);

            if (safety < 0)
                  threat = -safety;
            

            return threat;
        }


        /// <summary>
        /// Calculates the penalty for a hanging piece,
        /// i.e. a piece that is not defended and is vulnerable to capture by an opponent's piece.
        /// </summary>
        /// <param name="move">The move to evaluate.</param>
        /// <returns>The penalty for a hanging piece.</returns>
        public static int HangingPenalty(Move move)
        {
            // Get the hanging piece for the current player
            var hangingPiece = HangingPiece(move.PieceMoved.Color);

            int debuff = 0;  

            if (hangingPiece == null)
                return 0;

            // Add the value of the hanging piece to the penalty
            debuff += hangingPiece.Value * 100;

            // Add the threatening value of the opponent's pieces that can capture the hanging piece to the penalty
            // Since they can move out of the threat, and even capture
            debuff += ThreateningValue(move, hangingPiece);
           
            // If the move made was a capture and the hanging piece's value is less than the captured
            // then reset the penalty.
            if (move.IsCapture && hangingPiece.Value <= move.CapturedPiece.Value)
                debuff = 0;

            // Checks if the hanging piece is the piece moved, if so 
            // Check if it made a positive capture or trade.
            else if (hangingPiece.Equals(move.PieceMoved))
            {
                if (move.IsPositiveCapture || IsTrade(move))
                    debuff = 0;
                else
                    debuff -= GetPotentialThreatValue(move);
            }

            // If the threat made by the recent move is bigger than the hanging piece, 
            // then don't penalize.
            else if (ThreateningValue(move, hangingPiece) > hangingPiece.Value)
                debuff = 0;
            
            return debuff;
        }

        /// <summary>
        /// Determines if the next move allows for a draw by checking if it leads to a threefold repetition.
        /// </summary>
        /// <param name="board">The board.</param>
        /// <param name="color">The color of the player whose turn it is.</param>
        /// <returns>True if the next move allows for a draw, false otherwise.</returns>
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

        /// <summary>
        /// Returns the highest valued hanging piece of the given color.
        /// </summary>
        /// <param name="color">The color of the pieces to check for hanging pieces.</param>
        /// <returns>The highest valued hanging piece of the given color, or null if there is none.</returns>
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

        /// <summary>
        /// Determines if a move results in a trade between pieces of equal value.
        /// </summary>
        /// <param name="move">The move to evaluate.</param>
        /// <returns>True if the move results in a trade of pieces of equal value, false otherwise.</returns>
        public static bool IsTrade(Move move)
        {
            if (move.IsCapture)
            {
                if (move.PieceMoved.Value == move.CapturedPiece.Value)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if a move is bad for the given game phase.
        /// </summary>
        /// <param name="move">The move to check.</param>
        /// <param name="phase">The game phase to check.</param>
        /// <returns>True if the move is bad for the given phase, false otherwise.</returns>
        public static bool IsBadForPhase(Move move, GamePhase phase) 
        { 
            if(phase == GamePhase.Opening)
            {
                if (BoardLogic.RevokesCastlingRights(move))
                    return true;
                
            }
            return false;
        }

        /// <summary>
        /// Gets the added morale after a specific move is made
        /// multiplied by its Success Rate
        /// </summary>
        /// <param name="move">Move made</param>
        /// <param name="successRate">Sucess rate of move</param>
        /// <returns>The added morale as a double</returns>
        public static double GetAddedMorale(Move move, double successRate)
        {
            double moraleBonus = 0;
            if (move.IsEnPassantCapture)
                moraleBonus += RealConstants.EnPassantMorale;
            else if (move.IsCapture)
                moraleBonus += RealConstants.SuccessfullCapture * 1.5;
            else if (move.IsCheck)
                moraleBonus += RealConstants.SuccessfullCapture;
           
            return moraleBonus * successRate;
        }
    }
}
