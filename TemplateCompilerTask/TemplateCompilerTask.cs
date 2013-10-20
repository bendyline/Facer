using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Bendyline.Base;
using System.IO;

namespace Bendyline.UI.TemplateCompiler.Tasks
{
    public class TemplateCompilerTask : Task
    {
        public String InputFolder { get; set; }
        public String OutputFolder { get; set; }
        public String Name { get; set; }

        [Output]
        public String OutputPath { get; set; }

        public override bool Execute()
        {
            MSBuildLogger.Current.Initialize(this);
            
            Log.LogMessage("Executing template compilation task (input:{0} outputPath:{1} name:{2} in {3})", this.InputFolder, this.OutputFolder, this.Name, Directory.GetCurrentDirectory());

            try
            {
                TemplateCompilerEngine tce = new TemplateCompilerEngine();

                tce.InputFolder = this.InputFolder;
                tce.OutputFolder = this.OutputFolder;
                tce.Name = this.Name + ".t";

                this.OutputPath = FileUtilities.EnsurePath(this.OutputFolder, this.Name + ".t.js");

                tce.Execute();

            }
            catch (Exception e)
            {
                Log.LogError("Could not execute task due to unspecified error: {0} at {1}", e.Message, e.StackTrace);
            }

            return true;
        }
    }
}
