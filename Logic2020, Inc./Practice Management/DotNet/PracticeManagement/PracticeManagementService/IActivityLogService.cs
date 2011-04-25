using System.Collections.Generic;
using System.ServiceModel;

using DataTransferObjects;
using DataTransferObjects.ContextObjects;

namespace PracticeManagementService
{
	[ServiceContract]
	public interface IActivityLogService
	{
	    /// <summary>
	    /// Retrives a list of activity log items for the specified period.
	    /// </summary>
	    /// <param name="context"></param>
	    /// <param name="pageSize">A page size.</param>
	    /// <param name="pageNo">A number of the page to be retrived.</param>
	    /// <returns>The list of the <see cref="ActivityLogItem"/></returns>
	    [OperationContract]
		List<ActivityLogItem> ActivityLogList(ActivityLogSelectContext context, int pageSize, int pageNo);

	    /// <summary>
	    /// Retrives a number of the activity log items for the specified period.
	    /// </summary>
	    /// <param name="context"></param>
	    /// <returns>The number of user's activities.</returns>
	    [OperationContract]
        int ActivityLogGetCount(ActivityLogSelectContext context);

	    /// <summary>
	    /// Add item to log
	    /// </summary>
	    /// <param name="activityTypeId">Id of log type</param>
	    /// <param name="logData">Xml as string of log</param>
        [OperationContract]
        void ActivityLogInsert(int activityTypeId, string logData);

        /// <summary>
        /// Returns database version
        /// </summary>
        /// <returns>Returns database version</returns>
        [OperationContract]
        string GetDatabaseVersion();
    }
}

