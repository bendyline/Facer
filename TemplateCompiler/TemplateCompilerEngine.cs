using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bendyline.Base;
using System.IO;
using System.Text.RegularExpressions;

namespace Bendyline.UI.TemplateCompiler
{
    public class TemplateCompilerEngine
    {
        private String outputFolder;
        private String name;
        private String inputFolder;
        private bool forceUpdate = false;
        private DateTime CompilerVersionDateTime = new DateTime(2013, 9, 16, 23, 59, 0);
        private List<Template> templates;
        private bool snapOutputFolderToTemplates = true;

        public bool ForceUpdate
        {
            get
            {
                return this.forceUpdate;
            }

            set
            {
                this.forceUpdate = value;
            }
        }

        public String OutputFolder
        {
            get 
            {
                return this.outputFolder; 
            }

            set 
            {
                this.outputFolder = value; 
            }
        }

        public DirectoryInfo EffectiveOutputFolder
        {
            get
            {
                String effectiveOutputFolder = this.OutputFolder;

                if (this.snapOutputFolderToTemplates)
                {
                    int lastIndex = outputFolder.LastIndexOf("\\gs\\", StringComparison.InvariantCultureIgnoreCase);

                    if (lastIndex >= 0)
                    {
                        effectiveOutputFolder = effectiveOutputFolder.Substring(0, lastIndex + 4);
                        effectiveOutputFolder += "\\t\\";
                    }
                }

                // snap everything we do to, file wise, to lower case
                effectiveOutputFolder = effectiveOutputFolder.ToLowerCase();

                DirectoryInfo di = new DirectoryInfo(effectiveOutputFolder);

                return di;
            }
        }

        public String InputFolder
        {
            get
            {
                return this.inputFolder;
            }
            set
            {
                this.inputFolder = value;
            }
        }

        public String Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public TemplateCompilerEngine()
        {
            this.templates = new List<Template>();
        }

        public void Execute()
        {
            DateTime updateDateTime = DateTime.MinValue;

            // if we're not forcing an update, and if the output file is newer than the compiler rebuild, use output file write time 
            // as the way to check if we need to recompile.
            if (!forceUpdate)
            {
                String sentinelFilePath = FileUtilities.EnsurePath(this.EffectiveOutputFolder.FullName, name + ".js");

                Log.Message("Considering effective output of {0}", this.EffectiveOutputFolder.FullName);

                FileInfo sentinelFile = new FileInfo(sentinelFilePath);

                if (sentinelFile.Exists)
                {
                    Log.Message("Found existing output file at {0}", sentinelFile.FullName);

                    if (sentinelFile.LastWriteTimeUtc.CompareTo(CompilerVersionDateTime) > 0)
                    {
                        updateDateTime = sentinelFile.LastWriteTimeUtc;
                    }
                }
            }

            if (this.ProcessFolder(this.InputFolder, updateDateTime))
            {
                if (this.templates.Count == 0)
                {
                    Log.Message("No templates were found in input folder {0}", this.InputFolder);
                    return;
                }

                this.ExportJsonFiles(this.OutputFolder, this.Name);
            }
        }

        public String GetFactoredCss()
        {
            StringBuilder sb = new StringBuilder();

            foreach (Template t in this.templates)
            {
                if (t.Css != null)
                {
                    sb.Append(this.ParseCss(t));
                }
            }

            if (sb.Length == 0)
            {
                return null;
            }

            return sb.ToString();
        
        }

        public String ParseCss(Template t)
        {
            return ParseCss(t.Css, t.Id);
        }

