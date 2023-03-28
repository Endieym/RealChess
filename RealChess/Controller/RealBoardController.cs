using RealChess.Model;
using RealChess.Model.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealChess.Model.ChessPieces.ChessPiece;

namespace RealChess.Controller
{
    internal class RealBoardController
    {
        private static Board _gameBoard;

        internal static void SetBoard(Board board)
        {
            _gameBoard = board;
        }

        internal static void UpdateReal(Move move)
        {
            _gameBoard.UpdateReal(move);
        }

        internal static int GetPlayerMorale(PieceColor color)
        {
            return _gameBoard.GetMorale(color);
        }

        internal static int GetPieceMorale(ChessPiece piece)
        {
            if (piece.Type == PieceType.KING)
                return 100;
            int successRate = GetPlayerMorale(piece.Color);

            // Maximum success rate is 100
            if (successRate > 100)
                return 100;
            return successRate;
        }


        internal static int CalculateSuccess(Move move)
        {
            if (move.DefendsCheck || move.IsEnPassantCapture)
                return 100;

            int successRate = GetPieceMorale(move.PieceMoved);

            // Maximum success rate is 100
            if (successRate > 100)
                return 100;
            return successRate ;
        }

        internal static bool TryMove(Move move)
        {
 
            Random rnd = new Random();
            double randomNum = rnd.NextDouble();

            return randomNum <= ((float)CalculateSuccess(move) /100);
        }

    }
}
