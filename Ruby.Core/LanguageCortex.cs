using Swiss;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ruby.Internal
{
    public class Phrase
    {
        public string Key { get; set; }
        public string Spec { get; set; }
        public string Sentence { get; set; }

        public Phrase()
        {
            Key = string.Empty;
            Spec = string.Empty;
            Sentence = string.Empty;
        }

        public Phrase(string key) : this()
        {
            Key = key;
        }

        public Phrase(string key, string spec) : this(key)
        {
            Spec = spec;
        }

        public Phrase(string key, string spec, string sentence) : this(key, spec)
        {
            Sentence = sentence;
        }

        public string GeneratePhrase(string key, string spec, Dictionary<string, string> values, bool quiet)
        {
            string output = Sentence;
            string pattern = @"\[(.*?)\]";
            string value = string.Empty;

            Regex search = new Regex(pattern);
            MatchCollection matches = search.Matches(output);

            foreach (Match match in matches)
            {
                value = match.Value;

                var valueWithoutBrackets = value.TrimCharactersWithin(new char[] { '[', ']' });
                key = values.Keys.First(k => valueWithoutBrackets.Equals(k));

                output = output.Replace(value, values[key]);
            }

            return output;
        }
    }

    public class LanguageCortex
    {
        public List<string> Targets { get; set; }
        public List<string> Keywords { get; set; }
        public List<string> Phrases { get; set; }

        public List<Phrase> Vocabulary { get; set; }

        public LanguageCortex() : base()
        {
            InitializeValues();
        }

        public void InitializeValues()
        {
            Targets = new List<string>();
            Keywords = new List<string>();
            Phrases = new List<string>();

            Vocabulary = GatherVocabulary(Core.FilePaths.Vocabulary);
        }

        private static List<Phrase> GatherVocabulary(string vocabPath)
        {
            List<Phrase> phrases = new List<Phrase>();
            string key = string.Empty;
            string spec = string.Empty;

            string[] lines = File.ReadAllLines(vocabPath);
            string[] splitter;

            foreach (string line in lines)
            {
                splitter = line.Split('|');

                key = splitter[0];
                spec = splitter[1];

                Phrase phrase = new Phrase(splitter[0], splitter[1], splitter[2]);
                phrases.Add(phrase);
            }

            return phrases;
        }
    }
}
