using System;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Text;
using System.Windows.Forms;

namespace SmartOCR
{
    /// <summary>
    /// Application configuration
    /// </summary>
    public class Configuration
    {
        public Point mainWindowLocation = new Point(100, 50);
        public Size mainWindowSize = new Size(800, 600);
        
        public bool histogramVisible = false;
        public bool statisticsVisible = false;
        public bool resultsVisible = false;
        public bool splitFileVisible = false;
        public bool almodwanaVisible = false;
        public bool documentEditorVisible = false;
        public bool showPart = true;
        public bool openInNewDoc = false;
        public bool rememberOnChange = false;
        public int PageSize = 100100;
        public int OverlapLenght = 1000; 

        // Constructor
        public Configuration( )
        {
        }

        // Save configureation to file
        public void Save( string fileName )
        {
            try
            {
                // open file
                FileStream fs = new FileStream(fileName, FileMode.Create);
                // create XML writer
                XmlTextWriter xmlOut = new XmlTextWriter(fs, Encoding.Unicode);

                // use indenting for readability
                xmlOut.Formatting = Formatting.Indented;

                // start document
                xmlOut.WriteStartDocument();
                xmlOut.WriteComment("SmartOCR configuration file");

                // main node
                xmlOut.WriteStartElement("SmartOCR");

                // position node
                xmlOut.WriteStartElement("Position");
                xmlOut.WriteAttributeString("x", mainWindowLocation.X.ToString());
                xmlOut.WriteAttributeString("y", mainWindowLocation.Y.ToString());
                xmlOut.WriteAttributeString("width", mainWindowSize.Width.ToString());
                xmlOut.WriteAttributeString("height", mainWindowSize.Height.ToString());
                xmlOut.WriteEndElement();

                // settings node
                xmlOut.WriteStartElement("Settings");
                xmlOut.WriteAttributeString("histogram", histogramVisible.ToString());
                xmlOut.WriteAttributeString("statistics", statisticsVisible.ToString());
                xmlOut.WriteAttributeString("results", resultsVisible.ToString());
                xmlOut.WriteAttributeString("splitFile", splitFileVisible.ToString());
                xmlOut.WriteAttributeString("documentEditor", documentEditorVisible.ToString());
                
                xmlOut.WriteAttributeString("newOnChange", openInNewDoc.ToString());
                xmlOut.WriteAttributeString("rememberOnChange", rememberOnChange.ToString());
                xmlOut.WriteAttributeString("showPart", showPart.ToString());


                xmlOut.WriteEndElement();
                xmlOut.WriteEndElement();

                // close file
                xmlOut.Close();
            }
            catch (System.Exception ex)
            {

            }
        }

        // Load configureation from file
        public bool Load( string fileName )
        {
            bool ret = false;

            // check file existance
            if ( File.Exists( fileName ) )
            {
                FileStream fs = null;
                XmlTextReader xmlIn = null;

                try
                {
                    // open file
                    fs = new FileStream( fileName, FileMode.Open );
                    // create XML reader
                    xmlIn = new XmlTextReader( fs );

                    xmlIn.WhitespaceHandling = WhitespaceHandling.None;
                    xmlIn.MoveToContent( );

                    // check for main node
                    if ( xmlIn.Name != "SmartOCR" )
                        throw new ApplicationException( "" );

                    // move to next node
                    xmlIn.Read( );
                    if ( xmlIn.NodeType == XmlNodeType.EndElement )
                        xmlIn.Read( );

                    // check for position node
                    if ( xmlIn.Name != "Position" )
                        throw new ApplicationException( "" );

                    // read main window position
                    int x = Convert.ToInt32( xmlIn.GetAttribute( "x" ) );
                    int y = Convert.ToInt32( xmlIn.GetAttribute( "y" ) );
                    int width = Convert.ToInt32( xmlIn.GetAttribute( "width" ) );
                    int height = Convert.ToInt32( xmlIn.GetAttribute( "height" ) );

                    mainWindowLocation = new Point( x, y );
                    mainWindowSize = new Size( width, height );

                    // move to next node
                    xmlIn.Read( );
                    if ( xmlIn.NodeType == XmlNodeType.EndElement )
                        xmlIn.Read( );

                    // check for position node
                    if ( xmlIn.Name != "Settings" )
                        throw new ApplicationException( "" );

                    histogramVisible = Convert.ToBoolean( xmlIn.GetAttribute( "histogram" ) );
                    statisticsVisible = Convert.ToBoolean( xmlIn.GetAttribute( "statistics" ) );
                    splitFileVisible = Convert.ToBoolean(xmlIn.GetAttribute("splitFile"));
                    documentEditorVisible = Convert.ToBoolean(xmlIn.GetAttribute("documentEditor"));
                    
                    resultsVisible = Convert.ToBoolean(xmlIn.GetAttribute("results"));
                    openInNewDoc = Convert.ToBoolean( xmlIn.GetAttribute( "newOnChange" ) );
                    rememberOnChange = Convert.ToBoolean( xmlIn.GetAttribute( "rememberOnChange" ) );
                    showPart = Convert.ToBoolean(xmlIn.GetAttribute("showPart"));


                    ret = true;
                }
                catch(System.Exception ex)
                {
                }
                finally
                {
                    if ( xmlIn != null )
                        xmlIn.Close( );
                }
            }
            return ret;
        }
    }
}
