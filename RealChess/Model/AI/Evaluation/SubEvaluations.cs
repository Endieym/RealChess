using RealChess.Model.Bitboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealChess.Model.ChessPieces.ChessPiece;
using static RealChess.Model.AI.Evaluation.EvaluationConstants;
using RealChess.Model.ChessPieces;
using static RealChess.Model.Bitboard.BoardLogic;

namespace RealChess.Model.AI.Evaluation
{
    /// <summary>
    /// Contains methods for evaluating specific aspects of the chess game state,
    /// such as pawn structure, king safety, and passed pawns.
    /// </summary>
    internal static class SubEvaluations
    {
        private static Board _gameBoard;
        private static Player whitePlayer;
        private static Player blackPlayer;

        /// <summary>
        /// Sets the game board and players for evaluation.
        /// </summary>
        /// <param name="board">The game board to evaluate.</param>
        public static void SetBoard(Board board)
        {
            _gameBoard = board;
            whitePlayer = board.GetPlayer1();
            blackPlayer = board.GetPlayer2();
        }

        /// <summary>
        /// Counts the total value of material on the board for a specific player.
        /// </summary>
        /// <param name="color">The color of the player whose material will be counted.</param>
        /// <returns>The total value of material on the board for the specified player.</returns>
        public static int CountMaterial(PieceColor color)
        {
            var pieces = color == PieceColor.WHITE ? whitePlayer.Pieces : blackPlayer.Pieces;

            int pieceCount = 0;
            // Counts value of pieces on a specific player
            foreach (var piece in pieces.Values)
            {
                if (piece.Type != PieceType.KING)
                    pieceCount += piece.Value;
            }

            return pieceCount;
        }

        /// <summary>
        /// Counts the number of bishops of the specified color and returns the number of bishops.
        /// </summary>
        /// <param name="color">The color of the bishops to count.</param>
        /// <returns>The number of bishop pairs on the board of the specified color.</returns>

        public static int CountBishop(PieceColor color)
        {
            var pieces = GetPieces(color);

            int bishopCount = 0;
            foreach (var piece in pieces)
            {
                if (piece.Type == PieceType.BISHOP)
                    bishopCount++;
            }
            return bishopCount;
        }

        /// <summary>
        /// Evaluates the safety of the king based on the pieces around it and the control of the squares in its perimeter.
        /// </summary>
        /// <param name="color">The color of the king to evaluate</param>
        /// <param name="kingPerimeter">The bitboard representing the perimeter around the king</param>
        /// <returns>The king's safety score</returns>
        public static int EvaluateKingPerimeter(PieceColor color, ulong kingPerimeter)
        {
            var playerPieces = GetPieces(color);
            var enemyPieces = GetPieces(BoardOperations.GetOppositeColor(color));

            int kingSafety = 0;


            // Evaluates the safety of friendly pieces surrounding the king
            foreach (var piece in playerPieces)
            {
                if ((piece.GetPosition() & kingPerimeter) > 0)
                {
                    kingSafety += BoardLogic.EvaluatePieceSafety(piece);
                    kingPerimeter ^= piece.GetPosition();
                }
            }

            // Evaluates the danger of enemy pieces around the king
            foreach (var piece in enemyPieces)
            {
                if ((piece.GetPosition() & kingPerimeter) > 0)
                {
                    kingSafety -= piece.Value;
                    kingPerimeter ^= piece.GetPosition();
                }
            }

            // Evaluates the control of the squares in the king's perimeter
            while (kingPerimeter > 0)
            {
                ulong position = kingPerimeter & ~(kingPerimeter - 1);
                kingSafety += BoardLogic.EvaluateSquareControl(color, position);
                kingPerimeter &= kingPerimeter - 1;
            }

            return kingSafety;
        }

