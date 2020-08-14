using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.JsonCompare
{
    public class CompareDeserializer
    {
        public class CompareItem
        {
            public string name { get; set; }
            public string type { get; set; }
            public string comparison { get; set; }
            public List<CompareItem> comparisonObject { get; set; }
            public string lValue { get; set; }
            public string rValue { get; set; }
        }

        public List<CompareItem> deserialzeJsonCompareItem(string json)
        {
            return  JsonConvert.DeserializeObject<List<CompareItem>>(json);
        }

    }
}
