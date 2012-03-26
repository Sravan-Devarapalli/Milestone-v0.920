<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BillableAndNonBillableGraph.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.BillableAndNonBillableGraph" %>
<script type="text/javascript">
    function pageLoad() {
        ChangeBar();
    }

    function ChangeBar() {
        var biilable = document.getElementById('<%= hdnBillable.ClientID %>');
        var nonbiilable = document.getElementById('<%= hdnNonBillable.ClientID %>');
        var lblBillablePercent = document.getElementById('<%= lblBillablePercent.ClientID %>');
        var billableTD = document.getElementById('<%= billableTD.ClientID %>');
        var nonbillableTD = document.getElementById('<%= nonbillableTD.ClientID %>');
        if (billableTD != null && nonbillableTD != null) {
            var biilableval = Number(biilable.value);
            var nonbiilableval = Number(nonbiilable.value);
            if (biilableval == 0 && nonbiilableval == 0) {

                billableTD.style.backgroundColor = "white";
                billableTD.style.width = "600px";
                billableTD.innerHTML = "";

                nonbillableTD.style.width = "0px";
                nonbillableTD.style.backgroundColor = "white";
                nonbillableTD.innerHTML = "";

                lblBillablePercent.innerHTML = "";

            }
            else {
                var billablePercent = parseInt((100 * biilableval / (biilableval + nonbiilableval)));
                var nonbillablePercent = 100 - billablePercent;

                billableTD.style.backgroundColor = "#7fd13b";
                billableTD.style.width = (billablePercent * 6) + "px";
                billableTD.innerHTML = biilableval != 0 ? biilableval : "";

                nonbillableTD.style.backgroundColor = "#BEBEBE";
                nonbillableTD.style.width = (nonbillablePercent * 6) + "px";
                nonbillableTD.innerHTML = nonbiilableval != 0 ? nonbiilableval : "";

                lblBillablePercent.innerHTML = billablePercent + "% Billable";
            }
        }
    }

</script>
<asp:HiddenField ID="hdnBillable" runat="server" />
<asp:HiddenField ID="hdnNonBillable" runat="server" />
<table style="width: 600px;height:90px;">
    <tr style="height: 54px; border: 1px solid black;">
        <td valign="middle" id="billableTD" runat="server" style="text-align: center; color: Black;
            vertical-align: middle; height: 54px;">
        </td>
        <td valign="middle" id="nonbillableTD" runat="server" style="text-align: center;
            color: Black; vertical-align: middle; height: 54px;">
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblBillablePercent" runat="server"></asp:Label>
        </td>
        <td>
        </td>
    </tr>
</table>

