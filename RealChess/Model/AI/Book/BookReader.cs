using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ilf.pgn;
using ilf.pgn.Data;

namespace RealChess.Model.AI.Book
{
    internal static class BookReader
    {
        private static Database blackBook;
        private static Database whiteBook;
        public static void InitialiseDatabases()
        {
            var reader = new PgnReader();

            string currentDirectory = Directory.GetCurrentDirectory();
            
            var blackPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..","..","Model","AI","Book","BlackGames.pgn"));
            Console.WriteLine(blackPath);
            var whitePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "Model", "AI", "Book", "WhiteGames.pgn"));

            blackBook = reader.ReadFromFile(blackPath);
            whiteBook = reader.ReadFromFile(whitePath);

            var game = blackBook.Games[0];

            Console.WriteLine("{0} vs {1} in {2} ({3})\n", game.WhitePlayer, game.BlackPlayer, game.Site, game.Year);

        }
    }
}
