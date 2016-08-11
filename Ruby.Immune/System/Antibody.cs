using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Immune
{
    [Serializable]
    internal class Antibody
    {
        public enum Type
        {
            None,
            Questioner,
            Adjective,
            Preposition,
            Verb,
            Noun
        };

        public string Text { get; set; }
        public string TaggedText { get; set; }

        public double Importance { get; set; }

        public Type POS { get; set; }

        public HashSet<string> Cells { get; set; }
        public HashSet<Antibody> Children { get; set; }

        public Antibody FirstChild { get { return Children.FirstOrDefault(); } }

        public Antibody(string taggedText, string cell = "")
        {
            string[] microBits = taggedText.Split('/');

            string word = microBits[0];
            string part = microBits[1];

            TaggedText = taggedText;
            Importance = 0;

            if (part.Contains("VB")) { POS = Type.Verb; }
            else if (part.Contains("NN")) { POS = Type.Noun; }
            else if (part.Contains("JJ")) { POS = Type.Adjective; }
            else if (part.StartsWith("W")) { POS = Type.Questioner; }
            else if (part.StartsWith("PRP") || part.StartsWith("IN")) { POS = Type.Preposition; }

            if(POS != null)
            {
                Text = word;
                Importance = Convert.ToInt16(POS) * .25;
            }

            if(word.Equals("par"))
            {
                Importance = .25;
            }

            Children = new HashSet<Antibody>();

            Cells = new HashSet<string>();
            Cells.Add(cell);
        }

        public double Strength()
        {
            return Importance / Cells.Count;
        }

        public bool HasChild(string str)
        {
            return Children.Any(child => child.Text.Equals(str));
        }

        public void AddCell(string cell)
        {
            Cells.Add(cell);
        }

        public void AddChild(Antibody n)
        {
            Antibody existing = Children.FirstOrDefault(child => child.TaggedText.Equals(n.TaggedText));

            if(existing != null)
            {
                Children.Remove(existing);
            }

            Children.Add(n);
        }

        public int GetNumberOffspring()
        {
            int offspring = 1;

            foreach (var child in Children)
            {
                offspring += child.GetNumberOffspring();
            }

            return offspring;
        }

        public void Merge(Antibody nd)
        {
            Cells.Add(nd.Cells.First());
        }

        public bool IsParent()
        {
            return Children.Count > 0;
        }

        public override int GetHashCode()
        {
            return Text.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Antibody nd = obj as Antibody;

            if (nd != null)
            {
                return nd.Text.Equals(this.Text);
            }

            return false;
        }
    }
}
