using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PraticeManagement.FilterObjects
{
    [Serializable]
    public class ActivityLogFilter
    {
        public string EventSourceSelected { get; set; }
        public DateTime? FromDateFilterValue { get; set; }
        public DateTime? ToDateFilterValue { get; set; }
        public string PersonSelected { get; set; }
        public string ProjectSelected { get; set; }
        public int CurrentIndex { get; set; }
        public bool FiltersChanged { get; set; }
    }
}
