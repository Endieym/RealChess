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
    internal static class SubEvaluations
    {
        private static Board _gameBoard;
        private static Player whitePlayer;
        private static Player blackPlayer;

        // Sets the game board and players
        public static void SetBoard(Board board)
        {
            _gameBoard = board;
            whitePlayer = board.GetPlayer1();
            blackPlayer = board.GetPlayer2();
        }


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

        public static int CountBishopPair(PieceColor color)
        {
            var pieces = color == PieceColor.WHITE ? whitePlayer.Pieces : blackPlayer.Pieces;

            int bishopCount = 0;
            foreach(var piece in pieces.Values)
            {
                if (piece.Type == PieceType.BISHOP)
                    bishopCount++;
            }
            return bishopCount;

        }

        public static int EvaluateKingPerimeter(PieceColor color, ulong kingPerimeter)
        {
            var player = color == PieceColor.WHITE ? _gameBoard.GetPlayer1() :
                _gameBoard.GetPlayer2();

            var enemy = color == PieceColor.WHITE ? _gameBoard.GetPlayer2() :
                _gameBoard.GetPlayer1();

            int kingSafety = 0;

            
            foreach (var piece in player.Pieces.Values)
            {
                if ((piece.GetPosition() & kingPerimeter) > 0)
                {
                    kingSafety += BoardLogic.EvaluatePieceSafety(piece);
                    kingPerimeter ^= piece.GetPosition();
                }
            }

            foreach (var piece in enemy.Pieces.Values)
            {
                if ((piece.GetPosition() & kingPerimeter) > 0)
                {
                    kingSafety -= piece.Value;
                    kingPerimeter ^= piece.GetPosition();
                }
            }

            while (kingPerimeter > 0)
            {
                ulong position = kingPerimeter & ~(kingPerimeter - 1);
                kingSafety += BoardLogic.EvaluateSquareControl(color, position);
                kingPerimeter &= kingPerimeter - 1;
            }

            return kingSafety;

        }

        public static int EvaluatePosition(ChessPiece piece, GamePhase phase)
        {
            if (phase == GamePhase.Opening && !(piece.Type == PieceType.PAWN ||
                PreprocessedTables.MinorPieces.Contains(piece.Type)))
                return 0;

            int index = (int)Math.Log(piece.GetPosition(), 2);

            if (piece.Color == PieceColor.BLACK)
                index = 63 - index;

            return PreprocessedTables.PieceSquareTable(piece.Type, phase)[index];

        }

        public static int PawnShield(PieceColor color ,ulong kingPerimeter)
        {
            var playerPieces = color == PieceColor.WHITE ? _gameBoard.GetPlayer1().Pieces :
                _gameBoard.GetPlayer2().Pieces;

            int pawnShield = 1;

            ulong pawnPos = (kingPerimeter & KingSidePawns) > 0 ? KingSidePawns : QueenSidePawns;
           
            if (color == PieceColor.BLACK) pawnPos >>= 40;
            pawnPos |= color == PieceColor.WHITE ? pawnPos >> 8 : pawnPos << 8;

            var pieces = playerPieces.Values.ToList();
            pieces.Sort();

            int index = 0;

            while(pieces.Count > index && pieces[index].Type == PieceType.PAWN)
            {
                var piecePos = pieces[index++].GetPosition();

                if ((piecePos & kingPerimeter) > 0
                    && (piecePos & pawnPos) > 0)
                    pawnShield *= pawnShieldBuff;

            }
            return pawnShield;
        }

        /// <summary>
        /// Function which evaluates piece mobility
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="ocuppied">Ocuppied board</param>
        /// <returns></returns>
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
                centerControl &= centerControl - 1;// reset LS1B
                mobility += 2;
            }
            while (attacks != 0)
            {
                attacks &= attacks - 1; // reset LS1B
                mobility++;
            }
            return mobility;
        }

        public static int EvaluatePawnChain(List<Pawn> pawns)
        {
            int countDefendedPawns = 0;

            List<ChessPiece> pawnsPieces = new List<ChessPiece>(pawns.Cast<ChessPiece>());

            foreach (Pawn pawn in pawns)
            {
                if (GetInfluencers(pawnsPieces, pawn.GetPosition()).Count > 0)
                    countDefendedPawns++;
            }

            return countDefendedPawns;
        }

        public static int EvaluateBackwardPawns(List<Pawn> pawns)
        {
            return pawns.Count - EvaluatePawnChain(pawns);

        }

        public static bool IsDoubledPawn(Pawn pawn, ulong pawnsMask)
        {

            return BoardOperations.IsBlocked(pawn.Color,pawn.GetPosition(), pawnsMask);
        }

        public static int EvaluatePassedPawns(List<Pawn> playerPawns, ulong pawnsMask, ulong enemyPawns)
        {
            int passedPawnsEval = 0;

            foreach(var pawn in playerPawns)
            {
                if (IsPassed(pawn, enemyPawns) && !IsDoubledPawn(pawn, pawnsMask))
                    passedPawnsEval += passedPawnBuff;
            }

            return passedPawnsEval;

        }

        public static bool IsPassed(Pawn pawn, ulong occupation)
        {
            return !BoardOperations.IsAdjacentOrBlocked(pawn, occupation);
        }

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
            int playerKingRank = BoardOperations.GetRank(playerKing);
            int playerKingFile = BoardOperations.GetFile(playerKing);

            int dstBetweenKingsRank = Math.Abs(playerKingRank - enemyKingRank);
            int dstBetweenKingsFile = Math.Abs(playerKingFile - enemyKingFile);

            eval += 14 - dstBetweenKingsFile;

            return eval * 10;
        }
    }
}
