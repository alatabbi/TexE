namespace NHunspellComponent.Spelling
{
   partial class SpellingOptionsFormBasic
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
         this.chIgnoreWordsWithDigits = new System.Windows.Forms.CheckBox();
         this.chIgnoreWordsInUppercase = new System.Windows.Forms.CheckBox();
         this.chUseAutoCorrection = new System.Windows.Forms.CheckBox();
         this.bSave = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // chIgnoreWordsWithDigits
         // 
         this.chIgnoreWordsWithDigits.AutoSize = true;
         this.chIgnoreWordsWithDigits.Location = new System.Drawing.Point(12, 12);
         this.chIgnoreWordsWithDigits.Name = "chIgnoreWordsWithDigits";
         this.chIgnoreWordsWithDigits.Size = new System.Drawing.Size(136, 17);
         this.chIgnoreWordsWithDigits.TabIndex = 0;
         this.chIgnoreWordsWithDigits.Text = "Ignore words with digits";
         this.chIgnoreWordsWithDigits.UseVisualStyleBackColor = true;
         this.chIgnoreWordsWithDigits.CheckedChanged += new System.EventHandler(this.chIgnoreWordsWithDigits_CheckedChanged);
         // 
         // chIgnoreWordsInUppercase
         // 
         this.chIgnoreWordsInUppercase.AutoSize = true;
         this.chIgnoreWordsInUppercase.Location = new System.Drawing.Point(165, 12);
         this.chIgnoreWordsInUppercase.Name = "chIgnoreWordsInUppercase";
         this.chIgnoreWordsInUppercase.Size = new System.Drawing.Size(170, 17);
         this.chIgnoreWordsInUppercase.TabIndex = 0;
         this.chIgnoreWordsInUppercase.Text = "Ignore words un UPPERCASE";
         this.chIgnoreWordsInUppercase.UseVisualStyleBackColor = true;
         this.chIgnoreWordsInUppercase.CheckedChanged += new System.EventHandler(this.chIgnoreWordsInUppercase_CheckedChanged);
         // 
         // chUseAutoCorrection
         // 
         this.chUseAutoCorrection.AutoSize = true;
         this.chUseAutoCorrection.Location = new System.Drawing.Point(12, 35);
         this.chUseAutoCorrection.Name = "chUseAutoCorrection";
         this.chUseAutoCorrection.Size = new System.Drawing.Size(121, 17);
         this.chUseAutoCorrection.TabIndex = 0;
         this.chUseAutoCorrection.Text = "Use Auto-Correction";
         this.chUseAutoCorrection.UseVisualStyleBackColor = true;
         this.chUseAutoCorrection.CheckedChanged += new System.EventHandler(this.chUseAutoCorrection_CheckedChanged);
         // 
         // bSave
         // 
         this.bSave.Location = new System.Drawing.Point(12, 59);
         this.bSave.Name = "bSave";
         this.bSave.Size = new System.Drawing.Size(136, 23);
         this.bSave.TabIndex = 1;
         this.bSave.Text = "Save settings";
         this.bSave.UseVisualStyleBackColor = true;
         this.bSave.Click += new System.EventHandler(this.bSave_Click);
         // 
         // SpellingOptionsFormBasic
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(345, 94);
         this.Controls.Add(this.bSave);
         this.Controls.Add(this.chUseAutoCorrection);
         this.Controls.Add(this.chIgnoreWordsInUppercase);
         this.Controls.Add(this.chIgnoreWordsWithDigits);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
         this.Name = "SpellingOptionsFormBasic";
         this.ShowInTaskbar = false;
         this.Text = "Spelling Options";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.CheckBox chIgnoreWordsWithDigits;
      private System.Windows.Forms.CheckBox chIgnoreWordsInUppercase;
      private System.Windows.Forms.CheckBox chUseAutoCorrection;
      private System.Windows.Forms.Button bSave;
   }
}