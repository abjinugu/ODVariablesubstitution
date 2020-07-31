<!DOCTYPE html>
<html>

<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>OctopsDeployUtil</title>
  <link rel="stylesheet" href="https://stackedit.io/style.css" />
</head>

<body class="stackedit">
  <div class="stackedit__html"><h2 id="octopus-deploy-variable-substitution-utility">Octopus Deploy Variable Substitution Utility</h2>
<p>This simple utility does the following things</p>
<ul>
<li>Reads the contents from <a href="https://github.com/xpologistics/ODVariablesubstitution/blob/master/OctopusDeployConfigTransformer/GithubConfigObjects.json" title="GithubConfigObjects.json">GithubConfigObjects.json</a> file.</li>
<li>Foreach application listed in the json file, it downloads the respective configuration file along with the release config file into a folder with the same name as its Octopus Deploy Project Name.</li>
<li>It then transforms the config file using the variables from Octopus Deploy’s Central Config Library set.</li>
<li>Compares the transformed file with the actual file and publishes the differences in a new file with the prefix “diff-”</li>
</ul>
<h4 id="githubconfigobjects.json-explained">GithubConfigObjects.json explained</h4>
<p><strong>octopusprojectname</strong>: Name of the project in Octopus Deploy<br>
<strong>configtype</strong>: Configuration file type. ‘webconfig’, ‘appconfig’ or ‘appsettingjson’<br>
<strong>repository</strong>: Name of the github repository<br>
<strong>configlocation</strong>: Location of the config file in repository<br>
<strong>sourceconfig</strong>: Config file name<br>
<strong>transformconfig</strong>: Transformation file that has placeholders for Octopus Variables<br>
<strong>compareconfig</strong>: Config file that is being used for Transformation comparision. Prod config file if you are comparing prod configurations. Name the file after the OD project.<br>
<strong>gitbranch</strong>: Name of the git branch</p>
<h5 id="example">Example</h5>
<pre><code>{
  "GitConfigObjects": [
    {
      "octopusprojectname": "XpoConnect-IntegrationServices-Api",
      "configtype": "webconfig",
      "repository": "xpoconnect-integration-services-backend",
      "configlocation": "src/backend/Api/Xpo.Connect.IntegrationService.Backend.Api.Site",
      "sourceconfig": "Web.config",
      "transformconfig": "Web.Release.config",
      "compareconfig": "XpoConnect-IntegrationServices-Api.config",
      "gitbranch": "dev"

    },
    {
      "octopusprojectname": "XpoConnectServices-Api",
      "configtype": "appsettingjson",
      "repository": "xpoconnect-services-api",
      "configlocation": "src/backend/Api/XpoConnect.Services.Api.Site",
      "sourceconfig": "appsettings.json",
      "transformconfig": "appsettings.Release.json",
      "compareconfig": "XpoConnectServices-Api.json",
      "gitbranch": "dev"
    }
  ]
}
</code></pre>
<h3 id="how-to-run">How to run</h3>
<h4 id="publish-code-into-one-single-exe-file">Publish code into one single exe file</h4>
<blockquote>
<p>dotnet publish -r win-x64 -c Release -o output /p:PublishSingleFile=true</p>
</blockquote>
<p>This will publish OctopusDeployConfigTransformer.exe file in ‘output’ directory. Copy GithubConfigObjects.json file and the files that need to be compared to the same directory.</p>
<p>Run the below command with appropriate values. This will download the transformed config files in to the directories named after their Octopus Deploy Project and diff files in the main directory with the prefix “diff-”</p>
<blockquote>
<p>.\OctopusDeployConfigTransformer.exe “xlmbuildndeploy” “&lt;your_gitPAT&gt;” “&lt;your_ocotpus_deploy_api_key&gt;” “GithubConfigObjects.json” “Prod,”</p>
</blockquote>
</div>
</body>

</html>
