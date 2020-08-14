using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.JsonCompare.CompareDeserializer;

namespace App
{
    class Model
    {
        private string[] keys;

        private string json1;
        private string json2;
        private List<CompareItem> result;

        private Action newResCallback;

        public void setKeys(string[] keys)
        {
            this.keys = keys;
            findDifferences();
        }

        public void setjson1(string json1)
        {
            this.json1 = json1;
            findDifferences();
        }

        public void setjson2(string json2)
        {
            this.json2 = json2;
            findDifferences();
        }

        public void findDifferences()
        {
            Task.Run(() =>
            {
                bool sucess = false;
                try
                {
                    string[] keysToUse = keys;
                    if (keysToUse == null) keysToUse = new string[0];

                    result = new JsonCompare.CompareJson().compareJson(json1, json2, keysToUse);
                    sucess = true;
                }
                catch (Exception e)
                {
                }
                if (sucess)
                    if (newResCallback != null)
                        RunOnUIThread.Run(() =>
                        {
                            newResCallback();
                        });
            });
        }

        public List<CompareItem> getLastresultValidResult()
        {
            return result;
        }

        public void subscribeToNewResults(Action callback)
        {
            newResCallback = callback;
        }

        public void subscribeToProcessingBegin()
        {

        }

    }
}
