namespace ATABBI.TexE
{
    partial class TextDocument
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextDocument));
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spellCheckToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.checkWordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemCheckTest = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unExtToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.cUserControl1 = new ICSharpCode.AvalonEdit.CControls.CUserControl();
            this.bindingNavigator1 = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabelFileName = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabelTracker = new System.Windows.Forms.ToolStripLabel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.spellCheckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dictionaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SpellCheckerTextBox = new TestingApplication.CustomTextBox();
            this.spellingWorker1 = new NHunspellComponent.Spelling.SpellingWorker();
            this.mainMenu.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigator1)).BeginInit();
            this.bindingNavigator1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolsToolStripMenuItem,
            this.menuItem1});
            this.mainMenu.Location = new System.Drawing.Point(5, 5);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(923, 24);
            this.mainMenu.TabIndex = 1;
            this.mainMenu.Visible = false;
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.spellCheckToolStripMenuItem1,
            this.checkWordToolStripMenuItem});
            this.toolsToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.toolsToolStripMenuItem.MergeIndex = 1;
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // spellCheckToolStripMenuItem1
            // 
            this.spellCheckToolStripMenuItem1.Name = "spellCheckToolStripMenuItem1";
            this.spellCheckToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F7)));
            this.spellCheckToolStripMenuItem1.Size = new System.Drawing.Size(177, 22);
            this.spellCheckToolStripMenuItem1.Text = "Spell Check";
            this.spellCheckToolStripMenuItem1.Click += new System.EventHandler(this.spellCheckToolStripMenuItem1_Click);
            // 
            // checkWordToolStripMenuItem
            // 
            this.checkWordToolStripMenuItem.Name = "checkWordToolStripMenuItem";
            this.checkWordToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F7;
            this.checkWordToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.checkWordToolStripMenuItem.Text = "Check Word";
            this.checkWordToolStripMenuItem.Click += new System.EventHandler(this.checkWordToolStripMenuItem_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem2,
            this.menuItemCheckTest,
            this.searchToolStripMenuItem,
            this.extToolStripMenuItem,
            this.unExtToolStripMenuItem});
            this.menuItem1.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.menuItem1.MergeIndex = 2;
            this.menuItem1.Name = "menuItem1";
            this.menuItem1.Size = new System.Drawing.Size(89, 20);
            this.menuItem1.Text = "&MDI Document";
            // 
            // menuItem2
            // 
            this.menuItem2.Enabled = false;
            this.menuItem2.Name = "menuItem2";
            this.menuItem2.Size = new System.Drawing.Size(128, 22);
            this.menuItem2.Text = "Spell Check";
            // 
            // menuItemCheckTest
            // 
            this.menuItemCheckTest.Enabled = false;
            this.menuItemCheckTest.Name = "menuItemCheckTest";
            this.menuItemCheckTest.Size = new System.Drawing.Size(128, 22);
            this.menuItemCheckTest.Text = "Check Test";
            this.menuItemCheckTest.Click += new System.EventHandler(this.menuItemCheckTest_Click);
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.Enabled = false;
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.searchToolStripMenuItem.Text = "Search";
            // 
            // extToolStripMenuItem
            // 
            this.extToolStripMenuItem.Name = "extToolStripMenuItem";
            this.extToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.extToolStripMenuItem.Text = "Ext";
            // 
            // unExtToolStripMenuItem
            // 
            this.unExtToolStripMenuItem.Name = "unExtToolStripMenuItem";
            this.unExtToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.unExtToolStripMenuItem.Text = "unExt";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.elementHost1);
            this.panel1.Controls.Add(this.bindingNavigator1);
            this.panel1.Controls.Add(this.SpellCheckerTextBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(5, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(923, 496);
            this.panel1.TabIndex = 3;
            // 
            // elementHost1
            // 
            this.elementHost1.BackColor = System.Drawing.Color.MintCream;
            this.elementHost1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elementHost1.Location = new System.Drawing.Point(0, 0);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.elementHost1.Size = new System.Drawing.Size(923, 467);
            this.elementHost1.TabIndex = 5;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = this.cUserControl1;
            // 
            // bindingNavigator1
            // 
            this.bindingNavigator1.AddNewItem = null;
            this.bindingNavigator1.CanOverflow = false;
            this.bindingNavigator1.CountItem = this.bindingNavigatorCountItem;
            this.bindingNavigator1.DeleteItem = null;
            this.bindingNavigator1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bindingNavigator1.GripMargin = new System.Windows.Forms.Padding(10);
            this.bindingNavigator1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripProgressBar1,
            this.bindingNavigatorSeparator2,
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem,
            this.toolStripLabelFileName,
            this.toolStripSeparator1,
            this.toolStripLabelTracker});
            this.bindingNavigator1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.bindingNavigator1.Location = new System.Drawing.Point(0, 467);
            this.bindingNavigator1.Margin = new System.Windows.Forms.Padding(2);
            this.bindingNavigator1.MaximumSize = new System.Drawing.Size(0, 30);
            this.bindingNavigator1.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.bindingNavigator1.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.bindingNavigator1.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.bindingNavigator1.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.bindingNavigator1.Name = "bindingNavigator1";
            this.bindingNavigator1.Padding = new System.Windows.Forms.Padding(2);
            this.bindingNavigator1.PositionItem = this.bindingNavigatorPositionItem;
            this.bindingNavigator1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.bindingNavigator1.Size = new System.Drawing.Size(923, 29);
            this.bindingNavigator1.Stretch = true;
            this.bindingNavigator1.TabIndex = 4;
            this.bindingNavigator1.Text = "bindingNavigator1";
            // 
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(36, 22);
            this.bindingNavigatorCountItem.Text = "of {0}";
            this.bindingNavigatorCountItem.ToolTipText = "Total number of items";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(38, 22);
            this.toolStripLabel1.Text = "Ready";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 22);
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorMoveFirstItem
            // 
            this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstItem.Image")));
            this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            this.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveFirstItem.Text = "Move first";
            // 
            // bindingNavigatorMovePreviousItem
            // 
            this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousItem.Image")));
            this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            this.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMovePreviousItem.Text = "Move previous";
            // 
            // bindingNavigatorSeparator
            // 
            this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorPositionItem
            // 
            this.bindingNavigatorPositionItem.AccessibleName = "Position";
            this.bindingNavigatorPositionItem.AutoSize = false;
            this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(50, 21);
            this.bindingNavigatorPositionItem.Text = "0";
            this.bindingNavigatorPositionItem.ToolTipText = "Current position";
            // 
            // bindingNavigatorSeparator1
            // 
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorMoveNextItem
            // 
            this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextItem.Image")));
            this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            this.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveNextItem.Text = "Move next";
            // 
            // bindingNavigatorMoveLastItem
            // 
            this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastItem.Image")));
            this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            this.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveLastItem.Text = "Move last";
            // 
            // toolStripLabelFileName
            // 
            this.toolStripLabelFileName.Name = "toolStripLabelFileName";
            this.toolStripLabelFileName.Size = new System.Drawing.Size(0, 22);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabelTracker
            // 
            this.toolStripLabelTracker.AutoSize = false;
            this.toolStripLabelTracker.Name = "toolStripLabelTracker";
            this.toolStripLabelTracker.Size = new System.Drawing.Size(100, 22);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.spellCheckToolStripMenuItem,
            this.dictionaryToolStripMenuItem,
            this.selectAllToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(157, 70);
            // 
            // spellCheckToolStripMenuItem
            // 
            this.spellCheckToolStripMenuItem.Name = "spellCheckToolStripMenuItem";
            this.spellCheckToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.spellCheckToolStripMenuItem.Text = "Suggestions";
            // 
            // dictionaryToolStripMenuItem
            // 
            this.dictionaryToolStripMenuItem.Name = "dictionaryToolStripMenuItem";
            this.dictionaryToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.dictionaryToolStripMenuItem.Text = "Dictionary";
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.selectAllToolStripMenuItem.Text = "Select All";
            // 
            // SpellCheckerTextBox
            // 
            this.SpellCheckerTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SpellCheckerTextBox.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SpellCheckerTextBox.IsPassWordProtected = false;
            this.SpellCheckerTextBox.IsSpellingAutoEnabled = false;
            this.SpellCheckerTextBox.IsSpellingEnabled = true;
            this.SpellCheckerTextBox.Location = new System.Drawing.Point(896, 472);
            this.SpellCheckerTextBox.Name = "SpellCheckerTextBox";
            this.SpellCheckerTextBox.Size = new System.Drawing.Size(24, 21);
            this.SpellCheckerTextBox.TabIndex = 1000;
            // 
            // spellingWorker1
            // 
            this.spellingWorker1.Editor = this.SpellCheckerTextBox;
            this.spellingWorker1.IsEditorSpellingAutoEnabled = false;
            // 
            // TextDocument
            // 
            this.ClientSize = new System.Drawing.Size(933, 506);
            this.Controls.Add(this.mainMenu);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Simplified Arabic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mainMenu;
            this.Name = "TextDocument";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Load += new System.EventHandler(this.TextDoc_Load);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigator1)).EndInit();
            this.bindingNavigator1.ResumeLayout(false);
            this.bindingNavigator1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem menuItem1;
        private System.Windows.Forms.ToolStripMenuItem menuItem2;
        private System.Windows.Forms.ToolStripMenuItem menuItemCheckTest;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.BindingNavigator bindingNavigator1;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
        private System.Windows.Forms.ToolStripLabel toolStripLabelFileName;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabelTracker;
        private System.Windows.Forms.ToolStripMenuItem extToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unExtToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem spellCheckToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dictionaryToolStripMenuItem;
        private NHunspellComponent.Spelling.SpellingWorker spellingWorker1;
        private TestingApplication.CustomTextBox SpellCheckerTextBox;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem spellCheckToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem checkWordToolStripMenuItem;
        private ICSharpCode.AvalonEdit.CControls.CUserControl cUserControl;
        private ICSharpCode.AvalonEdit.CControls.CUserControl cUserControl1;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
         
    }
}