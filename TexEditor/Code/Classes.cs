using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using Microsoft.VisualBasic;
using ICSharpCode.AvalonEdit.Utils;
 
 
namespace Classes
{
    public class BibTex
    {
        public string UID;
        public string Text;
        public Dictionary<string, string> Description = new Dictionary<string, string>();
        public BibTex()
        {
        }
        public string this[string key]
        {
            get
            {
                if (!this.Description.ContainsKey(key))
                    return "";
                return this.Description[key];
            }
            
        }
        public static List<BibTex> Parse(string fileName)
        {
            List<BibTex> bibtex = new List<BibTex>();
            try
            {
               
                //string path = @"Z:\30\1\ref.bib";
                StringBuilder entry = new StringBuilder();
                List<string> source = new List<string>();
                Regex entryID = new Regex("@\\w+\\s*\\{\"*(?<ID>.*?)\"*,");
                Regex entryData = new Regex("([^=,]*)=(\"(?:\\\\.|[^\"\\\\]+)*?\"|[^,\"]*)");//"([^=,]*)=\\s*[\"\\|{](\"(?:\\\\.|[^\"\\\\]+)*\"|[^,\"]*)\\s*[\"|\\}]");
                string txt = "";
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {

                    using (StreamReader reader = FileReader.OpenStream(fs, new System.Text.UTF8Encoding(false)))
                    {
                        txt = reader.ReadToEnd();
                    }
                }
                txt = Regex.Replace(txt,"\r\n", " ");
                txt = Regex.Replace(txt, "\\s+", " ");
                                  
                string result = entryID.Replace(txt, delegate(Match m) {
                    return "\r\n"+ m.Value ;
                });

                result = entryData.Replace(result, delegate(Match m)
                {
                    return "XXOOXX" + m.Value;
                });

                foreach (string line in result.Split(new string[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries))
                 {
                     if (entryID.Match(line).Success)
                     {
                         source.Add(Regex.Replace(entry.ToString(), "\\s+", " "));
                         entry.Clear();
                         entry.Append(line + " ");
                     }
                     else
                     {
                         entry.Append(line + " ");
                     }
                 }

                 foreach (string s in source)
                 {
                     Match mId = entryID.Match(s);
                     if (mId.Success)
                     {
                         BibTex bibitem = new BibTex();
                         bibitem.UID = mId.Groups[1].Captures[0].Value;
                         bibitem.Text = s;
                         string[] arr = s.Split(new string[] { "XXOOXX" }, StringSplitOptions.None);
                         foreach (string t in arr)
                         {
                             string[] tt = t.Split("=".ToCharArray());
                             if (tt.Length==2)
                                bibitem.Description.Add(tt[0].Trim(), tt[1]);
                         }
                         bibtex.Add(bibitem);
                     }
                 }
                 return bibtex;
             }
             catch (System.Exception ex)
             {
                 return bibtex;
             }
           
        }
    }
    public static class TexHelper
    {

