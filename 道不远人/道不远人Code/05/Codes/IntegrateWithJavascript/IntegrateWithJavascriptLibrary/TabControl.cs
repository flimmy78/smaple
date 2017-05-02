using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Web.UI.Design;

[assembly: WebResource("IntegrateWithJavascriptLibrary.tab.js", "text/javascript")]
[assembly: WebResource("IntegrateWithJavascriptLibrary.Tabs.css", "text/css", PerformSubstitution = true)]
[assembly: WebResource("IntegrateWithJavascriptLibrary.tab-line.gif", "image/gif")]
[assembly: WebResource("IntegrateWithJavascriptLibrary.tab.gif", "image/gif")]
[assembly: WebResource("IntegrateWithJavascriptLibrary.tab-left.gif", "image/gif")]
[assembly: WebResource("IntegrateWithJavascriptLibrary.tab-right.gif", "image/gif")]
[assembly: WebResource("IntegrateWithJavascriptLibrary.tab-hover.gif", "image/gif")]
[assembly: WebResource("IntegrateWithJavascriptLibrary.tab-hover-left.gif", "image/gif")]
[assembly: WebResource("IntegrateWithJavascriptLibrary.tab-hover-right.gif", "image/gif")]
[assembly: WebResource("IntegrateWithJavascriptLibrary.tab-active.gif", "image/gif")]
[assembly: WebResource("IntegrateWithJavascriptLibrary.tab-active-left.gif", "image/gif")]
[assembly: WebResource("IntegrateWithJavascriptLibrary.tab-active-right.gif", "image/gif")]

namespace IntegrateWithJavascriptLibrary
{
    [ParseChildren(typeof(TabPage))]
    [ControlBuilder(typeof(TabBuilder))]
    [Designer(typeof(ContainerControlDesigner))]
    public class TabControl:WebControl,ICallbackEventHandler
    {
        bool _supportJS;

        [DefaultValue(0)]
        [Category("Behavior")]
        public int SelectedTabIndex
        {
            get
            {
                if (ViewState["SelectedTabIndex"] == null)
                    return 0;
                else
                    return (int)ViewState["SelectedTabIndex"];
            }
            set
            {
                ViewState["SelectedTabIndex"] = value;
            }
        }

        protected override void AddParsedSubObject(object obj)
        {
            if (obj is TabPage)
                base.AddParsedSubObject(obj);
        }

        string GetActivedTabContent()
        {
            TabPage activedTab = this.Controls[SelectedTabIndex] as TabPage;
            StringBuilder sb = new StringBuilder();
            StringWriter tw = new StringWriter(sb);
            HtmlTextWriter htw = new HtmlTextWriter(tw);
            activedTab.RenderControl(htw);
            return sb.ToString();
        }

        #region ICallbackEventHandler 成员

        public string GetCallbackResult()
        {
            return GetActivedTabContent();
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            SelectedTabIndex = int.Parse(eventArgument);
        }

        #endregion

        void DetermineJS()
        {
            if (!DesignMode)
            {
                if (Page.Request.Browser.EcmaScriptVersion.Major > 0 && Page.Request.Browser.W3CDomVersion.Major > 0)
                {
                    this._supportJS = true;
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            RegisterClientCSSResource("IntegrateWithJavascriptLibrary.Tabs.css");
            DetermineJS();
            if (_supportJS)
            {
                Page.ClientScript.RegisterClientScriptResource(this.GetType(), "IntegrateWithJavascriptLibrary.jquery.js");
                Page.ClientScript.RegisterClientScriptResource(this.GetType(), "IntegrateWithJavascriptLibrary.tab.js");
            }

            base.OnPreRender(e);
        }

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "ajax__tab_xp ajax__tab_container ajax__tab_default");
            writer.AddAttribute(HtmlTextWriterAttribute.Id, this.UniqueID);
            writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "ajax__tab_header");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            for (int i = 0; i < Controls.Count; i++)
            {
                TabPage tab = Controls[i] as TabPage;
                if (i == SelectedTabIndex)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "ajax__tab_active");
                }
                string callbackRef = string.Format("tabsOnCallback('{0}',{1});{2};" ,
                    this.ClientID,i.ToString(),
                    Page.ClientScript.GetCallbackEventReference(this, i.ToString(), "tabsOnResult","'"+this.ClientID+"'",true));
                writer.AddAttribute(HtmlTextWriterAttribute.Onclick, callbackRef);
                writer.RenderBeginTag(HtmlTextWriterTag.Span);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "ajax__tab_outer");
                writer.RenderBeginTag(HtmlTextWriterTag.Span);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "ajax__tab_inner");
                writer.RenderBeginTag(HtmlTextWriterTag.Span);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "ajax__tab_tab");
                writer.RenderBeginTag(HtmlTextWriterTag.Span);
                writer.Write(tab.Title);
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderEndTag();
            }
            writer.RenderEndTag();
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "ajax__tab_body");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "ajax__tab_panel");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            Controls[SelectedTabIndex].RenderControl(writer);
            writer.RenderEndTag();
            writer.RenderEndTag();
        }

        void RegisterClientCSSResource(string cssResource)
        {
            if (Page.Header != null)
            {
                string cssId = cssResource.Replace('.', '_');
                foreach (Control ctr in Page.Header.Controls)
                {
                    if (ctr.ID == cssId)
                        return;
                }
                string cssRef = Page.ClientScript.GetWebResourceUrl(this.GetType(), cssResource);
                HtmlLink link = new HtmlLink();
                link.Href = cssRef;
                link.Attributes.Add("type", "text/css");
                link.Attributes.Add("rel", "stylesheet");
                Page.Header.Controls.Add(link);
            }
            else
            {
                throw new NotSupportedException("页面没有Header对象");
            }
        }
    }


    internal class TabBuilder : ControlBuilder
    {
        public override bool AllowWhitespaceLiterals()
        {
            return false;
        }

        public override Type GetChildControlType(string tagName, System.Collections.IDictionary attribs)
        {
            if (string.Compare(tagName, "tab", true) == 0)
            {
                return typeof(TabPage);
            }
            return null;
        }
    }
}
