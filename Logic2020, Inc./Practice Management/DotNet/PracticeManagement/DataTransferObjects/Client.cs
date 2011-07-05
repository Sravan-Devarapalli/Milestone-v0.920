using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    /// <summary>
    /// Data Transfer Object for a Client entity
    /// </summary>
    [DataContract]
	[Serializable]
	public class Client : IEquatable<Client>
	{
		#region Fields

		private decimal _defaultDiscount;
        private int _defaultTerms;
        private bool _inactive;
        private string _name;

		#endregion

		#region Properties

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
        /// Clients have a common discount, which can be used to
        /// set a project's discount when first created
        /// </summary>
        [DataMember]
        public decimal DefaultDiscount
        {
            get { return _defaultDiscount; }
            set { _defaultDiscount = value; }
        }

        /// <summary>
        /// Terms are expressed in "net x" where
        /// x is the number of days from the time
        /// the invoice is delivered before payment is expected
        /// 
        /// A client generally has terms for any project, so a
        /// project can start with this value when created
        /// </summary>
        [DataMember]
        public int DefaultTerms
        {
            get { return _defaultTerms; }
            set { _defaultTerms = value; }
        }

		/// <summary>
        /// A client has a name.
        /// </summary>
        /// <remarks>
        /// A client may have a plethora of other information
        /// we find useful.  Right now, name is the attribute
        /// for display.
        /// 
        /// This is a candidate or candidate key.
        /// </remarks>
        [DataMember]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Client can be filtered from display if this is true.
        /// </summary>
        /// <remarks>
        /// It should be rare that a client has any other value but
        /// <value>false</value> here.  Inactive records should
        /// largely be considered deleted, and not retrieved from the
        /// dataase.
        /// </remarks>
        [DataMember]
        public bool Inactive
        {
            get { return _inactive; }
            set { _inactive = value; }
        }

		/// <summary>
		/// Gets or sets an ID of the default Salesperson for the client.
		/// </summary>
		[DataMember]
		public int DefaultSalespersonId
		{
			get;
			set;
		}

        /// <summary>
        /// Gets or sets an ID of the default Director for the client.
        /// </summary>
        [DataMember]
        public int? DefaultDirectorId
        {
            get;
            set;
        }

        /// <summary>
		/// Gets or sets a list of the groups for the client.
		/// </summary>
		/// <remarks>The list is always empty when the object just created.</remarks>
		[DataMember]
		public List<ProjectGroup> Groups
		{
			get;
			set;
		}

        /// <summary>
        /// Projects for this client are chargeable by default.
        /// </summary>
        [DataMember]
        public bool IsChargeable
        {
            get;
            set;
        }

        [DataMember]
        public bool? IsMarginColorInfoEnabled
        {
            get;
            set;
        }

        [DataMember]
        public List<ClientMarginColorInfo> ClientMarginInfo
        {
            get;
            set;
        }

		#endregion

		#region Construction

		/// <summary>
		/// Creates a new empty instance of the <see cref="Client"/> class.
		/// </summary>
		public Client()
		{
		    Groups = new List<ProjectGroup>();
		}

		#endregion

        #region IEquatable<Client> members

        public bool Equals(Client other)
        {
            if (other.Id.HasValue && Id.HasValue)
                return other.Id.Value == Id.Value;

            return false;
        }

        #endregion
	}
}
