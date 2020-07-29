using System;
using System.Collections.Generic;
using System.Text;

namespace OctoClientWrapper.POCO
{
    public class GitConfigObject
    {
        public string octopusprojectname { get; set; }
        public string configtype { get; set; }
        public string repository { get; set; }
        public string configlocation { get; set; }
        public string sourceconfig { get; set; }
        public string transformconfig { get; set; }
        public string gitbranch { get; set; }

    }

    public class ConfigObjects
    {
        public List<GitConfigObject> GitConfigObjects { get; set; }

    }
}
