namespace NHunspellComponent
{
   partial class TestForm
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestForm));
         this.bCkeckAll = new System.Windows.Forms.Button();
         this.chAutoSpelling = new System.Windows.Forms.CheckBox();
         this.customPaintRichText21 = new NHunspellComponent.CustomPaintRichText();
         this.customMaskedTextBox1 = new NHunspellComponent.CustomMaskedTextBox();
         this.SuspendLayout();
         // 
         // bCkeckAll
         // 
         this.bCkeckAll.Location = new System.Drawing.Point(12, 12);
         this.bCkeckAll.Name = "bCkeckAll";
         this.bCkeckAll.Size = new System.Drawing.Size(75, 23);
         this.bCkeckAll.TabIndex = 9;
         this.bCkeckAll.Text = "Check All";
         this.bCkeckAll.UseVisualStyleBackColor = true;
         this.bCkeckAll.Click += new System.EventHandler(this.bCkeckAll_Click);
         // 
         // chAutoSpelling
         // 
         this.chAutoSpelling.AutoSize = true;
         this.chAutoSpelling.Checked = true;
         this.chAutoSpelling.CheckState = System.Windows.Forms.CheckState.Checked;
         this.chAutoSpelling.Location = new System.Drawing.Point(93, 16);
         this.chAutoSpelling.Name = "chAutoSpelling";
         this.chAutoSpelling.Size = new System.Drawing.Size(85, 17);
         this.chAutoSpelling.TabIndex = 10;
         this.chAutoSpelling.Text = "AutoSpelling";
         this.chAutoSpelling.UseVisualStyleBackColor = true;
         this.chAutoSpelling.CheckedChanged += new System.EventHandler(this.chAutoSpelling_CheckedChanged);
         // 
         // customPaintRichText21
         // 
         this.customPaintRichText21.IsPassWordProtected = false;
         this.customPaintRichText21.IsSpellingAutoEnabled = false;
         this.customPaintRichText21.IsSpellingEnabled = false;
         this.customPaintRichText21.Location = new System.Drawing.Point(12, 41);
         this.customPaintRichText21.Name = "customPaintRichText21";
         this.customPaintRichText21.Size = new System.Drawing.Size(574, 320);
         this.customPaintRichText21.TabIndex = 14;
         this.customPaintRichText21.Text = "1234567890";
         this.customPaintRichText21.UnderlinedSections = ((System.Collections.Generic.Dictionary<int, int>)(resources.GetObject("customPaintRichText21.UnderlinedSections")));
         // 
         // customMaskedTextBox1
         // 
         this.customMaskedTextBox1.IsPassWordProtected = false;
         this.customMaskedTextBox1.IsSpellingAutoEnabled = false;
         this.customMaskedTextBox1.IsSpellingEnabled = true;
         this.customMaskedTextBox1.Location = new System.Drawing.Point(184, 12);
         this.customMaskedTextBox1.Name = "customMaskedTextBox1";
         this.customMaskedTextBox1.Size = new System.Drawing.Size(402, 20);
         this.customMaskedTextBox1.TabIndex = 11;
         // 
         // TestForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(598, 373);
         this.Controls.Add(this.customPaintRichText21);
         this.Controls.Add(this.customMaskedTextBox1);
         this.Controls.Add(this.chAutoSpelling);
         this.Controls.Add(this.bCkeckAll);
         this.Name = "TestForm";
         this.Text = "Form1";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Button bCkeckAll;
      private System.Windows.Forms.CheckBox chAutoSpelling;
      private CustomMaskedTextBox customMaskedTextBox1;
      private CustomPaintRichText customPaintRichText21;
   }
}

