using RealChess.Controller;
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

        private const int infinity = 999999;
        private const int negativeInfinity = -infinity;

        // Sets the game board
        public static void SetBoard(Board board)
        {
            _gameBoard = board;
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
            List<Move> allMoves = _gameBoard.GetAllPlayerMoves(color);
            //Random rnd = new Random();          
            //Move bestMove = allMoves[rnd.Next(allMoves.Count)];
            Move bestMove = null;
            int moveScore;
            int bestMoveScore = negativeInfinity;
            foreach(Move move in allMoves)
            {
                _gameBoard.MakeTemporaryMove(move);
                moveScore = BoardEvaluation.Evaluate(color);
                if(moveScore > bestMoveScore)
                {
                    bestMove = move;
                    bestMoveScore = moveScore;
                }
                _gameBoard.UndoMove();

            }
            return bestMove;
        }


    }
}
