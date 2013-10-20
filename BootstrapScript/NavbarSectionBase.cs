// NavbarSectionBase.cs
//

using System;
using System.Collections.Generic;
using BL.UI;

namespace BL.BS
{
    public class NavbarSectionBase : ItemsControl
    {

        public void SetActive(int index, bool active)
        {
            if (index > this.ItemControls.Count)
            {
                return;
            }
            
            Item item = (Item)this.ItemControls[index];

            item.Active = active;
        }
    }
}
