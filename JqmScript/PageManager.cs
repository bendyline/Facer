using System;
using System.Collections.Generic;
using System.Linq;
using jQueryApi;

namespace BL.JQM
{
    public class PageManager
    {
        private Dictionary<String, Page> pages = new Dictionary<String, Page>();
        private Page activePage;
        private static PageManager current = new PageManager();

        public static PageManager Current
        {
            get
            {
                return current;
            }
        }

        public void ClearPage(String pageName)
        {
            this.pages.Remove(pageName);
        }

        public void RegisterPage(Page page)
        {
            this.pages[page.Name] = page;

            if (this.activePage == null)
            {
                this.activePage = page;
                this.activePage.Visible = true;
            }
        }

        public void ChangePage(String pageName, jQueryPageChangeOptions pageChangeOptions)
        {
            Page newPage = this.pages[pageName];

            if (this.activePage != null)
            {
                this.activePage.Visible = false;
            }

            newPage.Visible = true;

            this.activePage = newPage;

            Script.Literal("$.mobile.changePage($(\"#\" + {0}), {1})", pageName, pageChangeOptions);
        }
    }
}
