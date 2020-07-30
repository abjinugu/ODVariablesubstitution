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

namespace OctoClientWrapper.Extensions
{   
    public sealed class OctoClientSingle
    {
        static string server = "http://deploy.xpo.pvt/";
        static string apiKey = "API-******";
        static OctopusServerEndpoint endpoint = new OctopusServerEndpoint(server, apiKey);
        static OctopusRepository repository = new OctopusRepository(new OctopusServerEndpoint(server, apiKey));
        List<LibraryVariableSetResource> librarySets;
        List<MachineResource> machines;
        List<ChannelResource> channels;     


        private static readonly Lazy<OctoClientSingle>
            lazy =
            new Lazy<OctoClientSingle>
                (() => new OctoClientSingle() {                     
                    librarySets = repository.LibraryVariableSets.FindAll(),
                    machines = repository.Machines.FindAll().ToList(),
                    channels = repository.Channels.FindAll().ToList(),                    
                }
                );

        public static OctoClientSingle Instance { get { return lazy.Value; } }

        private OctoClientSingle()
        {
        }

        public  OctopusServerEndpoint Endpoint
        {
            get { return endpoint; }
        }
        public  OctopusRepository Repository
        {
            get { return repository; }
        }
        public  List<LibraryVariableSetResource> LibrarySets
        {
            get { return librarySets; }
        }

        public List<MachineResource> Machines { get => machines; }
        public List<ChannelResource> Channels { get => channels; }        
    }
}
