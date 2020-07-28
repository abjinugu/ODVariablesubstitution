using System;
using System.Collections.Generic;
using System.Text;
using OctoClientWrapper.Contract;
using Octopus.Client.Model;
using Octopus;
using Octopus.Client;
using OctoClientWrapper.POCO;
using System.Linq;
using OctoClientWrapper.Extensions;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml.Linq;
using System.Xml;

namespace OctoClientWrapper.POCO
{
    public static class OctopusClientProxy
    {        
        static List<LibraryVariableSetResource> librarySets;
        static string server = "http://deploy.xpo.pvt/";
        static string apiKey = "API-**************";             // Get this from your 'profile' page in the Octopus Web Portal            
         
        public static OctopusServerEndpoint Endpoint {
            get { return new OctopusServerEndpoint(server, apiKey); } 
        }
        public static OctopusRepository Repository {
            get { return new OctopusRepository(Endpoint); }
        }
        public static List<LibraryVariableSetResource> LibrarySets {
            get { return Repository.LibraryVariableSets.FindAll(); }
        }
    }
}
