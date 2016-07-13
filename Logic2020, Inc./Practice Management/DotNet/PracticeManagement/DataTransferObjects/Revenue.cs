using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [Serializable]
    [DataContract]
    public class Revenue
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool IsDefault { get; set; }

        public static int Compare(Revenue obj1, Revenue obj2)
        {
            if (obj1 == null && obj2 != null)
            {
                return -1;
            }
            else if (obj1 != null && obj2 == null)
            {
                return 1;
            }
            else if (obj1 == null && obj2 == null)
            {
                return 0;
            }
            else
            {
                return string.Compare(obj1.Name, obj2.Name);
            }
        }
    }
}

