namespace Tetris_Game_Template
{
    partial class ScoreScreen
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.unavaiableLabel = new System.Windows.Forms.Label();
            this.escapeLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // unavaiableLabel
            // 
            this.unavaiableLabel.AutoSize = true;
            this.unavaiableLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.unavaiableLabel.Location = new System.Drawing.Point(25, 73);
            this.unavaiableLabel.Name = "unavaiableLabel";
            this.unavaiableLabel.Size = new System.Drawing.Size(374, 17);
            this.unavaiableLabel.TabIndex = 0;
            this.unavaiableLabel.Text = "We are sorry, this feature it\'s currently unavaiable";
            // 
            // escapeLabel
            // 
            this.escapeLabel.AutoSize = true;
            this.escapeLabel.Location = new System.Drawing.Point(71, 125);
            this.escapeLabel.Name = "escapeLabel";
            this.escapeLabel.Size = new System.Drawing.Size(290, 17);
            this.escapeLabel.TabIndex = 1;
            this.escapeLabel.Text = "Press escape to go back to the menu screen";
            // 
            // ScoreScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SkyBlue;
            this.Controls.Add(this.escapeLabel);
            this.Controls.Add(this.unavaiableLabel);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "ScoreScreen";
            this.Size = new System.Drawing.Size(438, 325);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.ScoreScreen_PreviewKeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label unavaiableLabel;
        private System.Windows.Forms.Label escapeLabel;
    }
}
