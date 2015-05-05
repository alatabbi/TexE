using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Document;
using System.Windows.Media;
using System.Windows;
using ATABBI.TexE.Forms;

namespace ATABBI.TexE
{
    public partial class FindReplaceWindow : ToolWindow
    {


        public FindReplaceWindow()
        {
            InitializeComponent();            
        }

        private void PropertyWindow_Load(object sender, EventArgs e)
        {

        }
        public override void CallUpdateContent()
        {
            if (this.Visible && this.DockPanel.ActiveDocument != null)
                this.UpdateContent(this.DockPanel.ActiveDocument);
        }


        public override void UpdateContent(IDockContent doc)
        {
            if (!this.Visible)
                return;
            this.Textdoc = doc as TextDocument;
        }
        private void Find()
        {
            string s = this.cboFindF.Text;
            int ind = this.Textdoc.txtEditor.Text.IndexOf(s, this.Textdoc.txtEditor.TextArea.Caret.Offset);

            if (ind < 0)
            {
                ind = this.Textdoc.txtEditor.Text.IndexOf(s, 0);
            }
            if (ind < 0)
            {
                System.Windows.MessageBox.Show("Text not found");
                return;
            }
            this.Textdoc.txtEditor.TextArea.Caret.Offset = ind;
            this.Textdoc.txtEditor.Select(ind, s.Length);
            this.Textdoc.txtEditor.TextArea.Caret.BringCaretToView();
        }
        private void btnFindNext_Click(object sender, EventArgs e)
        {
            Find();
           
        }

        private void cboFindF_TextChanged(object sender, EventArgs e)
        {
            //Find();
        }

        private void cboFindF_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter))
            {
                this.Find();
            }
        }

        private void btnReplaceAll_Click(object sender, EventArgs e)
        {

        }

        private void Replace()
        {
            string s = this.cboFindR.Text;
            int ind = this.Textdoc.txtEditor.Text.IndexOf(s, this.Textdoc.txtEditor.TextArea.Caret.Offset);

            if (ind < 0)
            {
                ind = this.Textdoc.txtEditor.Text.IndexOf(s, 0);
            }
            if (ind < 0)
            {
                System.Windows.MessageBox.Show("Text not found");
                return;
            }
            this.Textdoc.txtEditor.TextArea.Caret.Offset = ind;
            this.Textdoc.txtEditor.Select(ind, s.Length);
            this.Textdoc.txtEditor.TextArea.Caret.BringCaretToView();
            this.Textdoc.txtEditor.Document.Replace(ind, s.Length, this.cboReplace.Text);
            
        }

        private void btnReplaceNext_Click(object sender, EventArgs e)
        {
                this.Replace();
        }
       


        

        private void btnFindAll_Click(object sender, EventArgs e)
        {
           
            this.Textdoc.txtEditor.TextArea.TextView.LineTransformers.Add(new ColorizKeywordEdit(this.Textdoc.txtEditor, this.cboFindF.Text));
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            int ind = -1;
            bool found = false;
            foreach (object tr in this.Textdoc.txtEditor.TextArea.TextView.LineTransformers)
            {
                ind++;
                if (tr.GetType() == typeof(ColorizKeywordEdit))
                {
                    found = true;
                    break;
                }
            }   
            if (found )
                this.Textdoc.txtEditor.TextArea.TextView.LineTransformers.RemoveAt(ind);
        }
        

         
    }
}