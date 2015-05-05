using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ArabicStructure;

namespace NaiveBayesClassifier
{

    public class NaiveBayes
    {
        public enum ClassificationResult
        {
            First = -1,
            Undetermined = 0,
            Second = 1
        }

        private List<string> spamMails;

        private List<string> notSpamMails;

        private int countSpamMails;

        private int countNotSpamMails;

        string fileName = "";

        string classes1FolderName = "";
        string classes2FolderName = "";

        public NaiveBayes(string classes1FolderName, string classes2FolderName)
        {
            //string[] listFileName = Directory.GetFiles(classesFolderName, "*.txt");
            //spamMails[0] = listFileName[0];
            //notSpamMails[0] = listFileName[1];
            
            spamMails = this.Read(classes1FolderName);//@"C:\Users\Administrator\Desktop\30\Maknaz\data miner\classes\phy\");
            notSpamMails = this.Read(classes2FolderName);//@"C:\Users\Administrator\Desktop\30\Maknaz\data miner\classes\s\");
            countSpamMails = spamMails.Count();
            countNotSpamMails = notSpamMails.Count();
            this.classes1FolderName = classes1FolderName;
            this.classes2FolderName = classes2FolderName;
        }

        public static Parser Parser;

        public ClassificationResult ClassifyFile(string fileName)
        {
            try
            {
                this.fileName = fileName;
                string inputText = File.ReadAllText(this.fileName, Encoding.GetEncoding(1256));
                return this.Classify(inputText);
            }
            catch (System.Exception ex)
            {
                return ClassificationResult.Undetermined;
            }
        }

        public ClassificationResult Classify (string text)
        {
            text= Regex.Replace(text.normaliseText(), @"\p{C}|\p{Z}|\p{P}|\p{S}|\d", " ");
            //NaiveBayes.Parser = new NaiveBayesClassifier.Parser(this.classesFolderName);

            Parser parser = new Parser(this.classes1FolderName);
            parser.Run();
            foreach(var spamMail in spamMails)
            {
                parser.Parse(spamMail);
            }
            var spamWords = parser.WordCounter;
            parser = new Parser(this.classes2FolderName);
            foreach (var notSpamMail in notSpamMails)
            {
                parser.Parse(notSpamMail);
            }
            var notSpamWords = parser.WordCounter;
            return CheckIf(text, countSpamMails, spamWords, countNotSpamMails, notSpamWords);
        }

        private ClassificationResult CheckIf (string text, int countSpamMails, Dictionary<string, int> spamWordList, int countNotSpamMails, Dictionary<string, int> notSpamWordList)
        {
            var stringArray = text.Split(' ');
            var sumQ = 0.0;
            var wordCounter = 0;
            foreach (var item in stringArray)
            {
                if ( item.Trim().Length <=2 ||string.IsNullOrEmpty(item.Trim()))
                    continue;
                var q = CalculateQ(item.ToLower(), countSpamMails, spamWordList, countNotSpamMails, notSpamWordList);
                sumQ += q;
                wordCounter++;
            }
            if (sumQ / wordCounter > 1  )
            {
                return ClassificationResult.First;
            }
            else if (sumQ / wordCounter < 1)
            {
                return ClassificationResult.Second;
            }
            else
            {
                return ClassificationResult.Undetermined;
            }
        }
        
        private double CalculateQ(string word, int countSpamMails, Dictionary<string,int> spamWordList, int countNotSpamMails, Dictionary<string,int> notSpamWordList)
        {
            try
            {
                double wordCountSpam = 1;
                if (spamWordList.ContainsKey(word))
                {
                    wordCountSpam = spamWordList[word];
                }
                double Ph1e = 0.5 * wordCountSpam / countSpamMails;
                double wordCountNotSpam = 1;
                if (notSpamWordList.ContainsKey(word))
                {
                    wordCountNotSpam = notSpamWordList[word];
                }
                double Ph2e = 0.5 * wordCountNotSpam / countNotSpamMails;
                double q = Ph1e / Ph2e;
                return q;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return 1.0;
           
        }
        
        public List<string> Read(string pathToFiles)
        {
            List<string> list = new List<string>();
            // DirectoryInfo currentdirectoryInfo =new DirectoryInfo(Environment.CurrentDirectory);
            // DirectoryInfo parent = currentdirectoryInfo.Parent.Parent.Parent;
            DirectoryInfo directoryInfo = new DirectoryInfo(pathToFiles);
            if (directoryInfo.Exists)
            {
                foreach (var file in directoryInfo.EnumerateFiles())
                {
                    var text = String.Empty;
                    using (FileStream filestream = file.OpenRead())
                    {
                        System.IO.StreamReader streamReader = new System.IO.StreamReader(filestream, Encoding.GetEncoding(1256));
                        text = streamReader.ReadToEnd();
                    }
                    list.Add(text);
                }
            }
            return list;
        }
    }

    public class Parser
    {

        string classesFolderName = "";
        
        private Dictionary<string, int> wordCounter = new Dictionary<string, int>();
        
        private SortedDictionary<string, string> Classes1 = new SortedDictionary<string, string>();

        private Dictionary<string, Dictionary<string, int>> Classes = new Dictionary<string, Dictionary<string, int>>();

        private string[] listFileName;

        public Parser(string classesFolderName)
        {
            this.classesFolderName = classesFolderName;
        }

        public Dictionary<string, int> WordCounter
        {
            get { return wordCounter; }
            set { wordCounter = value; }
        }
   
        public void Run()
        {
            listFileName = Directory.GetFiles(classesFolderName, "*.txt");
            foreach (string s in listFileName)
            {
                string cc = s.Split("\\".ToCharArray())[s.Split("\\".ToCharArray()).Length - 1];
                Classes1.Add(cc, "");
                foreach (string line in File.ReadAllLines(s, Encoding.GetEncoding(1256)))
                {
                    string li = line;
                    li = Regex.Replace(line.normaliseText(), @"\p{C}|\p{Z}|\p{P}|\p{S}|\d", " ");
                    string[] arr = li.Split(new string[] { " " }, StringSplitOptions.None);
                    foreach (string r in arr)
                    {
                        if (r.Length > 2 && !string.IsNullOrEmpty(r.Trim()))
                            Classes1[cc] += r.normaliseText() + ",";
                    }
                }
            }
        }
        
        public void Parse(string textToParse)
        {
            textToParse = Regex.Replace(textToParse.normaliseText(), @"\p{C}|\p{Z}|\p{P}|\p{S}|\d", " ");
            var stringArray = textToParse.Split(' ');
            foreach (var str in stringArray)
            {
                if (wordCounter.ContainsKey(str.ToLower()))
                {
                    wordCounter[str.ToLower()] += 1;
                }
                else
                {
                    wordCounter.Add(str.ToLower(), 1);
                }
            }
        }
    }

}
