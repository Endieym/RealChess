using RealChess.Model.ChessPieces;
using RealChess.View;
using static RealChess.Controller.BoardController;
using static RealChess.Controller.GameController;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RealChess.Model;
using RealChess.Controller;
using static RealChess.Model.ChessPieces.ChessPiece;
using System.IO;

namespace RealChess
{
    public partial class ChessForm : Form
    {

        // class member array of Panels to track chessboard tiles
        private Panel[,] _chessBoardPanels;

        // current chess piece clicked
        private static ChessPieceControl _currentPieceClicked = null;

        private static bool InProcess = false;

        private static bool IsWhiteAi = false;
        private static bool IsBlackAi = false;
        
        private const int tileSize = 65;
        private const int gridSize = 8;
        public ChessForm()
        {
            InitializeComponent();
            this.Load += new EventHandler(ChessForm_Load);
            whiteAI.CheckOnClick = true;
            blackAI.CheckOnClick = true;

            whiteAI.CheckedChanged += whiteAI_CheckedChanged;
            blackAI.CheckedChanged += blackAI_CheckedChanged;
        }

        // Go back to main menu
        private void home_Click(object sender, EventArgs e)
        {

            MainPage home = new MainPage();
            home.Show();
            this.Hide();

        }
        

        // event handler of Form Load... init things here
        private void ChessForm_Load(object sender, EventArgs e)
        {
            EnableClicks();
            var clr1 = Color.Green;
            var clr2 = Color.White;
            
            // initialize the chess board panels
            _chessBoardPanels = new Panel[gridSize, gridSize];

            // initialize the board data structure

            Board chessBoard = new Board();
            Dictionary<int, ChessPiece> dict1 = chessBoard.GetPlayer1().Pieces;
            Dictionary<int, ChessPiece> dict2 = chessBoard.GetPlayer2().Pieces;

            // double for loop to handle all rows and columns
            for (var row = 0; row < gridSize; row++)
            {
                for (var col = 0; col < gridSize; col++)
                {
                    
                    // create new Panel control which will be one 
                    // chess board tile
                    var newPanel = new Panel
                    {
                        Size = new Size(tileSize, tileSize),
                        Location = new Point(tileSize * col + 10, tileSize * row + 30)
                    };
                    // add to Form's Controls so that they show up
                    Controls.Add(newPanel);

                    // Add the click event handler to the panel
                    newPanel.Click += new EventHandler(Panel_Click);

                    // add to our 2d array of panels for future use
                    _chessBoardPanels[row, col] = newPanel;

                    // color the backgrounds
                    if (row % 2 == 0)
                        newPanel.BackColor = col % 2 != 0 ? clr1 : clr2;
                    else
                        newPanel.BackColor = col % 2 != 0 ? clr2 : clr1;

                    if (row < 2 || row > 5)
                    {
                        // Create a new ChessPieceControl
                        ChessPieceControl pieceControl = new ChessPieceControl();

                        // Set the piece on the control
                        if (row > 5)
                            pieceControl.SetPiece(dict1[row * gridSize + col]);
                        else
                            pieceControl.SetPiece(dict2[ row * gridSize + col]);

                        // Add an event listener to the UserControl
                        pieceControl.Controls["piecePic"].Click += new EventHandler(PieceControl_Click);

                        // Adds the ChessPieceControl to the panel
                        newPanel.Controls.Add(pieceControl);
                    }
                }
            }

            SetBoard(_chessBoardPanels, tileSize,gridSize);
            BoardController.SetBoard(chessBoard);
            
            if (IsReal)
                RealBoardController.SetBoard(chessBoard);

            GenerateSideBar();


        }


        public static bool IsColorAi(PieceColor color)
        {
            return color == PieceColor.WHITE ? IsWhiteAi: IsBlackAi;
        }