        public static Dictionary<string, string[]> TexEntries()
        {
            Dictionary<string, string[]> entiries = new Dictionary<string, string[]>();
            string[] groups = new string[] { "Math", "Greek letters", "relations", "Functions" };
            string[] m = new string[] { @"\[ \]", @"$ $", @"$$ $$" };

            string[] oc = new string[]{
                @"\lbrace \rbrace",
                @"\lVert \rVert",
                @"\langle \rangle",
                @"\lceil \rceil",
                @"\floor \rfloor"};




            #region
            string[] g = new string[] { 
                    @"\Gamma",
                    @"\Delta",
                    @"\Lambda",
                    @"\Phi",
                    @"\Pi",
                    @"\Psi",
                    @"\Sigma",
                    @"\Theta",
                    @"\Upsilon",
                    @"\Xi",
                    @"\Omega",
                    @"\alpha",
                    @"\beta",
                    @"\gamma",
                    @"\delta",
                    @"\epsilon",
                    @"\zeta",
                    @"\eta",
                    @"\theta",
                    @"\iota",
                    @"\kappa",
                    @"\lambda",
                    @"\mu",
                    @"\nu",
                    @"\xi",
                    @"\pi",
                    @"\rho",
                    @"\sigma",
                    @"\tau",
                    @"\upsilon",
                    @"\phi",
                    @"\chi",
                    @"\psi",
                    @"\omega",
                    @"\digamma",
                    @"\varepsilon",
                    @"\varkappa",
                    @"\varphi",
                    @"\varpi",
                    @"\varrho",
                    @"\varsigma",
                    @"\vartheta" };
            #endregion

            #region
            string[] r = new string[] { 
                    @"\backepsilon",
                    @"\because",
                    @"\between",
                    @"\blacktriangleleft",
                    @"\blacktriangleright",
                    @"\bowtie",
                    @"\dashv",
                    @"\frown",
                    @"\in",
                    @"\mid",
                    @"\models",
                    @"\ni",
                    @"\nmid",
                    @"\notin",
                    @"\nparallel",
                    @"\nshortmid",
                    @"\nshortparallel",
                    @"\nsubseteq",
                    @"\nsubseteqq",
                    @"\nsupseteq",
                    @"\nsupseteqq",
                    @"\ntriangleleft",
                    @"\ntrianglelefteq",
                    @"\ntriangleright",
                    @"\ntrianglerighteq",
                    @"\nvdash",
                    @"\nVdash",
                    @"\nvDash",
                    @"\nVDash",
                    @"\parallel",
                    @"\perp",
                    @"\pitchfork",
                    @"\propto",
                    @"\shortmid",
                    @"\shortparallel",
                    @"\smallfrown",
                    @"\smallsmile",
                    @"\smile",
                    @"\sqsubset",
                    @"\sqsubseteq",
                    @"\sqsupset",
                    @"\sqsupseteq",
                    @"\subset",
                    @"\Subset",
                    @"\subseteq",
                    @"\subseteqq",
                    @"\subsetneq",
                    @"\subsetneqq",
                    @"\supset",
                    @"\Supset",
                    @"\supseteq",
                    @"\supseteqq",
                    @"\supsetneq",
                    @"\supsetneqq",
                    @"\therefore",
                    @"\trianglelefteq",
                    @"\trianglerighteq",
                    @"\varpropto",
                    @"\varsubsetneq",
                    @"\varsubsetneqq",
                    @"\varsupsetneq",
                    @"\varsupsetneqq",
                    @"\vartriangle",
                    @"\vartriangleleft",
                    @"\vartriangleright",
                    @"\vdash",
                    @"\Vdash",
                    @"\vDash",
                    @"\Vvdash",
            };
            #endregion

                                #region
                                string[] binary = new string[]{
                    @"\amalg",
                    @"\ast",
                    @"\barwedge",
                    @"\bigcirc",
                    @"\bigtriangledown",
                    @"\bigtriangleup",
                    @"\boxdot",
                    @"\boxminus",
                    @"\boxplus",
                    @"\boxtimes",
                    @"\bullet",
                    @"\cap",
                    @"\Cap",
                    @"\cdot",
                    @"\centerdot",
                    @"\circ",
                    @"\circledast",
                    @"\circledcirc",
                    @"\circleddash",
                    @"\cup",
                    @"\Cup",
                    @"\curlyvee",
                    @"\curlywedge",
                    @"\dagger",
                    @"\ddagger",
                    @"\diamond",
                    @"\div",
                    @"\divideontimes",
                    @"\dotplus",
                    @"\doublebarwedge",
                    @"\gtrdot",
                    @"\intercal",
                    @"\leftthreetimes",
                    @"\lessdot",
                    @"\ltimes",
                    @"\mp",
                    @"\odot",
                    @"\ominus",
                    @"\oplus",
                    @"\oslash",
                    @"\otimes",
                    @"\pm",
                    @"\rightthreetimes",
                    @"\rtimes",
                    @"\setminus",
                    @"\smallsetminus",
                    @"\sqcap",
                    @"\sqcup",
                    @"\star",
                    @"\times",
                    @"\triangleleft",
                    @"\triangleright",
                    @"\uplus",
                    @"\vee",
                    @"\veebar",
                    @"\wedge",
                    @"\wr"};
                                #endregion
                                #region
                                string[] variants = new string[]{
                    @"\approx",
                    @"\approxeq",
                    @"\asymp",
                    @"\backsim",
                    @"\backsimeq",
                    @"\bumpeq",
                    @"\Bumpeq",
                    @"\circeq",
                    @"\cong",
                    @"\curlyeqprec",
                    @"\curlyeqsucc",
                    @"\doteq",
                    @"\doteqdot",
                    @"\eqcirc",
                    @"\eqsim",
                    @"\eqslantgtr",
                    @"\eqslantless",
                    @"\equiv",
                    @"\fallingdotseq",
                    @"\geq",
                    @"\geqq",
                    @"\geqslant",
                    @"\gg",
                    @"\ggg",
                    @"\gnapprox",
                    @"\gneq",
                    @"\gneqq",
                    @"\gnsim",
                    @"\gtrapprox",
                    @"\gtreqless",
                    @"\gtreqqless",
                    @"\gtrless",
                    @"\gtrsim",
                    @"\gvertneqq",
                    @"\leq",
                    @"\leqq",
                    @"\leqslant",
                    @"\lessapprox",
                    @"\lesseqgtr",
                    @"\lesseqqgtr",
                    @"\lessgtr",
                    @"\lesssim",
                    @"\ll",
                    @"\lll",
                    @"\lnapprox",
                    @"\lneq",
                    @"\lneqq",
                    @"\lnsim",
                    @"\lvertneqq",
                    @"\ncong",
                    @"\neq",
                    @"\ngeq",
                    @"\ngeqq",
                    @"\ngeqslant",
                    @"\ngtr",
                    @"\nleq",
                    @"\nleqq",
                    @"\nleqslant",
                    @"\nless",
                    @"\nprec",
                    @"\npreceq",
                    @"\nsim",
                    @"\nsucc",
                    @"\nsucceq",
                    @"\prec",
                    @"\precapprox",
                    @"\preccurlyeq",
                    @"\preceq",
                    @"\precnapprox",
                    @"\precneqq",
                    @"\precnsim",
                    @"\precsim",
                    @"\risingdotseq",
                    @"\sim",
                    @"\simeq",
                    @"\succ",
                    @"\succapprox",
                    @"\succcurlyeq",
                    @"\succeq",
                    @"\succnapprox",
                    @"\succneqq",
                    @"\succnsim",
                    @"\succsim",
                    @"\thickapprox",
                    @"\thicksim",
                    @"\triangleq"};
                                #endregion
                                #region

                                string[] arrows = new string[]{
                    @"\circlearrowleft",
                    @"\circlearrowright",
                    @"\curvearrowleft",
                    @"\curvearrowright",
                    @"\downdownarrows",
                    @"\downharpoonleft",
                    @"\downharpoonright",
                    @"\hookleftarrow",
                    @"\hookrightarrow",
                    @"\leftarrow",
                    @"\Leftarrow",
                    @"\leftarrowtail",
                    @"\leftharpoondown",
                    @"\leftharpoonup",
                    @"\leftleftarrows",
                    @"\leftrightarrow",
                    @"\Leftrightarrow",
                    @"\leftrightarrows",
                    @"\leftrightharpoons",
                    @"\leftrightsquigarrow",
                    @"\Lleftarrow",
                    @"\longleftarrow",
                    @"\Longleftarrow",
                    @"\longleftrightarrow",
                    @"\Longleftrightarrow",
                    @"\longmapsto",
                    @"\longrightarrow",
                    @"\Longrightarrow",
                    @"\looparrowleft",
                    @"\looparrowright",
                    @"\Lsh",
                    @"\mapsto",
                    @"\multimap",
                    @"\nLeftarrow",
                    @"\nLeftrightarrow",
                    @"\nRightarrow",
                    @"\nearrow",
                    @"\nleftarrow",
                    @"\nleftrightarrow",
                    @"\nrightarrow",
                    @"\nwarrow",
                    @"\rightarrow",
                    @"\Rightarrow",
                    @"\rightarrowtail",
                    @"\rightharpoondown",
                    @"\rightharpoonup",
                    @"\rightleftarrows",
                    @"\rightleftharpoons",
                    @"\rightrightarrows",
                    @"\rightsquigarrow",
                    @"\Rrightarrow",
                    @"\Rsh",
                    @"\searrow",
                    @"\swarrow",
                    @"\twoheadleftarrow",
                    @"\twoheadrightarrow",
                    @"\upharpoonleft",
                    @"\upharpoonright",
                    @"\upuparrows"};
                                #endregion
                                #region
                                string[] functions = new string[]{
                    @"\arccos",
                    @"\arcsin",
                    @"\arctan",
                    @"\arg",
                    @"\cos",
                    @"\cosh",
                    @"\cot",
                    @"\coth",
                    @"\csc",
                    @"\deg",
                    @"\det",
                    @"\dim",
                    @"\exp",
                    @"\gcd",
                    @"\hom",
                    @"\inf",
                    @"\injlim",
                    @"\ker",
                    @"\lg",
                    @"\lim",
                    @"\liminf",
                    @"\limsup",
                    @"\ln",
                    @"\log",
                    @"\max",
                    @"\min",
                    @"\Pr",
                    @"\projlim",
                    @"\sec",
                    @"\sin",
                    @"\sinh",
                    @"\sup",
                    @"\tan",
                    @"\tanh",
                    @"\varinjlim",
                    @"\varprojlim",
                    @"\varliminf",
                    @"\varlimsup"};
                                #endregion
                                #region
                                string[] operators = new string[]{
                    @"\int",
                    @"\oint",
                    @"\bigcap",
                    @"\bigcup",
                    @"\bigodot",
                    @"\bigoplus",
                    @"\bigotimes",
                    @"\bigsqcup",
                    @"\biguplus",
                    @"\bigvee",
                    @"\bigwedge",
                    @"\coprod",
                    @"\prod",
                    @"\sum"};

                                #endregion
                                #region
                                string[] others = new string[] {
                    @"\angle",
                    @"\backprime",
                    @"\bigstar",
                    @"\blacklozenge",
                    @"\blacksquare",
                    @"\blacktriangle",
                    @"\blacktriangledown",
                    @"\bot",
                    @"\clubsuit",
                    @"\diagdown",
                    @"\diagup",
                    @"\diamondsuit",
                    @"\emptyset",
                    @"\exists",
                    @"\flat",
                    @"\forall",
                    @"\heartsuit",
                    @"\infty",
                    @"\lozenge",
                    @"\measuredangle",
                    @"\nabla",
                    @"\natural",
                    @"\neg",
                    @"\nexists",
                    @"\prime",
                    @"\sharp",
                    @"\spadesuit",
                    @"\sphericalangle",
                    @"\square p",
                    @"\surd",
                    @"\top",
                    @"\triangle",
                    @"\triangledown",
                    @"\varnothing"};

            #endregion
            entiries.Add("Math", m);
            entiries.Add("Open Close", oc);
            entiries.Add("Greek letters", g);
            entiries.Add("Relations", r);
            entiries.Add("Funtions", functions);
            entiries.Add("Arrows", arrows);

            entiries.Add("Operators", operators);
            entiries.Add("Binary Op", binary);
            entiries.Add("Variants", variants);
            entiries.Add("Others", others);
            return entiries;
        }

    }
    public static class Helper
    {
        public static string NormalizeArabic1(string txt)
        {
            string Harakat = "ًٌٍَُِْ";
            //string Alfs = "ÃÅÇÂ";
            for (int i = 0; i < Harakat.Length; i++)
            {
                //while (txt.IndexOf(Harakat[i]) > -1)
                txt = txt.Replace(Harakat[i].ToString(), "");
            }

            //for (int i = 0; i < Alfs.Length; i++)
            //{
            //    while (txt.IndexOf(theList[i]) > -1)
            //        txt = txt.Replace(theList[i].ToString(), rep_with);
            //}

            return txt;
        }


