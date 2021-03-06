﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryConverter
{
    /// <summary>
    /// Helper class for toString overload.
    /// </summary>
    class StrokeList : List<StrokeConverter>
    {
        public override string ToString()
        {
            String output = "";
            foreach (var stroke in this)
            {
                output += stroke.ToString() + "/";
            }
            if (output.EndsWith("/"))
            {
                output = output.Substring(0, output.Length - 1);
            }
            return output;
        }
    }
    /// <summary>
    /// Converts from WinSteno to Plover stroke format
    /// </summary>
    class StrokeConverter
    {
        /// <summary>
        /// Plover left part of the stroke
        /// </summary>
        public List<char> LeftStroke { get; set; }
        /// <summary>
        /// Plover right part of the stroke
        /// </summary>
        public List<char> RightStroke { get; set; }
        
        public StrokeConverter()
        {
            this.LeftStroke = new List<char>();
            this.RightStroke = new List<char>();
        }
        /// <summary>
        /// WinSteno uses caps to differentiate left and right strokes and blackslash for as stroke separator.
        /// Also, language characters remapping will be applied.
        /// To get the idea: "ABCdef\Ae" -> { "ABC-dfz", "A-i" } 
        /// </summary>
        /// <param name="winStenoStroke">Winsteno stork</param>
        /// <returns>Translated strokes</returns>
        public static StrokeList Parse(string winStenoStroke)
        {
            StrokeList output = new StrokeList();
            CultureInfo ci = new CultureInfo("it-it");
            foreach (String singleStroke in winStenoStroke.Split('\\'))
            {
                StrokeConverter s = new StrokeConverter();
                if (!String.IsNullOrEmpty(singleStroke))
                {
                    foreach (char c in singleStroke.ToCharArray())
                    {

                        if (char.IsUpper(c) || c == '*')
                        {
                            s.LeftStroke.Add(char.ToUpper(c, ci)); //Remapper.RemapLeft(char.ToUpper(c, ci));
                            s.LeftStroke.OrderBy(_c => Remapper.ITALIAN_L.IndexOf(_c));
                        }
                        else
                        {
                            s.RightStroke.Add(c);  //Remapper.RemapRight(char.ToUpper(c, ci));
                            s.RightStroke.OrderBy(_c => Remapper.ITALIAN_R.IndexOf(_c));
                        }
                    }
                    output.Add(s);
                }
            }
            return output;
        }

        /// <summary>
        /// Formats to plover stroke
        /// </summary>
        /// <returns>String like ABC-ABC</returns>
        public override string ToString()
        {
            return String.Format("{0}{1}", 
                string.Join("",LeftStroke), 
                string.Join("",RightStroke)).Replace('$','#');
        }
    }
}