        public void GenerateSideBar()
        {
            // Add the control to the form's controls
            if (IsReal)
            {
                var moralPanel = new Panel
                {
                    Size = new Size(tileSize * 4, tileSize * gridSize ),
                    Location = new Point(tileSize * gridSize + 20, 30),
                    Tag = "moralPanel",
                    BackColor = Color.White,

                };

                PictureBox vsPic = new PictureBox
                {
                    Height = 150,
                    Width = 200,
                    Image = (Image)Properties.Resources.ResourceManager.GetObject("vsLogo"),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Name = "vsPic"
                };

                // Add pieces panel
                var blackPanel = new Panel
                {
                    Size = new Size(tileSize * 4, tileSize * gridSize/2 - vsPic.Height/2),
                    Location = new Point(0,0),
                    Name = "blackPanel",
                    BackColor = Color.White,

                };
                
                blackPanel.Controls.Add(new Label());
                PictureBox blackPic = new PictureBox
                {
                    Height = 100,
                    Width = 100,
                    Image = (Image)Properties.Resources.ResourceManager.GetObject("black_king"),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Anchor = AnchorStyles.None,
                    Name = "blackPic",


                };

                blackPic.Location = new Point((blackPanel.Width - blackPic.Width) / 2,5);
                Label blackLabel = new Label
                {
                    Text = "Black Morale",
                    Name = "blackLabel",
                    Width = 72,
                    TextAlign = ContentAlignment.MiddleCenter,

                };
                blackLabel.Location = new Point((blackPanel.Width - blackLabel.Width)/2, blackPic.Bottom + 11);

                ProgressBar blackBar = new ProgressBar
                {
                    Value = 70,
                    Minimum = 0,
                    Maximum = 100,
                    Width = 200,
                    Height = 20,
                    Name = "blackBar",


                };

                blackBar.Location = new Point((blackPanel.Width - blackBar.Width)/2, blackLabel.Bottom + 10);
                
                Label blackPercentage = new Label
                {
                    Text = "70%",
                    Location = new Point((blackPanel.Width - blackBar.Width) / 2 + blackBar.Width, blackLabel.Bottom + 11),
                    Name = "blackPercent",

                };

                
                vsPic.Location = new Point((blackPanel.Width - vsPic.Width) / 2, blackBar.Bottom+10);

                // Add morale panel
                var whitePanel = new Panel
                {
                    Size = new Size(tileSize * 4, tileSize * gridSize / 2 - vsPic.Height / 2),
                    Location = new Point(0, vsPic.Bottom +10),
                    Name = "whitePanel",
                    BackColor = Color.White,
                };


                ProgressBar whiteBar = new ProgressBar
                {
                    Value = 70,
                    Minimum = 0,
                    Maximum = 100,
                    Width = 200,
                    Height = 20,
                    Name = "whiteBar",


                };   
                whiteBar.Location = new Point((whitePanel.Width - whiteBar.Width) / 2, 0);
                
                Label whitePercentage = new Label
                {
                    Text = "70%",
                    Location = new Point((whitePanel.Width - whiteBar.Width) / 2 + whiteBar.Width , 0),
                    Name = "whitePercent",

                };
                Label whiteLabel = new Label
                {
                    Text = "White Morale",
                    Name = "whiteLabel",
                    Width = 72,
                    TextAlign = ContentAlignment.MiddleCenter,

                };
                whiteLabel.Location = new Point((whitePanel.Width - whiteLabel.Width) / 2, whiteBar.Bottom + 20);

                PictureBox whitePic = new PictureBox
                {
                    Height = 100,
                    Width = 100,
                    Image = (Image)Properties.Resources.ResourceManager.GetObject("white_king"),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Anchor = AnchorStyles.None,
                    Name = "whitePic",

                };

                whitePic.Location = new Point((whitePanel.Width - whitePic.Width) / 2, whiteLabel.Bottom );
                
                //Bitmap gifImage = new Bitmap(150,200);

                //// Read the image from the resources using its name
                //gifImage = Properties.Resources.fightGif;

                //vsPic.Image = gifImage;
                //// Start the animation using the ImageAnimator class
                //ImageAnimator.Animate(vsPic.Image, (sender, e) =>
                //{
                //    // Redraw the picture box control when the frame changes
                //    vsPic.Invalidate();
                //});

                // Adds the controls for the black part
                blackPanel.Controls.Add(blackPic);
                blackPanel.Controls.Add(blackLabel);
                blackPanel.Controls.Add(blackBar);
                blackPanel.Controls.Add(blackPercentage);

                // Adds the controls for the white part
                whitePanel.Controls.Add(whiteBar);
                whitePanel.Controls.Add(whitePercentage);
                whitePanel.Controls.Add(whiteLabel);
                whitePanel.Controls.Add(whitePic);
                
                //Adds the panels to the general panel
                moralPanel.Controls.Add(blackPanel);
                moralPanel.Controls.Add(vsPic);
                moralPanel.Controls.Add(whitePanel);
                Controls.Add(moralPanel);
                RealController.SetSidePanel(moralPanel);

            }
            else
            {
                // Create a new RichTextBox control
                RichTextBox chessNotationBox = new RichTextBox
                {
                    Size = new Size(200, tileSize * gridSize),
                    Location = new Point(tileSize * gridSize + 20, 30)

                };
                chessNotationBox.ReadOnly = true;
                Controls.Add(chessNotationBox);

            }
        }
        public static void DisableClicks()
        {
            InProcess = true;
            
        }
        public static void EnableClicks()
        {
            InProcess = false;
        }

