using Ruby.Internal;
using Ruby.Muscle.Muscles.Muscles.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Ruby.Muscle
{
    internal class Calculator : Reflex
    {
        public string Function = "(((5+2)*(2-1))/((2+9)+((7-2)-1))*8)";
        public string Operators = "+-/*";

        public Calculator() : base()
        {
            Name = "Math";
            ID = "153";

            var chars = Function.ToCharArray();

            Symbol root = new Symbol(string.Empty);

            GenerateTree(root, chars, 0);
        }

        public void GenerateTree(Symbol root, char[] chars, int index)
        {
            Symbol next = new Symbol(string.Empty);
            Char current = chars[index];

            if(current == '(')
            {
                GenerateTree(next, chars, ++index);

                if (root.leftChild == null)
                    root.AddLeft(next);
                else if (root.rightChild == null)
                    root.AddRight(next);

                GenerateTree(root, chars, ++index);
            }
            else if(current == ')')
            {
                return;
            }
            else if(Operators.Contains(current))
            {
                root.Sym = current.ToString();
                GenerateTree(next, chars, ++index);

                if (root.leftChild == null)
                    root.AddLeft(next);
                else if (root.rightChild == null)
                    root.AddRight(next);
            }
            else
            {
                root.Sym = current.ToString();
                return;
            }
        }

        public override string Execute()
        {
            string input = Core.External.UserInput, spec = string.Empty, function = string.Empty, response = string.Empty;
            double total = 0, addition = 0;
            int adjustment = 0;
            bool found = false;

            char[] operators = new char[] { '+', '-', '*', '/' };
            string[] spaces;

            List<char> presentOps = new List<char>();
            List<double> nums = new List<double>();

            function = input.Contains("\"") ? input.Split('"')[1] : input;
            function = Regex.Replace(function, @"\s+", "");

            presentOps = function.ToCharArray().Where(ch => operators.Contains(ch)).ToList();
            spaces = Regex.Replace(function, @"[\+|\-|*|/]+", " ").Split(' ');

            foreach (var num in spaces)
            {
                nums.Add(Convert.ToDouble(num));
            }

            for (int i = 0; i < presentOps.Count(); i++)
            {
                if (presentOps.ElementAt(i).Equals('*'))
                {
                    addition = nums.ElementAt(i - adjustment) * nums.ElementAt((i - adjustment) + 1);
                    found = true;
                }
                else if (presentOps.ElementAt(i).Equals('/'))
                {
                    addition = nums.ElementAt(i - adjustment) / nums.ElementAt((i - adjustment) + 1);
                    found = true;
                }

                if (found)
                {
                    total = addition;
                    Prune(ref nums, ref adjustment, ref i, ref addition);
                    found = false;
                    adjustment++;
                }
            }

            if (nums.Count > 0)
            {
                adjustment = 0;

                for (int i = 0; i < presentOps.Count(); i++)
                {
                    if (presentOps.ElementAt(i).Equals('+'))
                    {
                        addition = nums.ElementAt(i - adjustment) + nums.ElementAt((i - adjustment) + 1);
                        found = true;
                    }
                    else if (presentOps.ElementAt(i).Equals('-'))
                    {
                        addition = nums.ElementAt(i - adjustment) - nums.ElementAt((i - adjustment) + 1);
                        found = true;
                    }

                    if (found)
                    {
                        total = addition;
                        Prune(ref nums, ref adjustment, ref i, ref addition);
                        found = false;
                    }

                    adjustment++;
                }
            }

            double tot = total;

            return String.Format("--{0} = {1}", Core.External.UserInput, total.ToString());
        }

        private void Prune(ref List<double> nums, ref int adjustment, ref int i, ref double addition)
        {
            nums.RemoveAt(i - adjustment);

            if (nums.Count > 0)
            {
                nums.RemoveAt(i - adjustment);
                nums.Insert(i - adjustment, addition);
            }
        }
    }
}
