using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sbava
{
    static class O
    {
        private static bool verbose = false;

        public static void setVerbose(bool v) { verbose = v;  }

        public static void Log(string message)
        {
            Console.WriteLine(message);
        }

        public static void Verb(string message)
        {
            if (verbose) Console.WriteLine(message);
        }
    }
}
