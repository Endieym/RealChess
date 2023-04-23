using RealChess.Model;
using RealChess.Model.ChessPieces;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using static RealChess.Model.ChessPieces.ChessPiece;

namespace RealChess.Controller
{
    /// <summary>
    /// Class responsible for game UI when in "Real" mode
    /// </summary>
    internal static class RealController
    {
        private static Panel sidePanel;
        private static Panel whitePanel;
        private static Panel blackPanel;
        private static PictureBox whitePic;
        private static PictureBox blackPic;
        private static Label blackLabel;
        private static Label whiteLabel;
        private static Label blackPercent;
        private static Label whitePercent;
        private static ProgressBar whiteBar;
        private static ProgressBar blackBar;
        private static PictureBox vsPic;


        /// <summary>
        /// Sets the controls from the side panel
        /// </summary>
        /// <param name="panel">Side panel</param>
        public static void SetSidePanel(Panel panel)
        {
            sidePanel = panel;

            whitePanel = sidePanel.Controls.Find("whitePanel", true).FirstOrDefault() as Panel;
            blackPanel = sidePanel.Controls.Find("blackPanel", true).FirstOrDefault() as Panel;
            whitePic = sidePanel.Controls.Find("whitePic", true).FirstOrDefault() as PictureBox;
            blackPic = sidePanel.Controls.Find("blackPic", true).FirstOrDefault() as PictureBox;
            blackLabel = sidePanel.Controls.Find("blackLabel", true).FirstOrDefault() as Label;
            whiteLabel = sidePanel.Controls.Find("whiteLabel", true).FirstOrDefault() as Label;
            blackPercent = sidePanel.Controls.Find("blackPercent", true).FirstOrDefault() as Label;
            whitePercent = sidePanel.Controls.Find("whitePercent", true).FirstOrDefault() as Label;
            whiteBar = sidePanel.Controls.Find("whiteBar", true).FirstOrDefault() as ProgressBar;
            blackBar = sidePanel.Controls.Find("blackBar", true).FirstOrDefault() as ProgressBar;
            vsPic = sidePanel.Controls.Find("vsPic", true).FirstOrDefault() as PictureBox;
        }

        /// <summary>
        /// Starts capture animation
        /// </summary>
        /// <param name="move">Capture tried</param>
        public static void ShowMove(Move move)
        {
            ChessForm.DisableClicks();
            ShowPiece(move.CapturedPiece);
            SetMoraleMove(move);
            if (move.PieceMoved.Color == PieceColor.WHITE)
            {
                whitePanel.Visible = true;
                blackPercent.Visible = false;
                blackBar.Visible = false;
            }
            else
            {
                blackPanel.Visible = true;
                whitePercent.Visible = false;
                whiteBar.Visible = false;
            }

            Bitmap gifImage = new Bitmap(150, 200);

            // Read the image from the resources using its name
            gifImage = Properties.Resources.fightGif;

            vsPic.Image = gifImage;
            // Start the animation using the ImageAnimator class

            // Wait for 2 seconds using a while loop and a Stopwatch
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (stopwatch.ElapsedMilliseconds < 2000)
            {
                Application.DoEvents();
            }
                       
            stopwatch.Stop();

            ChessForm.EnableClicks();
            // Stop the animation
            ImageAnimator.StopAnimate(gifImage, (sender, e) =>
            {
                // Redraw the PictureBox control one last time to display the final image
                vsPic.Invalidate();
            });
            
        }

        
        /// <summary>
        /// Shows the piece on the sidebar
        /// </summary>
        /// <param name="piece">Piece clicked</param>
        public static void ShowPiece(ChessPiece piece)
        {

            if (piece.Color == ChessPiece.PieceColor.WHITE)
                ShowWhite();

            else
                ShowBlack();
                
            
            ChangeLabel(piece);
            ChangePicture(piece);
            SetMoralePiece(piece);
        }

