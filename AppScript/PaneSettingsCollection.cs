// SensorSystemCollection.cs
//

using System;
using System.Collections.Generic;
using System.Collections;

namespace BL.UI.App
{
    public class PaneSettingsCollection 
    {
        private ArrayList paneSettings;

        public PaneSettings this[int index]
        {
            get
            {
                while (this.paneSettings.Count <= index)
                {
                    PaneSettings ps = new PaneSettings();

                    this.Add(ps);
                }

                return (PaneSettings)this.paneSettings[index];
            }
        }

        public IEnumerator GetEnumerator()
        {
            return this.paneSettings.GetEnumerator();
        }

        public PaneSettingsCollection()
        {
            this.paneSettings = new ArrayList();
        }

        public void Add(PaneSettings paneSettings)
        {
            this.paneSettings.Add(paneSettings);
        }        
    }
}
