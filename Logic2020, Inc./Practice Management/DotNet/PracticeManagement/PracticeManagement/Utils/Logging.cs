using System.ServiceModel;
using PraticeManagement.ActivityLogService;

namespace PraticeManagement.Utils
{
    public class Logging
    {
        /// <summary>
        /// Writes error message to the AL
        /// </summary>
        /// <param name="excMsg"></param>
        /// <param name="excSrc"></param>
        /// <param name="innerExcMsg"></param>
        /// <param name="innerExcSrc"></param>
        /// <param name="srcUrl"></param>
        /// <param name="srcQuery"></param>
        /// <param name="userName"></param>
        public static void LogErrorMessage(string excMsg, string excSrc, string innerExcMsg, string innerExcSrc, string srcUrl, string srcQuery, string userName)
        {
            string logText = string.Format(Constants.ActityLog.ErrorLogMessage,
                userName, srcUrl, srcQuery,
                excMsg, excSrc, innerExcMsg, innerExcSrc);

            using (var serviceClient = new ActivityLogServiceClient())
            {
                try
                {
                    // Insert into the database
                    serviceClient.ActivityLogInsert(Constants.ActityLog.ErrorMessageId, logText);
                }
                catch (CommunicationException)
                {
                    serviceClient.Abort();
                }
            }
        }
    }
}

