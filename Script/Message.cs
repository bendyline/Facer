using System;
using System.Collections.Generic;
using BL.UI;
using System.Runtime.CompilerServices;
using System.Html;

namespace BL.UI
{
    public enum MessageType
    {
        OKOnly = 0,
        OKCancel = 1
    }

    public class Message : Control, IDialogManager
    {
        private Dialog parentDialog;

        [ScriptName("e_ok")]
        private InputElement okButton;

        [ScriptName("e_cancel")]
        private InputElement cancelButton;

        [ScriptName("e_message")]
        private InputElement messageArea;

        [ScriptName("e_heading")]
        private InputElement headingArea;

        private MessageType type;

        private bool isAffirm = false;
        private String content;
        private String heading;

        private Action affirmAction;
        private Action closeAction;

        public MessageType Type
        {
            get
            {
                return this.type;
            }

            set
            {
                this.type = value;

                this.Update();
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

                
                this.Update();
            }
        }
        
        public String Heading
        {
            get
            {
                return this.heading;
            }

            set
            {
                this.heading = value;

                
                this.Update();
            }
        }

        internal Action AffirmAction
        {
            get
            {
                return this.affirmAction;
            }

            set
            {
                this.affirmAction = value;
            }
        }

        internal Action CloseAction
        {
            get
            {
                return this.closeAction;
            }

            set
            {
                this.closeAction = value;
            }
        }

        public Dialog ParentDialog
        {
            get
            {
                return this.parentDialog;
            }

            set
            {
                this.parentDialog = value;

                this.parentDialog.Closing += parentDialog_Closing;
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.okButton != null)
            {
                this.okButton.AddEventListener("click", this.HandleOkButton, true);
            }

            if (this.cancelButton != null)
            {
                this.cancelButton.AddEventListener("click", this.HandleCancelButton, true);
            }
        }

        protected override void OnUpdate()
        {
            if (this.messageArea != null)
            {
                if (String.IsNullOrEmpty(this.content))
                {
                    this.messageArea.Style.Display = "none";
                }
                else
                {
                    ElementUtilities.SetText(this.messageArea, this.content);
                    this.messageArea.Style.Display = "";
                }
            }
            
            if (this.headingArea != null)
            {
                if (String.IsNullOrEmpty(this.heading))
                {
                    this.headingArea.Style.Display = "none";
                }
                else
                {
                    ElementUtilities.SetText(this.headingArea, this.heading);
                    this.headingArea.Style.Display = "";
                }
            }

            if (this.cancelButton != null)
            {
                if (this.Type == MessageType.OKOnly)
                {
                    this.cancelButton.Style.Display = "none";
                }
                else
                {
                    this.cancelButton.Style.Display = "";
                }
            }
        }

        private void parentDialog_Closing(object sender, EventArgs e)
        {
            if (this.isAffirm && this.type == MessageType.OKCancel)
            {
                if (this.affirmAction != null)
                {
                    this.affirmAction();
                }
            }
            else
            {
                if (this.closeAction != null)
                {
                    this.closeAction();
                }
            }
        }

        private void HandleOkButton(ElementEvent ee)
        {
            if (this.parentDialog != null)
            {
                this.isAffirm = true;
                this.parentDialog.Hide();
            }
        }

        private void HandleCancelButton(ElementEvent ee)
        {
            if (this.parentDialog != null)
            {
                this.parentDialog.Hide();
            }
        }

        public static void ShowMessage(String heading, String content, Action closeAction)
        {
            Show(heading, content, MessageType.OKOnly, null, closeAction);
        }

        public static void Show(String heading, String content, MessageType messageType, Action affirmAction, Action closeAction)
        {
            Message m = new Message();
            m.Content = content;
            m.Heading = heading;
            m.Type = messageType;

            m.AffirmAction = affirmAction;
            m.CloseAction = closeAction;

            Dialog d = new Dialog();
            
            d.Content = m;
            d.DisplayCloseButton = false;
            
            if (Context.Current.IsSmallFormFactor || Window.InnerWidth < 420)
            {
                d.MaxWidth = 310;
                d.MaxHeight = 290;
            }
            else
            {
                d.MaxWidth = 400;
                d.MaxHeight = 290;
            }
            
            d.Show();
        }
    }
}
