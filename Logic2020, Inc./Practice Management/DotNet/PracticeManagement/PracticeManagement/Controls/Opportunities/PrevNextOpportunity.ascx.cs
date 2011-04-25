using System;
using System.Web.UI.WebControls;
using DataTransferObjects;
using PraticeManagement.OpportunityService;
using System.ServiceModel;
using System.Web;
using System.Collections.Generic;
using PraticeManagement.Utils;

namespace PraticeManagement.Controls.Opportunities
{
    public partial class PrevNextOpportunity : System.Web.UI.UserControl
    {
        //private string IndexParam
        //{
        //    get
        //    {
        //        return Request.QueryString[PraticeManagement.Constants.QueryStringParameterNames.Index];
        //    }
        //}

        //private int Index
        //{
        //    get
        //    {
        //        return IndexParam == null ? -1 : int.Parse(IndexParam);
        //    }
        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                odsPrevNext.Select();
        }

        protected string GetOpportinityDetailsLink(int opportunityId)
        {
            var detailsLink = Utils.Urls.OpportunityDetailsLink(opportunityId);

            return Utils.Generic.GetTargetUrlWithReturn(detailsLink, Request.Url.AbsoluteUri);
        }

        //protected void ProjectSelected(object source, ObjectDataSourceStatusEventArgs e)
        //{
        //    var res = e.ReturnValue as Opportunity[];

        //    var index = Index;
        //    if (res != null && res.Length > 0 && index >= 0)
        //    {                
        //        if (res.Length == 2)
        //        {
        //            hlRight.NavigateUrl = GetOpportinityDetailsLink(res[1].Id.Value, index + 1);
        //            lblRight.InnerText= res[1].Client.Name;
        //            captionRight.InnerText = Utils.Generic.AddEllipsis(res[1].Name, 30, Constants.Formatting.Ellipsis);
        //            divRight.Visible = true;

        //            crStatusRight.ButtonCssClass = OpportunitiesHelper.GetIndicatorClassByStatus(res[1].Status.Name);
        //            crStatusRight.ButtonProjectNameToolTip = res[1].Status.Name;
        //        }
        //        else
        //        {
        //            if (index == 0)
        //            {
        //                divLeft.Visible = false;

        //                hlRight.NavigateUrl = GetOpportinityDetailsLink(res[0].Id.Value, index + 1);
        //                lblRight.InnerText = res[0].Client.Name;
        //                captionRight.InnerText = Utils.Generic.AddEllipsis(res[0].Name, 30, Constants.Formatting.Ellipsis);

        //                crStatusRight.ButtonCssClass = OpportunitiesHelper.GetIndicatorClassByStatus(res[0].Status.Name);
        //                crStatusRight.ButtonProjectNameToolTip = res[0].Status.Name;

        //                return;
        //            }
        //            else
        //            {
        //                divRight.Visible = false;
        //            }
        //        }

        //        hlLeft.NavigateUrl = GetOpportinityDetailsLink(res[0].Id.Value, index - 1);
        //        captionLeft.InnerText = Utils.Generic.AddEllipsis(res[0].Name, 30, Constants.Formatting.Ellipsis);
        //        lblLeft.InnerText = res[0].Client.Name;

        //        crStatusLeft.ButtonCssClass = OpportunitiesHelper.GetIndicatorClassByStatus(res[0].Status.Name);
        //        crStatusLeft.ButtonProjectNameToolTip = res[0].Status.Name;
        //    }
        //    else
        //    {
        //        divPrevNextMainContent.Visible = false;
        //    }
        //}        
    }
}
