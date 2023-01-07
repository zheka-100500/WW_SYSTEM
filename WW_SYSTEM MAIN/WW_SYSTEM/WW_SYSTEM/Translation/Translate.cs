using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.Translation
{
    [Serializable]
    public class Translate
    {
        public string TranslateKey { get; private set; }
        public string TranslateValue { get; private set; }

        public Translate(string key, string value)
        {
            TranslateKey = key.ToUpper();
            TranslateValue = value;
        }
    }
}
