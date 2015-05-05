using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using VS2005ToolBox;
using System.Net;
using System.IO;
using Classes;

namespace ATABBI.TexE
{
    public partial class Toolbox : ToolWindow
    {
        public Toolbox()
        {
            InitializeComponent();
        }
        public override void UpdateContent(IDockContent doc)
        {
            base.UpdateContent(doc);
            if (!this.Visible)
                return;
            this.Textdoc = doc as TextDocument;
        }
        public override void CallUpdateContent()
        {
            if (this.Visible && this.DockPanel.ActiveDocument !=null)
                this.UpdateContent(this.DockPanel.ActiveDocument);
        }
       
        
        private void Toolbox_Load(object sender, EventArgs e)
        {
            if (!this.Visible)
                return;

            
             int i = 0;
             foreach (KeyValuePair<string, string[]> kvp in TexHelper.TexEntries())
             {
                 ToolBox.VSTreeNode newGroup = new ToolBox.VSTreeNode();
                 newGroup.Text = String.Format(kvp.Key);
                 toolBox1.Nodes.Add(newGroup);
                 if (toolBox1.SelectedNode == null)
                 {
                     toolBox1.SelectedNode = toolBox1.Nodes[i];
                 }
                 
                 foreach (string s in kvp.Value)
                 {
                     Random rndImage = new Random();
                     ToolBox.VSTreeNode newSubItem = new ToolBox.VSTreeNode();
                     newSubItem.Text = String.Format(s);
                     string x = s;
                     if (char.IsUpper(s.Substring(1, s.Length - 1)[0]))
                         x = s + "_U";
                     else
                        x = s + "_L";

                     if (this.imageList.Images.ContainsKey(x.Substring(1, x.Length - 1) + ".png"))
                         newSubItem.ImageIndex = imageList.Images.IndexOfKey(x.Substring(1, x.Length - 1).ToLower() + ".png");
                     else
                         //newSubItem.ImageIndex = imageList.Images.IndexOfKey(s.Substring(1, x.Length - 1).ToLower() + ".png");
                         newSubItem.ImageIndex = 0;// rndImage.Next(imageList.Images.Count);
                     //if (char.IsUpper(s.Substring(1, s.Length - 1)[0]))
                     //    newSubItem.ImageIndex = 0;
                    newSubItem.ToolTipCaption = newSubItem.Text;
                    newSubItem.ToolTipText = s;
                    toolBox1.Nodes[i].Nodes.Add(newSubItem);
                    toolBox1.SelectedNode.Expand();

                    continue;
                    if (kvp.Key == "Open Close" || kvp.Key == "Math")
                    {
                        continue;
                    }
                    try
                    {
                        string url = "http://frog.isima.fr/cgi-bin/bruno/tex2png--20.cgi?" + @s;
                        HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);

                        using (HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse())
                        {
                            using (Stream stream = httpWebReponse.GetResponseStream())
                            {
                                if (char.IsUpper(s.Substring(1, s.Length - 1)[0]))  
                                    Image.FromStream(stream).Save(@"Z:\imgs" + s + "_U.png");
                                else
                                    Image.FromStream(stream).Save(@"Z:\imgs" + s + "_L.png");
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                    }


                 }
                 i++;
             }
        }

        private void toolBox1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                if (this.Textdoc != null)
                {
                    string s = e.Node.Text;
                    this.Textdoc.txtEditor.Document.Insert(this.Textdoc.txtEditor.TextArea.Caret.Offset, s);
                    this.Textdoc.txtEditor.TextArea.Caret.BringCaretToView();
                }
            }
            catch (System.Exception ex)
            {
            }
        }
        
    }
}