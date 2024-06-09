using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sbava
{
    class Javas
    {
        private List<JavaDist> distributions;
        private JavaDist defaultDist;
        private SavedSettings savedSettings=null;

        public Javas()
        {
            LoadFromSettings();
            if (distributions==null || distributions.Count==0 )
            {
                Refresh(false);
            }
            O.Verb("Loaded " + distributions.Count + " Java installations");
        }

        public JavaDist getOldest()
        {
            if (distributions==null || distributions.Count<=0) return null;
            JavaDist oldest = distributions[0];
            foreach (JavaDist dist in distributions)
            {
                if (oldest.compareVersion(dist) == 1) oldest = dist;
            }
            return oldest;
        }

        public JavaDist getNewest()
        {
            if (distributions == null || distributions.Count <= 0) return null;
            JavaDist newest = distributions[0];
            foreach (JavaDist dist in distributions)
            {
                if (newest.compareVersion(dist) == -1) newest = dist;
            }
            return newest;
        }

        public JavaDist getDefault()
        {
            return defaultDist;
        }

        public JavaDist getFromVersionNumber(int number)
        {
            if (distributions == null || distributions.Count <= 0) return null;
            foreach(JavaDist dist in distributions)
            {
                if (dist.getIntVersion() == number) return dist;
            }
            return null;
        }

        public JavaDist getFromVersion(string version)
        {
            if (distributions == null || distributions.Count <= 0) return null;
            foreach (JavaDist dist in distributions)
            {
                if (dist.getVersion().Equals(version)) return dist;
            }
            return null;
        }

        public void setDefault(JavaDist javaDist)
        {
            if (distributions!=null)
                if (distributions.Count>0)
                {
                    foreach (JavaDist dist in distributions)
                        if (javaDist.Equals(dist))
                        {
                            defaultDist = javaDist;
                            savedSettings.defaultDist = javaDist;
                            savedSettings.Save();
                        }
                }
        }

        private void LoadFromSettings()
        {
            if (savedSettings==null)
            {
                savedSettings = new SavedSettings();
                savedSettings.Reload();
            }
            distributions = savedSettings.dists;
            defaultDist = savedSettings.defaultDist;
            if (defaultDist == null)
                if (distributions != null && distributions.Count > 0)
                {
                    defaultDist = distributions[0];
                    savedSettings.defaultDist = defaultDist;
                    savedSettings.Save();
                }
        }

        public void Refresh(bool wide)
        {
            List<JavaDist> refreshed = new List<JavaDist>();
            O.Log("Refreshing distributions...");

            if (wide)
            {
                refreshed = ScanFolder("\\");
            } else
            {
                string programFiles = Environment.ExpandEnvironmentVariables("%ProgramW6432%");
                string programFilesX86 = Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%");
                refreshed = ScanFolder(programFiles);
                refreshed.AddRange(ScanFolder(programFilesX86));
            }

            distributions = refreshed;
            savedSettings.dists = refreshed;
            if (defaultDist == null)
                if (distributions != null && distributions.Count > 0)
                {
                    defaultDist = distributions[0];
                    savedSettings.defaultDist = defaultDist;
                }
            savedSettings.Save();
        }

        public void showList()
        {
            Table tab = new Table();
            foreach (JavaDist d in distributions)
            {
                tab.add(d.getVersion(), d.getPath());
            }
            tab.render();
        }

        private static List<JavaDist> ScanFolder(String folder)
        {
            List<JavaDist> result = new List<JavaDist>();
            try
            {
                List<String> paths = Directory.EnumerateDirectories(folder, "*.*").ToList();
                foreach (String folder2 in paths)
                {
                    //Console.WriteLine(folder2);
                    List<JavaDist> temp = ScanFolder(folder2);
                    if (temp!=null && temp.Count()>0) result.AddRange(temp);
                }
                List<String> files = Directory.EnumerateFiles(folder, "java.exe").ToList();
                foreach (String file in files)
                {
                    if (file.ToLower().EndsWith("\\bin\\java.exe"))
                    {
                        String path = file.Substring(0, file.Length - "\\bin\\java.exe".Length);
                        if (File.Exists(path + "\\release."))
                        {
                            JavaDist dist = null;
                            try
                            {
                                dist = new JavaDist(path);
                                result.Add(dist);
                                O.Verb("Added " + dist.getVersion());
                            }
                            catch (Exception)
                            {
                                // ignore
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            return result;
        }
    }
}
