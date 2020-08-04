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
using Newtonsoft.Json;

namespace OctoClientWrapper.Service
{
    public class OctoClientHelper : IOctoClientHelper
    {
        OctoClientSingle octoClientSingle;
        
        public OctoClientHelper()
        {
            octoClientSingle = OctoClientSingle.Instance;
        }

        public void DownloadConfigFiles(string strfile, out string apiresponse)
        {
            apiresponse = string.Empty;
            try
            {
                ConfigObjects configObjects = JsonConvert.DeserializeObject<ConfigObjects>(File.ReadAllText(strfile));

                //1. Download Config Files
                foreach (var configObject in configObjects.GitConfigObjects)
                {
                    Helper.DownLoadConfigFile(configObject, out apiresponse);
                }                

            }
            catch (Exception e)
            {
                apiresponse = e.Message;
            }
        }

        public void DownloadAndTransformConfigFiles(string strfile, string environment, out string apiresponse)
        {
            apiresponse = string.Empty;
            try
            {
                ConfigObjects configObjects = JsonConvert.DeserializeObject<ConfigObjects>(File.ReadAllText(strfile));

                
                foreach (var configObject in configObjects.GitConfigObjects)
                {
                    //1. Download Config Files
                    Console.WriteLine(string.Format("downloading files {0} and {1}", configObject.sourceconfig, configObject.transformconfig));
                    Helper.DownLoadConfigFile(configObject, out apiresponse);

                    //2. Transform Config Files
                    Console.WriteLine(string.Format("transforming {0} for {1}", configObject.sourceconfig, configObject.octopusprojectname));
                    TransformFile(configObject.octopusprojectname, configObject.octopusprojectname, environment, (TransformType)Enum.Parse(typeof(TransformType), configObject.configtype));

                    //3. Compare files
                    Console.WriteLine(string.Format("comparing files {0} and {1}", configObject.sourceconfig, configObject.compareconfig));
                    configObject.compareconfig.CompareFiles(configObject.octopusprojectname + "/" + configObject.sourceconfig, "diff-" + configObject.compareconfig, (TransformType)Enum.Parse(typeof(TransformType), configObject.configtype));

                }

            }
            catch (Exception e)
            {
                apiresponse = e.Message;
            }
        }        

        public async System.Threading.Tasks.Task<List<LibraryVariableSetResource>> GetProjectVariablesAsync(string projectname, string environment)
        {
            using (var client = await OctopusAsyncClient.Create(octoClientSingle.Endpoint))
            {
                var projectsTask = client.Repository.Projects.FindByName("Platform-MP-Quotes");
                var project = projectsTask.Result;                
                var variablesetTask = client.Repository.VariableSets.Get(project.VariableSetId);
                var variableSet = variablesetTask.Result;

            }
            return new List<LibraryVariableSetResource>();
        }

        public List<VariableViewModel> GetAllProjectAndLibraryVariablesWithScopes(string projectName, string scope)
        {
            //string projectName = "[ProjectName]";
            ProjectResource project = octoClientSingle.Repository.Projects.FindOne(p => p.Name == projectName);

            var variablesList = new List<VariableViewModel>();

            //Dictionary to get Names from Ids
            Dictionary<string, string> scopeNames = octoClientSingle.Repository.Environments.FindAll().ToDictionary(x => x.Id, x => x.Name);
            octoClientSingle.Machines.ForEach(x => scopeNames[x.Id] = x.Name);            
            octoClientSingle.Channels.Where(c=>c.ProjectId == project.Id).ToList().ForEach(x => scopeNames[x.Id] = x.Name);
            //octoClientSingle.Repository.Projects.GetChannels(project).Items.ToList().ForEach(x => scopeNames[x.Id] = x.Name);
            octoClientSingle.Repository.DeploymentProcesses.Get(project.DeploymentProcessId).Steps.SelectMany(x => x.Actions).ToList().ForEach(x => scopeNames[x.Id] = x.Name);


            //Get All Library Set Variables
            List<LibraryVariableSetResource> librarySets = octoClientSingle.LibrarySets;

            foreach (var libraryVariableSetResource in librarySets)
            {

                var variables = octoClientSingle.Repository.VariableSets.Get(libraryVariableSetResource.VariableSetId);
                var variableSetName = libraryVariableSetResource.Name;
                foreach (var variable in variables.Variables)
                {
                    variablesList.Add(new VariableViewModel(variable, variableSetName, scopeNames));
                }

            }

            //Get All Project Variables for the Project
            var projectSets = octoClientSingle.Repository.VariableSets.Get(project.VariableSetId);

            foreach (var variable in projectSets.Variables)
            {
                variablesList.Add(new VariableViewModel(variable, projectName, scopeNames));
            }
            var scopedVariables =  variablesList.Where(v => (v.Scope == scope || v.Scope is null)).ToList();

            return scopedVariables;
        }

        public void TransformFile(string sourcefiledir, string projectname, string environment, TransformType transformType)
        {
            switch (transformType)
            {
                case TransformType.appconfig:
                    TransformAppConfigFile(sourcefiledir, projectname, environment);
                    break;
                case TransformType.webconfig:
                    TransformWebConfigFile(sourcefiledir, projectname, environment);
                    break;
                case TransformType.appsettingjson:
                    TransformJsonFile(sourcefiledir, projectname, environment);
                    break;
                default:
                    TransformWebConfigFile(sourcefiledir, projectname, environment);
                    break;

            }
                
        }

