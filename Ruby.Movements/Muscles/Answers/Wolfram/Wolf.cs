using Ruby.Internal;
using Swiss.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using WolframAlphaNET;
using WolframAlphaNET.Objects;

namespace Ruby.Movements
{
    internal class Wolf : Reflex
    {
        WolframAlpha wolf;

        public Wolf() : base()
        {
            Name = "Wolfram";
            ID = "169";
        }

        public override void Initialize()
        {
            try
            {
                wolf = new WolframAlpha(Core.FilePaths.WolframKey);
            }
            catch(Exception e)
            {
                Disable();
            }
        }

        public override string Execute()
        {
            return GetAnswer();
        }

        public string GetAnswer()
        {
            string input = Core.External.UserInput;
            string result = string.Empty;

            string url = CreateAlphaURL(input);
            string xml = InternetUtility.MakeWebRequest(url, 10000);

            if(xml != null)
            {
                string[] splitter = Regex.Split(xml, "<plaintext>|</plaintext>");

                if (splitter.Length >= 3)
                {
                    result = splitter[1].Replace("&quot;", "\"").Replace("&apos;", "'");

                    result = InterpretResult(result);
                }
            }

            return result;
        }

        //public static string MakeWebRequest(string url)
        //{
        //    string result = string.Empty;
        //    string line = string.Empty;

        //    WebRequest wrGETURL = WebRequest.Create(url);
        //    Stream objStream = wrGETURL.GetResponse().GetResponseStream();

        //    StreamReader objReader = new StreamReader(objStream);

        //    while (line != null)
        //    {
        //        line = objReader.ReadLine();
        //        result += line + "\n";
        //    }

        //    return result;
        //}

        public static string GetResultOfAlphaQuery(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xml);

            XmlNodeList nodes = doc.SelectNodes("queryresult/pod");

            var xxx = 0;

            return string.Empty;
        }

        public static string CreateAlphaURL(string search)
        {
            string firstChunk = "http://api.wolframalpha.com/v1/query?input=";
            string secondChunk = "&appid=PLTPVT-7HRT32UJQH&includepodid=Result";

            string url = String.Format(firstChunk + search + secondChunk);

            return url;
        }

        public static string InterpretResult(string result)
        {
            //Example -- 51.2 million people  (world rank: 24th)  (2014 estimate)

            string interpretation = result;

            List<string> tidbits = new List<string>();

            if (result.Contains("("))
            {
                string[] splitter = Regex.Split(result, @"\(|\)");

                for(int i = 1; i < splitter.Length; i++)
                {
                    string str = splitter[i];

                    if (str.Contains("rank"))
                    {
                        string[] microbits = str.Split(' ');

                        tidbits.Add(microbits[microbits.Length - 1] + " in the " + microbits[microbits.Length - 3]);
                    }
                    else if (str.Contains("estimate"))
                    {
                        string[] microbits = str.Split(' ');

                        tidbits.Add(" as of " + microbits[0]);
                    }
                }

                interpretation = splitter[0];

                if (tidbits.Count > 0)
                {
                    interpretation += "which is ";

                    foreach (string str in tidbits)
                    {
                        interpretation += str;
                    }
                }
            }

            return interpretation;
        }  
    }
}
