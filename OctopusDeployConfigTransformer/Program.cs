using System;
using OctoClientWrapper;
using OctoClientWrapper.Contract;
using OctoClientWrapper.Service;
using OctoClientWrapper.POCO;

namespace OctopusDeployConfigTransformer
{
    class Program
    {
        static void Main(string[] args)
        {
            string sourcefiledir = @"C:\Git\xpoconnect-integration-services-backend\src\backend\Api\Xpo.Connect.IntegrationService.Backend.Api.Site";
            string projectname = "XpoConnect-IntegrationServices-Api";
            string environment = "Prod,";

            Console.WriteLine("Hello World!");
            IOctoClientHelper octoClientHelper = new OctoClientHelper();
            //var variablesTask = octoClientHelper.GetProjectVariablesAsync("", "", "");
            //var variables = variablesTask.Result;

            //var allvariables = octoClientHelper.GetAllProjectAndLibraryVariablesWithScopes("Platform-MP-Quotes", "Prod,");

            octoClientHelper.TransformFile(sourcefiledir, projectname, environment, TransformType.webconfig);

            sourcefiledir = @"C:\Git\xpoconnect-services-api\src\backend\Api\XpoConnect.Services.Api.Site";
            projectname = "XpoConnectServices-Api";            
            octoClientHelper.TransformFile(sourcefiledir, projectname, environment, TransformType.appsettingjson);

        }
    }
}

