using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swiss;
using Ruby.Internal;

namespace Ruby.Movements
{
    internal abstract class Answer : Response
    {
        public Dictionary<string, string> Values { get; set; }

        public Dictionary<string, string[]> PrimarySpecs { get; set; }
        public Dictionary<string, string[]> SecondarySpecs { get; set; }

        public string Key { get; set; }
        public string Spec { get; set; }
        public string DefaultSpec { get; set; }
  
        public Answer() : base()
        {
            Spec = string.Empty;
            DefaultSpec = string.Empty;
            Key = string.Empty;

            Values = new Dictionary<string, string>();

            SetPrimarySpecs();
            SetSecondarySpecs();
        }

        public abstract void SetPrimarySpecs();
        public abstract void SetSecondarySpecs();
        public abstract void SetDefaultSpec();

        public abstract void GatherValues();

        public virtual bool Stretch() { return true; }
        public virtual void WarmUp() { return; }

        private void DetermineSpec()
        {
            string input = Core.External.UserInput;
            string designatedSpec = string.Empty;

            string primary = GetPrimarySpec(input);
            string secondary = GetSecondarySpec(input);

            if(primary != null && secondary != null)
            {
                designatedSpec = primary + "_" + secondary;
            }
            else if(primary != null && secondary == null)
            {
                designatedSpec = primary;
            }

            Spec = designatedSpec;
        }

        private string GetPrimarySpec(string input)
        {
            var spec = PrimarySpecs.Keys.FirstOrDefault(key => PrimarySpecs[key].Any(str => input.ContainsIgnoreCase(str)));
            return spec ?? DefaultSpec;
        }

        private string GetSecondarySpec(string input)
        {
            return SecondarySpecs.Keys.FirstOrDefault(key => SecondarySpecs[key].Any(str => input.ContainsIgnoreCase(str)));
        }

        public override string Execute()
        {
            Reset();
            WarmUp();

            DetermineSpec();
            GatherValues();

            if(Values == null)
            {
                return string.Format("It appears that {0} is unavailable right now", Name);
            }
            else
            {
                return GenerateOutput(Key, Spec, Values);
            }
        }

        protected void Complain()
        {
            Swiss.SpeechUtility.SpeakText("That response is not working right now", Core.Internal.isQuiet);
        }

        protected void Reset()
        {
            Spec = string.Empty;
            Values = new Dictionary<string, string>();
        }

        public override void GenerateRecognizedPhrases() 
        { 
        
        }

        protected string GenerateOutput(string key, string spec, Dictionary<string, string> values)
        {
            if (values.ContainsKey("Default"))
            {
                return values["Default"];
            }

            Phrase target = Core.Language.Vocabulary.FirstOrDefault(nd => nd.Key.Equals(key) && nd.Spec.Equals(spec));

            string output = "I am not sure how to respond to that sir";

            if (target != null)
            {
                output = target.GeneratePhrase(key, spec, values, Core.Internal.isQuiet);
            }

            return output;
        }
    }
}
