using RealChess.Model.ChessPieces;
using System;
using System.Linq;
using static RealChess.Model.ChessPieces.ChessPiece;
using static RealChess.Model.AI.Evaluation.EvaluationConstants;
using static RealChess.Model.Bitboard.BoardLogic;
using RealChess.Model.Bitboard;

namespace RealChess.Model.AI.Evaluation
{
    /// <summary>
    /// The Evaluation class contains methods for evaluating the current state of a chess game.
    /// These methods are used by the engine to generate scores for different aspects of the game
    /// such as material, mobility, pawn structure, and king safety.
    /// </summary>
    internal static class MajorEvaluations
    {
        private static Board _gameBoard;

        /// <summary>
        /// Sets the current chess board used for evaluation.
        /// </summary>
        /// <param name="board">The Board object to set as the current chess board.</param>
        public static void SetBoard(Board board)
        {
            _gameBoard = board;
        }

        /// <summary>
        /// Evaluates the control of squares on the board by a specific player, taking into account the game phase.
        /// </summary>
        /// <param name="color">The color of the player whose control is being evaluated.</param>
        /// <param name="phase">The current game phase.</param>
        /// <returns>The evaluation score as an integer.</returns>
        public static int EvaluatePlayerControl(PieceColor color)
        {
            var pieces = GetPieces(color);

            int countControl = 0;

            // Loop through each piece on the board
            foreach (var piece in pieces)
            {
                // Check if the piece is not a king
                // since king shouldn't be active (in the middlegame) 
                if (!(piece.Type == PieceType.KING))
                    countControl += SubEvaluations.EvaluatePieceMobility(piece);
            }
            return countControl;
        }

        /// <summary>
        /// Evaluates the material strength of a specific colored player by counting the number and value of their pieces.
        /// </summary>
        /// <param name="color">The color of the player whose material is being evaluated.</param>
        /// <returns>The evaluation score as an integer.</returns>
        public static int EvaluatePlayerMaterial(PieceColor color)
        {
            int MaterialEvaluation = 0;

            // count the material (value) of the player's pieces
            MaterialEvaluation += SubEvaluations.CountMaterial(color) * 100;

            // if the player has two bishops, add a bonus
            if (SubEvaluations.CountBishop(color) >= 2)
                MaterialEvaluation += BishopPairBuff;

            var pieces = (color == PieceColor.WHITE ? _gameBoard.WhitePlayer :
                _gameBoard.BlackPlayer).Pieces;

            // if the player's rooks are connected, add a bonus
            if (AreRooksConnected(pieces, color, _gameBoard.BitBoard))
                MaterialEvaluation += RooksConnectedBuff;

            return MaterialEvaluation;
        }

        /// <summary>
        /// Evaluates the safety of all pieces of a player.
        /// </summary>
        /// <param name="color">The color of the player whose pieces are being evaluated.</param>
        /// <returns>The evaluation score as an integer.</returns>
        public static int EvaluatePlayerSafety(PieceColor color)
        {
            int safety = 0;

            var pieces = color == PieceColor.WHITE ? _gameBoard.GetPlayer1().Pieces :
                _gameBoard.GetPlayer2().Pieces;

            // Evaluates safety of every piece
            foreach (var piece in pieces.Values)
            {
                // Evaluates safety of every non-king piece
                if (piece.Type != PieceType.KING)
                {
                    safety += BoardLogic.EvaluatePieceSafety(piece);
                }
            }
            return safety;
        }


        /// <summary>
        /// Evaluates the danger of all pieces of a player.
        /// </summary>
        /// <param name="color">The color of the player whose danger is being evaluated.</param>
        /// <returns>The evaluation score as an integer.</returns>
        public static int EvaluatePlayerDanger(PieceColor color)
        {
            int safety = 0;

            var pieces = color == PieceColor.WHITE ? _gameBoard.GetPlayer1().Pieces :
                _gameBoard.GetPlayer2().Pieces;

            // Evaluates danger of every piece
            foreach (var piece in pieces.Values)
            {
                if (piece.Type != PieceType.KING)
                {
                    safety += Math.Min(BoardLogic.EvaluatePieceSafety(piece), 0);
                }
            }
            return safety;

        }

