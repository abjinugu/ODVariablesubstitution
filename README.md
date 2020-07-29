---


---

<h2 id="octopus-deploy-variable-substitution-utility">Octopus Deploy Variable Substitution Utility</h2>
<p>This simple utility does the following things</p>
<ul>
<li>Reads the contents from <a href="https://github.com/xpologistics/ODVariablesubstitution/blob/master/OctopusDeployConfigTransformer/GithubConfigObjects.json" title="GithubConfigObjects.json">GithubConfigObjects.json</a> file.</li>
<li>Foreach application listed in the json file, it downloads the respective configuration file along with the release config file in a folder with the same name as its Octopus Deploy Project Name.</li>
<li>It then transforms the config file using the variables from Octopus Deploy’s Central Config Library set.</li>
</ul>
<h3 id="pre-requisites-before-running-the-code">Pre-requisites before running the code</h3>
<ul>
<li>DotNet Core 3.1</li>
<li>Make sure proper values have been set for Octopus Deploy in <a href="https://github.com/xpologistics/ODVariablesubstitution/blob/master/OctoClientWrapper/Extensions/OctoClientSingle.cs">OctoClientSingle.cs</a> file.</li>
<li>Github credentials need to be updated in <a href="https://github.com/xpologistics/ODVariablesubstitution/blob/master/OctoClientWrapper/POCO/Constants.cs">Constants.cs</a> file.</li>
<li>We will make these two arguments for exe in the final build.</li>
</ul>
<h3 id="how-to-run">How to run</h3>
<ul>
<li>Open solution <a href="https://github.com/xpologistics/ODVariablesubstitution/blob/master/OctopusDeployConfigTransformer/OctopusDeployConfigTransformer.sln" title="OctopusDeployConfigTransformer.sln">OctopusDeployConfigTransformer.sln</a> in Visual Studio.</li>
<li>Set OctopusDeployConfigTransformer project as startup project</li>
<li>Update GithubConfigObjects.json as per your requirement. Make sure the objects types are not changes since they are directly mapped to <a href="https://github.com/xpologistics/ODVariablesubstitution/blob/master/OctoClientWrapper/POCO/GitConfigObject.cs">POCO</a> object inside.</li>
<li>Hit F5 for debug and Ctrl + F5 to run the application without debugging enabled</li>
<li>Output will be stored in the bin directory.</li>
</ul>

