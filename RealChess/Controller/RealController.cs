using RealChess.Model.ChessPieces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RealChess.Controller
{
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
            
        }
        public static void ShowPiece(ChessPiece piece)
        {
            
            if(piece.Color == ChessPiece.PieceColor.WHITE)
                ShowWhite();
                
            
            else
                ShowBlack();
                
            
            ChangeLabel(piece);
            ChangePicture(piece);
        }
        public static void ResetToMorale()
        {
            whitePanel.Visible = true;
            blackPanel.Visible = true;

            // Resets the text 
            whiteLabel.Text = "White Morale";
            blackLabel.Text = "Black Morale";

            // Resets the pieces images
            whitePic.Image = LoadPieceImage(new King()); 
            blackPic.Image = LoadPieceImage(new King { Color = ChessPiece.PieceColor.BLACK}); 

        }
        public static void ChangeLabel(ChessPiece piece)
        {
            var label = piece.Color == ChessPiece.PieceColor.WHITE ? whiteLabel :
                blackLabel;
            var text = piece.Type.ToString().ToLower();
            var first = char.ToUpper(text[0]);
            label.Text = first + text.Substring(1);
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
        }
        public static void HideWhite()
        {
            whitePanel.Visible = false;
        }
        public static void ShowBlack()
        {
            blackPanel.Visible = true;
            whitePanel.Visible = false;

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
