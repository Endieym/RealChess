using RealChess.Controller;
using RealChess.Model.AI;
using RealChess.Model.AI.Book;
using RealChess.Model.Bitboard;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static RealChess.Model.ChessPieces.ChessPiece;

namespace RealChess.Model
{
    internal static class ComputerPlay
    {
        private static Board _gameBoard;

        private static bool inBook = true;

        private const int infinity = 999999;
        private const int negativeInfinity = -infinity;

        // Sets the game board
        public static void SetBoard(Board board)
        {
            _gameBoard = board;
            BookReader.InitialiseDatabases();   
            MoveTranslator.SetBoard(board);
            BoardEvaluation.SetBoard(board);
        }

        // Plays a move for a specific color
        public static void PlayMove(PieceColor playerColor)
        {
            ChessForm.DisableClicks();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (stopwatch.ElapsedMilliseconds < 700)
            {
                Application.DoEvents();
            }

            stopwatch.Stop();

            ChessForm.EnableClicks();
            GameController.MovePiece(ChooseBestMove(playerColor));
        }
        
        // Chooses the best move for a specific color
        public static Move ChooseBestMove(PieceColor color)
        {
            Random rnd = new Random();          

            List<Move> bestMoves = inBook ? GetBookMoves(color): GetBestMovesList(color);

            Move bestMove = bestMoves[rnd.Next(bestMoves.Count)];
            
            return bestMove;
        }

        public static List<Move> GetBookMoves(PieceColor color)
        {
            var moves = BookReader.GetPossibleBookMoves(color, _gameBoard.GetAllMoves(), _gameBoard.GetAllPlayerMoves(color));
            if (moves.Count == 0)
            {
                inBook = false;
                moves = GetBestMovesList(color);

            }
            return moves;
        }


        /// <summary>
        /// Returns a list of the best possible moves according to evaluation
        /// </summary>
        /// <param name="color"> The color of the player</param>
        /// <returns></returns>
        public static List<Move> GetBestMovesList(PieceColor color)
        {
            List<Move> allMoves = _gameBoard.GetAllPlayerMoves(color);

            List<Move> bestMovesList = new List<Move>();

            double moveScore;
            double bestMoveScore = negativeInfinity;
            foreach (Move move in allMoves)
            {
                if (move.IsPromotion)
                {

                }
                else
                {
                    _gameBoard.MakeTemporaryMove(move);

                }
                moveScore = BoardEvaluation.EvaluateForPlayer(color);

                if (!MoveChecker.IsGoodMove(move))
                {
                    moveScore -= (move.PieceMoved.Value * 100);

                    if (moveScore > 0)
                        moveScore *= -1;
                }

                if (MoveChecker.IsBadForPhase(move, _gameBoard.CurrentPhase))
                    moveScore -= BitboardConstants.MovePenalty;              
                
                if (GameController.IsReal && move.PieceMoved.Type != PieceType.KING)
                   moveScore += RealBoardController.CalculateSuccess(move) /10;
                
                if (move.Type == Move.MoveType.Draw)
                    moveScore = 0;

                if (moveScore > bestMoveScore)
                {                   
                        bestMovesList.Clear();
                        bestMovesList.Add(move);
                        bestMoveScore = moveScore;

                }

                else if (moveScore == bestMoveScore)
                {
                        bestMovesList.Add(move);

                }

                _gameBoard.UndoMove();
        
                if (move.Type == Move.MoveType.Checkmate)
                {
                    bestMovesList.Clear();
                    bestMovesList.Add(move);
                    return bestMovesList;

                }

            }
            return bestMovesList;

        }
           

    }
}