        public void TransformJsonFile(string sourcefiledir, string projectname, string environment)
        {
            try
            { 
                string sourcefilePath = sourcefiledir + "/appsettings.json";
                string destfilePath = sourcefiledir + "/appsettings.release.json";
                string destenvfilePath = sourcefiledir + "/appsettings."+ environment.Trim(",".ToCharArray())+ ".json";

                Console.WriteLine("**********fetch all variables and values from octopus deploy*********");
                var variables = GetAllProjectAndLibraryVariablesWithScopes(projectname, environment);

                Console.WriteLine("**********replace variables with values from octopus deploy*********");
                //2.a find all placeholders that start with '#{' and end with '}'
                string pattern = @"#{.*?\}";
                string appconfig = destfilePath.readFile();
                MatchCollection matches = Regex.Matches(appconfig, pattern);
                foreach (Match match in matches)
                {
                    foreach (Capture capture in match.Captures)
                    {
                        try
                        {
                            string strvalue = string.Empty;
                            Console.WriteLine("Transforming = {0}", capture.Value);
                            string strkey = capture.Value.TrimStart("#{".ToCharArray()).TrimEnd('}').ToLower();
                            strvalue = variables.Where(variable => variable.Name == strkey).Select(svariable => svariable.Value).FirstOrDefault();
                            appconfig = appconfig.Replace(capture.Value, strvalue);

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("No config found for {0}", capture.Value);
                            continue;
                        }
                    }

                    //3. overwrite contents in appsettings.json with contents from appsettings.release.json
                    //4. overwrite contents in appsettings.<environment>.json with contents from appsettings.release.json
                }
                using (StreamWriter writer = new StreamWriter(sourcefilePath, false, Encoding.UTF8))
                {
                    writer.WriteLine(appconfig);
                }

                using (StreamWriter writer = new StreamWriter(destenvfilePath, false, Encoding.UTF8))
                {
                    writer.WriteLine(appconfig);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void TransformWebConfigFile(string sourcefiledir, string projectname, string environment)
        {
            string sourcefilePath = sourcefiledir + "/web.config";
            string transformfilePath = sourcefiledir + "/web.release.config";

            Console.WriteLine("**********fetch all variables and values from octopus deploy*********");            
            var variables = GetAllProjectAndLibraryVariablesWithScopes(projectname, environment);

            Console.WriteLine("**********Transform web.config with web.release.config*********");            
            Helper.TransformConfig(sourcefilePath, transformfilePath);

            Console.WriteLine("**********replace variables with values from octopus deploy*********");            
            try
            {
                //find all placeholders that start with '#{' and end with '}'
                string pattern = @"#{.*?\}";                
                string appconfig = sourcefilePath.readFile();               

                MatchCollection matches = Regex.Matches(appconfig, pattern);
                foreach (Match match in matches)
                {
                    foreach (Capture capture in match.Captures)
                    {
                        try
                        {
                            string strkey = capture.Value.TrimStart("#{".ToCharArray()).TrimEnd('}').ToLower();
                            string strvalue = string.Empty;
                            strvalue = variables.Where(variable => variable.Name == strkey).Select(svariable => svariable.Value).FirstOrDefault();
                            Console.WriteLine("Transforming = {0}", capture.Value);
                            appconfig = appconfig.Replace(capture.Value, strvalue);                            
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("No config found for {0}", capture.Value);
                            Console.WriteLine(ex.Message);
                            continue;
                        }

                    }
                }
                using (StreamWriter writer = new StreamWriter(sourcefilePath, false, Encoding.UTF8))
                {
                    writer.WriteLine(appconfig);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void TransformAppConfigFile(string sourcefiledir, string projectname, string environment)
        {
            string sourcefilePath = sourcefiledir + "/app.config";
            string transformfilePath = sourcefiledir + "/app.release.config";

            Console.WriteLine("**********fetch all variables and values from octopus deploy*********");            
            var variables = GetAllProjectAndLibraryVariablesWithScopes(projectname, environment);

            Console.WriteLine("**********Transform web.config with app.release.config*********");            
            Helper.TransformConfig(sourcefilePath, transformfilePath);

            Console.WriteLine("**********replace variables with values from octopus deploy*********");             
            try
            {
                //find all placeholders that start with '#{' and end with '}'
                string pattern = @"#{.*?\}";
                string appconfig = sourcefilePath.readFile();

                MatchCollection matches = Regex.Matches(appconfig, pattern);
                foreach (Match match in matches)
                {
                    foreach (Capture capture in match.Captures)
                    {
                        try
                        {
                            string strkey = capture.Value.TrimStart("#{".ToCharArray()).TrimEnd('}').ToLower();
                            string strvalue = string.Empty;
                            strvalue = variables.Where(variable => variable.Name == strkey).Select(svariable => svariable.Value).FirstOrDefault();
                            Console.WriteLine("Transforming = {0}", capture.Value);
                            appconfig = appconfig.Replace(capture.Value, strvalue);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("No config found for {0}", capture.Value);
                            Console.WriteLine(ex.Message);
                            continue;
                        }

                    }
                }
                using (StreamWriter writer = new StreamWriter(sourcefilePath, false, Encoding.UTF8))
                {
                    writer.WriteLine(appconfig);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}

