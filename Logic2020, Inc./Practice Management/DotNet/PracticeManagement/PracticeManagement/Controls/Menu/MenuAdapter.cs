using System;
using System.IO;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace PraticeManagement.Controls.Menu
{
    public class MenuAdapter : System.Web.UI.WebControls.Adapters.MenuAdapter
    {
        private PraticeManagement.Controls.Adapters.WebControlAdapterExtender _extender = null;
        private PraticeManagement.Controls.Adapters.WebControlAdapterExtender Extender
        {
            get
            {
                if (((_extender == null) && (Control != null)) ||
                    ((_extender != null) && (Control != _extender.AdaptedControl)))
                {
                    _extender = new PraticeManagement.Controls.Adapters.WebControlAdapterExtender(Control);
                }

                System.Diagnostics.Debug.Assert(_extender != null, "CSS Friendly adapters internal error", "Null extender instance");
                return _extender;
            }
        }

        private string CssPrefix
        {
            get { return (Control is CssFriendlyMenu) && !string.IsNullOrEmpty((Control as CssFriendlyMenu).CssClassPrefix) ? (Control as CssFriendlyMenu).CssClassPrefix : "AspNet-Menu"; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Extender.AdapterEnabled)
            {
                RegisterScripts();
            }
        }

        private void RegisterScripts()
        {
            Extender.RegisterScripts();

			/* 
			 * Modified for support of compiled CSSFriendly assembly
			 * 
			 * We will first search for embedded JavaScript files. If they are not
			 * found, we default to the standard approach.
			 */

			Type type = this.GetType();

			// TreeViewAdapter.js
            //string resource = "PraticeManagement.Scripts.MenuAdapter.js";
            //string filePath = Page.ClientScript.GetWebResourceUrl(type, resource);
            string resource = "MenuAdapter.js";
            string filePath = string.Empty;
			
			// if filePath is empty, use the old approach
			if ( String.IsNullOrEmpty(filePath) )
			{
				string folderPath = WebConfigurationManager.AppSettings.Get("CSSFriendly-JavaScript-Path");
				if (String.IsNullOrEmpty(folderPath))
				{
                    folderPath = "~/Scripts/";
				}
				filePath = folderPath.EndsWith("/") ? folderPath + "MenuAdapter.js" : folderPath + "/TreeViewAdapter.js";
			}

            if (!Page.ClientScript.IsClientScriptIncludeRegistered(resource))
                Page.ClientScript.RegisterClientScriptInclude(resource, Page.ResolveUrl(filePath));

			// Menu.css -- only add if it is embedded
            resource = "Menu.css";
            filePath = string.Empty;
			
			// if filePath is not empty, embedded CSS exists -- register it
			if (!String.IsNullOrEmpty(filePath))
			{
				string cssTag = "<link href=\"" + Page.ResolveUrl(filePath) + "\" type=\"text/css\" rel=\"stylesheet\"></link>";
				if (!Page.ClientScript.IsClientScriptBlockRegistered(type, resource))
					Page.ClientScript.RegisterClientScriptBlock(type, resource, cssTag, false);
			}

			// IEMenu6.css -- only add if it is embedded
            resource = "IEMenu6.css";
            filePath = string.Empty;
			
			// if filePath is not empty, embedded CSS exists -- register it
			if (!String.IsNullOrEmpty(filePath))
			{
				string cssTag = "<link href=\"" + Page.ResolveUrl(filePath) + "\" type=\"text/css\" rel=\"stylesheet\"></link>";
				if (!Page.ClientScript.IsClientScriptBlockRegistered(type, resource))
					Page.ClientScript.RegisterClientScriptBlock(type, resource, cssTag, false);
			}
		}

        protected override void RenderBeginTag(HtmlTextWriter writer)
        {
            if (Extender.AdapterEnabled)
            {
                Extender.RenderBeginTag(writer, string.Format("{0}-{1}", CssPrefix, Control.Orientation.ToString()));
            }
            else
            {
                base.RenderBeginTag(writer);
            }
        }

        protected override void RenderEndTag(HtmlTextWriter writer)
        {
            if (Extender.AdapterEnabled)
            {
                Extender.RenderEndTag(writer);
            }
            else
            {
                base.RenderEndTag(writer);
            }
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (Extender.AdapterEnabled)
            {
                writer.Indent++;
                BuildItems(Control.Items, true, writer, CssPrefix);
                writer.Indent--;
                writer.WriteLine();
            }
            else
            {
                base.RenderContents(writer);
            }
        }

        private void BuildItems(MenuItemCollection items, bool isRoot, HtmlTextWriter writer, string prefix)
        {
            if (items.Count > 0)
            {
                writer.WriteLine();

                writer.WriteBeginTag("ul");
                if (isRoot)
                {
                    writer.WriteAttribute("class", prefix);
                }
                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Indent++;
                               
                foreach (MenuItem item in items)
                {
                    BuildItem(item, writer, prefix);
                }

                writer.Indent--;
                writer.WriteLine();
                writer.WriteEndTag("ul");
            }
        }

        private void BuildItem(MenuItem item, HtmlTextWriter writer, string prefix)
        {
            var menu = Control as System.Web.UI.WebControls.Menu;
            if ((menu != null) && (item != null) && (writer != null))
            {
                writer.WriteLine();
                writer.WriteBeginTag("li");

                string theClass = string.Format("{0}-{1}", prefix,(item.ChildItems.Count > 0) ? "WithChildren" : "Leaf");
                string selectedStatusClass = GetSelectStatusClass(item, prefix);
                if (!String.IsNullOrEmpty(selectedStatusClass))
                {
                    theClass += selectedStatusClass;
                }
                writer.WriteAttribute("class", theClass);

                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Indent++;
                writer.WriteLine();

                if (((item.Depth < menu.StaticDisplayLevels) && (menu.StaticItemTemplate != null)) ||
                    ((item.Depth >= menu.StaticDisplayLevels) && (menu.DynamicItemTemplate != null)))
                {
                    writer.WriteBeginTag("div");
                    writer.WriteAttribute("class", GetItemClass(menu, item, prefix));
                    writer.Write(HtmlTextWriter.TagRightChar);
                    writer.Indent++;
                    writer.WriteLine();

                    MenuItemTemplateContainer container = new MenuItemTemplateContainer(menu.Items.IndexOf(item), item);
                    if ((item.Depth < menu.StaticDisplayLevels) && (menu.StaticItemTemplate != null))
                    {
                        menu.StaticItemTemplate.InstantiateIn(container);
                    }
                    else
                    {
                        menu.DynamicItemTemplate.InstantiateIn(container);
                    }
                    container.DataBind();
                    container.RenderControl(writer);

                    writer.Indent--;
                    writer.WriteLine();
                    writer.WriteEndTag("div");
                }
                else
                {
                    if (IsLink(item))
                    {
                        writer.WriteBeginTag("a");
                        if (!String.IsNullOrEmpty(item.NavigateUrl))
                        {
                            writer.WriteAttribute("href", Page.Server.HtmlEncode(menu.ResolveClientUrl(item.NavigateUrl)));
                        }
                        else
                        {
                            writer.WriteAttribute("href", Page.ClientScript.GetPostBackClientHyperlink(menu, "b" +  item.ValuePath.Replace(menu.PathSeparator.ToString(), "\\"), true));
                        }

                        writer.WriteAttribute("class", GetItemClass(menu, item, prefix));
                        PraticeManagement.Controls.Adapters.WebControlAdapterExtender.WriteTargetAttribute(writer, item.Target);

                        if (!String.IsNullOrEmpty(item.ToolTip))
                        {
                            writer.WriteAttribute("title", item.ToolTip);
                        }
                        else if (!String.IsNullOrEmpty(menu.ToolTip))
                        {
                            writer.WriteAttribute("title", menu.ToolTip);
                        }
                        writer.Write(HtmlTextWriter.TagRightChar);
                        writer.Indent++;
                        writer.WriteLine();
                    }
                    else
                    {
                        writer.WriteBeginTag("span");
                        writer.WriteAttribute("class", GetItemClass(menu, item, prefix));
                        writer.Write(HtmlTextWriter.TagRightChar);
                        writer.Indent++;
                        writer.WriteLine();
                    }

                    if (!String.IsNullOrEmpty(item.ImageUrl))
                    {
                        writer.WriteBeginTag("img");
                        writer.WriteAttribute("src", menu.ResolveClientUrl(item.ImageUrl));
                        writer.WriteAttribute("alt", !String.IsNullOrEmpty(item.ToolTip) ? item.ToolTip : (!String.IsNullOrEmpty(menu.ToolTip) ? menu.ToolTip : item.Text));
                        writer.Write(HtmlTextWriter.SelfClosingTagEnd);
                    }

                    writer.Write(item.Text);

                    if (IsLink(item))
                    {
                        writer.Indent--;
                        writer.WriteEndTag("a");
                    }
                    else
                    {
                        writer.Indent--;
                        writer.WriteEndTag("span");
                    }

                }

                if ((item.ChildItems != null) && (item.ChildItems.Count > 0))
                {
                    BuildItems(item.ChildItems, false, writer, prefix);
                }

                writer.Indent--;
                writer.WriteLine();
                writer.WriteEndTag("li");
            }
        }

        private bool IsLink(MenuItem item)
        {
            return (item != null) && item.Enabled && ((!String.IsNullOrEmpty(item.NavigateUrl)) || item.Selectable);
        }

        private string GetItemClass(System.Web.UI.WebControls.Menu menu, MenuItem item, string prefix)
        {
            string value = string.Format("{0}-NonLink", prefix);
            if (item != null)
            {
                if (((item.Depth < menu.StaticDisplayLevels) && (menu.StaticItemTemplate != null)) ||
                    ((item.Depth >= menu.StaticDisplayLevels) && (menu.DynamicItemTemplate != null)))
                {
                    value = string.Format("{0}-Template", prefix);
                }
                else if (IsLink(item))
                {
                    value = string.Format("{0}-Link", prefix);
                }
                string selectedStatusClass = GetSelectStatusClass(item, prefix);
                if (!String.IsNullOrEmpty(selectedStatusClass))
                {
                    value += " " + selectedStatusClass;
                }
            }
            return value;
        }

        private string GetSelectStatusClass(MenuItem item, string prefix)
        {
            string value = "";
            if (item.Selected)
            {
                value += string.Format(" {0}-Selected", prefix);
            }
            else if (IsChildItemSelected(item))
            {
                value += string.Format(" {0}-ChildSelected", prefix);
            }
            else if (IsParentItemSelected(item))
            {
                value += string.Format(" {0}-ParentSelected", prefix);
            }
            return value;
        }

        private bool IsChildItemSelected(MenuItem item)
        {
            bool bRet = false;

            if ((item != null) && (item.ChildItems != null))
            {
                bRet = IsChildItemSelected(item.ChildItems);
            }

            return bRet;
        }

        private bool IsChildItemSelected(MenuItemCollection items)
        {
            bool bRet = false;

            if (items != null)
            {
                foreach (MenuItem item in items)
                {
                    if (item.Selected || IsChildItemSelected(item.ChildItems))
                    {
                        bRet = true;
                        break;
                    }
                }
            }

            return bRet;
        }

        private bool IsParentItemSelected(MenuItem item)
        {
            bool bRet = false;

            if ((item != null) && (item.Parent != null))
            {
                if (item.Parent.Selected)
                {
                    bRet = true;
                }
                else
                {
                    bRet = IsParentItemSelected(item.Parent);
                }
            }

            return bRet;
        }
    }
}

