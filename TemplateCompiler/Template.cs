using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bendyline.UI.TemplateCompiler
{
    public class Template
    {
        private String id;
        private String content;
        private String css;

        public String Id
        {
            get
            {
                return this.id;
            }

            set
            {
                this.id = value;
            }
        }

        public String Content
        {
            get
            {
                return this.content;
            }

            set
            {
                this.content = value;
            }
        }

        public String Css
        {
            get
            {
                return this.css;
            }

            set
            {
                this.css = value;
            }
        }
    }
}
