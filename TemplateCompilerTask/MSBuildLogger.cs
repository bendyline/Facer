using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Bendyline.Base
{
    public class MSBuildLogger
    {
        private static MSBuildLogger current;
        private bool initialized = false;
        private Task task;

        public Task Task
        {
            get
            {
                return task;
            }

            set
            {
                task = value;
            }
        }

        public static MSBuildLogger Current
        {
            get
            {
                if (current == null)
                {
                    current = new MSBuildLogger();
                }

                return current;
            }
        }

        public MSBuildLogger()
        {
        }

        public void Initialize(Task task)
        {
            if (initialized)
            {
                return;
            }

            this.task = task;

            Log.ItemAdded += new LogItemEventHandler(Log_ItemAdded);

            initialized = true;
        }

        private void Log_ItemAdded(object sender, LogItemEventArgs e)
        {
            try
            {
                if (e.Item.Status == LogStatus.UnexpectedError || e.Item.Status == LogStatus.Critical)
                {
                    this.task.Log.LogError(e.Item.Message);
                }
                else
                {
                    this.task.Log.LogMessage(MessageImportance.Normal, e.Item.Message);
                }
            }
            catch (Exception) 
            { 
                ; 
            }
        }
    }
}
