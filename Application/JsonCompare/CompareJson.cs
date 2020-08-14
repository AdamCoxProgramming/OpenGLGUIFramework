using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.JsonCompare.CompareDeserializer;

namespace App.JsonCompare
{
    class CompareJson
    {
        public List<CompareItem> compareJson(string json1, string json2, string [] keys)
        {
            string json = new JavaScriptRunner().runJavaScript(formatJavascript(json1, json2,keys));
            return new CompareDeserializer().deserialzeJsonCompareItem(json);
        }

        private string formatJavascript(string json1, string json2, string[] keys )
        {
            string res = "var obj1 = " + json1 + "\n var obj2 = " + json2 + "\n" + formatKeysString(keys) + "\n" + Properties.Resources.JSONCompare;
            return res;
        }

        private string formatKeysString(string [] keys)
        {
            string res = "var keys = [";

            foreach(string key in keys)
            {
                res += "'" + key + "',";
            }
            res += "]";
            return res;
        }
    }
}
