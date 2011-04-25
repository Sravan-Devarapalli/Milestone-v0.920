using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [DataContract]
    [Serializable]
    public class EmailTemplate
    {
        #region Properties
        /// <summary>
        /// Gets or sets an ID of the email template.
        /// </summary>
        [DataMember]
        public int? Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the email template.
        /// </summary>
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the To address of the email template.
        /// </summary>
        [DataMember]
        public string EmailTemplateTo
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Cc address of the email template.
        /// </summary>
        [DataMember]
        public string EmailTemplateCc
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the subject of the email template.
        /// </summary>
        [DataMember]
        public string Subject
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the body of the email template.
        /// </summary>
        [DataMember]
        public string Body
        {
            get;
            set;
        } 
        #endregion
    }
}

