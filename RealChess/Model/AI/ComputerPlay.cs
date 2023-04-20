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
using static RealChess.Model.AI.Evaluation.EvaluationConstants;
using RealChess.Model.AI.Evaluation;

namespace RealChess.Model.AI
{
    internal static class ComputerPlay
    {
        private static Board _gameBoard;

        private static bool inBook = true;

 

        // Sets the game board
        public static void SetBoard(Board board)
        {
            _gameBoard = board;
            BookReader.InitialiseDatabases();   
            MoveTranslator.SetBoard(board);
            BoardEvaluation.SetBoard(board);
            MoveEvaluation.SetBoard(board);
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
                Move tempMove = move;

                if (move.IsPromotion)
                    tempMove = MoveEvaluation.ChooseBestPromotion(move);
                    
                
                _gameBoard.MakeTemporaryMove(tempMove);
                moveScore = MoveEvaluation.GetEvaluationForMove(tempMove);
                
                
                if (moveScore > bestMoveScore)
                {                   
                        bestMovesList.Clear();
                        bestMovesList.Add(tempMove);
                        bestMoveScore = moveScore;

                }

                else if (moveScore == bestMoveScore)
                {
                        bestMovesList.Add(tempMove);

                }

                if (tempMove.IsPromotion)
                    BoardUpdate.UndoPromotion(tempMove);

                _gameBoard.UndoMove();
        
                if (move.Type == Move.MoveType.Checkmate)
                {
                    bestMovesList.Clear();
                    bestMovesList.Add(tempMove);
                    return bestMovesList;

                }

            }
            return bestMovesList;

        }

        
           

    }
}
