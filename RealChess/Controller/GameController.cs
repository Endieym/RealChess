using RealChess.Model;
using RealChess.View;
using RealChess.View.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static RealChess.Model.ChessPieces.ChessPiece;

namespace RealChess.Controller
{
    internal static class GameController
    {
        // Current chess piece clicked
        private static ChessPieceControl _currentPieceClicked = null;

        // The panel board
        private static Panel[,] _panelBoard;

        private static int _tileSize;

        private static int _gridSize;

        private static PieceColor turnColor = PieceColor.WHITE;

        // Sets the panel board which represents all the squares on the chessboard
        public static void SetBoard(Panel[,] panelBoard, int tileSize, int gridSize)
        {
            _gridSize = gridSize;
            _tileSize = tileSize;
            _panelBoard = panelBoard;
        }

        // Checks if a move is legal
        internal static bool IsLegalMove(ChessPieceControl pieceSource, Panel targetPanel)
        {
            foreach (ChessPieceControl c in targetPanel.Controls)
                if (c.Equals(pieceSource) || c.Piece.Color == pieceSource.Piece.Color)
                    return false;
            return true;
        }

        internal static bool IsTurn(ChessPieceControl pieceSource)
        {
            return pieceSource.Piece.Color == turnColor;
        }

        // Highlights the legal squares the current piece can traverse to
        internal static void ShowLegalMoves(ChessPieceControl pieceSource)
        {
            // Gets both the legal moves and captures of the clicked piece control
            List<Move> movesList = BoardController.GetMovesList(pieceSource.Piece);
            List<Move> capturesList = BoardController.GetCapturesList(pieceSource.Piece);
            
            // Shows the legal moves
            foreach(Move move in movesList)
            {
                LegalMoveControl legalMoveControl = new LegalMoveControl();
                legalMoveControl.Transfer += LegalMoveControl_Move;
                legalMoveControl.CurrentMove = move;
                _currentPieceClicked = pieceSource;
                _panelBoard[move.EndSquare / 8, move.EndSquare % 8].Controls.Add(legalMoveControl);
            }

            // Shows the legal captures 
            foreach (Move capture in capturesList)
            {
                LegalMoveControl legalMoveControl = new LegalMoveControl();
                legalMoveControl.SetCapture();
                legalMoveControl.CurrentMove = capture;
                legalMoveControl.Transfer += LegalMoveControl_Move;
                _currentPieceClicked = pieceSource;
                
                _panelBoard[capture.EndSquare / 8, capture.EndSquare % 8].Controls.Add(legalMoveControl);
                legalMoveControl.BringToFront();
                legalMoveControl.BackColor = Color.Transparent;
            }
            
        }

        internal static void ClearLegalMoves(ChessPieceControl pieceSource)
        {
            // Gets both the legal moves and captures of the clicked piece control
            List<Move> movesList = BoardController.GetMovesList(pieceSource.Piece);
            List<Move> capturesList = BoardController.GetCapturesList(pieceSource.Piece);
            
            // Hides the legal moves
            foreach (Move move in movesList)
            {
                Panel currentPanel = _panelBoard[move.EndSquare / 8, move.EndSquare % 8];
                foreach (Control c in currentPanel.Controls)
                {
                    if (c is LegalMoveControl)
                        currentPanel.Controls.Remove(c);
                }
            }

            // Hides the legal captures
            foreach (Move capture in capturesList)
            {
                Panel currentPanel = _panelBoard[capture.EndSquare / 8, capture.EndSquare % 8];
                foreach (Control c in currentPanel.Controls)
                {
                    if (c is LegalMoveControl)
                        currentPanel.Controls.Remove(c);
                }
            }
        }



        private static void LegalMoveControl_Move(object sender, TransferEventArgs e)
        {
            // Transfer the selected piece to the clicked panel
            MovePiece(_currentPieceClicked, e.CurrentMove);           
        }

