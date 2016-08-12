using Swiss;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Internal
{
    public class Command
    {
        public string Text { get; set; }
        public string ID { get; set; }
        public int Line { get; set; }

        public override string ToString()
        {
            return String.Format("{0} - {1} - {2}", Text, ID, Line);
        }
    }

    public class MemoryCortex
    {
        public string PreviousAnswer { get; set; }
        public string PreviousCommand { get; set; }

        public List<Command> Commands { get; set; }

        public List<string> RecentCommands { get; set; }

        public MemoryCortex()
        {
            InitializeValues();
        }

        public void InitializeValues()
        {
            PreviousAnswer = string.Empty;
            PreviousCommand = string.Empty;

            RecentCommands = new List<string>();
            Commands = GatherCommands();
        }

        public void AddCommand(string command)
        {
            RecentCommands.Add(command);
            PreviousCommand = command;
        }

        public string CheckIfSeenBefore(string input)
        {
            var result = string.Empty;
            var match = Commands.FirstOrDefault(cmd => cmd.Text.EqualsIgnoreCase(input));

            if(match != null)
            {
                result = match.ID;
            }

            return result;
        }

        private List<Command> GatherCommands(bool includeMuscles = true)
        {
            List<Command> commands = new List<Command>();
            int num = 1;

            File.ReadAllLines(Core.FilePaths.Memory).ForEach((line) =>
            {
                string[] bits = line.Split('|');
                string phrase = bits[0].TrimPunctuation();
                string id = bits[1];

                if (!phrase.Equals("x") && !phrase.Equals("r"))
                {
                    commands.Add(new Command() { Text = phrase, ID = id, Line = num });
                }

                num++;
            });

            return commands;
        }
    }
}
