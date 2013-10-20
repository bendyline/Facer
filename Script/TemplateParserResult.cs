// TemplateParser.cs
//

using System;
using System.Collections.Generic;
using System.Html;

namespace BL.UI
{
    public class TemplateParserResult
    {
        private String markup;
        private List<List<int>> controlElementPaths;
        private List<Control> controls;
        private List<String> controlIds;
        private List<int> itemsContainer;

        private List<List<int>> elementElementPaths;
        private List<String> elementIds;

        public List<int> ItemsContainer
        {
            get
            {
                return this.itemsContainer;
            }

            set
            {
                this.itemsContainer = value;
            }
        }

        public String Markup
        {
            get
            {
                return this.markup;
            }

            set
            {
                this.markup = value;
            }
        }

        public List<List<int>> ElementElementPaths
        {
            get
            {
                return this.elementElementPaths;
            }
        }

        public List<String> ElementIds
        {
            get
            {
                return this.elementIds;
            }
        }

        public List<List<int>> ControlElementPaths
        {
            get
            {
                return this.controlElementPaths;
            }
        }

        public List<String> ControlIds
        {
            get
            {
                return this.controlIds;
            }
        }

        public List<Control> Controls
        {
            get
            {
                return this.controls;
            }
        }

        public TemplateParserResult()
        {
            this.controls = new List<Control>();
            this.controlElementPaths = new List<List<int>>();
            this.controlIds = new List<string>();
            this.elementElementPaths = new List<List<int>>();
            this.elementIds = new List<string>();
        }

        public void AddElement(List<int> elementPath, String id)
        {
            this.elementElementPaths.Add(elementPath);
            this.elementIds.Add(id);
        }
        public void AddControl(Control c, List<int> elementPath, String id)
        {
            this.controls.Add(c);
            this.controlElementPaths.Add(elementPath);
            this.controlIds.Add(id);
        }
    }
}