        public String ParseCss(String css, String id)
        {
            StringBuilder result = new StringBuilder();

            int nextLeft = css.IndexOf("{");
            int nextRight = 0;
            int lastStart = 0;

            while (nextLeft >= 0)
            {
                int interiorGroups = 0;
                nextRight = nextLeft + 1;

                nextRight = css.IndexOf("}", nextRight);
                int interiorLeft = css.IndexOf("{", nextLeft + 1);

                while ((interiorLeft >= 0 && interiorLeft < nextRight) || interiorGroups > 0)
                {
                    if (interiorLeft >= 0 && interiorLeft < nextRight)
                    {
                        interiorGroups++;
                        interiorLeft = css.IndexOf("{", interiorLeft + 1);
                    }
                    else
                    {
                        nextRight = css.IndexOf("}", nextRight + 1);
                        interiorGroups--;
                    }
                }

                if (nextRight > 0)
                {
                    String content = css.Substring(lastStart, nextLeft - lastStart);

                    int nextHash = content.IndexOf("#");
         
                    if (nextHash < 0)
                    {
                        String tagStart = content.Trim().ToLowerCase();

                        if (tagStart.StartsWith("body"))
                        {
                            content = String.Format(" .{0}{1}", id.Replace(".", "-"), tagStart.Substring(4));
                        }
                    }
                    else
                    {
                        while (nextHash >= 0)
                        {
                            content = String.Format("{0}.{1}-{2}", content.Substring(0, nextHash), id.Replace(".", "-"), content.Substring(nextHash + 1, content.Length - (nextHash + 1)));
                            nextHash = content.IndexOf("#", nextHash + 1);
                        }
                    }
                   
                    result.Append(content.Trim());

                    nextRight++;
                    
                    // recursively process the interior of a CSS elemnet (e.g., a @media query)
                    String interior = "{" + ParseCss(css.Substring(nextLeft + 1, (nextRight - nextLeft) - 2), id) + "}";
                    result.Append(interior);
                    lastStart = nextRight;
                    nextLeft = css.IndexOf("{", nextRight);
                }
                else
                {
                    nextLeft = -1;
                }
            }

            result.Append(css.Substring(lastStart, css.Length - lastStart).Trim());

            return result.ToString();
        }

        public String GetCrunchedJson()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[");
            bool isFirst = true;

            foreach (Template t in this.templates)
            {
                if (!isFirst)
                {
                    sb.Append(",");
                }

                sb.Append("{");

                sb.Append("\"id\":\"");
                sb.Append(t.Id);
                sb.Append("\",\"content\":\"");

                String content = t.Content.Replace("\\", "\\\\");
                content = content.Replace("\"", "\\\"");

                content = XmlUtilities.StripWhiteSpace(ref content);

                // strip sample content
                content = content.Replace("lorem ipsum", "");

                // strip comments
                int nextComment = content.IndexOf("/*");

                while (nextComment >= 0)
                {
                    int endOfComment = content.IndexOf("*/", nextComment);

                    if (endOfComment > nextComment)
                    {
                        content = content.Substring(0, nextComment) + content.Substring(endOfComment + 2);
                        nextComment = content.IndexOf("/*");
                    }
                    else
                    {
                        nextComment = -1;
                    }
                }


                // strip sample content placeholders
                nextComment = content.IndexOf("|*");

                while (nextComment >= 0)
                {
                    int endOfComment = content.IndexOf("*|", nextComment);

                    if (endOfComment > nextComment)
                    {
                        content = content.Substring(0, nextComment) + content.Substring(endOfComment + 2);
                        nextComment = content.IndexOf("|*");
                    }
                    else
                    {
                        nextComment = -1;
                    }
                }

                sb.Append(content);
                sb.Append("\"");

                isFirst = false;
                sb.Append("}");
            }

            sb.Append("]");

            return sb.ToString();
        }

        public static string RemoveWhiteSpaceFromStylesheets(string content)
        {
            content = Regex.Replace(content, @"[a-zA-Z]+#", "#");
            content = Regex.Replace(content, @"[\n\r]+\s*", string.Empty);
            content = Regex.Replace(content, @"\s+", " ");
            content = Regex.Replace(content, @"\s?([:,;{}])\s?", "$1");
            content = content.Replace(";}", "}");
            content = Regex.Replace(content, @"([\s:]0)(px|pt|%|em)", "$1");
            content = Regex.Replace(content, @"/\*[\d\D]*?\*/", string.Empty);

            return content;
        }

