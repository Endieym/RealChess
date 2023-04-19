using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ilf.pgn;
using ilf.pgn.Data;
using RealChess.Model.ChessPieces;
using static RealChess.Model.ChessPieces.ChessPiece;
using static RealChess.Model.AI.Book.MoveTranslator;

namespace RealChess.Model.AI.Book
{
    internal static class BookReader
    {
        private static Database blackBook;
        private static Database whiteBook;

        private static List<Game> MatchingWhiteGames;

        private static List<Game> MatchingBlackGames;

        public static int GamesQuantity { get; set; }

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

            var moveTest = MatchingBlackGames[0].MoveText.GetMoves().ElementAt(0);
            Console.WriteLine(moveTest);
            Console.WriteLine(moveTest.TargetSquare);
            Console.WriteLine(SquaresEqual(moveTest.TargetSquare, 36));

            Console.WriteLine(MoveEqual(moveTest, new Move(36, new Pawn())));
        }


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

        public static List<Move> GetPossibleBookMoves(PieceColor color,List<Move> currentMoves, List<Move> possibleMoves)
        {
            List<Move> moves = new List<Move>();

            var masterMoves = GetBookMoves(color, currentMoves);
            
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

            GamesQuantity = MatchingBlackGames.Count + MatchingWhiteGames.Count;
        }

        //public static void PrintGames()
        //{
        //    Console.Clear();
        //    foreach(var game in MatchingBlackGames)
        //    {
        //        Console.WriteLine(game);
        //    }

        //    foreach (var game in MatchingWhiteGames)
        //    {
        //        Console.WriteLine(game);
        //    }

        //    Console.WriteLine();
        //    Console.WriteLine(GamesQuantity);
        //}

        //public static void TestGame(List<Move> moves)
        //{
        //    var game = MatchingBlackGames[0];

        //    Console.WriteLine(GamesEqual(game, moves));
        //}

        public static bool GamesEqual(Game masterGame, List<Move> moves)
        {
            int index = 0;

            var masterMoves = masterGame.MoveText.GetMoves();

            foreach (var move in masterMoves)
            {
                if (!MoveEqual(move, moves[index]))
                    return false;
                index++;

                if (moves.Count == index)
                    break;
            }

            if (moves.Count != index)
                return false;

            return true;
        }




    }
}
