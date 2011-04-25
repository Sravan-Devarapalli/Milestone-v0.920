using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    /// <summary>
    /// Data Transfer Object for a Client entity
    /// </summary>
    [DataContract]
    [Serializable]
    public class ProjectGroup
    {
        #region Fields

        private string _name;
        private bool _inUse;
        private bool _isActive;

        #endregion

        #region Properties

        public static string DefaultGroupName
        {
            get { return Constants.GroupNames.DefaultGroupName; }
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

        /// <summary>
        /// A Group has a name
        /// </summary>
        [DataMember]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
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

        #endregion
    }
}

