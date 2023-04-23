using RealChess.Model.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using static RealChess.Model.Bitboard.BoardOperations;
using static RealChess.Model.ChessPieces.ChessPiece;
using RealChess.Model.AI;

namespace RealChess.Model.Bitboard
{

    /// <summary>
    /// The BoardLogic class provides utility functions for the Chess game/engine.
    /// </summary>
    internal static class BoardLogic
    {
        private static Board _gameBoard;

        /// <summary>
        /// Sets the game board to be used for the rest of the class.
        /// </summary>
        /// <param name="board">The game board to be set.</param>
        public static void SetBoard(Board board)
        {
            _gameBoard = board;

        }
        /// <summary>
        /// Gets the pieces of a specific player, and returns whether or not the rooks
        /// are connected
        /// </summary>
        /// <param name="pieces">Dictionary of pieces of the player</param>
        /// <param name="color">Color of the player</param>
        /// <param name="occupied">Bitboard representing all occupied squares</param>
        /// <returns>Returns true if the rooks of the player are connected, false otherwise</returns>
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
        /// <param name="move">The move to check</param>
        /// <returns>Returns true if the move revokes castling rights, false otherwise</returns>
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

        /// <summary>
        /// Gets the bitboard representation of the squares surrounding the king of a given color.
        /// </summary>
        /// <param name="color">The color of the king</param>
        /// <returns>A bitboard representing the squares surrounding the king</returns>
        public static ulong GetKingPerimeter(PieceColor color)
        {
            King king = _gameBoard.GetKing(color);
            return king.GenerateMovesMask();

        }

        /// <summary>
        /// Returns the number of open files on the chessboard.
        /// An open file is a file (column) without any pawns on it.
        /// </summary>
        /// <param name="whitePlayer">The player with the white pieces</param>
        /// <param name="blackPlayer">The player with the black pieces</param>
        /// <returns>The number of open files</returns>
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

