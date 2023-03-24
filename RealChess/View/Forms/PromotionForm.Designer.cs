namespace RealChess.View.Forms
{
    partial class PromotionForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PromotionForm));
            this.queenPic = new System.Windows.Forms.PictureBox();
            this.knightPic = new System.Windows.Forms.PictureBox();
            this.rookPic = new System.Windows.Forms.PictureBox();
            this.bishopPic = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.queenPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.knightPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rookPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bishopPic)).BeginInit();
            this.SuspendLayout();
            // 
            // queenPic
            // 
            this.queenPic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.queenPic.Cursor = System.Windows.Forms.Cursors.Default;
            this.queenPic.Image = global::RealChess.Properties.Resources.black_queen;
            this.queenPic.InitialImage = ((System.Drawing.Image)(resources.GetObject("queenPic.InitialImage")));
            this.queenPic.Location = new System.Drawing.Point(13, 9);
            this.queenPic.Margin = new System.Windows.Forms.Padding(0);
            this.queenPic.Name = "queenPic";
            this.queenPic.Size = new System.Drawing.Size(94, 94);
            this.queenPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.queenPic.TabIndex = 1;
            this.queenPic.TabStop = false;
            // 
            // knightPic
            // 
            this.knightPic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.knightPic.Cursor = System.Windows.Forms.Cursors.Default;
            this.knightPic.Image = global::RealChess.Properties.Resources.black_knight;
            this.knightPic.InitialImage = ((System.Drawing.Image)(resources.GetObject("knightPic.InitialImage")));
            this.knightPic.Location = new System.Drawing.Point(13, 114);
            this.knightPic.Margin = new System.Windows.Forms.Padding(0);
            this.knightPic.Name = "knightPic";
            this.knightPic.Size = new System.Drawing.Size(94, 92);
            this.knightPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.knightPic.TabIndex = 2;
            this.knightPic.TabStop = false;
            // 
            // rookPic
            // 
            this.rookPic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.rookPic.Cursor = System.Windows.Forms.Cursors.Default;
            this.rookPic.Image = global::RealChess.Properties.Resources.black_rook;
            this.rookPic.InitialImage = ((System.Drawing.Image)(resources.GetObject("rookPic.InitialImage")));
            this.rookPic.Location = new System.Drawing.Point(13, 219);
            this.rookPic.Margin = new System.Windows.Forms.Padding(0);
            this.rookPic.Name = "rookPic";
            this.rookPic.Size = new System.Drawing.Size(94, 91);
            this.rookPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.rookPic.TabIndex = 3;
            this.rookPic.TabStop = false;
            // 
            // bishopPic
            // 
            this.bishopPic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.bishopPic.Cursor = System.Windows.Forms.Cursors.Default;
            this.bishopPic.Image = global::RealChess.Properties.Resources.black_bishop;
            this.bishopPic.InitialImage = ((System.Drawing.Image)(resources.GetObject("bishopPic.InitialImage")));
            this.bishopPic.Location = new System.Drawing.Point(13, 323);
            this.bishopPic.Margin = new System.Windows.Forms.Padding(0);
            this.bishopPic.Name = "bishopPic";
            this.bishopPic.Size = new System.Drawing.Size(94, 91);
            this.bishopPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.bishopPic.TabIndex = 4;
            this.bishopPic.TabStop = false;
            // 
            // PromotionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(120, 422);
            this.Controls.Add(this.bishopPic);
            this.Controls.Add(this.rookPic);
            this.Controls.Add(this.knightPic);
            this.Controls.Add(this.queenPic);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PromotionForm";
            this.Text = "PromotionForm";
            ((System.ComponentModel.ISupportInitialize)(this.queenPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.knightPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rookPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bishopPic)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox queenPic;
        private System.Windows.Forms.PictureBox knightPic;
        private System.Windows.Forms.PictureBox rookPic;
        private System.Windows.Forms.PictureBox bishopPic;
    }
}