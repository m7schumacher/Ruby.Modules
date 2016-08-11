using edu.stanford.nlp.ling;
using edu.stanford.nlp.tagger.maxent;
using java.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Swiss;
using Ruby.Internal;

namespace Ruby.Immune
{
    public class Extensions
    {
        private static string TAGGER_PATH;
        private static MaxentTagger tagger;

        public static void Initialize()
        {
            TAGGER_PATH = @"models\wsj-0-18-bidirectional-nodistsim.tagger";
            tagger = new MaxentTagger(TAGGER_PATH);
        }

        public static string TagPartsOfSpeech(string txt)
        {
            var sentences = MaxentTagger.tokenizeText(new java.io.StringReader(txt)).toArray();

            ArrayList phr = sentences.First() as ArrayList;
            var taggedSentence = tagger.tagSentence(phr);

            return Sentence.listToString(taggedSentence, false);
        }

        /// <summary>
        /// Method tags and breaks text into parts of speech
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string[] TagAndBreak(string textToBeTaggedAndBroken)
        {
            string[] macrobits = TagPartsOfSpeech(textToBeTaggedAndBroken).Split(' ');
            return macrobits;
        }

        public static string FilterUselessStrings(string phrase)
        {
            if (String.IsNullOrEmpty(phrase)) { return string.Empty; }

            string text = Regex.Replace(phrase, @"\?|!|\.", "");
            text = Regex.Replace(text, "\".*\"|'.*'", "");
            text = Regex.Replace(text, "'s|'", "");

            string[] bits = text.SplitOnWhiteSpace();

            string[] exiles = new string[] { "is", "the", "it" };

            return bits.Where(bit => !exiles.Contains(bit)).ToString(" ");
        }

        public static string[] FilterOnPOS(string[] bits)
        {
            List<string> valids = new List<string>();
            string[] valid = new string[] { "/WRB", "/NN", "/VB", "/JJ", "/RB", "/PRP", "/IN" };

            //return bits.Select(bit => bit.Split('/')[0])
            //    .Where(bit => !valid.Any(suf => bit.Contains(suf)))
            //    .ToArray();

            foreach (string str in bits)
            {
                var firstBit = str.Split('/')[0];
                bool invalid = !valid.Any(suf => str.Contains(suf));

                if (!invalid)
                {
                    valids.Add(str);
                }
            }

            return valids.ToArray();
        }

        public static void AppendLineToFile(string path, string line)
        {
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(line);
            }
        }

        public static void WriteInputToMemory(string id, string input, string spec)
        {
            string output = input + "|" + id;
            output = spec.Length > 0 ? output + "|" + spec : output;

            AppendLineToFile(Core.FilePaths.Memory, output);
        }

        public static string[] StripOfPOS(string[] bits)
        {
            return bits.Select((s) => s.Split('/')[0]).ToArray();
        }
    }
}
