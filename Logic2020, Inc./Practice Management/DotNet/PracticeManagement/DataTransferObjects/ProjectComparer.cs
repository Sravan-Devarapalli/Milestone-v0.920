using System;
using System.Collections;

namespace DataTransferObjects
{
    public class ProjectComparer : IComparer
    {
        #region Private Variables

        private string sortBy = string.Empty;

        #endregion Private Variables

        #region Properties

        public string SortBy
        {
            get
            {
                return sortBy;
            }
            set
            {
                sortBy = value;
            }
        }

        #endregion Properties

        #region Constructor

        public ProjectComparer()
        {
            //default constructor
        }

        public ProjectComparer(string pSortBy)
        {
            sortBy = pSortBy;
        }

        #endregion Constructor

        #region Methods

        public Int32 Compare(Object pFirstObject, Object pObjectToCompare)
        {
            if (!(pFirstObject is Project))
                return 0;
            switch (sortBy)
            {
                case "Client":
                    return String.Compare(((Project)pFirstObject).Client.Name, ((Project)pObjectToCompare).Client.Name);
                case "Project":
                    return String.Compare(((Project)pFirstObject).Name, ((Project)pObjectToCompare).Name);
                case "End Date":
                    return Nullable.Compare(((Project)pFirstObject).EndDate, ((Project)pObjectToCompare).EndDate);
                case "Start Date":
                    return Nullable.Compare(((Project)pFirstObject).StartDate, ((Project)pObjectToCompare).StartDate);
                case "Project #":
                    return string.Compare(((Project)pFirstObject).ProjectNumber,
                                          ((Project)pObjectToCompare).ProjectNumber);
                default:
                    return 0;
            }
        }

        #endregion Methods
    }
}
