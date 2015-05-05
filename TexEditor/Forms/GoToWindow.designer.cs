namespace ATABBI.TexE
{
    partial class GoToWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GoToWindow));
            this.buttonGoTo = new System.Windows.Forms.Button();
            this.maskedTextBox1 = new System.Windows.Forms.MaskedTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonGoTo
            // 
            this.buttonGoTo.Location = new System.Drawing.Point(71, 44);
            this.buttonGoTo.Name = "buttonGoTo";
            this.buttonGoTo.Size = new System.Drawing.Size(75, 23);
            this.buttonGoTo.TabIndex = 0;
            this.buttonGoTo.Text = "Go To";
            this.buttonGoTo.UseVisualStyleBackColor = true;
            this.buttonGoTo.Click += new System.EventHandler(this.buttonGoTo_Click);
            // 
            // maskedTextBox1
            // 
            this.maskedTextBox1.Location = new System.Drawing.Point(15, 46);
            this.maskedTextBox1.Mask = "000000";
            this.maskedTextBox1.Name = "maskedTextBox1";
            this.maskedTextBox1.Size = new System.Drawing.Size(45, 20);
            this.maskedTextBox1.TabIndex = 1;
            this.maskedTextBox1.ValidatingType = typeof(int);
            this.maskedTextBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.maskedTextBox1_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Line: ";
            // 
            // GoToWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(157, 101);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.maskedTextBox1);
            this.Controls.Add(this.buttonGoTo);
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Float;
            this.HideOnClose = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GoToWindow";
            this.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Float;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TabText = "Properties";
            this.Text = "Go to Line";
            this.Load += new System.EventHandler(this.PropertyWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.Button buttonGoTo;
        private System.Windows.Forms.MaskedTextBox maskedTextBox1;
        private System.Windows.Forms.Label label1;

    }
}