        public static int[] FindAll(string matchStr, string searchedStr, int startPos)
        {
            int foundPos = -1; // -1 represents not found.
            int count = 0;
            List<int> foundItems = new List<int>();
            do
            {
                foundPos = searchedStr.IndexOf(matchStr, startPos, StringComparison.Ordinal);
                if (foundPos > -1)
                {
                    startPos = foundPos + 1;
                    count++;
                    foundItems.Add(foundPos + 1);

                    //Console.WriteLine("Found item at position: " + foundPos.ToString());
                }
            } while (foundPos > -1 && startPos < searchedStr.Length);
            return ((int[])foundItems.ToArray());
        }
        public static object getEnum(Type t, object o)
        {
            try
            {
                foreach (FieldInfo fi in t.GetFields())
                    if (fi.Name == o.ToString())
                        return fi.GetValue(null);
                return null;
            }
            catch (System.Exception ex)
            {

                return null;
                //throw new Exception(string.Format("Can't convert {0} to {1}", Value, t.ToString()));
            }
        }
        public static System.Guid getGuid(object o)
        {
            System.Guid g = System.Guid.Empty;
            if (o == null)
                return g;
            try
            {
                return (System.Guid)System.Data.SqlTypes.SqlGuid.Parse(o.ToString());
            }
            catch (System.Exception Ex)
            {
                return g;
            }
        }
        public static int getInt(object o)
        {
            int IntegerValue;
            try
            {
                IntegerValue = System.Convert.ToInt32(o);
            }
            catch (System.Exception Ex)
            {

                return -1;
            }
            return IntegerValue;

        }
        public static double getDouble(object o)
        {
            double value;
            try
            {
                value = System.Convert.ToDouble(o);
            }
            catch (System.Exception Ex)
            {

                return -1;
            }
            return value;

        }
        public static decimal getDecimal(object o)
        {
            decimal value;
            try
            {
                value = System.Convert.ToDecimal(o);
            }
            catch (System.Exception Ex)
            {
                return 0;
            }
            return value;

        }
        public static bool getBoolean(object o)
        {
            try
            {
                return System.Convert.ToBoolean(o);
            }
            catch (System.Exception Ex)
            {
                return false;
            }
        }
        public static System.DateTime getDate(object o)
        {
            System.DateTime datetime = System.DateTime.MinValue;
            try
            {
                //CultureInfo culture = new CultureInfo("en-GB");
                return (System.DateTime)System.Convert.ToDateTime(o.ToString());//, culture);
            }
            catch (System.Exception Ex)
            {
                return datetime;
            }


        }
        public static System.Byte[] getBytes(object o)
        {
            try
            {
                if (o != null)
                    return (System.Byte[])o;
                return null;


            }
            catch (System.Exception Ex)
            {
                return null;
            }
        }
        public static string ConvertNullToEmptyString(object input)
        {
            return (input == null ? "" : input.ToString());
        }

    }
    public static class WorkHelper
    {

