/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BL.UI
{

    public class TemplateParser
    {


        public TemplateParserResult Parse(String parentId, String templateCssPrefix, String markup)
        {
            int curDepth = 0;
            int controlParsingMode = 0;
            List<int> depthStack = new List<int>();
            StringBuilder resultingMarkup = new StringBuilder();
            int lastWhitespace = 0;
            int ignoreAppendMarkup = 0;
            List<List<Control>> itemStacks = new List<List<Control>>();
            List<String> controlTagNames = new List<String>();

            Control controlToAddItemsTo = null;
            String controlCssPrefix = templateCssPrefix.Replace(".", "-").ToLowerCase();

            depthStack[0] = -1;

            List<Control> controlStack = new List<Control>();
            List<int> controlLevel = new List<int>();

            TemplateParserResult tpr = new TemplateParserResult();

            int nextLeftSign = markup.IndexOf("<");

            while (nextLeftSign >= 0)
            {
                // append most recent website.
                resultingMarkup.Append(markup.Substring(lastWhitespace, nextLeftSign));
 
                // find this tags closer
                int nextRightSign = markup.IndexOf(">", nextLeftSign + 1);
                
                if (nextRightSign > nextLeftSign)
                {
                    // update our depth stack.
                    depthStack[curDepth]++;
                    
                    // is this closer tag? e.g, </foo>
                    if (markup.CharAt(nextLeftSign + 1) == '/')
                    {
                        curDepth--;

                        if (depthStack.Count >= curDepth)
                        {
                            depthStack.RemoveAt(depthStack.Count - 1);
                        }

                        // pop off the current control from the stack if its at or deeper than the current control 
                        if (controlStack.Count > 0 && controlLevel[controlLevel.Count - 1] >= curDepth)
                        {
                            controlLevel.RemoveAt(controlLevel.Count - 1);
                            controlStack.RemoveAt(controlStack.Count - 1);
                        }

                        String tagName = markup.Substring(nextLeftSign + 2, nextRightSign);

                        if (tagName.IndexOf(".") >= 0 || tagName == "Items" || tagName == "Content")
                        {
                            String lastControlTagName = controlTagNames[controlTagNames.Count - 1];

                            controlTagNames.RemoveAt(controlTagNames.Count - 1);
                                
                            if (lastControlTagName == null)
                            {
                                lastControlTagName = "div";
                            }

                            if (ignoreAppendMarkup <= 0)
                            {
                                resultingMarkup.Append("</" + lastControlTagName + ">");
                            }
                        }
                        else if (ignoreAppendMarkup <= 0)
                        {
                            resultingMarkup.Append("</" + tagName + ">");
                        }

                        lastWhitespace = nextRightSign + 1;
                        nextLeftSign = markup.IndexOf("<", lastWhitespace);

                        // if we're within a <Items> section, then end it.
                        if (controlParsingMode > 0 && tagName == "Items")
                        {
                            if (controlToAddItemsTo is ItemsControl)
                            {
                                foreach (Control c in itemStacks[controlParsingMode-1])
                                {
                                    ((ItemsControl)controlToAddItemsTo).AddItemControl(c);
                                }
                            }

                            int lastIndex = controlStack.Count - 1;

                            // look up the control stack to find the previous control that was an items or content control
                            Control lastItemsOrContentControl = controlToAddItemsTo;

                            do
                            {
                                controlToAddItemsTo = controlStack[lastIndex];
                                lastIndex--;
                            }
                            while (lastIndex >= 0 && !( (controlToAddItemsTo is ItemsControl || controlToAddItemsTo is ContentControl) && controlToAddItemsTo != lastItemsOrContentControl));

                            controlParsingMode--;
                            ignoreAppendMarkup--;
                        }
                        // are we in a content control and seeing a <Content>
                        else if (controlParsingMode > 0 && tagName == "Content")
                        {
                            if (controlToAddItemsTo is ContentControl)
                            {
                                // note that there really should only be one control in this set.
                                foreach (Control c in itemStacks[controlParsingMode - 1])
                                {
                                    ((ContentControl)controlToAddItemsTo).Content = c;
                                }
                            }

                            int lastIndex = controlStack.Count - 1;

                            // look up the control stack to find the previous control that was an items or content control
                            Control lastItemsOrContentControl = controlToAddItemsTo;

                            do
                            {
                                controlToAddItemsTo = controlStack[lastIndex];
                                lastIndex--;
                            }
                            while (lastIndex >= 0 && !((controlToAddItemsTo is ItemsControl || controlToAddItemsTo is ContentControl) && controlToAddItemsTo != lastItemsOrContentControl));

                            controlParsingMode--;
                            ignoreAppendMarkup--;
                        }
                    }
                    else
                    {
                        String tagName = null;
                        String attributes = null;
                        Dictionary<String, String> attributeColl = null;
                        String id = null;

                        bool isSingleton = (markup.CharAt(nextRightSign - 1) == '/');

                        // find the start of attributes within the tag
                        int nextSpace = markup.IndexOf(" ", nextLeftSign);

                        if (nextSpace < nextRightSign && nextSpace > nextLeftSign)
                        {
                            tagName = markup.Substring(nextLeftSign + 1, nextSpace);
                            attributes = markup.Substring(nextSpace + 1, nextRightSign);
                            attributeColl = GetAttributes(attributes);

                            if (attributeColl.ContainsKey("id"))
                            {
                                id = attributeColl["id"];
                            }
                        }
                        else
                        {
                            // fetch the tag name
                            if (isSingleton)
                            {
                                tagName = markup.Substring(nextLeftSign + 1, nextRightSign - 1); 
                            }
                            else
                            {
                                tagName = markup.Substring(nextLeftSign + 1, nextRightSign);
                            }
                        }

                        int period = tagName.LastIndexOf(".");

                        if (period >= 0)
                        {
                            Control c = ControlManager.Current.Create(tagName);

                            if (c != null)
                            {
                                if (attributeColl != null)
                                {
                                    this.ApplyAttributesToControl(c, attributeColl);
                                }

                                controlStack.Add(c);
                                controlLevel.Add(curDepth);


                                if (id != null)
                                {
                                    c.Id = parentId + "-" + id;
                                }


                                // if we're within a <Content> or <Items> section, save the control
                                if (controlParsingMode > 0)
                                {
                                    itemStacks[controlParsingMode-1].Add(c);

                                    if (id != null)
                                    {
                                        tpr.AddControl(c, null, id);
                                    }
                                }
                                // otherwise keep editing the resulting markup.
                                else
                                {
                                    tagName = c.TagName;

                                    if (tagName == null) { tagName = "div"; }

                                    if (ignoreAppendMarkup <= 0)
                                    {
                                        resultingMarkup.Append("<" + tagName);
                                    }

                                    tpr.AddControl(c, depthStack.Clone(), id);
                                }
                            }
                            else
                            {
                                Debug.Fail(String.Format("Control {0} declared in template {1} was not found.", tagName, markup));
                            }
                        }
                        else if (tagName == "Items")
                        {
                            if (controlStack.Count > 0)
                            {
                                itemStacks[controlParsingMode] = new List<Control>();
                                controlParsingMode++;
                                controlToAddItemsTo = controlStack[controlStack.Count - 1];
                            }

                            ignoreAppendMarkup++;
                        }
                        else if (tagName == "Content")
                        {
                            if (controlStack.Count > 0)
                            {
                                itemStacks[controlParsingMode] = new List<Control>();
                                controlParsingMode++;
                                controlToAddItemsTo = controlStack[controlStack.Count - 1];
                            }

                            ignoreAppendMarkup++;
                        }
                        else
                        {
                            if (id != null)
                            {
                                tpr.AddElement(depthStack.Clone(), id);
                            }

                            if (tagName == "ItemsContainer")
                            {
                                tpr.ItemsContainer = depthStack.Clone();

                                tagName = "div";
                            }
                            else if (tagName == "ContentContainer")
                            {
                                tpr.ContentContainer = depthStack.Clone();

                                tagName = "div";
                            }

                            if (ignoreAppendMarkup <= 0)
                            {
                                resultingMarkup.Append("<" + tagName);
                            }
                        }

                        if (attributeColl != null && ignoreAppendMarkup <= 0)
                        {
                            String className = attributeColl["class"];

                            if (className == null)
                            {
                                className = String.Empty;
                            }

                            if (id != null)
                            {
                                if (className.Length > 0)
                                {
                                    className += " ";
                                }

                                className += controlCssPrefix + "-" + id;
                            }

                            if (className.Length > 0)
                            {
                                resultingMarkup.Append(" class=\"" + className + "\"");
                            }
                            
                            foreach (KeyValuePair<String, String> kvp in attributeColl)
                            {
                                if (kvp.Key != "id" && kvp.Key != "class")
                                {
                                    if (kvp.Key == "src")
                                    {
                                        resultingMarkup.Append(String.Format(" {0}=\"{1}\"", kvp.Key, Context.Current.ResourceBasePath + kvp.Value));
                                    }
                                    else
                                    {
                                        resultingMarkup.Append(String.Format(" {0}=\"{1}\"", kvp.Key, kvp.Value));
                                    }
                                }
                            }
                        }

                        // is this a singleton tag?  e.g., <foo/>
                        if (isSingleton)
                        {
                            // pop off the current control from the stack if its at or deeper than the current control 
                            if (controlStack.Count > 0 && controlLevel[controlLevel.Count - 1] >= curDepth)
                            {
                                controlLevel.RemoveAt(controlLevel.Count - 1);
                                controlStack.RemoveAt(controlStack.Count - 1);
                            }

                            if (ignoreAppendMarkup <= 0)
                            {
                                resultingMarkup.Append(String.Format("></{0}>", tagName));
                            }
                        }
                        // assume this is a regular tag e.g., <foo>
                        else
                        {
                            if (ignoreAppendMarkup <= 0)
                            {
                                resultingMarkup.Append(">");
                            }
                            curDepth++;

                            controlTagNames.Add(tagName);
                            depthStack[curDepth] = -1;
                        }

                        lastWhitespace = nextRightSign + 1;
                        nextLeftSign = markup.IndexOf("<", lastWhitespace);
                    }
                }
                else
                {
                    Debug.Fail("Closing tag without an end tag detected in template markup.");
                    lastWhitespace = nextLeftSign + 1;
                    nextLeftSign = markup.IndexOf("<", nextLeftSign + 1);
                }
            }
            tpr.Markup = resultingMarkup.ToString();
            return tpr;
        }

        private Dictionary<String, String> GetAttributes(String attributes)
        {
            int nextEqualSign = attributes.IndexOf("=");
            int lastEnd = 0;

            Dictionary<String, String> attributeColl = new Dictionary<string, string>();

            while (nextEqualSign >= 0)
            {
                char separatorChar = attributes.CharAt(nextEqualSign + 1);

                int nextSeparator = attributes.IndexOf(separatorChar, nextEqualSign + 2);

                if (nextSeparator > nextEqualSign)
                {
                    String attributeName = attributes.Substring(lastEnd, nextEqualSign).Trim();
                    String attributeValue = attributes.Substring(nextEqualSign + 2, nextSeparator);


                    attributeColl[attributeName] = attributeValue;

                    lastEnd = nextSeparator + 2;
                    nextEqualSign = attributes.IndexOf("=", lastEnd);
                }
                else
                {
                    nextEqualSign = attributes.IndexOf("=", nextEqualSign + 1);
                }

            }

            return attributeColl;
        }

        private void ApplyAttributesToControl(Control c, Dictionary<String, String> attributeColl)
        {
            foreach (KeyValuePair<String, String> kvp in attributeColl)
            {
                if (!IsRestrictedHtmlPropertyName(kvp.Key))
                {
                    c.SetProperty(kvp.Key, kvp.Value);
                }
            }
        }

        private bool IsRestrictedHtmlPropertyName(String propName)
        {
            if (propName == "id" || propName == "style" || propName == "class" || propName == "width" || propName == "height")
            {
                return true;
            }

            return false;
        }
    }
}
