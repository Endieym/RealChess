using System.Collections.Generic;
using System.IO;
using System.Linq;
using ilf.pgn;
using ilf.pgn.Data;
using static RealChess.Model.ChessPieces.ChessPiece;
using static RealChess.Model.AI.Book.MoveTranslator;

namespace RealChess.Model.AI.Book
{

    /// <summary>
    /// A class responsible for reading and managing opening book data in PGN format.
    /// </summary>
    internal static class BookReader
    {
        private static Database blackBook;
        private static Database whiteBook;

        private static List<Game> MatchingWhiteGames;

        private static List<Game> MatchingBlackGames;

        /// <summary>
        /// Initializes the black and white opening book databases by reading from PGN files.
        /// </summary>
        public static void InitialiseDatabases()
        {
            var reader = new PgnReader();

            string currentDirectory = Directory.GetCurrentDirectory();

            var blackPath = Path.GetFullPath(Path.Combine(currentDirectory, "..", "..", "Model", "AI", "Book", "BlackGames.pgn"));
            var whitePath = Path.GetFullPath(Path.Combine(currentDirectory, "..", "..", "Model", "AI", "Book", "WhiteGames.pgn"));

            blackBook = reader.ReadFromFile(blackPath);
            whiteBook = reader.ReadFromFile(whitePath);

            MatchingBlackGames = blackBook.Games;
            MatchingWhiteGames = whiteBook.Games;
        }

        /// <summary>
        /// Gets a list of book moves for a given player color and list of all moves which happened thus far.
        /// </summary>
        /// <param name="color">The player color for which to retrieve book moves.</param>
        /// <param name="currentMoves">The current list of moves to match with book moves.</param>
        /// <returns>A list of book moves</returns>
        public static List<ilf.pgn.Data.Move> GetBookMoves(PieceColor color, List<Move> currentMoves)
        {
            List<ilf.pgn.Data.Move> moves = new List<ilf.pgn.Data.Move>();

            var games = color == PieceColor.WHITE ? MatchingWhiteGames : MatchingBlackGames;

            foreach (var game in games)
            {
                var masterMoves = game.MoveText.GetMoves().ToList();

                if (masterMoves.ElementAtOrDefault(currentMoves.Count) == null) continue;

                var lastMove = masterMoves.ElementAt(currentMoves.Count);

                moves.Add(lastMove);
            }

            return moves;
        }

        /// <summary>
        /// Gets a list of possible book moves for a given player color, list of moves thus far, and a list of possible moves.
        /// </summary>
        /// <param name="color">The player color for which to retrieve possible book moves.</param>
        /// <param name="currentMoves">The current list of moves to match against possible book moves.</param>
        /// <param name="possibleMoves">The list of possible moves to match against book moves.</param>
        /// <returns>A list of possible book moves.</returns>
        public static List<Move> GetPossibleBookMoves(PieceColor color, List<Move> currentMoves, List<Move> possibleMoves)
        {
            List<Move> moves = new List<Move>();
            var masterMoves = GetBookMoves(color, currentMoves);

            // Checks for every book move if it can be played, if yes add it to the list of final moves
            foreach (var move in possibleMoves)
            {
                foreach (var masterMove in masterMoves)
                {
                    if (MoveEqual(masterMove, move))
                    {
                        moves.Add(move);
                    }
                }
            }

            return moves;
        }

        /// <summary>
        /// Updates the list of matching games based on the list of moves made.
        /// </summary>
        public static void UpdateGames(List<Move> moves)
        {
            var index = moves.Count - 1;

            foreach (var game in MatchingBlackGames.ToArray())
            {
                var gameMoves = game.MoveText.GetMoves().ToList();


                if (gameMoves.Count <= index || !MoveEqual(gameMoves[index], moves[index]))
                    MatchingBlackGames.Remove(game);

            }

            foreach (var game in MatchingWhiteGames.ToArray())
            {
                var gameMoves = game.MoveText.GetMoves().ToList();


                if (gameMoves.Count <= index || !MoveEqual(gameMoves[index], moves[index]))
                    MatchingWhiteGames.Remove(game);

            }
        }
    }
}
