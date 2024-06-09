using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace sbava
{
    class Program
    {
        private static readonly string[] dictionary = { "use", "u", "scan", "s", "default", "d", "list", "l", "choose", "ch",
            "newest", "n", "oldest", "o", "compile", "co", "verbose", "v", "which", "w" };

        private static bool verbose = false;
        private static bool compiler = false;
        private static string version = null;
        private static Javas distributions;

        public static void Main(string[] args)
        {
            string arg = "";
            bool startedArgPart = false;
            int startArgPart = -1;
            for (int c = 0; c < args.Length; c++)
            {
                string command = args[c];
                if (startedArgPart)
                {
                    arg = arg + " " + command;
                }
                else
                {
                    if (command.ToLower().Equals("verbose") || command.ToLower().Equals("v")) verbose = true; // verbose needs preview
                    if (!int.TryParse(command, out _) && !dictionary.Contains(command))
                    {
                        startedArgPart = true;
                        arg = command;
                        startArgPart = c;
                    }
                }
            }

            O.setVerbose(verbose);

            O.Verb("sbava " + Assembly.GetExecutingAssembly().GetName().Version);
            distributions = new Javas();

            JavaDist version = distributions.getDefault();

            // cycle through commands

            int cmdNumber = 0;
            if (startArgPart == -1) startArgPart = args.Length;

            while (cmdNumber < startArgPart)
            {
                string command = args[cmdNumber];

                if (IsValidVersion(command))
                {
                    version=setVersion(command);
                }
                else
                    switch (command.ToLower())
                    {
                        case "use":
                        case "u":
                            if (nextVersion(args, cmdNumber, startArgPart))
                            {
                                cmdNumber++;
                                version=setVersion(args[cmdNumber]);
                            }
                            else
                            {
                                O.Log("ERROR: you must specify a valid version for command 'use'");
                            }
                            break;
                        case "scan":
                        case "s":
                            bool wide = next("wide", "wide", args, cmdNumber, startArgPart);
                            if (wide) cmdNumber++;
                            distributions.Refresh(wide);
                            break;
                        case "default":
                        case "d":
                            break;
                        case "which":
                        case "w":

                            break;
                        case "list":
                        case "l":
                            break;
                        case "choose":
                        case "ch":
                            break;
                        case "compile":
                        case "co":
                            compiler = true;
                            break;
                    }
                cmdNumber++;
            }

            O.Log("Will launch\n"+version.getPath()+"\\java" + (compiler ? "c" : "") + " " + arg);

        }

        private static JavaDist setVersion(string v)
        {
            JavaDist result;
            Match m = Regex.Match(v, pattern);
            if (m.Success)
            {
                result = distributions.getFromVersion(m.Value);
                if (result == null)
                {
                    O.Log("ERROR: The version you specified (" + v + ") is not present");
                    Environment.Exit(1);
                }
            }

            int iVersion = 0;
            if (int.TryParse(v, out iVersion))
            {
                if ((iVersion > 0) && (iVersion < 99))
                {
                    result = distributions.getFromVersionNumber(iVersion);
                    if (result == null)
                    {
                        O.Log("ERROR: The version you specified (" + v + ") is not present");
                        Environment.Exit(1);
                    }
                    else return result;
                }
                else
                {
                    O.Log("ERROR: You must specify a valid version number.");
                    Environment.Exit(1);
                }
            }

            string vCleaned = v.Trim().ToLower();
            if (vCleaned.Equals("newest") || vCleaned.Equals("n"))
            {
                result = distributions.getNewest();
                return result;
            }
            if (vCleaned.Equals("oldest") || vCleaned.Equals("o"))
            {
                result = distributions.getOldest();
                return result;
            }
            O.Log("ERROR: You must specify a valid version number.");
            Environment.Exit(1);
            return null;
        }

        private static bool next(string s1, string s2, string[] args, int cmdN, int startArgPart)
        {
            if (cmdN >= (startArgPart - 1)) return false;
            if (args[cmdN + 1].ToLower().Equals(s1)) return true;
            if (args[cmdN + 1].ToLower().Equals(s2)) return true;
            return false;
        }

        private static bool nextVersion(string[] args, int cmdN, int startArgPart)
        {
            if (cmdN >= (startArgPart - 1)) return false;
            return IsValidVersion(args[cmdN + 1]);
        }


        private const string pattern = @"\d{1,3}[.]\d{1,3}[.]\d{1,3}([_]\d{1,4})?";
        private static bool IsValidVersion(string s)
        {
            Match m = Regex.Match(s, pattern);
            if (m.Success) return true;

            int iVersion = 0;
            if (int.TryParse(s, out iVersion))
            {
                if ((iVersion > 0) && (iVersion < 99)) return true;
            }

            if (s.ToLower().Equals("newest")) return true;
            if (s.ToLower().Equals("n")) return true;
            if (s.ToLower().Equals("oldest")) return true;
            if (s.ToLower().Equals("o")) return true;

            return false;
        }
    }
}
