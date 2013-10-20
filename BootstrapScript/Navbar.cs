// Class1.cs
//

using System;
using System.Collections.Generic;
using System.Html;
using jQueryApi;
using BL.UI;

namespace BL.BS
{

    public class Navbar : ItemsControl
    {
        private int activeIndex = -1;

        public override string DefaultClass
        {
            get
            {
                return "navbar navbar-default";
            }
        }

        public override string TagName
        {
            get
            {
                return "nav";
            }
        }

        public void Select(int index)
        {
            if (this.activeIndex >= 0)
            {
                this.SetActive(this.activeIndex, false);
            }

            this.activeIndex = index;

            this.SetActive(this.activeIndex, true);
        }

        public void SetActive(int index, bool value)
        { 
            int totalIndex = 0;

            foreach (NavbarSectionBase nbs in this.ItemControls)
            {
                ICollection<Item> items = (ICollection<Item>)nbs.ItemControls;

                if (totalIndex + items.Count > index)
                {
                    nbs.SetActive(index - totalIndex, value);
                    return;
                }

                totalIndex += items.Count;
            }
        }
    }
}
