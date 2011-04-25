using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PracticeManagement
{
    public interface IEMailDeliveryProvider
    {
        void SendMail(List<string> to, List<string> cc, string from, string subject, string body);
    }
}

