using System;
using System.Collections.Generic;
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
        public static void ShowWhite()
        {
            whitePanel.Visible = true;
        }
        public static void HideWhite()
        {
            whitePanel.Visible = false;
        }
        public static void ShowBlack()
        {
            blackPanel.Visible = true;
        }
        public static void HideBlack()
        {
            blackPanel.Visible = false;
        }
    }
}
