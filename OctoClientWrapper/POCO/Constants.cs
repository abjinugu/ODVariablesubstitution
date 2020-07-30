using System;
using System.Collections.Generic;
using System.Text;

namespace OctoClientWrapper.POCO
{
    public static class Constants
    {
        public static readonly string giturl = "https://api.github.com/repos/xpologistics";
        public static string gitusername = "";
        public static string gitusertoken = "";
    }

    public static class AcceptHeaders
    {
        public static readonly string jsonaccept = "application/vnd.github.v3+json";
        public static readonly string jsonLukeCageaccept = "application/vnd.github.luke-cage-preview+json";
    }
}
