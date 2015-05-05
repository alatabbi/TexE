using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.IO;
using Classes;

namespace ATABBI.TexE
{
    public partial class GoToWindow : ToolWindow
    {
        public GoToWindow()
        {
            InitializeComponent();
            //comboBox.SelectedIndex = 0;
        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
        
        private void PropertyWindow_Load(object sender, EventArgs e)
        {
            this.maskedTextBox1.Focus();
          
        }
        public override void CallUpdateContent()
        {
            if (this.Visible && this.DockPanel.ActiveDocument != null)
                this.UpdateContent(this.DockPanel.ActiveDocument);
        }

        public override void UpdateContent(IDockContent doc)
        {
            this.Textdoc = doc as TextDocument;
            if (this.Textdoc==null)
                return;
            this.label1.Text = string.Format("Line: 1 - {0}", this.Textdoc.txtEditor.Document.LineCount-1);
        }
        private void buttonGoTo_Click(object sender, EventArgs e)
        {
            GoTo();
        }
        private void GoTo()
        {
            int l = Helper.getInt(this.maskedTextBox1.Text);
            if (l > 0 && l <= this.Textdoc.txtEditor.Document.LineCount - 1)
                this.Textdoc.txtEditor.ScrollTo(l - 1, 0);
            this.DockState = WeifenLuo.WinFormsUI.Docking.DockState.Hidden;
        }

        private void maskedTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                GoTo();
        }

      
    }
}