using RealChess.Controller;
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

        public static void SetBoard(Board board)
        {
            _gameBoard = board;
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
            Random rnd = new Random();
            
            return allMoves[rnd.Next(allMoves.Count)];
        }
    }
}
