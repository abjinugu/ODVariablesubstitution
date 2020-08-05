using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Web.XmlTransform;
using Newtonsoft.Json.Linq;
using OctoClientWrapper.POCO;
using RestSharp;
using JsonDiffPatchDotNet;
using XmlDiffLib;
using DiffPlex;
using System.Linq;
using System.Text.RegularExpressions;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace OctoClientWrapper.Extensions
{
    public static class Helper
    {
        public static void TransformConfig(string configFileName, string transformFileName)
        {
            try
            {
                using (var document = new XmlTransformableDocument())
                {
                    document.PreserveWhitespace = true;
                    document.Load(configFileName);

                    using (var transform = new XmlTransformation(transformFileName))
                    {
                        if (transform.Apply(document))
                        {
                            document.Save(configFileName);
                        }
                        else
                        {
                            throw new Exception("Transformation Failed");
                        }
                    }
                }

            }
            catch (Exception xmlException)
            {
                Console.WriteLine(xmlException.Message);
            }


        }

        public static JObject readJsonFromFile(this string filePath)
        {
            JObject o1 = null;
            String subjectString = string.Empty;
            try
            {
                o1 = JObject.Parse(File.ReadAllText(filePath));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return o1;
        }

        public static string readFile(this string filePath)
        {
            String subjectString = string.Empty;
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    // Read the stream to a string, and write the string to the console.
                    subjectString = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                //DO NOTHING
            }
            return subjectString;
        }

        public static string readXMLFileToJSON(this string filePath)
        {
            String subjectString = string.Empty;
            var json = string.Empty;
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    // Read the stream to a string, and write the string to the console.
                    subjectString = sr.ReadToEnd();
                }

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(subjectString);

                json = JsonConvert.SerializeXmlNode(doc);
            }
            catch (Exception e)
            {
                //DO NOTHING
            }
            return json;
        }

        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string getJName(this JToken jtoken)
        {
            return ((Newtonsoft.Json.Linq.JProperty)jtoken).Name.ToLower();
        }

        public static string getJValue(this JToken jtoken)
        {
            return ((Newtonsoft.Json.Linq.JProperty)jtoken).Value.ToString();
        }

        public static void DownLoadConfigFile(GitConfigObject gitConfigObject, out string apiresponse)
        {
            apiresponse = string.Empty;

            try
            {
                var token = string.Format("{0}:{1}", Constants.gitusername, Constants.gitusertoken);

                //string jsonInput = string.Format("{{\"title\": \"PR from {0} to {1}\", \"body\": \"{2}\", \"head\": \"{3}\",  \"base\": \"{4}\" }}", fromBranch, toBranch, "Auto PR from TOOL", fromBranch, toBranch);

                string filename = string.Empty;
                string gitclienturl = string.Empty;

                switch ((TransformType)Enum.Parse(typeof(TransformType), gitConfigObject.configtype))
                {
                    case TransformType.appconfig:                        
                        gitclienturl = string.Format("{0}/{1}/contents/{2}/{3}?ref={4}", Constants.giturl, gitConfigObject.repository, gitConfigObject.configlocation, gitConfigObject.sourceconfig, gitConfigObject.gitbranch);
                        DownLoadFile(gitclienturl, token, gitConfigObject.octopusprojectname, gitConfigObject.sourceconfig, out apiresponse);
                        
                        gitclienturl = string.Format("{0}/{1}/contents/{2}/{3}?ref={4}", Constants.giturl, gitConfigObject.repository, gitConfigObject.configlocation, gitConfigObject.transformconfig, gitConfigObject.gitbranch);
                        DownLoadFile(gitclienturl, token, gitConfigObject.octopusprojectname, filename, out apiresponse);
                        break;
                    case TransformType.webconfig:                        
                        gitclienturl = string.Format("{0}/{1}/contents/{2}/{3}?ref={4}", Constants.giturl, gitConfigObject.repository, gitConfigObject.configlocation, gitConfigObject.sourceconfig, gitConfigObject.gitbranch);
                        DownLoadFile(gitclienturl, token, gitConfigObject.octopusprojectname, gitConfigObject.sourceconfig, out apiresponse);
                        
                        gitclienturl = string.Format("{0}/{1}/contents/{2}/{3}?ref={4}", Constants.giturl, gitConfigObject.repository, gitConfigObject.configlocation, gitConfigObject.transformconfig, gitConfigObject.gitbranch);
                        DownLoadFile(gitclienturl, token, gitConfigObject.octopusprojectname, gitConfigObject.transformconfig, out apiresponse);
                        break;                        
                    case TransformType.appsettingjson:                                                
                        gitclienturl = string.Format("{0}/{1}/contents/{2}/{3}?ref={4}", Constants.giturl, gitConfigObject.repository, gitConfigObject.configlocation, gitConfigObject.sourceconfig, gitConfigObject.gitbranch);
                        DownLoadFile(gitclienturl, token, gitConfigObject.octopusprojectname, gitConfigObject.sourceconfig, out apiresponse);
                        
                        gitclienturl = string.Format("{0}/{1}/contents/{2}/{3}?ref={4}", Constants.giturl, gitConfigObject.repository, gitConfigObject.configlocation, gitConfigObject.transformconfig, gitConfigObject.gitbranch);
                        DownLoadFile(gitclienturl, token, gitConfigObject.octopusprojectname, gitConfigObject.transformconfig, out apiresponse);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                apiresponse = e.Message;
            }
        }

        public static void DownLoadFile(string gitclienturl, string token, string downloadPath, string fileName, out string apiresponse)
        {
            apiresponse = string.Empty;

            try
            {
                //check if download path exists
                if (!Directory.Exists(downloadPath))
                {
                    Directory.CreateDirectory(downloadPath);
                }


                var client = new RestClient(gitclienturl);

                var request = new RestRequest();
                // easily add HTTP Headers
                request.AddHeader("Authorization", string.Format("Basic {0}", token.Base64Encode()));

                request.AddHeader("Accept", AcceptHeaders.jsonaccept);

                // execute the request
                IRestResponse response = client.Execute(request);
                if (response.IsSuccessful)
                {
                    var content = response.Content; // raw content as string

                    var o1 = JObject.Parse(content);
                    var pr = o1["download_url"].Value<string>();
                    var sha = o1["sha"].Value<string>();

                    var oclient = new RestClient(pr);
                    var orequest = new RestRequest();
                    // easily add HTTP Headers
                    orequest.AddHeader("Authorization", string.Format("Basic {0}", token.Base64Encode()));

                    orequest.AddHeader("Accept", AcceptHeaders.jsonaccept);
                    oclient.DownloadData(request);
                    File.WriteAllBytes(downloadPath+"/"+fileName, oclient.DownloadData(request));


                }
            }
            catch (Exception e)
            {
                apiresponse = e.Message;
            }

        }

        public static void CompareFiles(this string sourcefile, string targetfile, string difffilepath, TransformType transformType)
        {

            var jdp = new JsonDiffPatch();
            try { 

                if (transformType == TransformType.appsettingjson)
                {
                
                    var left = JToken.Parse(sourcefile.readFile());
                    var right = JToken.Parse(targetfile.readFile());

                    JToken patch = jdp.Diff(left, right);
                    if(patch != null)
                        File.WriteAllText(difffilepath, patch.ToString());
                    else
                        File.WriteAllText(difffilepath, "Files are identical");
                }
                else
                {                
                    var left = JToken.Parse(sourcefile.readXMLFileToJSON());
                    var right = JToken.Parse(targetfile.readXMLFileToJSON());

                    JToken patch = jdp.Diff(left, right);
                    if (patch != null)
                        File.WriteAllText(difffilepath, patch.ToString());
                    else
                        File.WriteAllText(difffilepath, "Files are identical");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void MergeJsonFiles(string sourcefile, string targetfile, string[] outputfiles)
        {
            var jsonmergesettings = new JsonMergeSettings() { MergeArrayHandling = MergeArrayHandling.Union };
            var sourceJson = sourcefile.readJsonFromFile();
            var targetJson = targetfile.readJsonFromFile();
            sourceJson.Merge(targetJson, jsonmergesettings);
            foreach (var outputfile in outputfiles)
            {
                using (StreamWriter writer = new StreamWriter(outputfile, false, Encoding.UTF8))
                {
                    writer.WriteLine(sourceJson.ToString());
                }
            }
        }
    }
}
