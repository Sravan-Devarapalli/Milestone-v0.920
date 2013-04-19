using System;
using System.Runtime.Serialization;
using System.Web;

namespace DataTransferObjects
{
    [DataContract]
    [Serializable]
    public class BusinessGroup
    {
        #region Fields

        private string _name;
        private bool _inUse;
        private bool _isActive;

        #endregion

        #region Properties

        public static string DefaultBusinessGroupName
        {
            get { return Constants.GroupNames.DefaultBusinessGroupName; }
        }

        public static string DefaultBusinessGroupCode
        {
            get { return Constants.GroupCodes.DefaultBusinessGroupCode; }
        }

        /// <summary>
        /// System-generated identifier for the client
        /// </summary>
        [DataMember]
        public int? Id
        {
            get;
            set;
        }

        [DataMember]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string HtmlEncodedName
        {
            get
            {
                return HttpUtility.HtmlEncode(Name);
            }
        }

        /// <summary>
        /// True if client have any project with this group.
        /// </summary>
        [DataMember]
        public bool InUse
        {
            get { return _inUse; }
            set { _inUse = value; }
        }

        [DataMember]
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public int ClientId { get; set; }

        #endregion
    }
}

