using System.Web.UI;
using System.Web.UI.WebControls;

namespace PraticeManagement.Controls.Generic.Buttons
{
    public class ShadowedHyperlink : HyperLink
    {
        //Renders custom button with the following markup        
        //<span class="btn-body"> 
        //    <span class="btn-text">Add Anything What You Need Here Twice</span> 
        //    <span class="btn-text-shadow">Add Anything What You Need Here Twice</span> 
        //</span> 

        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.AddAttribute("class", "btn-body");
            writer.RenderBeginTag(HtmlTextWriterTag.Span);

            writer.AddAttribute("class", "btn-text");
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(Text);
            writer.RenderEndTag();

            writer.AddAttribute("class", "btn-text-shadow");
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(Text);
            writer.RenderEndTag();

            writer.RenderEndTag();
        }
    }
}
