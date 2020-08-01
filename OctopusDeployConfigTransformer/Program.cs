using System;
using OctoClientWrapper;
using OctoClientWrapper.Contract;
using OctoClientWrapper.Service;
using OctoClientWrapper.POCO;
using Newtonsoft.Json;

namespace OctopusDeployConfigTransformer
{
    class Program
    {
        static void Main(string[] args)
        {
            string apiresponse = string.Empty;            
            if (args.Length < 4)
            {
                Console.WriteLine("Missing Arguments");
            }
            else
            {
                var gitusername = args[0] ?? string.Empty;
                var gitusertoken = args[1] ?? string.Empty;
                var octopusdeplopyapikey = args[2] ?? string.Empty;
                var jsonFilePath = args[3] ?? string.Empty;
                var environment = args[4] ?? string.Empty;

                Constants.gitusername = gitusername.Length > 0 ? gitusername : Constants.gitusername;
                Constants.gitusertoken = gitusertoken.Length > 0 ? gitusertoken : Constants.gitusertoken;
                Constants.octopusdeplopyapikey = octopusdeplopyapikey.Length > 0 ? octopusdeplopyapikey : Constants.octopusdeplopyapikey;                
                IOctoClientHelper octoClientHelper = new OctoClientHelper();
                try
                {

                    octoClientHelper.DownloadAndTransformConfigFiles(jsonFilePath, environment, out apiresponse);
                    Console.WriteLine(apiresponse);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }


            }
        }
    }
}

