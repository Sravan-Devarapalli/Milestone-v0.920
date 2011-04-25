using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [DataContract]
    [Serializable]
    public class EmailData
    {
        #region Properties

        /// <summary>
        /// Data to be converted into the template
        /// </summary>
        [DataMember]
        public DataSet Data
        {
            get;
            set;
        }

        /// <summary>
        /// Email template
        /// </summary>
        [DataMember]
        public EmailTemplate EmailTemplate
        {
            get;
            set;
        }

        #endregion

        #region Methods

        public bool IsEmpty
        {
            get { return Data == null || Data.Tables[0].Rows.Count == 0; }
        }

        public IEnumerable<string> DataHeaders
        {
            get 
            {                
                return from DataColumn column in Data.Tables[0].Columns select column.ColumnName;
            }
        }

        #endregion

    }
}

