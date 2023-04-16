using RealChess.Model.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealChess.Model.Bitboard.BoardOperations;
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

        /// <summary>
        /// Method which checks if a move revokes castling rights
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        public static bool RevokesCastlingRights(Move move)
        {
            // If king is already castled
            if (_gameBoard.GetKing(move.PieceMoved.Color).Castled)
                return false;

            // If the move is a normal king move, it revokes rights
            if (move.PieceMoved.Type == PieceType.KING &&
                !(move.IsKingSideCastle || move.IsQueenSideCastle))
                return true;
            // If the move is a rook move, it revokes castling rights
            else if (move.PieceMoved.Type == PieceType.ROOK)
                return true;

            return false;
        }


        public static ulong GetKingPerimeter(PieceColor color)
        {
            King king = _gameBoard.GetKing(color);
            return king.GenerateMovesMask();

        }

        // Returns the number of open files on the board
        public static int OpenFiles(Player whitePlayer, Player blackPlayer)
        {
            var whitePieces = whitePlayer.Pieces;
            var blackPieces = blackPlayer.Pieces;

            ulong occupiedMask = 0;
            foreach (var piece in whitePieces)
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

        public static bool IsThreefoldRepetition(string currentPos, List<string> positions)
        {
            int count = 0;

            foreach (string pos in positions)
            {
                if (pos == currentPos)
                {
                    count++;
                    if (count >= 3)
                    {
                        return true; // Threefold repetition
                    }
                }
            }

            return false; // Not a threefold repetition
        }

        // Returns a list of pieces which defend a specific piece
        public static List<ChessPiece> GetDefenders(Player player, ulong position)
        {

            var pieces = player.Pieces;

            List<ChessPiece> defenders = new List<ChessPiece>();

            //ulong pieceMask = pieceUnderDefense.GetPosition();

            // Iterates over every piece of the player defending
            // And adds the pieces which defend the given square to the list
            foreach (var piece in pieces.Values)
            {
                //if (piece.Type == PieceType.KING) continue; // Kings cannot be defenders

                var pieceMoves = _gameBoard.GetAttacksMask(piece);

                if ((pieceMoves & position) > 0) defenders.Add(piece);

            }

            //Sorts by the value of the defenders
            defenders.Sort();
            return defenders;
        }


        // Returns a list of pieces which attack a specific piece

        public static List<ChessPiece> GetAttackers(Player opposingPlayer, ulong position)
        {

            var pieces = opposingPlayer.Pieces;

            List<ChessPiece> attackers = new List<ChessPiece>();

            //ulong pieceMask = pieceUnderAttack.GetPosition();

            // Iterates over every piece of the player attacking
            // And adds the pieces which attack the given square to the list
            foreach (var piece in pieces.Values)
            {
                //if (piece.Type == PieceType.KING) continue; // Kings cannot be attackers

                var pieceMoves = _gameBoard.GetAttacksMask(piece);

                if ((pieceMoves & position) > 0) attackers.Add(piece);

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

            List<ChessPiece> defenders = GetDefenders(defender, piece.GetPosition());
            List<ChessPiece> attackers = GetAttackers(attacker, piece.GetPosition());

            return defenders.Count - attackers.Count;

        }

        // Evaluates the safety of a piece, by the worth of the defense and attack
        public static int EvaluateSafety(PieceColor color, ulong position, int squareValue)
        {
            var defender = color == PieceColor.WHITE ? _gameBoard.GetPlayer1() :
                 _gameBoard.GetPlayer2();

            var attacker = color == PieceColor.WHITE ? _gameBoard.GetPlayer2() :
                 _gameBoard.GetPlayer1();

            // Gets the defenders and attackers on the piece
            List<ChessPiece> defenders = GetDefenders(defender, position);
            List<ChessPiece> attackers = GetAttackers(attacker, position);

            // Adds the value of the piece itself to the defense value, 
            // since capturing the piece will be worth the value of the piece aswell

            return -CalculateExchange(attackers, defenders, squareValue);

        }

        public static int EvaluateSquareControl(PieceColor color, ulong position)
        {
            return EvaluateSafety(color, position, 0);
        }


        public static int EvaluatePieceSafety(ChessPiece piece)
        {
            return EvaluateSafety(piece.Color, piece.GetPosition(), piece.Value);
        }

        // Returns the value of the capture
        public static int GetCaptureValue(ChessPiece pieceCaptured, ChessPiece pieceAttacking)
        {

            var defender = pieceCaptured.Color == PieceColor.WHITE ? _gameBoard.GetPlayer1() :
                _gameBoard.GetPlayer2();

            var attacker = pieceCaptured.Color == PieceColor.WHITE ? _gameBoard.GetPlayer2() :
                 _gameBoard.GetPlayer1();


            // Gets the defenders and attackers on the piece
            List<ChessPiece> defenders = GetDefenders(defender, pieceCaptured.GetPosition());
            List<ChessPiece> attackers = GetAttackers(attacker, pieceCaptured.GetPosition());


            // Puts the piece attacking at the first exchange order
            attackers.Remove(pieceAttacking);
            attackers.Insert(0, pieceAttacking);

            // Calculates the exchange and returns the outcome
            return CalculateExchange(attackers, defenders, pieceCaptured.Value);

        }

        public static int CalculateExchange(List<ChessPiece> attackers, List<ChessPiece> defenders, int squareValue)
        {
            // Adds the value of the piece itself to the defense value, 
            // since capturing the piece will be worth the value of the piece aswell


            if (attackers.Count == 0)
                return 0;

            // If the piece is not under attack or defense, return 0
            if (attackers.Count == 0 && defenders.Count == 0)
                return 0;

            
            
            for (int i = 0; i < attackers.Count; i++)
            {
                // Add the defender's value
                // Since it can be captured
                if (i > 0)
                    squareValue += defenders[i - 1].Value;

                //// Exchange between the last attacker, and the last defender
                //defenseValue = -defenseValue;

                // If there are no more defenders, return 
                if (defenders.ElementAtOrDefault(i) == null)
                    return squareValue;

                // If a defender still exists, subtract the attacker's value,
                // Since it can be captured
                squareValue -= attackers[i].Value;

                // If the piece capturing, is worth less than the piece captured
                if (squareValue > 0)
                    return squareValue * 100;

            }
            return squareValue;

        }
        /// <summary>
        /// Checks if the game has transitioned to middlegame from opening - castled
        /// </summary>
        /// <returns>True if finished opening</returns>
        public static bool FinishedOpening()
        {

            if (_gameBoard.GetPlayer1().GetKing().Castled && _gameBoard.GetPlayer2().GetKing().Castled)
                return true;
            
            return false;
        }

        /// <summary>
        /// Checks if the game has transitioned to endgame
        /// </summary>
        /// <returns>Returns true if in endgame</returns>
        public static bool FinishedMiddleGame()
        {

            int whiteMaterial = BoardEvaluation.ValuePieces(PieceColor.WHITE);
            int blackMaterial = BoardEvaluation.ValuePieces(PieceColor.BLACK);

            if (whiteMaterial <= 12 && blackMaterial <= 12)
                return true;

            return false;
        }

        public enum GamePhase
        {
            Opening,
            Middlegame,
            Endgame

        }



    }
}
