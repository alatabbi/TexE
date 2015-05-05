using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHunspell;

namespace ATABBI.TexE.Code
{
    public class SpellingEngine
    {
        private static NetSpell.SpellChecker.Spelling   spell;
        private static NetSpell.SpellChecker.Dictionary.WordDictionary dic ;
        public static NetSpell.SpellChecker.Spelling SpellChecker1
        {
            get
            {
                if (spell == null)
                {
                    spell = new NetSpell.SpellChecker.Spelling();
                    dic = new NetSpell.SpellChecker.Dictionary.WordDictionary();
                    spell.Dictionary = dic;
                }
                return spell; 
            }
        }

        private static Hunspell hunspell; 

         public static Hunspell Hunspeller
        {
            get
            {
                if (hunspell == null)
                {
                    hunspell = new Hunspell("en_GB.aff", "en_GB.dic");
                }
                return hunspell; 
            }
 
        }

       
         
    }
}
