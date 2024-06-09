using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sbava
{
    class SavedSettings : ApplicationSettingsBase
    {
        [DefaultSettingValue(null), UserScopedSetting]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public List<JavaDist> dists
        {
            get { return (List<JavaDist>)this["dists"]; }
            set { this["dists"] = value; }
        }

        [DefaultSettingValue(null), UserScopedSetting]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public JavaDist defaultDist
        {
            get { return (JavaDist)this["defaultDist"]; }
            set { this["defaultDist"] = value; }
        }

    }
}
