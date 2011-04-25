using System.Collections.Generic;
using System.ServiceModel.Activation;
using DataAccess;
using DataTransferObjects;
using DataTransferObjects.ContextObjects;

namespace PracticeManagementService
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class ActivityLogService : IActivityLogService
	{
		#region IActivityLogService Members

	    /// <summary>
	    /// Retrives a list of activity log items for the specified period.
	    /// </summary>
	    /// <param name="context"></param>
	    /// <param name="pageSize">A page size.</param>
	    /// <param name="pageNo">A number of the page to be retrived.</param>
	    /// <returns>The list of the <see cref="ActivityLogItem"/></returns>
	    public List<ActivityLogItem> ActivityLogList(ActivityLogSelectContext context, int pageSize, int pageNo)
		{
	        return ActivityLogDAL.ActivityLogListByPeriod(context, pageSize, pageNo);
		}

	    /// <summary>
	    /// Retrives a number of the activity log items for the specified period.
	    /// </summary>
	    /// <param name="context"></param>
	    /// <returns>The number of user's activities.</returns>
	    public int ActivityLogGetCount(ActivityLogSelectContext context)
		{
	        return ActivityLogDAL.ActivityLogGetCount(context);
		}

        /// <summary>
        /// Add item to log
        /// </summary>
        /// <param name="activityTypeId">Id of log type</param>
        /// <param name="logData">Xml as string of log</param>
	    public void ActivityLogInsert(int activityTypeId, string logData)
	    {
	        ActivityLogDAL.ActivityLogInsert(activityTypeId, logData);
	    }


        /// <summary>
        /// Returns database version
        /// </summary>
        /// <returns>Returns database version</returns>
        public string GetDatabaseVersion()
        {
            return ActivityLogDAL.GetDatabaseVersion();
        }

        #endregion
    }
}

