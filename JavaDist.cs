using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sbava
{
    [Serializable]
    class JavaDist
    {
        private String path = null;
        private String version = null;
        private String release = null;

        public JavaDist(String path, String version)
        {
            this.path = path;
            this.version = version;
        }

        public JavaDist(String pathFound) // generate from path
        {
            StreamReader streamReader = new StreamReader(pathFound + "\\release.");
            String line = streamReader.ReadLine();
            while (line != null)
            {
                this.release += (this.release == null ? "" : "\n") + line;
                String v = get("JAVA_VERSION", line);
                if (v != null)
                {
                    this.path = pathFound;
                    this.version = v;
                }
                line = streamReader.ReadLine();
            }
            streamReader.Close();
            if (this.version == null) throw new Exception("Cannot find 'release'");
        }

        private String get(String key, String line)
        {
            if (line == null) return null;
            int p = line.IndexOf('=');
            if (p <= 0) return null;
            String candidateKey = line.Substring(0, p);
            String value = line.Substring(p + 1);
            if (candidateKey.ToLower().Equals(key.ToLower()))
            {
                if (value.StartsWith("\"")) value = value.Substring(1);
                if (value.EndsWith("\"")) value = value.Substring(0, value.Length - 1);
                return value;
            }
            else return null;
        }

        public int getIntVersion()
        {
            if (version.StartsWith("1."))
            {
                return int.Parse(version.Substring(2, 1));
            }
            String v = version.Substring(0, version.IndexOf(".") );
            return int.Parse(v);
        }

        public String getVersion()
        {
            return version;
        }

        public String getPath()
        {
            return path;
        }

        public int compareVersion(JavaDist javaDist)
        {
            string[] tokens = { ".", "_" };
            string[] numbers = this.version.Split(tokens, StringSplitOptions.None);
            if (numbers.Length < 3 || numbers.Length > 4) throw new FormatException("cannot decode " + this.version);
            int shift = (numbers.Length == 4 ? 1 : 0);
            long v1 = int.Parse(numbers[2 + shift]) + (long)int.Parse(numbers[1 + shift]) * 1000L + (long)int.Parse(numbers[0 + shift]) * 1000000L;

            numbers = javaDist.version.Split(tokens, StringSplitOptions.None);
            if (numbers.Length < 3 || numbers.Length > 4) throw new FormatException("cannot decode " + javaDist.version);
            shift = (numbers.Length == 4 ? 1 : 0);
            long v2 = int.Parse(numbers[2 + shift]) + (long)int.Parse(numbers[1 + shift]) * 1000L + (long)int.Parse(numbers[0 + shift]) * 1000000L;
            if (v1 > v2) return 1;
            if (v1 < v2) return -1;
            return 0;
        }


        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            JavaDist dist = (JavaDist)obj;
            return (dist.getVersion().Equals(this.getVersion()) && dist.getPath().Equals(this.getPath()));
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            JavaDist dist = (JavaDist)this;
            return (dist.getVersion() + dist.getPath()).GetHashCode();
        }
    }
}
