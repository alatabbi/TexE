namespace NHunspellComponent.Spelling
{
   partial class SpellingFormBasic
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
            this.label1 = new System.Windows.Forms.Label();
            this.textShowBox = new System.Windows.Forms.RichTextBox();
            this.bIgnoreOnce = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.bIgnoreAll = new System.Windows.Forms.Button();
            this.bAddToDictionary = new System.Windows.Forms.Button();
            this.bChange = new System.Windows.Forms.Button();
            this.bChangeAll = new System.Windows.Forms.Button();
            this.bAutoCorrect = new System.Windows.Forms.Button();
            this.bOptions = new System.Windows.Forms.Button();
            this.bUndo = new System.Windows.Forms.Button();
            this.bCancel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.suggestionBox = new System.Windows.Forms.ListBox();
            this.buttonNextLine = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Not in Dictionary:";
            // 
            // textShowBox
            // 
            this.textShowBox.Location = new System.Drawing.Point(15, 25);
            this.textShowBox.Name = "textShowBox";
            this.textShowBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.textShowBox.Size = new System.Drawing.Size(313, 91);
            this.textShowBox.TabIndex = 1;
            this.textShowBox.Text = "";
            // 
            // bIgnoreOnce
            // 
            this.bIgnoreOnce.Location = new System.Drawing.Point(334, 52);
            this.bIgnoreOnce.Name = "bIgnoreOnce";
            this.bIgnoreOnce.Size = new System.Drawing.Size(96, 23);
            this.bIgnoreOnce.TabIndex = 2;
            this.bIgnoreOnce.Text = "&Ignore Once";
            this.bIgnoreOnce.UseVisualStyleBackColor = true;
            this.bIgnoreOnce.Click += new System.EventHandler(this.bIgnoreOnce_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 119);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Suggestions:";
            // 
            // bIgnoreAll
            // 
            this.bIgnoreAll.Location = new System.Drawing.Point(334, 81);
            this.bIgnoreAll.Name = "bIgnoreAll";
            this.bIgnoreAll.Size = new System.Drawing.Size(96, 23);
            this.bIgnoreAll.TabIndex = 2;
            this.bIgnoreAll.Text = "I&gnore &All";
            this.bIgnoreAll.UseVisualStyleBackColor = true;
            this.bIgnoreAll.Click += new System.EventHandler(this.bIgnoreAll_Click);
            // 
            // bAddToDictionary
            // 
            this.bAddToDictionary.Location = new System.Drawing.Point(334, 110);
            this.bAddToDictionary.Name = "bAddToDictionary";
            this.bAddToDictionary.Size = new System.Drawing.Size(96, 23);
            this.bAddToDictionary.TabIndex = 2;
            this.bAddToDictionary.Text = "&Add to Dictionary";
            this.bAddToDictionary.UseVisualStyleBackColor = true;
            this.bAddToDictionary.Click += new System.EventHandler(this.bAddToDictionary_Click);
            // 
            // bChange
            // 
            this.bChange.Location = new System.Drawing.Point(334, 164);
            this.bChange.Name = "bChange";
            this.bChange.Size = new System.Drawing.Size(96, 23);
            this.bChange.TabIndex = 2;
            this.bChange.Text = "&Change";
            this.bChange.UseVisualStyleBackColor = true;
            this.bChange.Click += new System.EventHandler(this.bChange_Click);
            // 
            // bChangeAll
            // 
            this.bChangeAll.Location = new System.Drawing.Point(334, 193);
            this.bChangeAll.Name = "bChangeAll";
            this.bChangeAll.Size = new System.Drawing.Size(96, 23);
            this.bChangeAll.TabIndex = 2;
            this.bChangeAll.Text = "Change A&ll";
            this.bChangeAll.UseVisualStyleBackColor = true;
            this.bChangeAll.Click += new System.EventHandler(this.bChangeAll_Click);
            // 
            // bAutoCorrect
            // 
            this.bAutoCorrect.Location = new System.Drawing.Point(334, 222);
            this.bAutoCorrect.Name = "bAutoCorrect";
            this.bAutoCorrect.Size = new System.Drawing.Size(96, 23);
            this.bAutoCorrect.TabIndex = 2;
            this.bAutoCorrect.Text = "AutoCo&rrect";
            this.bAutoCorrect.UseVisualStyleBackColor = true;
            this.bAutoCorrect.Click += new System.EventHandler(this.bAutoCorrect_Click);
            // 
            // bOptions
            // 
            this.bOptions.Location = new System.Drawing.Point(12, 261);
            this.bOptions.Name = "bOptions";
            this.bOptions.Size = new System.Drawing.Size(81, 23);
            this.bOptions.TabIndex = 2;
            this.bOptions.Text = "&Options...";
            this.bOptions.UseVisualStyleBackColor = true;
            this.bOptions.Click += new System.EventHandler(this.bOptions_Click);
            // 
            // bUndo
            // 
            this.bUndo.Location = new System.Drawing.Point(99, 261);
            this.bUndo.Name = "bUndo";
            this.bUndo.Size = new System.Drawing.Size(81, 23);
            this.bUndo.TabIndex = 2;
            this.bUndo.Text = "Undo";
            this.bUndo.UseVisualStyleBackColor = true;
            this.bUndo.Click += new System.EventHandler(this.bUndo_Click);
            // 
            // bCancel
            // 
            this.bCancel.Location = new System.Drawing.Point(334, 261);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(96, 23);
            this.bCancel.TabIndex = 2;
            this.bCancel.Text = "Close";
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 225);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Dictionary language:";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(122, 225);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(206, 21);
            this.comboBox1.TabIndex = 5;
            // 
            // suggestionBox
            // 
            this.suggestionBox.FormattingEnabled = true;
            this.suggestionBox.Location = new System.Drawing.Point(15, 135);
            this.suggestionBox.Name = "suggestionBox";
            this.suggestionBox.ScrollAlwaysVisible = true;
            this.suggestionBox.Size = new System.Drawing.Size(313, 82);
            this.suggestionBox.TabIndex = 6;
            // 
            // buttonNextLine
            // 
            this.buttonNextLine.Location = new System.Drawing.Point(334, 23);
            this.buttonNextLine.Name = "buttonNextLine";
            this.buttonNextLine.Size = new System.Drawing.Size(96, 23);
            this.buttonNextLine.TabIndex = 7;
            this.buttonNextLine.Text = "Next Line";
            this.buttonNextLine.UseVisualStyleBackColor = true;
            this.buttonNextLine.Click += new System.EventHandler(this.buttonNextLine_Click);
            // 
            // SpellingFormBasic
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(442, 298);
            this.Controls.Add(this.buttonNextLine);
            this.Controls.Add(this.suggestionBox);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.bAutoCorrect);
            this.Controls.Add(this.bAddToDictionary);
            this.Controls.Add(this.bChangeAll);
            this.Controls.Add(this.bChange);
            this.Controls.Add(this.bIgnoreAll);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bUndo);
            this.Controls.Add(this.bOptions);
            this.Controls.Add(this.bIgnoreOnce);
            this.Controls.Add(this.textShowBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SpellingFormBasic";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Spelling";
            this.ResumeLayout(false);
            this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.RichTextBox textShowBox;
      private System.Windows.Forms.Button bIgnoreOnce;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Button bIgnoreAll;
      private System.Windows.Forms.Button bAddToDictionary;
      private System.Windows.Forms.Button bChange;
      private System.Windows.Forms.Button bChangeAll;
      private System.Windows.Forms.Button bAutoCorrect;
      private System.Windows.Forms.Button bOptions;
      private System.Windows.Forms.Button bUndo;
      private System.Windows.Forms.Button bCancel;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.ComboBox comboBox1;
      private System.Windows.Forms.ListBox suggestionBox;
      private System.Windows.Forms.Button buttonNextLine;
   }
}