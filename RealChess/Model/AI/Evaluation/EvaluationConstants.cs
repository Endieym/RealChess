using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealChess.Model.AI.Evaluation
{
    internal static class EvaluationConstants
    {
        public const ulong Center = 0x1818000000;
        public const ulong OuterCenter = 0x3c24243c0000;

        public const ulong KingSidePawns = 0xe0000000000000;
        public const ulong QueenSidePawns = 0x7000000000000;

        public const int movePenalty = 50;
        public const int tradeBonus = 3;

        public const int infinity = 99999999;
        public const int negativeInfinity = -infinity;

        public const int BishopPairBuff = 30;
        public const int RooksConnectedBuff = 30;
        public const int pawnShieldBuff = 3;

        public const int pawnChainBuff = 7;
        public const int backwardPawnPenalty = 4;
        public const int passedPawnBuff = 10;
    }
}
