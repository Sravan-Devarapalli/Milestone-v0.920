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
                case "Account":
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

                case "Practice Area":
                    return Practice.Compare(((Project)pFirstObject).Practice,
                                          ((Project)pObjectToCompare).Practice);
                case "Division":
                    return ProjectDivision.Compare(((Project)pFirstObject).Division,
                                          ((Project)pObjectToCompare).Division);

                case "Sales Person":
                    return string.Compare(((Project)pFirstObject).SalesPersonName,
                                          ((Project)pObjectToCompare).SalesPersonName);
                case "Channel":
                    return Channel.Compare(((Project)pFirstObject).Channel,
                                          ((Project)pObjectToCompare).Channel);

                case "Channel-Sub":
                    return string.Compare(((Project)pFirstObject).SubChannel,
                                          ((Project)pObjectToCompare).SubChannel);

                case "Revenue Type":
                    return Revenue.Compare(((Project)pFirstObject).RevenueType,
                                          ((Project)pObjectToCompare).RevenueType);
                case "Offering":
                    return Offering.Compare(((Project)pFirstObject).Offering,
                                          ((Project)pObjectToCompare).Offering);
                case "Status":
                    return string.Compare(((Project)pFirstObject).Status.Name,
                                          ((Project)pObjectToCompare).Status.Name);
                case "New/Extension":
                    return string.Compare(((Project)pFirstObject).BusinessType.ToString(),
                                          ((Project)pObjectToCompare).BusinessType.ToString());
                default:
                    return 0;
            }
        }

        #endregion Methods
    }
}