        /// <summary>
        /// Evaluates the position of a chess piece based on its type, position and game phase.
        /// </summary>
        /// <param name="piece">The chess piece to evaluate.</param>
        /// <param name="phase">The current game phase.</param>
        /// <returns>An integer representing the value of the chess piece's position.</returns>
        public static int EvaluatePosition(ChessPiece piece, GamePhase phase)
        {
            if (phase == GamePhase.Opening && !(piece.Type == PieceType.PAWN ||
                PreprocessedTables.MinorPieces.Contains(piece.Type)))
                return 0;

            int index = (int)Math.Log(piece.GetPosition(), 2);

            if (piece.Color == PieceColor.BLACK)
                index = 63 - index;

            // Return the value of the chess piece's position based on its type and the game phase
            return PreprocessedTables.PieceSquareTable(piece.Type, phase)[index];

        }

        /// <summary>
        /// Evaluates the strength of the pawn shield in front of the king. A strong pawn shield can increase the safety of the king.
        /// </summary>
        /// <param name="color">Color of the king</param>
        /// <param name="kingPerimeter">The perimeter around the king</param>
        /// <returns>The strength of the pawn shield. A higher value indicates a stronger pawn shield</returns>
        public static int PawnShield(PieceColor color, ulong kingPerimeter)
        {
            int pawnShield = 1;

            // Gets the bitmask of the pawn shield squares
            ulong pawnPos = (kingPerimeter & KingSidePawns) > 0 ? KingSidePawns : QueenSidePawns;

            
            if (color == PieceColor.BLACK) pawnPos >>= 40;

            // Pawn shield pawns should move one square max
            pawnPos |= color == PieceColor.WHITE ? pawnPos >> 8 : pawnPos << 8;
            
            kingPerimeter |= color == PieceColor.WHITE? kingPerimeter >> 8 : kingPerimeter << 8;

            var pieces = GetPieces(color).ToList();
            pieces.Sort();

            int index = 0;

            // Multiplies each time a pawn exists in the pawn shield
            while (pieces.Count > index && pieces[index].Type == PieceType.PAWN)
            {
                var piecePos = pieces[index++].GetPosition();

                if ((piecePos & kingPerimeter) > 0
                    && (piecePos & pawnPos) > 0)
                    pawnShield *= pawnShieldBuff;
            }

            return pawnShield;
        }

        /// <summary>
        /// Evaluates the mobility of a chess piece.
        /// The more squares a piece can move to, the higher the mobility.
        /// </summary>
        /// <param name="piece">The chess piece to evaluate</param>
        /// <returns>The mobility value for the piece</returns>
        public static int EvaluatePieceMobility(ChessPiece piece)
        {
            ulong ocuppied = _gameBoard.BitBoard;
            // Gets the possible moves bitmask for the piece
            ulong attacks = piece.Type == PieceType.PAWN ? ((Pawn)piece).GetCaptures() :
                piece.GenerateLegalMoves(ocuppied);

            attacks |= piece.GetPosition();

            int mobility = 0;
            ulong centerControl = Center;

            centerControl &= attacks;

            // Center is more valuable square to control
            while (centerControl != 0)
            {
                centerControl &= centerControl - 1; // reset LS1B
                mobility += 2;
            }
            while (attacks != 0)
            {
                attacks &= attacks - 1; // reset LS1B
                mobility++;
            }
            return mobility;
        }

        /// <summary>
        /// Evaluates the pawn chain based on the given list of pawns and the endgame weight.
        /// </summary>
        /// <param name="pawns">The list of pawns to evaluate the chain from.</param>
        /// <param name="endgameWeight">The endgame weight used for the evaluation.</param>
        /// <returns>The evaluation of the pawn chain.</returns>
        public static double EvaluatePawnChain(List<Pawn> pawns, double endgameWeight)
        {
            double chainEval = 0;

            var chain = GetPawnChain(pawns);

            List<Pawn> defendedPawns = chain.defendedPawns;
            List<Pawn> backwardPawns = chain.backwardPawns;

            chainEval += defendedPawns.Count * pawnChainBuff * endgameWeight;

            chainEval -= backwardPawns.Count * backwardPawnPenalty * endgameWeight;

            return chainEval;
        }


