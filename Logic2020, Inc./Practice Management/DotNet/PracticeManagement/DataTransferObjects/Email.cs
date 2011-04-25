using System.Collections.Generic;
namespace DataTransferObjects
{
    public class Email
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> To { get; private set; }
        public List<string> Cc { get; private set; }

        public Email()
        {
            To = new List<string>();
            Cc = new List<string>();
        }
    }
}

