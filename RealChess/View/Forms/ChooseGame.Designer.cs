namespace RealChess.View.Forms
{
    partial class ChooseGame
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
            this.NormalLabel = new System.Windows.Forms.Label();
            this.RealLabel = new System.Windows.Forms.Label();
            this.CloseButton = new System.Windows.Forms.PictureBox();
            this.normalChess = new System.Windows.Forms.PictureBox();
            this.realChess = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.CloseButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.normalChess)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.realChess)).BeginInit();
            this.SuspendLayout();
            // 
            // NormalLabel
            // 
            this.NormalLabel.AutoSize = true;
            this.NormalLabel.Font = new System.Drawing.Font("Rockwell", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NormalLabel.Location = new System.Drawing.Point(37, 9);
            this.NormalLabel.Name = "NormalLabel";
            this.NormalLabel.Size = new System.Drawing.Size(94, 27);
            this.NormalLabel.TabIndex = 2;
            this.NormalLabel.Text = "Normal";
            // 
            // RealLabel
            // 
            this.RealLabel.AutoSize = true;
            this.RealLabel.Font = new System.Drawing.Font("Rockwell", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RealLabel.Location = new System.Drawing.Point(297, 9);
            this.RealLabel.Name = "RealLabel";
            this.RealLabel.Size = new System.Drawing.Size(61, 27);
            this.RealLabel.TabIndex = 3;
            this.RealLabel.Text = "Real";
            // 
            // CloseButton
            // 
            this.CloseButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CloseButton.Image = global::RealChess.Properties.Resources.CloseIcon;
            this.CloseButton.Location = new System.Drawing.Point(388, -1);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(27, 27);
            this.CloseButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.CloseButton.TabIndex = 4;
            this.CloseButton.TabStop = false;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // normalChess
            // 
            this.normalChess.Cursor = System.Windows.Forms.Cursors.Hand;
            this.normalChess.Image = global::RealChess.Properties.Resources.KingLogo;
            this.normalChess.Location = new System.Drawing.Point(12, 44);
            this.normalChess.Name = "normalChess";
            this.normalChess.Size = new System.Drawing.Size(153, 153);
            this.normalChess.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.normalChess.TabIndex = 1;
            this.normalChess.TabStop = false;
            this.normalChess.Click += new System.EventHandler(this.normalChess_Click);
            // 
            // realChess
            // 
            this.realChess.Cursor = System.Windows.Forms.Cursors.Hand;
            this.realChess.Image = global::RealChess.Properties.Resources.RealLogo;
            this.realChess.Location = new System.Drawing.Point(250, 44);
            this.realChess.Name = "realChess";
            this.realChess.Size = new System.Drawing.Size(153, 153);
            this.realChess.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.realChess.TabIndex = 0;
            this.realChess.TabStop = false;
            this.realChess.Click += new System.EventHandler(this.realChess_Click);
            // 
            // ChooseGame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(415, 209);
            this.ControlBox = false;
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.RealLabel);
            this.Controls.Add(this.NormalLabel);
            this.Controls.Add(this.normalChess);
            this.Controls.Add(this.realChess);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ChooseGame";
            this.Text = "ChooseGame";
            ((System.ComponentModel.ISupportInitialize)(this.CloseButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.normalChess)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.realChess)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox realChess;
        private System.Windows.Forms.PictureBox normalChess;
        private System.Windows.Forms.Label NormalLabel;
        private System.Windows.Forms.Label RealLabel;
        private System.Windows.Forms.PictureBox CloseButton;
    }
}