﻿using System;
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

        public const int movePenalty = 50;

        public const int infinity = 999999;
        public const int negativeInfinity = -infinity;

        public const int BishopPairBuff = 50;
    }
}