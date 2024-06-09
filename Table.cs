using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sbava
{
    class Table
    {
        private List<string> keys= new List<string>();
        private List<string> values = new List<string>();
        private int keyW = 0;

        public  Table()
        {

        }

        public void add(string key, string value)
        {
            if (key == null) key = "";
            if (value == null) value = "";
            if (key.Length > keyW) keyW = key.Length;
            keys.Add(key);
            values.Add(value);
        }

        public void render()
        {
            int conW = Console.WindowWidth-1;
            if (keyW > (conW / 2)) keyW = (conW / 2);

            int valW = conW - keyW - 2;

            for (int c=0; c<keys.Count;c++)
            {
                string k = adapt(keys[c],keyW);
                string v = adapt(values[c],valW);
                string[] ks = k.Split('\n');
                string[] vs = v.Split('\n');
                if (ks.Length > vs.Length) { for (int i = 0; i < (ks.Length - vs.Length); i++) v += "\n"; vs = v.Split('\n'); }
                if (ks.Length < vs.Length) { for (int i = 0; i < (vs.Length - ks.Length); i++) k += "\n"; ks = k.Split('\n'); }
                
                for (int i=0;i<ks.Length;i++)
                {
                    Console.Write(ks[i].PadRight(keyW).Substring(0, keyW)+" ");
                    Console.WriteLine(vs[i]);
                }
            }


        }

        public string adapt(string s, int l)
        {
            string[] token = { "\\", "_", "-", " ", "+" };
            string[] tokenized = s.Split(token, StringSplitOptions.None); 
            int p = 0;
            for(int c=0; c<tokenized.Length;c++)
            {
                p = p + tokenized[c].Length;
                if (p<(s.Length))
                {
                    tokenized[c] += s[p];
                    p++;
                }                
            }

            string result = "";
            string lastline = "";
            foreach(string t in tokenized)
            {
                if ((lastline.Length+t.Length)<=l)
                {
                    lastline += t;
                } else
                {
                    result += lastline + "\n";
                    int resLen = t.Length;
                    string temp = t;
                    while (resLen>l)
                    {
                        result = result + temp.Substring(0, l)+"\n";
                        resLen -= l;
                        try
                        {
                            temp = temp.Substring(l);
                        } catch (ArgumentOutOfRangeException e)
                        {
                            temp = "";
                        }
                    }
                    lastline = temp;
                }
            }
            if (!lastline.Equals("") && !result.Equals("")) result += "\n" + lastline; else result = lastline;
            return result;
        }
    }
}
