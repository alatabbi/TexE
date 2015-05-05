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
    public partial class PropertyWindow : ToolWindow
    {
        public PropertyWindow()
        {
            InitializeComponent();
            //comboBox.SelectedIndex = 0;
        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox.SelectedIndex != 0)
            {
                this.splitContainer1.Panel1Collapsed=true;
                this.splitContainer1.Panel2Collapsed = false;
            }
            else
            {
                this.splitContainer1.Panel2Collapsed = true;
                this.splitContainer1.Panel1Collapsed = false; 

            }
        }
        
        private void PropertyWindow_Load(object sender, EventArgs e)
        {
            comboBox.SelectedIndex = 1;
            comboBox.SelectedIndex = 0;
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
            propertyGrid.SelectedObject = this.Textdoc.txtEditor;
            applySettings();
        }

        private void propertyGrid_FontChanged(object sender, EventArgs e)
        {
        }

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (e.ChangedItem.PropertyDescriptor.DisplayName == "FontSize")
            {
                Textdoc.txtEditor.FontSize =  Helper.getDouble(e.ChangedItem.Value);
                //this.Textdoc.txtEditor.Font.Size= 
                Properties.Settings.Default.EditorFont = new Font(this.Textdoc.txtEditor.FontFamily.ToString(), (float) Helper.getDouble(e.ChangedItem.Value));
              
            }
            Properties.Settings.Default.Save();
            
        }

        private void applySettings()
        {
            if (ATABBI.TexE.Properties.Settings.Default.EditorFont != null)
                this.Textdoc.txtEditor.FontSize = ATABBI.TexE.Properties.Settings.Default.EditorFont.Size;
        }
      
    }
}