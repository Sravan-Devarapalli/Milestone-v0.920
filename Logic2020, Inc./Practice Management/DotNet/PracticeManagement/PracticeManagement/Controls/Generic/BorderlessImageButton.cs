using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PraticeManagement.Controls.Generic
{
    public class BorderlessImageButton : System.Web.UI.WebControls.ImageButton
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