        /// <summary>
        /// Returns a tuple of two lists of pawns: defended pawns and backward pawns.
        /// </summary>
        /// <param name="pawns">A list of Pawn objects.</param>
        /// <returns>A tuple containing two lists of Pawn objects:
        /// 1. A list contains pawns that are defended by other pieces,
        /// 2. A list contains pawns that are not defended and are considered backward pawns.</returns>
        public static (List<Pawn> defendedPawns, List<Pawn> backwardPawns) GetPawnChain(List<Pawn> pawns)
        {
            List<Pawn> defendedPawns = new List<Pawn>();
            List<Pawn> backwardPawns = new List<Pawn>();

            List<ChessPiece> pawnsPieces = new List<ChessPiece>(pawns.Cast<ChessPiece>());

            // Iterate over the list of pawns
            foreach (Pawn pawn in pawns)
            {
                // Check if the pawn is defended by other pieces on the board
                if (IsDefendedByOtherPieces(pawnsPieces, pawn))
                    defendedPawns.Add(pawn);
                else
                    backwardPawns.Add(pawn);
            }

            // Return a tuple containing the lists of defended and backward pawns
            return (defendedPawns, backwardPawns);
        }


        /// <summary>
        /// Evaluates the advancement of a given list of pawns.
        /// </summary>
        /// <param name="pawns">List of pawns to evaluate.</param>
        /// <returns>Advancement score for the given list of pawns.</returns>
        public static int EvaluatePawnsAdvancement(List<Pawn> pawns)
        {
            int advancement = 0;


            foreach (Pawn pawn in pawns)
            {
                advancement -= DistanceFromPromotion(pawn);
            }

            return advancement * 3;
        }

        /// <summary>
        /// Calculates the distance of a pawn from its promotion rank 
        /// (the last rank of the opposite color).
        /// </summary>
        /// <param name="pawn">The pawn to evaluate.</param>
        /// <returns>An integer representing the number of ranks the pawn is away from its promotion rank.</returns>
        public static int DistanceFromPromotion(Pawn pawn)
        {
            var promotionRank = pawn.Color == PieceColor.WHITE ? 7 : 0;

            var rank = BoardOperations.GetRank(pawn);

            return Math.Abs(promotionRank - rank);
        }

        /// <summary>
        /// Evaluates how well a pawn's promotion square is being defended.
        /// </summary>
        /// <param name="pawn">The pawn to evaluate.</param>
        /// <returns>The evaluation score.</returns>
        public static int PromotionSquareDefense(Pawn pawn)
        {
            var promotionSquare = pawn.Color == PieceColor.WHITE ? 0 : 56;
            promotionSquare += BoardOperations.GetFile(pawn);

            return CountSafety(pawn.Color, (ulong)1 << promotionSquare) * 2;
        }

        /// <summary>
        /// Check if a chess piece is defended by other pieces
        /// </summary>
        /// <param name="pieces">List of all chess pieces</param>
        /// <param name="piece">Chess piece to check if it's defended</param>
        /// <returns>True if the chess piece is defended, false otherwise</returns>
        public static bool IsDefendedByOtherPieces(List<ChessPiece> pieces, ChessPiece piece)
        {
            return GetInfluencers(pieces, piece.GetPosition()).Count > 0;
        }

        /// <summary>
        /// Determines if the given pawn is doubled, i.e., there is another pawn of the same color on the same file.
        /// </summary>
        /// <param name="pawn">The pawn to check.</param>
        /// <param name="pawnsMask">A bitboard representing the positions of all pawns on the board.</param>
        /// <returns>True if the pawn is doubled, false otherwise.</returns>
        public static bool IsDoubledPawn(Pawn pawn, ulong pawnsMask)
        {
            return BoardOperations.IsBlocked(pawn.Color, pawn.GetPosition(), pawnsMask);
        }

