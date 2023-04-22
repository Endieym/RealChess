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
    /// <summary>
    /// The ComputerPlay class handles the logic for the computer's moves during gameplay.
    /// It includes methods for setting the game board, initializing databases,
    /// and choosing the best moves.
    /// </summary>
    internal static class ComputerPlay
    {
        private static Board _gameBoard;

        private static bool inBook = true;



        /// <summary>
        /// Sets the game board used by the computer player.
        /// </summary>
        /// <param name="board">The current game board.</param>
        public static void SetBoard(Board board)
        {
            _gameBoard = board;
            BookReader.InitialiseDatabases();   
            MoveTranslator.SetBoard(board);
            BoardEvaluation.SetBoard(board);
            MoveEvaluation.SetBoard(board);
        }

        /// <summary>
        /// Plays a move for a specific color.
        /// </summary>
        /// <param name="playerColor">The color of the player to make a move.</param>
        public static void PlayMove(PieceColor playerColor)
        {
            ChessForm.DisableClicks();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Delay for 700 milliseconds - computer is too fast for graphics
            while (stopwatch.ElapsedMilliseconds < 700)
            {
                Application.DoEvents();
            }

            stopwatch.Stop();

            ChessForm.EnableClicks();
            GameController.MovePiece(ChooseBestMove(playerColor));
        }

        /// <summary>
        /// Chooses the best move for a specific color. First tries to retrieve a move from the opening book if one is available,
        /// otherwise calculates the best possible moves for the current position and selects one at random.
        /// </summary>
        /// <param name="color">The color of the player to make a move.</param>
        /// <returns>The best move to make.</returns>
        public static Move ChooseBestMove(PieceColor color)
        {
            Random rnd = new Random();          

            List<Move> bestMoves = inBook ? GetBookMoves(color): GetBestMovesList(color);

            Move bestMove = bestMoves[rnd.Next(bestMoves.Count)];
            
            return bestMove;
        }

        /// <summary>
        /// Retrieves a list of possible opening book moves for the specified color.
        /// </summary>
        /// <param name="color">The color of the player for whom to retrieve the book moves.</param>
        /// <returns>A list of possible opening book moves for the specified color.</returns>
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
        /// <param name="color">The color of the player</param>
        /// <returns>A list of the best possible moves</returns>
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
                if (move.EndSquare == 36)
                    Console.Write("helo");

                moveScore = MoveEvaluation.GetEvaluationForMove(tempMove);
                
                if (moveScore > bestMoveScore)
                {                   
                        bestMovesList.Clear();
                        bestMovesList.Add(tempMove);
                        bestMoveScore = moveScore;
                }

                else if (moveScore == bestMoveScore)
                        bestMovesList.Add(tempMove);

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