        /// <summary>
        /// Evaluates the activity of the king of a given color.
        /// </summary>
        /// <param name="color">The color of the king being evaluated.</param>
        /// <returns>The evaluation score as a double.</returns>
        public static double EvaluateKingActivity(PieceColor color)
        {
            double activity = 0;

            King king = _gameBoard.GetKing(color);

            // Evaluate king mobility and position during endgame
            activity += SubEvaluations.EvaluatePieceMobility(king);
            activity += SubEvaluations.EvaluatePosition(king, GamePhase.Endgame);

            // Evaluate forcing the king to the corner during endgame
            activity += SubEvaluations.ForceKingToCornerEndgameEval(color);

            // Evaluate distance of the king from backward pawns
            var pawns = BoardOperations.GetAllPawns(_gameBoard, BoardOperations.GetOppositeColor(color));
            activity += SubEvaluations.DistanceFromBackwardPawnsEval(pawns, color);

            return activity;

        }

        /// <summary>
        /// Evaluates the safety of the king of a player.
        /// </summary>
        /// <param name="color">The color of the player whose king's safety is being evaluated.</param>
        /// <returns>The evaluation score as an integer.</returns>
        public static int EvaluatePlayerKingSafety(PieceColor color)
        {
            // Evaluate the safety of the king based on the pieces in the perimeter
            ulong kingPerimeter = GetKingPerimeter(color);
            int kingSafety = SubEvaluations.EvaluateKingPerimeter(color, kingPerimeter);

            ulong safeSides = KingSidePawns | QueenSidePawns;

            ulong kingFront = _gameBoard.GetKing(color).GetPosition();

            if (color == PieceColor.WHITE)
                kingFront |= kingFront >> 8;
            else
                kingFront |= kingFront << 8;

            // If the king is protected by pawns, increase the king's safety score
            if ((kingFront & safeSides) > 0)
                kingSafety += SubEvaluations.PawnShield(color, kingPerimeter);

            return kingSafety;

        }


        /// <summary>
        /// Evaluates the development a player has with their pieces.
        /// The evaluation is based on the position of the pieces on the board,
        /// and whether the minor pieces have been developed or not.
        /// </summary>
        /// <param name="color">The color of the player to evaluate</param>
        /// <returns>The evaluation score as an integer</returns>
        public static int EvaluatePlayerDevelopment(PieceColor color)
        {
            var pieces = color == PieceColor.WHITE ? _gameBoard.GetPlayer1().Pieces :
                _gameBoard.GetPlayer2().Pieces;

            int development = 0;
            GamePhase currentPhase = _gameBoard.CurrentPhase;
            int undevelopedMinorPieces = 0;

            // Evaluate each piece's position and check for undeveloped minor pieces
            foreach (var piece in pieces.Values)
            {
                // If the piece is a minor piece (bishop or knight) and has not moved, add to undeveloped minor pieces count
                if (PreprocessedTables.MinorPieces.Contains(piece.Type) && !piece.HasMoved)
                    undevelopedMinorPieces += 5;

                // Evaluate the position of the piece based on the current game phase
                development += SubEvaluations.EvaluatePosition(piece, currentPhase);
            }

            development -= undevelopedMinorPieces;

            return development;
        }

        /// <summary>
        /// Evaluates the pawn structure of a player
        /// </summary>
        /// <param name="color">Player color</param>
        /// <param name="phase">Current game phase</param>
        /// <returns>The evaluation as a number</returns>
        public static double EvaluatePlayerPawnStructure(PieceColor color, GamePhase phase)
        {
            double structure = 0;

            var pawns = BoardOperations.GetAllPawns(_gameBoard, color);
            var enemyPawns = BoardOperations.GetAllPawns(_gameBoard, BoardOperations.GetOppositeColor(color));

            ulong pawnBoard = BoardOperations.GetPiecesPositions(pawns.Cast<ChessPiece>().ToList());
            ulong enemyPawnBoard = BoardOperations.GetPiecesPositions(enemyPawns.Cast<ChessPiece>().ToList());

            // Increase weight in endgame phase
            double endgameWeight = phase == GamePhase.Endgame ? 2 : 0.2;

            // Evaluate pawn chain
            structure += SubEvaluations.EvaluatePawnChain(pawns, endgameWeight);
            
            // Evaluate passed pawns
            structure += SubEvaluations.EvaluatePassedPawns(pawns, pawnBoard, enemyPawnBoard) * endgameWeight;

            // Evaluate pawn advancement
            structure += SubEvaluations.EvaluatePawnsAdvancement(pawns) * endgameWeight;

            return structure;
        }
    }
}