        /// <summary>
        /// Evaluates the passed pawns of a player on the board.
        /// </summary>
        /// <param name="playerPawns">List of pawns of the player.</param>
        /// <param name="pawnsMask">Bitboard representing the player's pawns positions.</param>
        /// <param name="enemyPawns">Bitboard representing the enemy's pawns positions.</param>
        /// <returns>The evaluation score of the passed pawns.</returns>
        public static int EvaluatePassedPawns(List<Pawn> playerPawns, ulong pawnsMask, ulong enemyPawns)
        {
            var passedPawns = GetPassedPawns(playerPawns, pawnsMask, enemyPawns);

            int passedPawnsEval = 0;

            passedPawnsEval += passedPawnBuff * passedPawns.Count;

            foreach (var pawn in passedPawns)
            {
                if (IsDefendedByOtherPieces(playerPawns.Cast<ChessPiece>().ToList(), pawn))
                    passedPawnsEval += passedPawnBuff;

                passedPawnsEval += PromotionSquareDefense(pawn);
            }

            return passedPawnsEval;
        }

        /// <summary>
        /// Returns a list of passed pawns for a player
        /// </summary>
        /// <param name="playerPawns">List of pawns for the player</param>
        /// <param name="pawnsMask">Bitboard representing all pawns on the board</param>
        /// <param name="enemyPawns">Bitboard representing all enemy pawns on the board</param>
        /// <returns>List of passed pawns for the player</returns>
        public static List<Pawn> GetPassedPawns(List<Pawn> playerPawns, ulong pawnsMask, ulong enemyPawns)
        {
            List<Pawn> passedPawns = new List<Pawn>();

            foreach (var pawn in playerPawns)
            {
                if (IsPassed(pawn, enemyPawns) && !IsDoubledPawn(pawn, pawnsMask))
                {
                    passedPawns.Add(pawn);
                }
            }
            return passedPawns;

        }

        /// <summary>
        /// Checks if a given pawn is passed, meaning there are no enemy pawns on the same or adjacent files that can block its advance.
        /// </summary>
        /// <param name="pawn">The pawn to be checked.</param>
        /// <param name="occupation">A bitboard representing the current state of the board.</param>
        /// <returns>True if the pawn is passed, false otherwise.</returns>
        public static bool IsPassed(Pawn pawn, ulong occupation)
        {
            return !BoardOperations.IsBlockedByPieces(pawn, occupation);
        }

        /// <summary>
        /// Evaluates the position of an enemy king during an endgame to force it to a corner.
        /// </summary>
        /// <param name="color">The color of the player's king to evaluate.</param>
        /// <returns>The evaluation score of the king's position.</returns>
        public static int ForceKingToCornerEndgameEval(PieceColor color)
        {
            int eval = 0;

            King playerKing = _gameBoard.GetKing(color);
            King enemyKing = _gameBoard.GetKing(BoardOperations.GetOppositeColor(color));

            // Position is better when the enemy king is closer to the corner
            int enemyKingRank = BoardOperations.GetRank(enemyKing);
            int enemyKingFile = BoardOperations.GetFile(enemyKing);

            int enemyKingDstFromCenterRank = Math.Max(3 - enemyKingRank, enemyKingRank - 4);
            int enemyKingDstFromCenterFile = Math.Max(3 - enemyKingFile, enemyKingFile - 4);

            eval += enemyKingDstFromCenterRank + enemyKingDstFromCenterFile;

            // Position will be better if we move the king closer to opponent king to help 
            // Checkmate

            eval += 14 - BoardOperations.GetDistancePieces(playerKing, enemyKing);

            return eval * 10;
        }

        /// <summary>
        /// Calculates the distance from the backward pawns of the player's pawn chain to the player's king.
        /// </summary>
        /// <param name="pawns">The list of pawns belonging to the player.</param>
        /// <param name="color">The color of the player's pieces.</param>
        /// <returns>The evaluation of the distance from the backward pawns of the player's pawn chain to the player's king.</returns>
        public static int DistanceFromBackwardPawnsEval(List<Pawn> pawns, PieceColor color)
        {
            King playerKing = _gameBoard.GetKing(color);

            var chain = GetPawnChain(pawns);

            var backwardPawns = chain.backwardPawns;

            int minDst = 0;

            foreach (var pawn in backwardPawns)
            {
                int dst = BoardOperations.GetDistancePieces(playerKing, pawn);

                if (minDst > dst)
                    minDst = dst;
            }
            return 14 - minDst;
        }
    }
}
