using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealChess.Model
{

    public class Board
    {
        public static int SIZE = 8;
        Player player1;
        Player player2;
        UInt64 bitBoard;
        public Board()
        {
            this.player1 = new Player(true);
            this.player2 = new Player(false);
            this.bitBoard = 0;

            foreach (var item in player1.Pieces)
            {
                bitBoard |= (UInt64)1 << item.Key;
            }

            foreach (var item in player2.Pieces)
            {
                bitBoard |= (UInt64)1 << item.Key;
            }
        }

        public Player GetPlayer1()
        {
            return this.player1;
        }
        public Player GetPlayer2()
        {
            return this.player2;
        }


    }
}
