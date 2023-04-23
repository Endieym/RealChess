using static RealChess.Model.Bitboard.BoardLogic;
using static RealChess.Model.ChessPieces.ChessPiece;
using static RealChess.Model.AI.Evaluation.MajorEvaluations;


namespace RealChess.Model.AI.Evaluation
{
    /// <summary>
    /// This class is responsible for evaluating the chess board for the AI player.
    /// It contains methods for evaluating the material count, board control, king safety,
    /// piece development, piece safety, and pawn structure for both white and black players.
    /// </summary>
    internal static class BoardEvaluation
    {
        public static int EndGameWeight { get; set; }
        private static Board _gameBoard;

        public static double Evaluation { get; set; }
        public static int MaterialCount { get; set; }

        /// <summary>
        /// Sets the chess board for the evaluation process.
        /// </summary>
        /// <param name="board">The current chess board.</param>
        public static void SetBoard(Board board)
        {
            _gameBoard = board;

            MajorEvaluations.SetBoard(board);
            SubEvaluations.SetBoard(board);
        }

        /// <summary>
        /// Evaluates the chess board for both players.
        /// A positive number means better for white, negative for black
        /// and zero is a draw.
        /// </summary>
        /// <param name="phase">The game phase (Opening or Endgame).</param>
        /// <returns>The evaluation score as a double.</returns>
        public static double Evaluate(GamePhase phase)
        {
            
            // The difference between white's pieces and black's pieces (by value)
            double evaluation = EvaluateMaterial();

            // Evaluates the difference board control of white against black's.
            evaluation += EvaluateBoardControl();

            // Evaluates king
            evaluation += EvaluateKing(phase);

            // Evaluates piece development for both players
            evaluation += EvaluatePieceDevelopment(phase);

            // Evaluates safety of all player's pieces
            evaluation += EvaluatePiecesSafety();

            // Evaluates pawn structure for both players
            evaluation += EvaluatePawnStructure(phase);

            return evaluation;

        }


        /// <summary>
        /// Evaluates the board position for a specific player
        /// </summary>
        /// <param name="color">The color of the player.</param>
        /// <returns>The evaluation score as a double.</returns>
        public static double EvaluateForPlayer(PieceColor color)
        {
            double evaluation = TaperedEval();
            evaluation *= color == PieceColor.WHITE ? 1 : -1;

            return evaluation;
        }

        /// <summary>
        /// Calculates the tapered evaluation score of the chess board for both players.
        /// Taper(Fade) Eval takes in account the transition of the game phase from opening t ending
        /// The evaluation score gradually shifts from being weighted more towards the opening phase 
        /// to being weighted more towards the ending phase as the game progresses.
        /// </summary>
        /// <returns>The evaluation score as a double.</returns>
        public static double TaperedEval()
        {
            // Get the current game phase weight
            double phase = PhaseEvaluation.GetPhaseWeight();

            // Evaluate the board for the opening and ending phase
            double opening = Evaluate(GamePhase.Opening);
            double endgame = Evaluate(GamePhase.Endgame);

            // Calculate the tapered evaluation score by combining the opening and ending scores based on the game phase weight
            double eval = ((opening * (256 - phase)) + (endgame * phase)) / 256;

            return eval;
        }

        /// <summary>
        /// Evaluates the material advantage of the white player over the black player.
        /// </summary>
        /// <returns>The material advantage as an integer.</returns>
        public static int EvaluateMaterial()
        {
            return EvaluatePlayerMaterial(PieceColor.WHITE) - EvaluatePlayerMaterial(PieceColor.BLACK);
        }

        /// <summary>
        /// Evaluates the board control for both players.
        /// Board control is the measure of how much of the board is being controlled by each player.
        /// </summary>
        /// <returns>The evaluation score as an integer.</returns>
        public static int EvaluateBoardControl()
        {
            return EvaluatePlayerControl(PieceColor.WHITE) - EvaluatePlayerControl(PieceColor.BLACK);

        }
        /// <summary>
        /// Calculates the evaluation score of the king for both players based on the game phase.
        /// In the endgame phase, the score is based on the king activity, whereas in the other phases,
        /// the score is based on the king safety.
        /// </summary>
        /// <param name="phase">The current game phase</param>
        /// <returns>The evaluation score as a double.</returns>
        public static double EvaluateKing(GamePhase phase)
        {
            if(phase == GamePhase.Endgame)
            {
                return EvaluateKingActivity(PieceColor.WHITE) - EvaluateKingActivity(PieceColor.BLACK);
            }
            return EvaluatePlayerKingSafety(PieceColor.WHITE) - EvaluatePlayerKingSafety(PieceColor.BLACK);
        }

        public static double EvaluatePieceDevelopment(GamePhase phase)
        {
            return (EvaluatePlayerDevelopment(PieceColor.WHITE, phase) - EvaluatePlayerDevelopment(PieceColor.BLACK, phase))
                * 1.5;
        }

        /// <summary>
        /// Calculates the evaluation score of the chess board based on the piece development of each player.
        /// </summary>
        /// <returns>The evaluation score as a double.</returns>
        public static int EvaluatePiecesSafety()
        {
            return (EvaluatePlayerDanger(PieceColor.WHITE) - EvaluatePlayerDanger(PieceColor.BLACK)) * 20;
        }

        /// <summary>
        /// Evaluates the pawn structure of the chess board for both players.
        /// </summary>
        /// <param name="phase">The current game phase.</param>
        /// <returns>The evaluation score as a double.</returns>
        public static double EvaluatePawnStructure(GamePhase phase)
        {
            return EvaluatePlayerPawnStructure(PieceColor.WHITE, phase) - EvaluatePlayerPawnStructure(PieceColor.BLACK, phase);
        }


    }
}
