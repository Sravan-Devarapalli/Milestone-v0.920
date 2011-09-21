using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PraticeManagement.Controls
{
    public abstract class PracticeManagementSearchPageBase : PracticeManagementPageBase
    {
        protected override void Display()
        {
            
        }

        public abstract string SearchText
        {
            get;
        }
    }
}
