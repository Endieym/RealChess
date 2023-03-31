using RealChess.Model.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealChess.Model.BoardOperations;
using static RealChess.Model.ChessPieces.ChessPiece;

namespace RealChess.Model.Bitboard
{
    internal static class BoardLogic

    {

        private static Board _gameBoard;
        
        // Sets the game board
        public static void SetBoard(Board board)
        {
            _gameBoard = board;
        }
        // Gets the pieces of a specific player, and returns whether or not the rooks
        // are connected
        public static bool AreRooksConnected(Dictionary<int, ChessPiece> pieces, PieceColor color, ulong occupied)
        {
            // Gets the rook pair
            Tuple<Rook, Rook> rookPair = GetRooks(pieces, color);

            // If a rook is missing (captured) return false
            if (rookPair.Item1 == null || rookPair.Item2 == null)
                return false;

            // Checks if the rooks are connected via bitboard
            if ((rookPair.Item1.GenerateLegalMoves(occupied) & rookPair.Item2.GetPosition()) > 0)
                return true;

            return false;
        }

        // Returns the number of open files on the board
        public static int OpenFiles(Player whitePlayer, Player blackPlayer)
        {
            var whitePieces = whitePlayer.Pieces;
            var blackPieces = blackPlayer.Pieces;

            ulong occupiedMask = 0;
            foreach(var piece in whitePieces)
            {
                if (piece.Value.Type == PieceType.PAWN)
                    occupiedMask |= piece.Value.GetPosition();
            }
            foreach (var piece in blackPieces)
            {
                if (piece.Value.Type == PieceType.PAWN)
                    occupiedMask |= piece.Value.GetPosition();
            }

            int openFiles = 0;
            // Iterate over each file
            for (int i = 0; i < 8; i++)
            {
                ulong fileBitboard = BitboardConstants.AFile << i;
                ulong occupiedSquaresOnFile = fileBitboard & occupiedMask;

                // If there are no pawns on the file, increment the count of open files
                if (occupiedSquaresOnFile == 0)
                {
                    openFiles++;
                }
            }

           
            return openFiles;
        }

        // Returns a list of pieces which defend a specific piece
        public static List<ChessPiece> GetDefenders(Player player, ChessPiece pieceUnderDefense )
        {

            var pieces = player.Pieces;

            List<ChessPiece> defenders = new List<ChessPiece>();
            ulong pieceMask = pieceUnderDefense.GetPosition();

            // Iterates over every piece of the player defending
            // And adds the pieces which defend the given square to the list
            foreach (var piece in pieces.Values)
            {
                if (piece.Type == PieceType.KING) continue; // Kings cannot be defenders

                var pieceMoves = _gameBoard.GetAttacksMask(piece);

                if ((pieceMoves & pieceMask) > 0) defenders.Add(piece);
                
            }
            
            //Sorts by the value of the defenders
            defenders.Sort();
            return defenders;
        }


        // Returns a list of pieces which attack a specific piece

        public static List<ChessPiece> GetAttackers(Player opposingPlayer, ChessPiece pieceUnderAttack)
        {

            var pieces = opposingPlayer.Pieces;

            List<ChessPiece> attackers = new List<ChessPiece>();

            ulong pieceMask = pieceUnderAttack.GetPosition();

            // Iterates over every piece of the player attacking
            // And adds the pieces which attack the given square to the list
            foreach (var piece in pieces.Values)
            {
                if (piece.Type == PieceType.KING) continue; // Kings cannot be attackers

                var pieceMoves = _gameBoard.GetAttacksMask(piece);

                if ((pieceMoves & pieceMask)>0) attackers.Add(piece);
                
            }
            // Sorts by the value of the attackers
            attackers.Sort();
            return attackers;
        }
        
        // Counts the number of pieces defending a piece minus the number of attackers
        public static int CountSafety(ChessPiece piece)
        {
            var defender = piece.Color == PieceColor.WHITE ? _gameBoard.GetPlayer1() :
                 _gameBoard.GetPlayer2();

            var attacker = piece.Color == PieceColor.WHITE ? _gameBoard.GetPlayer2() :
                 _gameBoard.GetPlayer1();

            List<ChessPiece> defenders = GetDefenders(defender, piece);
            List<ChessPiece> attackers = GetAttackers(attacker, piece);

            return defenders.Count - attackers.Count;

        }

        // Evaluates the safety of a piece, by the worth of the defense and attack
        public static int EvaluateSafety(ChessPiece piece)
        {
            var defender = piece.Color == PieceColor.WHITE ? _gameBoard.GetPlayer1() :
                 _gameBoard.GetPlayer2();
            
            var attacker = piece.Color == PieceColor.WHITE ? _gameBoard.GetPlayer2() :
                 _gameBoard.GetPlayer1();
            
            // Gets the defenders and attackers on the piece
            List<ChessPiece> defenders = GetDefenders(defender, piece);
            List<ChessPiece> attackers = GetAttackers(attacker, piece);          


            // Adds the value of the piece itself to the defense value, 
            // since capturing the piece will be worth the value of the piece aswell

            return CalculateExchange(attackers, defenders, piece);


        }

        // Returns the value of the capture
        public static int CaptureValue(ChessPiece pieceCaptured, ChessPiece pieceAttacking)
        {
            return EvaluateSafety(pieceCaptured) - pieceAttacking.Value;
        }

        // Returns the value of the capture
        public static int GetCaptureValue(ChessPiece pieceCaptured, ChessPiece pieceAttacking)
        {

            var defender = pieceCaptured.Color == PieceColor.WHITE ? _gameBoard.GetPlayer1() :
                _gameBoard.GetPlayer2();

            var attacker = pieceCaptured.Color == PieceColor.WHITE ? _gameBoard.GetPlayer2() :
                 _gameBoard.GetPlayer1();


            // Gets the defenders and attackers on the piece
            List<ChessPiece> defenders = GetDefenders(defender, pieceCaptured);
            List<ChessPiece> attackers = GetAttackers(attacker, pieceCaptured);


            // Puts the piece attacking at the first exchange order
            attackers.Remove(pieceAttacking);
            attackers.Insert(0, pieceAttacking);

            // Calculates the exchange and returns the outcome
            return CalculateExchange(attackers, defenders, pieceCaptured);

        }

        public static int CalculateExchange(List<ChessPiece> attackers, List<ChessPiece> defenders, ChessPiece pieceCaptured)
        {
            // Adds the value of the piece itself to the defense value, 
            // since capturing the piece will be worth the value of the piece aswell
            int defenseValue = pieceCaptured.Value;
            
            // If the piece is not under attack or defense, return 0
            if (attackers.Count == 0 && defenders.Count == 0)
                return 0;
            
            for (int i = 0; i < attackers.Count; i++)
            {
                // Add the defender's value
                // Since it can be captured
                if (i > 0)
                    defenseValue += defenders[i - 1].Value;

                // Exchange between the last attacker, and the last defender
                if (defenders.ElementAtOrDefault(i) == null)
                    return defenseValue;

                // If a defender still exists, add the attacker's value,
                // Since it can be captured
                defenseValue -= attackers[i].Value;

                if (defenseValue > 0)
                    return -defenseValue;

            }
            return defenseValue;

        }



    }
}