        public static int[] FindAll(string matchStr, string searchedStr, int startPos)
        {
            int foundPos = -1; // -1 represents not found.
            int count = 0;
            List<int> foundItems = new List<int>();
            do
            {
                foundPos = searchedStr.IndexOf(matchStr, startPos, StringComparison.Ordinal);
                if (foundPos > -1)
                {
                    startPos = foundPos + 1;
                    count++;
                    foundItems.Add(foundPos + 1);

                    //Console.WriteLine("Found item at position: " + foundPos.ToString());
                }
            } while (foundPos > -1 && startPos < searchedStr.Length);
            return ((int[])foundItems.ToArray());
        }
        public static List<DataTable> CloneTable(DataTable tableToClone, int countLimit)
        {
            List<DataTable> tables = new List<DataTable>();
            int count = 0;
            DataTable copyTable = null;
            foreach (DataRow dr in tableToClone.Rows)
            {
                if ((count++ % countLimit) == 0)
                {
                    copyTable = new DataTable();
                    // Clone the structure of the table.            
                    copyTable = tableToClone.Clone();
                    // Add the new DataTable to the list.            
                    tables.Add(copyTable);
                }
                // Import the current row.        
                copyTable.ImportRow(dr);

            }
            return tables;
        }
  
   
        public static object getEnum(Type t, object o)
        {
            try
            {
                foreach (FieldInfo fi in t.GetFields())
                    if (fi.Name == o.ToString())
                        return fi.GetValue(null);
                return null;
            }
            catch (System.Exception ex)
            {

                return null;
                //throw new Exception(string.Format("Can't convert {0} to {1}", Value, t.ToString()));
            }
        }
        public static System.Guid getGuid(object o)
        {
            System.Guid g = System.Guid.Empty;
            if (o == null)
                return g;
            try
            {
                return (System.Guid)System.Data.SqlTypes.SqlGuid.Parse(o.ToString());
            }
            catch (System.Exception Ex)
            {
                return g;
            }
        }
        public static int getInt(object o)
        {
            int IntegerValue;
            try
            {
                IntegerValue = System.Convert.ToInt32(o);
            }
            catch (System.Exception Ex)
            {

                return -1;
            }
            return IntegerValue;

        }
        public static double getDouble(object o)
        {
            double value;
            try
            {
                value = System.Convert.ToDouble(o);
                if (value < 0)
                {
                }
            }
            catch (System.Exception Ex)
            {

                return -1;
            }
            return value;

        }
        public static decimal getDecimal(object o)
        {
            decimal value;
            try
            {
                value = System.Convert.ToDecimal(o);
            }
            catch (System.Exception Ex)
            {
                return 0;
            }
            return value;

        }
        public static bool getBoolean(object o)
        {
            try
            {
                return System.Convert.ToBoolean(o);
            }
            catch (System.Exception Ex)
            {
                return false;
            }
        }
        public static System.DateTime getDate(object o)
        {
            System.DateTime datetime = System.DateTime.MinValue;
            try
            {
                //CultureInfo culture = new CultureInfo("en-GB");
                return (System.DateTime)System.Convert.ToDateTime(o.ToString());//, culture);
            }
            catch (System.Exception Ex)
            {
                return datetime;
            }
        }
        public static System.Byte[] getBytes(object o)
        {
            try
            {
                if (o != null)
                    return (System.Byte[])o;
                return null;


            }
            catch (System.Exception Ex)
            {
                return null;
            }
        }
        public static string ConvertNullToEmptyString(object input)
        {
            return (input == null ? "" : input.ToString());
        }
        public static string FormatDouble(double d)
        {
            return d.ToString("0.000##E+00");
        }
     }
     
