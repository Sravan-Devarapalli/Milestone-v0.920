
namespace PraticeManagement.Controls
{
    public partial class CalendarLegend : System.Web.UI.UserControl
    {
        public bool disableChevron
        {
            set
            {
                this.dvCollapsiblePanel.Visible = !value;
            }
        }
    }
}