        /// <summary>
        /// Sets the morale of a piece
        /// </summary>
        /// <param name="piece"></param>
        public static void SetMoralePiece(ChessPiece piece)
        {
            var success = RealBoardController.GetPieceMorale(piece);
            SetMorale(success, piece.Color);

        }

        /// <summary>
        /// Sets the morale of the move(success rate)
        /// </summary>
        /// <param name="move">Move tried</param>
        public static void SetMoraleMove(Move move)
        {
            // Gets the morale for both players
            var success = RealBoardController.CalculateSuccess(move);

            SetMorale(success, move.PieceMoved.Color);

        }

        /// <summary>
        /// Sets the morale of a player 
        /// </summary>
        /// <param name="morale">Morale number</param>
        /// <param name="color">Player number</param>
        public static void SetMorale(int morale, PieceColor color)
        {
            if (color == PieceColor.WHITE)
            {
                whiteBar.Value = morale;
                whitePercent.Text = morale.ToString() + '%';

            }
            else
            {
                blackBar.Value = morale;

                blackPercent.Text = morale.ToString() + '%';
            }
        }

        /// <summary>
        /// Resets sidebar to original
        /// </summary>
        public static void ResetToMorale()
        {
            whitePanel.Visible = true;
            blackPanel.Visible = true;

            whiteBar.Visible = true;
            whitePercent.Visible = true;
            blackBar.Visible = true;
            blackPercent.Visible = true;
            // Gets the morale for both players
            var whiteMorale = RealBoardController.GetPlayerMorale(PieceColor.WHITE);
            var blackMorale = RealBoardController.GetPlayerMorale(PieceColor.BLACK);

            whiteBar.Value = whiteMorale;
            blackBar.Value = blackMorale;

            whitePercent.Text = whiteMorale.ToString() + '%';
            blackPercent.Text = blackMorale.ToString() + '%';

            // Resets the text 
            whiteLabel.Text = "White Morale";
            blackLabel.Text = "Black Morale";

            // Resets the pieces images
            whitePic.Image = LoadPieceImage(new King()); 
            blackPic.Image = LoadPieceImage(new King { Color = ChessPiece.PieceColor.BLACK});
            
            vsPic.Image = (Image)Properties.Resources.ResourceManager.GetObject("vsLogo");
        }

        /// <summary>
        /// Changes the label of the sidebar to piece type
        /// </summary>
        /// <param name="piece"></param>
        public static void ChangeLabel(ChessPiece piece)
        {
            var label = piece.Color == ChessPiece.PieceColor.WHITE ? whiteLabel :
                blackLabel;
            var text = piece.Type.ToString().ToLower();
            var first = char.ToUpper(text[0]);
            label.Text = first + text.Substring(1);
        }

        public static void ShowMessage(string text, string caption)
        {
            Thread t = new Thread(() => MyMessageBox(text, caption));
            t.Start();
        }

        private static void MyMessageBox(object text, object caption)
        {
            MessageBox.Show((string)text, (string)caption);
        }

        public static void ChangePicture(ChessPiece piece)
        {
            var picBox = piece.Color == ChessPiece.PieceColor.WHITE ? whitePic :
                blackPic;

            picBox.Image = LoadPieceImage(piece);
        }
        public static void ShowWhite()
        {
            whitePanel.Visible = true;
            blackPanel.Visible = false;
            whiteBar.Visible = true;
            whitePercent.Visible = true;
        }
        public static void HideWhite()
        {
            whitePanel.Visible = false;
            
        }
        public static void ShowBlack()
        {
            blackPanel.Visible = true;
            whitePanel.Visible = false;
            blackBar.Visible = true;
            blackPercent.Visible = true;
        }
        public static void HideBlack()
        {
            blackPanel.Visible = false;
        }

        // Gets the image according to the piece inputted
        private static Image LoadPieceImage(ChessPiece piece)
        {
            string imageName = $"{piece.Color.ToString().ToLower()}_{piece.Type.ToString().ToLower()}";

            return Properties.Resources.ResourceManager.GetObject(imageName) as Image;

        }
    }
}