        internal static void MovePiece(ChessPieceControl pieceSource, Move move)
        {
            int key = move.EndSquare;
            Panel targetPanel = _panelBoard[key / 8, key % 8];

            if (move.IsEnPassantCapture)
                key += pieceSource.Piece.Color == PieceColor.WHITE ? 8 : -8;

            else if (move.IsKingSideCastle)
            {
                Panel castlePanel = _panelBoard[key / 8, key % 8 +1];
                Panel targetCastle = _panelBoard[key / 8, key % 8 - 1];

                ChessPieceControl chessPieceControl = null;
                foreach(Control c in castlePanel.Controls)
                {
                    if(c is ChessPieceControl)
                        chessPieceControl = (ChessPieceControl)c;   
                }
                chessPieceControl.Parent.Controls.Clear();
                targetCastle.Controls.Add(chessPieceControl);


            }
            else if(move.IsQueenSideCastle)
            {
                Panel castlePanel = _panelBoard[key / 8, key % 8 - 2];
                Panel targetCastle = _panelBoard[key / 8, key % 8 + 1];

                ChessPieceControl chessPieceControl = null;
                foreach (Control c in castlePanel.Controls)
                {
                    if (c is ChessPieceControl)
                        chessPieceControl = (ChessPieceControl)c;
                }
                chessPieceControl.Parent.Controls.Clear();
                targetCastle.Controls.Add(chessPieceControl);
            }

            Panel capturedPanel = _panelBoard[key / 8, key % 8];

            //foreach (Control c in targetPanel.Controls)
            //{
            //    if (c is ChessPieceControl)
            //        move.CapturedPiece = ((ChessPieceControl)c).Piece;
            //}

            // Clear controls of selected panel
            capturedPanel.Controls.Clear();

            

            //// Get pieces' current location on the board
            //var oldRow = (pieceSource.Parent.Location.Y - 30) / _tileSize;
            //var oldCol = (pieceSource.Parent.Location.X - 10) / _tileSize;
            //move.StartSquare = oldRow * _gridSize + oldCol;  //tileSize * col + 10, tileSize * row + 30

            //// Finds the new location of the piece
            //var row = (targetPanel.Location.Y - 30) / _tileSize;
            //var col = (targetPanel.Location.X - 10) / _tileSize;
            //move.EndSquare = row *_gridSize +col;  //tileSize * col + 10, tileSize * row + 30

            

            // Remove control from previous panel
            pieceSource.Parent.Controls.Remove(pieceSource);
            pieceSource.BackColor = Color.Transparent;
            // Move the selected ChessPieceControl to the target panel
            targetPanel.Controls.Add(pieceSource);

            // Remove the dots indicating which squares are legal to move to.
            ClearLegalMoves(pieceSource);

            FinalizeMove(move);
            
        }
        internal static void RemoveHighlight(PieceColor color)
        {
            int key = BoardController.GetKingPos(color);
            foreach(ChessPieceControl c in _panelBoard[key / 8, key % 8].Controls)
            {
                c.BackColor = Color.Transparent;
            }
        }
        internal static void HighlightCheck(PieceColor color)
        {
            int key = BoardController.GetKingPos(color);
            foreach (ChessPieceControl c in _panelBoard[key / 8, key % 8].Controls)
            {
                c.BackColor = Color.Red;
            }
        }
        internal static void FinalizeMove(Move move)
        {

            // Updates the data structure
            BoardController.UpdateBoard(move);

            var color = move.PieceMoved.Color == PieceColor.WHITE ?
                PieceColor.BLACK : PieceColor.WHITE;
            if (!BoardController.HasLegalMoves(color)&&
                move.IsCheck)
            {
                move.Type = Move.MoveType.Checkmate;
            }
            System.Media.SoundPlayer player;
            switch (move.Type)
            {
                case Move.MoveType.Capture:
                    player = new System.Media.SoundPlayer(Properties.Resources.capture);

                    break;

                case Move.MoveType.Check:
                    player = new System.Media.SoundPlayer(Properties.Resources.Check);
                    if (move.PieceMoved.Color == PieceColor.WHITE)
                        HighlightCheck(PieceColor.BLACK);
                    else
                        HighlightCheck(PieceColor.WHITE);
                    break;
                case Move.MoveType.Checkmate:
                    player = new System.Media.SoundPlayer(Properties.Resources.Check);

                    if (move.PieceMoved.Color == PieceColor.WHITE)
                    {
                        HighlightCheck(PieceColor.BLACK);
                        MessageBox.Show("Checkmate by WHITE!");

                    }
                    else 
                    {
                        HighlightCheck(PieceColor.WHITE);
                        MessageBox.Show("Checkmate by BLACK!");

                    }
                    break;
                default:
                    player = new System.Media.SoundPlayer(Properties.Resources.move);
                    
                    break;
            }

            if (move.DefendsCheck)
            {
                RemoveHighlight(move.PieceMoved.Color);
                
            }


            turnColor = turnColor == PieceColor.WHITE ? PieceColor.BLACK :
                PieceColor.WHITE;

            player.Play();
            
            ChessForm.ResetPieceClicked();

        }



    }
}
