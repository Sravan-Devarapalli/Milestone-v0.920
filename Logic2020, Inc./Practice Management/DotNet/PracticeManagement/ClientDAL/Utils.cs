using System;
using DataTransferObjects.TimeEntry;
using DataTransferObjects.Utils;

namespace DataAccess
{
    public class Utils
    {
        /// <summary>
        /// 	Converts CSV into int array
        /// </summary>
        /// <param name = "csv">Comma separated int values</param>
        /// <returns>Array of integers</returns>
        public static int[] StringToIntArray(string csv)
        {
            csv = csv.TrimEnd(DataTransferObjects.Utils.Generic.StringListSeparator);
            var values = csv.Split(DataTransferObjects.Utils.Generic.StringListSeparator);

            var res = new int[values.Length];

            for (var i = 0; i < values.Length; i++)
                res[i] = Convert.ToInt32(values[i]);

            return res;
        }

        #region Utils

        /// <summary>
        /// 	Converts ReviewStatus to bool?
        /// </summary>
        public static bool? ReviewStatus2Bool(ReviewStatus status)
        {
            switch (status)
            {
                case ReviewStatus.Approved:
                    return true;

                case ReviewStatus.Declined:
                    return false;
            }

            return null;
        }

        /// <summary>
        /// 	Converts ReviewStatus to bool?
        /// </summary>
        public static int? ReviewStatus2Int(string status)
        {
            if (status == null)
                return null;

            var reviewStatus = (ReviewStatus) Enum.Parse(typeof (ReviewStatus), status);

            switch (reviewStatus)
            {
                case ReviewStatus.Approved:
                    return 1;

                case ReviewStatus.Declined:
                    return 0;
            }

            return 2; // Needed to disctinct values in the database
        }

        /// <summary>
        /// 	Converts bool? to ReviewStatus
        /// </summary>
        public static ReviewStatus Bool2ReviewStatus(bool? status)
        {
            if (status.HasValue)
                return status.Value
                           ? ReviewStatus.Approved
                           : ReviewStatus.Declined;

            return ReviewStatus.Pending;
        }

        #endregion
    }
}
