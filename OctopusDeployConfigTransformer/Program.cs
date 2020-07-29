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
            IOctoClientHelper octoClientHelper = new OctoClientHelper();
            string jsonFilePath = @"C:\Git\ODVariablesubstitution\OctopusDeployConfigTransformer\GithubConfigObjects.json";

            octoClientHelper.DownloadAndTransformConfigFiles(jsonFilePath, "Prod,", out apiresponse);

            //octoClientHelper.DownloadConfigFiles(jsonFilePath, out apiresponse);

            //string sourcefiledir = @"C:\Git\xpoconnect-integration-services-backend\src\backend\Api\Xpo.Connect.IntegrationService.Backend.Api.Site";
            //string projectname = "XpoConnect-IntegrationServices-Api";
            //string environment = "Prod,";

            //octoClientHelper.TransformFile(sourcefiledir, projectname, environment, TransformType.webconfig);

            //sourcefiledir = @"C:\Git\xpoconnect-services-api\src\backend\Api\XpoConnect.Services.Api.Site";
            //projectname = "XpoConnectServices-Api";            
            //octoClientHelper.TransformFile(sourcefiledir, projectname, environment, TransformType.appsettingjson);

        }
    }
}

