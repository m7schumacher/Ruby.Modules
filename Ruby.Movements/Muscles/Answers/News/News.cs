using System.Collections.Generic;

namespace Ruby.Movements
{
    internal class News : Answer
    {
        public News() : base()
        {
            Name = "News";
        }

        public override void SetPrimarySpecs()
        {
            PrimarySpecs = new Dictionary<string, string[]>();
        }

        public override void SetSecondarySpecs()
        {
            SecondarySpecs = new Dictionary<string, string[]>();
        }

        public override void SetDefaultSpec()
        {
            DefaultSpec = string.Empty;
        }

        public override void GatherValues() { }

        public ItemNews[] GetNewsContent(string NewsParameters)
        {
            //XmlDocument doc = new XmlDocument();  

            ////Loading rss on it
            //doc.Load("http://news.yahoo.com/rss/");

            ////Looping every item in the XML
            //foreach (XmlNode node in doc.SelectNodes("rss/channel/item"))
            //{
            //    //Reading Title which is simple
            //    string title = node.SelectSingleNode("title").InnerText;

            //    //Putting all description text in string ndd
            //    string ndd =  node.SelectSingleNode("description").InnerText;

            //    XmlDocument xm = new XmlDocument();

            //    //Loading modified string as XML in xm with the root <me>
            //    xm.LoadXml("<me>"+ndd+"</me>");

            //    //Selecting node <p> which has the text
            //    XmlNode nodds = xm.SelectSingleNode("/me/p");

            //   //Putting inner text in the string ShStory
            //    //string shortstory = nodds.InnerText;

            //   //Showing the message box with the loaded data
            //   var xx = 0;
            //}
            

            //List<ItemNews> Details = new List<ItemNews>();

            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://news.google.com/news?q=" + NewsParameters + "&output=rss");
            //request.Method = "GET";

            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    Stream receiveStream = response.GetResponseStream();
            //    StreamReader readStream = null;

            //    readStream = response.CharacterSet == "" ? new StreamReader(receiveStream) : new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
  
            //    string data = readStream.ReadToEnd();

            //    //Declare DataSet for putting data in it.
            //    DataSet ds = new DataSet();
            //    StringReader reader = new StringReader(data);
            //    ds.ReadXml(reader);
            //    DataTable dtGetNews = new DataTable();

            //    if (ds.Tables.Count > 3)
            //    {
            //        dtGetNews = ds.Tables["item"];

            //        foreach (DataRow dtRow in dtGetNews.Rows)
            //        {
            //            ItemNews DataObj = new ItemNews();
            //            DataObj.title = dtRow["title"].ToString();
            //            DataObj.link = dtRow["link"].ToString();
            //            DataObj.item_id = dtRow["item_id"].ToString();
            //            DataObj.PubDate = dtRow["pubDate"].ToString();
            //            DataObj.Description = dtRow["description"].ToString();
            //            Details.Add(DataObj);
            //        }
            //    }
            //}

            

            //The QueryResult object contains the parsed XML from Wolfram|Alpha. Lets look at it.
            //The results from wolfram is split into "pods". We just print them.
            //if (results != null)
            //{
            //    foreach (Pod pod in results.Pods)
            //    {
            //        if (pod.SubPods != null)
            //        {
            //            foreach (SubPod subPod in pod.SubPods)
            //            {

            //            }
            //        }
            //    }
            //}

            
            ItemNews[] news = new ItemNews[5];
            return news;
        }

        //Define Class to return news data
        internal class ItemNews
        {
            public string title { get; set; }
            public string link { get; set; }
            public string item_id { get; set; }
            public string PubDate { get; set; }
            public string Description { get; set; }
        }

        //public override void Execute()
        //{
        //    GetNewsContent("sports");
        //}
    }
}
