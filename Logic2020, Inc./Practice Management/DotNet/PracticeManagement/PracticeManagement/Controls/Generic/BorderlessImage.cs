using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PraticeManagement.Controls.Generic
{
    public class BorderlessImage : System.Web.UI.WebControls.Image
    {
        public override System.Web.UI.WebControls.Unit BorderWidth
        {
            get
            {
                if (base.BorderWidth.IsEmpty)
                {
                    return System.Web.UI.WebControls.Unit.Pixel(0);
                }
                else
                {
                    return base.BorderWidth;
                }
            }
            set
            {
                base.BorderWidth = value;
            }
        }
    }
}
