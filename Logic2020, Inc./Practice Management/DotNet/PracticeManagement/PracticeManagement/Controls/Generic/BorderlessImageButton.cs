using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;

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

        protected override bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            // Control coordinates are sent in decimal by IE10 
            // Recreating the collection with corrected values            
            NameValueCollection modifiedPostCollection = new NameValueCollection();
            for (int i = 0; i < postCollection.Count; i++)
            {
                string actualKey = postCollection.GetKey(i);
                if (actualKey != null)
                {
                    string[] actualValueTab = postCollection.GetValues(i);
                    if (actualKey.EndsWith(".x") || actualKey.EndsWith(".y"))
                    {
                        string value = actualValueTab[0];
                        decimal dec;
                        Decimal.TryParse(value, out dec);
                        modifiedPostCollection.Add(actualKey, ((int)Math.Round(dec)).ToString());
                    }
                    else
                    {
                        foreach (string actualValue in actualValueTab)
                        {
                            modifiedPostCollection.Add(actualKey, actualValue);
                        }
                    }
                }
            }
            return base.LoadPostData(postDataKey, modifiedPostCollection);
        }

    }
}


