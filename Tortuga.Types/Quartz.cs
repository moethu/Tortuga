using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Tortuga.Quartz
{
    public class Result
    {
        public int totalHits { get; set; }
        public List<Hit> hits { get; set; }
    }

    public class Query
    {
        public Result result;

        public Query(string searchstring, string url = "http://www.quartzproject.org")
        { 
            System.Net.WebClient client = new System.Net.WebClient();
            string data = client.DownloadString(url+"/query?query=" + searchstring);

            this.result = Newtonsoft.Json.JsonConvert.DeserializeObject<Result>(data);
        }
    }

    public class Hit
    {
        public string id { get; set; }
        public List<List<object>> relevance { get; set; }
        public double score { get; set; }
        public Document document { get; set; }
    }

    public class Document
    {
        public string CPID { get; set; }
        public string version { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public List<string> altNames { get; set; }
        public string description { get; set; }
        public List<string> components { get; set; }
        public List<string> impurities { get; set; }
        public List<string> uniformats { get; set; }
        public List<string> uni_tags { get; set; }
        public List<string> masterformats { get; set; }
        public List<string> mf_tags { get; set; }
        public string teaser { get; set; }

        public static Data LoadRootObject(string id)
        {
            string version = id.Substring(id.Length - 3, 1);
            string url = String.Format("http://www.quartzproject.org/products/{1}/{0}.json", new object[] { id, version });
            System.Net.WebClient client = new System.Net.WebClient();
            string data = client.DownloadString(url);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Data>(data);
        }

        private Data internalData;

        public Data data
        {
            get
            { 
                if (internalData == null)
                    this.internalData = LoadRootObject(this.id + "-" + this.version);
                              
                return internalData;
            }
        }
    }




    /// <summary>
    /// Entry Data
    /// </summary>



    public class LcaResults
    {
        public List<LcaData> cradleToGate { get; set; }
        public List<LcaData> inUse { get; set; }
        public List<LcaData> endOfLife { get; set; }
    }

    public class Environmental
    {
        public string systemBoundary { get; set; }
        public string foregroundDataSource { get; set; }
        public string backforegroundDataSource { get; set; }
        public string postConsumerRecycledContent { get; set; }
        public string referenceCountry { get; set; }
        public string referenceYear { get; set; }
        public string endOfLifeTreatment { get; set; }
        public LcaResults lcaResults { get; set; }
    }

    public class Uniformat
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Masterformat
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Source
    {
        public string name { get; set; }
        public string url { get; set; }
        public int id { get; set; }
    }

    public class Data
    {
        public string CPID { get; set; }
        public long timestamp { get; set; }
        public string version { get; set; }
        public Environmental environmental { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public List<string> altNames { get; set; }
        public List<Uniformat> uniformats { get; set; }
        public List<Masterformat> masterformats { get; set; }
        public string recordType { get; set; }
        public string description { get; set; }
        public List<Source> sources { get; set; }
    }

    public class LcaData
    {
        public string value;
        public string unit;
        public string name;
    }




}
