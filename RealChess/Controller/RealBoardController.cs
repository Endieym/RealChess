using RealChess.Model;
using RealChess.Model.Bitboard;
using RealChess.Model.ChessPieces;
using System;
using System.Collections.Generic;
using static RealChess.Model.ChessPieces.ChessPiece;
using static RealChess.Model.RealConstants;

namespace RealChess.Controller
{
    internal class RealBoardController
    {
        private static Board _gameBoard;

        private static List<Move> lastSuccessfullMoves;
        // Sets the game board
        internal static void SetBoard(Board board)
        {
            _gameBoard = board;
            lastSuccessfullMoves = new List<Move>();
        }

        // Updates the morale of players according to the move made
        internal static void UpdateReal(Move move)
        {
            lastSuccessfullMoves.Add(move);
            var increasedPlayer = move.PieceMoved.Color == PieceColor.WHITE ? _gameBoard.GetPlayer1() :
                _gameBoard.GetPlayer2();

            var decreasedPlayer = move.PieceMoved.Color == PieceColor.WHITE ? _gameBoard.GetPlayer2() :
                _gameBoard.GetPlayer1();
            
            // Adds morale for an en passant capture
            if (move.IsEnPassantCapture)
            {
                increasedPlayer.EnPassant();
                decreasedPlayer.DecreaseMorale();

            }
            // Adds morale for a normal capture
            else if (move.IsCapture)
            {
                increasedPlayer.IncreaseMorale();
                decreasedPlayer.DecreaseMorale();
                if (move.PieceMoved.Type == PieceType.QUEEN)
                    ((Queen)move.PieceMoved).SacrificeBuff = false;
            }

            // Adds morale for a check
            else if (move.IsCheck)
            {
                increasedPlayer.IncreaseMorale();
            }
            
        }

        // Gets the morale of a player of specific color
        internal static int GetPlayerMorale(PieceColor color)
        {
            return _gameBoard.GetMorale(color);
        }

        // Balances the morale when getting too far away
        internal static void BalancePlayers()
        {
            var player1 = _gameBoard.GetPlayer1();
            var player2 = _gameBoard.GetPlayer2();

            if (player1.Morale > player2.Morale + 15)
            {
                player1.Morale -= (player2.Morale / 10);
                player2.Morale += player2.Morale / 10;
            }
            else if (player2.Morale > player1.Morale + 15)
            {
                player2.Morale -= (player1.Morale / 10);
                player1.Morale += player1.Morale / 10;
            }
            if (player1.Morale > 100)
                player1.Morale = 100;

            if (player2.Morale > 100)
                player2.Morale = 100;

        }

        // Returns the morale of the piece
        internal static int GetPieceMorale(ChessPiece piece)
        {
            // A king always has perfect morale
            if (piece.Type == PieceType.KING)
                return 100;

            int successRate = GetPlayerMorale(piece.Color);

            // Checks if the piece is adjacent to the king, if so, improve morale
            if ((piece.GetPosition() &
                _gameBoard.GetKing(piece.Color).GenerateMovesMask()) > 0)
                successRate += AdjacentToKing;

            var pieces = piece.Color == PieceColor.WHITE ? _gameBoard.GetPlayer1().Pieces :
                _gameBoard.GetPlayer2().Pieces;

            // Checks if the rooks are connected, if so, improve rook morale
            if (piece.Type == PieceType.ROOK && BoardLogic.AreRooksConnected(pieces, piece.Color, _gameBoard.GetBoard()))
                successRate += RooksConnected;

            int openFiles = BoardLogic.OpenFiles(_gameBoard.GetPlayer1(), _gameBoard.GetPlayer2());

            // Bishop gets buffed when the board is more open
            if (piece.Type == PieceType.BISHOP)
                successRate += Positionbuff * openFiles;

            // Knight gets buffed when the board is more closed
            else if (piece.Type == PieceType.KNIGHT)
                successRate += Positionbuff * (8 - openFiles);

            int safetyCount = BoardLogic.CountSafety(piece);
            
            // Buffs the piece by the number of pieces defending over attackers
            successRate += DefenseBuff * safetyCount;

            // Checks for queen
            if(piece.Type == PieceType.QUEEN)
            {
                if(lastSuccessfullMoves.Count > 1)
                {
                    // If the last move was a sacrifice, then boost queen morale
                    var lastIndex = lastSuccessfullMoves.Count - 1;
                    if (lastSuccessfullMoves[lastIndex].IsPositiveCapture &&
                        lastSuccessfullMoves[lastIndex - 1].PieceMoved.Equals(lastSuccessfullMoves[lastIndex].CapturedPiece))
                        ((Queen)piece).SacrificeBuff = true;
                    
                }
                if (((Queen)piece).SacrificeBuff)
                    successRate += SacrificeBuff;
                
            }
            // Maximum success rate is 100
            if (successRate > 100)
                return 100;

            return successRate;
        }

        // Returns the success rate of a specific attack
        internal static int CalculateSuccess(Move move)
        {
            // Any moves which defend check or are en passnt,
            // will always have perfect success chances
            if (move.DefendsCheck || move.IsEnPassantCapture)
                return 100;

            int successRate = GetPieceMorale(move.PieceMoved);
            

            // Maximum success rate is 100
            if (successRate > 100)
                return 100;
            return successRate ;
        }

        // Tries a moves and returns if successfull
        internal static bool TryMove(Move move)
        {
 
            Random rnd = new Random();
            double randomNum = rnd.NextDouble();

            return randomNum <= ((float)CalculateSuccess(move) /100);
        }

    }
}