        /// <summary>
        /// Method which checks if the current position has occurred three times in the game history
        /// </summary>
        /// <param name="currentPos">The current position</param>
        /// <param name="positions">A list of positions from previous moves in the game</param>
        /// <returns>True if the current position has occurred three times, false otherwise</returns>
        public static bool IsThreefoldRepetition(string currentPos, List<string> positions)
        {
            int count = 0;

            foreach (string pos in positions)
            {
                if (pos.Equals(currentPos))
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


        /// <summary>
        /// Determines if the current position is a dead position,
        /// meaning it is not possible for either player to win.
        /// </summary>
        /// <returns>True if the position is a dead position, false otherwise</returns>
        public static bool IsDeadPosition()
        {
            var whitePieces = GetPieces(PieceColor.WHITE);
            var blackPieces = GetPieces(PieceColor.BLACK);

            int whiteMinorPieces = 0;
            int blackMinorPieces = 0;

            int whiteBishops = 0;
            int blackBishops = 0;
            Bishop whiteBishop = null, blackBishop = null;


            int whitePieceCount = whitePieces.Count;
            int blackPieceCount = blackPieces.Count;

            int allPieceCount = whitePieceCount + blackPieceCount;

            if (allPieceCount == 2)
                return true;

            foreach(var piece in whitePieces)
            {
                if (PreprocessedTables.MinorPieces.Contains(piece.Type))
                    whiteMinorPieces++;

                if (piece.Type == PieceType.BISHOP)
                {
                    whiteBishop = (Bishop)piece;
                    whiteBishops++;

                }
            }
            foreach (var piece in whitePieces)
            {
                if (PreprocessedTables.MinorPieces.Contains(piece.Type))
                    blackMinorPieces++;

                if (piece.Type == PieceType.BISHOP)
                {
                    blackBishop = (Bishop)piece;
                    blackBishops++;
                }
            }

            if (allPieceCount == 3 && (whiteMinorPieces == 1 || blackMinorPieces == 1))
                return true;

            if (allPieceCount == 4 && whiteBishops == 1 && blackBishops == 1)
                return OnSameColor(whiteBishop, blackBishop);

            return false;
        }

        /// <summary>
        /// Returns a list of pieces that can attack a given position.
        /// </summary>
        /// <param name="pieces">List of pieces to check for attackers.</param>
        /// <param name="position">Position to check for attackers.</param>
        /// <returns>List of pieces that can attack the given position.</returns>
        public static List<ChessPiece> GetInfluencers(List<ChessPiece> pieces, ulong position)
        {

            List<ChessPiece> influencers = new List<ChessPiece>();

            // Iterates over every piece of the player attacking
            // And adds the pieces which attack the given square to the list
            foreach (var piece in pieces)
            {
                //if (piece.Type == PieceType.KING) continue; // Kings cannot be attackers

                var pieceMoves = _gameBoard.GetAttacksMask(piece);

                if ((pieceMoves & position) > 0) influencers.Add(piece);

            }
            // Sorts by the value of the influencers
            influencers.Sort();
            return influencers;
        }

        /// <summary>
        /// Counts the number of pieces defending a piece at a given position minus the number of attackers.
        /// </summary>
        /// <param name="color">The color of the defending pieces.</param>
        /// <param name="position">The position of the defended piece.</param>
        /// <returns>The difference between the number of defenders and attackers.</returns>
        public static int CountSafety(PieceColor color, ulong position)
        {
            var defenderPieces = GetPieces(color).ToList();
            var attackerPieces = GetPieces(GetOppositeColor(color)).ToList();

            List<ChessPiece> defenders = GetInfluencers(defenderPieces, position);
            List<ChessPiece> attackers = GetInfluencers(attackerPieces, position);

            return defenders.Count - attackers.Count;
        }

        /// <summary>
        /// Returns a collection of all pieces belonging to a given color on the game board.
        /// </summary>
        /// <param name="color">The color of the pieces to retrieve.</param>
        /// <returns>A collection of all pieces of the given color on the game board.</returns>
        public static Dictionary<int, ChessPiece>.ValueCollection GetPieces(PieceColor color)
        {
            var pieces = (color == PieceColor.WHITE ? _gameBoard.WhitePlayer :
                _gameBoard.BlackPlayer).Pieces.Values;

            return pieces;
        }

        /// <summary>
        /// Evaluates the safety of a piece by considering the worth of the defense and attack. The method calculates the difference between the worth of the defending and attacking pieces on the given position, with the worth of the defending pieces multiplied by -1 to indicate that more defenders make the position safer.
        /// </summary>
        /// <param name="color">The color of the player defending the position.</param>
        /// <param name="position">The position being defended.</param>
        /// <param name="squareValue">The value of the square being defended.</param>
        /// <returns>The safety evaluation of the position.</returns>
        public static int EvaluateSafety(PieceColor color, ulong position, int squareValue)
        {
            var defenderPieces = GetPieces(color).ToList();
            
            var attackerPieces = GetPieces(GetOppositeColor(color)).ToList();

            // Gets the defenders and attackers on the piece
            List<ChessPiece> defenders = GetInfluencers(defenderPieces, position);
            List<ChessPiece> attackers = GetInfluencers(attackerPieces, position);

            King attackerKing = _gameBoard.GetKing(GetOppositeColor(color));
            
            if(defenders.Count >= attackers.Count)
                attackers.Remove(attackerKing); 

            // Adds the value of the piece itself to the defense value, 
            // since capturing the piece will be worth the value of the piece aswell

            return -CalculateExchange(attackers, defenders, squareValue);
        }

        /// <summary>
        /// Evaluates the control of a square for a specific color, based on the safety of the square
        /// </summary>
        /// <param name="color">The color of the player whose control is evaluated.</param>
        /// <param name="position">The bitboard representing the square/s evaluated.</param>
        /// <returns>An integer representing the evaluation score.</returns>
        public static int EvaluateSquareControl(PieceColor color, ulong position)
        {
            return EvaluateSafety(color, position, 0);
        }

        /// <summary>
        /// Evaluates the safety of a chess piece based on the worth of the defense and attack.
        /// </summary>
        /// <param name="piece">The chess piece to be evaluated for safety.</param>
        /// <returns>An integer value representing the safety of the chess piece.</returns>
        public static int EvaluatePieceSafety(ChessPiece piece)
        {
            return EvaluateSafety(piece.Color, piece.GetPosition(), piece.Value);
        }

        /// <summary>
        /// Returns a list of pieces of a given color that are not well defended and can be captured by an opponent
        /// </summary>
        /// <param name="color">The color of the pieces to be evaluated</param>
        /// <returns>List of ChessPieces that are not well defended</returns>
        public static List<ChessPiece> GetHangingPieces(PieceColor color)
        {
            var playerPieces = color == PieceColor.WHITE ? _gameBoard.GetPlayer1().Pieces :
                _gameBoard.GetPlayer2().Pieces;

            List<ChessPiece> hangingList = new List<ChessPiece>();

            foreach(var piece in playerPieces.Values)
            {
                if (EvaluatePieceSafety(piece) < 0)
                    hangingList.Add(piece);
            }

            hangingList.Sort();
            return hangingList;
        }

        /// <summary>
        /// Calculates the value of the capture of a piece based on the attacking and defending pieces.
        /// </summary>
        /// <param name="pieceCaptured">The piece being captured.</param>
        /// <param name="pieceAttacking">The piece that made the capture.</param>
        /// <returns>The value of the capture.</returns>
        public static int GetCaptureValue(ChessPiece pieceCaptured, ChessPiece pieceAttacking)
        {
            var defender = GetPieces(pieceCaptured.Color).ToList();
            var attacker = GetPieces(pieceAttacking.Color).ToList();

            // Gets the defenders and attackers on the piece
            List<ChessPiece> defenders = GetInfluencers(defender, pieceCaptured.GetPosition());
            List<ChessPiece> attackers = GetInfluencers(attacker, pieceCaptured.GetPosition());

            // Puts the piece attacking at the first exchange order
            attackers.Remove(pieceAttacking);
            attackers.Insert(0, pieceAttacking);

            // Calculates the exchange and returns the outcome
            return CalculateExchange(attackers, defenders, pieceCaptured.Value);

        }

        /// <summary>
        /// Calculates the exchange of pieces between attackers and defenders
        /// </summary>
        /// <param name="attackers">List of ChessPieces attacking the square</param>
        /// <param name="defenders">List of ChessPieces defending the square</param>
        /// <param name="squareValue">Value of the square being attacked</param>
        /// <returns>The value of the capture, taking into account the values of the attacking and defending pieces</returns>
        public static int CalculateExchange(List<ChessPiece> attackers, List<ChessPiece> defenders, int squareValue)
        {
            // Adds the value of the piece itself to the defense value, 
            // since capturing the piece will be worth the value of the piece aswell

            
            if (attackers.Count == 0)
                return 0;

            // If the piece is not under attack or defense, return 0
            if (attackers.Count == 0 && defenders.Count == 0)
                return 0;

            

            int originalValue = squareValue;  
            
            for (int i = 0; i < attackers.Count; i++)
            {
                // Add the defender's value
                // Since it can be captured
                if (i > 0)
                {
                    // If another attacker exists, king can't take back.
                    if (defenders[i - 1].Type == PieceType.KING)
                        return squareValue + attackers[i-1].Value;

                    squareValue += defenders[i - 1].Value;
                }
                    

                //// Exchange between the last attacker, and the last defender
                //defenseValue = -defenseValue;

                // If there are no more defenders, return 
                if (defenders.ElementAtOrDefault(i) == null)
                    return originalValue;

                // If a defender still exists, subtract the attacker's value,
                // Since it can be captured
                squareValue -= attackers[i].Value;

                // If the piece capturing, is worth less than the piece captured
                if (squareValue > 0)
                    return squareValue;

            }
            return squareValue;

        }

        /// <summary>
        /// Determines if a given chess piece is threatening another chess piece.
        /// </summary>
        /// <param name="threat">The chess piece that is allegedly threatening.</param>
        /// <param name="threatened">The chess piece that is allegedly threatened.</param>
        /// <returns>True if the threat is present, false otherwise.</returns>
        public static bool IsThreateningPiece(ChessPiece threat, ChessPiece threatened)
        {
            if ((_gameBoard.GetAttacksMask(threat) & threatened.GetPosition()) > 0)
                return true;

            return false;
        }

        /// <summary>
        /// Returns a list of all enemy pieces that are threatened by a given move.
        /// </summary>
        /// <param name="move">The move to check for threatened pieces.</param>
        /// <returns>A list of all enemy pieces that are threatened by the move.</returns>
        public static List<ChessPiece> ThreatenedPieces(Move move)
        {
            // Gets the masks for the captures, according to piece type
            ulong movesMask = _gameBoard.GetAttacksMask(move.PieceMoved);

            // Checks colliding squares with enemy
            movesMask &= move.PieceMoved.Color == PieceColor.WHITE ? _gameBoard.BlackBoard :
                _gameBoard.WhiteBoard;

            var enemyPieces = (move.PieceMoved.Color == PieceColor.WHITE ? _gameBoard.BlackPlayer :
                _gameBoard.WhitePlayer).Pieces;

            List<ChessPiece> threatenedPieces = new List<ChessPiece>();
            
            foreach(var enemyPiece in enemyPieces.Values)
            {
                if ((enemyPiece.GetPosition() & movesMask) > 0)
                    threatenedPieces.Add(enemyPiece);
            }

            return threatenedPieces;
        }

        // Enum representing the game phase
        public enum GamePhase
        {
            Opening,
            Middlegame,
            Endgame

        }
    }
}