        private bool ExportJsonFiles(String outputFolder, String name)
        {
            DirectoryInfo di = this.EffectiveOutputFolder;

            if (this.snapOutputFolderToTemplates)
            {
                if (!di.Exists)
                {
                    di.Create();
                }
            }
            else
            {
                if (!di.Exists)
                {
                    Log.Error("Could not find output folder {0})", outputFolder);
                    return false;
                }
            }

            Log.Message("Exporting JSON files to {0} with name {1}.", di.FullName, this.Name);

            String json = GetCrunchedJson();
            String css = GetFactoredCss();

            String filePath = FileUtilities.EnsurePath(di.FullName, name + ".json").ToLowerCase();

            FileUtilities.SetTextToFile(filePath, json);

            filePath = FileUtilities.EnsurePath(di.FullName, name + ".css").ToLowerCase();
            css = RemoveWhiteSpaceFromStylesheets(css);

            FileUtilities.SetTextToFile(filePath, css);

            filePath = FileUtilities.EnsurePath(di.FullName, name + ".js").ToLowerCase();

            String content = String.Format("BL.UI.TemplateManager.get_current().addTemplateFile(\"{1}\", {0});", json, name.Substring(0, name.Length - 2).ToLowerCase());

            FileUtilities.SetTextToFile(filePath, content);
            return true;
        }

        private bool IsValidTemplateId(String id)
        {
            if (!id.Contains('-') || id.Contains('\'') || id.Contains('\"'))
            {
                Log.Error("Template with invalid ID was discovered {0}.  Template IDs should be namespaced with '-', and contain no quotes.", id);
                return false;
            }

            return true;
        }

        private bool ProcessFolder(String inputFolder, DateTime updateIfFileLaterThan)
        {
            Log.Message("Processing folder {0}.", inputFolder);

            List<FileInfo> filesToProcess = new List<FileInfo>();

            DirectoryInfo di = new DirectoryInfo(inputFolder);

            if (!di.Exists)
            {
                Log.Error("Could not find input folder {0}.", inputFolder);
                return false;
            }

            bool foundAnUpdatedFile = false;

            FileInfo[] files = di.GetFiles();

            foreach (FileInfo fi in files)
            {
                String extension = fi.Extension.ToLowerCase();

                if (extension == ".htm" || extension == ".html")
                {
                    filesToProcess.Add(fi);

                    if (fi.LastWriteTimeUtc.CompareTo(updateIfFileLaterThan) > 0)
                    {
                        foundAnUpdatedFile = true;
                    }
                }
            }

            if (!foundAnUpdatedFile)
            {
                Log.Message("Did not find an updated file; no compilation needed.");
                return false;
            }

            foreach (FileInfo fi in filesToProcess)
            {
                String content = FileUtilities.GetTextFromFile(fi.FullName);

                String id = XmlUtilities.GetTagInterior(ref content, "title");

                if (id == null || id == String.Empty)
                {
                    id = this.Name.ToLowerCase();

                    if (id.EndsWith(".t"))
                    {
                        id = id.Substring(0, id.Length - 2);
                    }

                    id = id.Replace(".", "-") + "-" + FileUtilities.GetBaseFileName(fi.Name).ToLowerCase();
                }

                if (IsValidTemplateId(id))
                {
                    String bodyContent = XmlUtilities.GetTagInterior(ref content, "body");
                    String cssContent = XmlUtilities.GetTagInterior(ref content, "style");

                    if (id != null && bodyContent != null)
                    {
                        Log.Message("Found template {0} in file {1}.", id, fi.Name);

                        Template t = new Template();

                        t.Id = id;
                        t.Content = bodyContent.Trim();
                        t.Css = cssContent;

                        this.templates.Add(t);
                    }
                }
            }

            return true;
        }

    }
}
