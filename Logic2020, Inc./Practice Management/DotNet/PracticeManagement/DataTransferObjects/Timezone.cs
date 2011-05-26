using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    /// <summary>
    /// Represents a TimeZone entity
    /// </summary>
    [DataContract]
    [Serializable]
    public class Timezone
    {
        /// <summary>
        /// Gets or sets a <see cref="TimeZone"/> Id value.
        /// </summary>
        [DataMember]
        public int Id
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets a  <see cref="TimeZone"/> GMT value.
        /// </summary>
        [DataMember]
        public string GMT
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets GMTName <see cref="TimeZone"/>.
        /// </summary>
        [DataMember]
        public string GMTName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a <see cref="TimeZone"/> IsActive value.
        /// </summary>
        [DataMember]
        public bool IsActive
        {
            get;
            set;
        }
    }
}

