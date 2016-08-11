using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using Ruby.Internal;
using System.Speech.Synthesis;
using System.Windows.Forms;

namespace Ruby.Mind
{
    public class LanguageCortex : Lobe
    {
        private PromptBuilder Prompt { get; set; }
        private SpeechSynthesizer Synth { get; set; }

        private bool IsFast { get; set; }

        public LanguageCortex() : base()
        {
            InitializeValues();
        }

        public override void InitializeValues()
        {
            Prompt = new PromptBuilder();
            Synth = new SpeechSynthesizer();

            Synth.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult);
        }

        public void Speak(string text, bool quiet)
        {
            if (quiet) { WhisperText(text); }
            else { SpeakText(text); }
        }

        private void SpeakText(string text)
        {
            if (!IsFast)
            {
                Synth.Rate += 1;
                IsFast = true;
            }

            Prompt.ClearContent();
            Prompt.AppendText(text);

            Synth.Speak(Prompt);
        }

        private void WhisperText(string text)
        {
            MessageBox.Show(text);
        }
    }
}
