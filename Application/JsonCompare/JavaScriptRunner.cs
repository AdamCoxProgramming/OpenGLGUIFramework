using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App
{
    class JavaScriptRunner
    {

        public string runJavaScript(string code)
        {
            var engine = new Jurassic.ScriptEngine();
            return (string)engine.Evaluate(code);
        }
    }
}
