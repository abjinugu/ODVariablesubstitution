using System;
using System.Collections.Generic;
using System.Text;
using Octopus;
using Octopus.Client.Model;
using OctoClientWrapper.POCO;

namespace OctoClientWrapper.Contract
{
    public interface IOctoClientHelper
    {
        public void DownloadConfigFiles(string strfile, out string apiresponse);

        public void DownloadAndTransformConfigFiles(string strfile, string environment, out string apiresponse);

        public System.Threading.Tasks.Task<List<LibraryVariableSetResource>> GetProjectVariablesAsync(string projectname, string environment);

        public List<VariableViewModel> GetAllProjectAndLibraryVariablesWithScopes(string projectName, string scope);
        public void TransformFile(string sourcefiledir, string projectname, string environment, TransformType transformType);

        public void TransformJsonFile(string sourcefiledir, string projectname, string environment);

        public void TransformWebConfigFile(string sourcefiledir, string projectname, string environment);

        public void TransformAppConfigFile(string sourcefiledir, string projectname, string environment);
        
    }
}