    public class ListResult
    {
        public int StartPosition
        {
            get
            {
                return System.Math.Max(this.Index * this.PageSize, 0);
                if (this.Position > 0)
                {
                    int ss = Position;
                }
                return System.Math.Max(this.Index * this.PageSize - this.OverlapLength, 0);
            }
        }
        public int EndPosition
        {
            get
            {
                return System.Math.Max(((this.Index + 1) * this.PageSize) + this.OverlapLength, 0);
                if (this.Position > 0)
                {
                    int ss = Position;
                }
                return System.Math.Max((this.Index + 1) * this.PageSize, 0);
            }
        }
        public int Length = 0;
        public string Text = "";
        public string FileName = "";
        public int PageSize = 10000;
        public int OverlapLength = 120;
        public int Position = 0;
        public override string ToString() { return Text; }
        public int Index = 0;
        public string NameAndLength
        {
            get
            {
                return Length > 0 ? Text + " (" + Length + ") " : Text;
            }
        }
        //lr.Position = System.Math.Max(this.bufferIndex * this.PageSize - this.OverlapLength, 0);
        //lr.EndPosition = System.Math.Max((this.bufferIndex+1) * this.PageSize , 0);
        //this.LoadFile(lr); 

    }
    public class ChangeEventArgs : EventArgs
    {
        public enum ChangeCommandName
        {
            None,
            ShowResults,
            HideResults
        }
        public ChangeCommandName commandName = ChangeCommandName.None;
        public List<object> Parameters;
        public ChangeEventArgs()
        {
            this.Parameters = new List<object>();
        }
    }
    public class ArabicStringComparer : IComparer<String>
    {
        public int Compare(string y, string x)
        {
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("ar-SA");
            return String.Compare(y, x, ci, System.Globalization.CompareOptions.Ordinal);
            //return String.Compare(y, x, StringComparison.Ordinal);
        }
    }
    public class ArabicStringComparerRTL : IComparer<String>
    {
        public int Compare(string y, string x)
        {
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("ar-SA");


            string s = y;
            char[] a = s.ToCharArray();
            Array.Reverse(a);
            s = new string(a);

            string t = x;
            char[] b = t.ToCharArray();
            Array.Reverse(b);
            t = new string(b);

            return String.Compare(s, t, ci, System.Globalization.CompareOptions.Ordinal);
            //return String.Compare(y, x, StringComparison.Ordinal);
        }
    }
}