        public void DisableSettings()
        {
            SettingsToolStripMenuItem.Enabled = false;
        }

        // Resets the current piece clicked to null
        public static void ResetPieceClicked()
        {
            // Resets the piece control's color to transparent
            if(_currentPieceClicked != null)
            {
                if(_currentPieceClicked.Piece.Type == PieceType.KING &&
                    ((King)_currentPieceClicked.Piece).InCheck)
                    _currentPieceClicked.BackColor = Color.Red;
                else
                    _currentPieceClicked.BackColor = Color.Transparent;

            }
            _currentPieceClicked = null;

        }
        public static ChessPieceControl GetCurrentPiece()
        {
            return _currentPieceClicked;
        }

        private void whiteAI_CheckedChanged(object sender, EventArgs e)
        {
            if (whiteAI.Checked)
            {
                SetAi(PieceColor.WHITE);

            }
            else
            {
                DisableAi(PieceColor.WHITE);
            }

        }

        private void blackAI_CheckedChanged(object sender, EventArgs e)
        {
            if (blackAI.Checked)
            {
                SetAi(PieceColor.BLACK);
                 
            }
            else
            {
                DisableAi(PieceColor.BLACK);
            }

        }

        // Event handler for when the piece control is clicked
        private void PieceControl_Click(object sender, EventArgs e)
        {
            if (InProcess)
                return;

            MouseEventArgs mouseArgs = e as MouseEventArgs;

            if (mouseArgs.Button != MouseButtons.Left)
                return;

            // Get the clicked piece
            PictureBox piecePic = sender as PictureBox;

            ChessPieceControl myPiece = (ChessPieceControl)piecePic.Parent;


            // Check if a piece was already clicked, if not, set currentPiece to it
            if (_currentPieceClicked is null)
            {
                //MessageBox.Show("Source");

                // Checks if the piece can be moved, according to whom's turn it is
                if (!IsTurn(myPiece))
                    return;

                _currentPieceClicked = myPiece;
                myPiece.BackColor = Color.Yellow;

                // Shows the legal moves that can be made with the piece
                ShowLegalMoves(myPiece);

            }

            else
            {

                // If a piece is already selected, move the current piece to the clicked panel
                Panel targetPanel = (Panel)myPiece.Parent;

                PieceColor currentColor = _currentPieceClicked.Piece.Color;
                var currentPiece = _currentPieceClicked;
                // Trigger the Click event of the target panel
                Panel_Click(targetPanel, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                
                if (myPiece.Piece.Color == currentColor && !myPiece.Equals(currentPiece))
                    PieceControl_Click(piecePic, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));;

            }
        }

        //private void ClearColor()
        //{
        //    for(int row = 0; row < gridSize; row++)
        //    {
        //        for(int col = 0; col < gridSize; col++)
        //        {
        //            foreach (ChessPieceControl c in _chessBoardPanels[row, col].Controls)
        //            {
        //                c.BackColor = Color.Transparent;
                        

        //            }
        //        }
        //    }
        //}
        
        // Event handler when a panel is clicked
        private void Panel_Click(object sender, EventArgs e)
        {   
            if(_currentPieceClicked != null)
                GameController.ClearLegalMoves(_currentPieceClicked);
            ResetPieceClicked();
            //MouseEventArgs mouseArgs = e as MouseEventArgs;

            //if (mouseArgs.Button != MouseButtons.Left)
            //    return;

            //if (_currentPieceClicked is null)
            //    return;

            
            ////MessageBox.Show("Target");

            //// Get the clicked panel
            //Panel myPanel = sender as Panel;           

            //if (!IsLegalMove(_currentPieceClicked, myPanel))
            //{
            //    ClearColor();
            //    _currentPieceClicked = null;
            //    return;
            //}
            //MovePiece(_currentPieceClicked, myPanel);

            //// Reset the selected ChessPieceControl
            //_currentPieceClicked = null;
            
        }

      
    }

}
