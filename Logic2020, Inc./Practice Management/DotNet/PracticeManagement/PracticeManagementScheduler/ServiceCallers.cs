using System;
using PracticeManagementScheduler.ConfigurationService;

namespace PracticeManagementScheduler
{
    internal class ServiceCallers
    {
        #region Generic

        private static TResult Invoke<TServ, TResult>(Func<TServ, TResult> func)
            where TServ : IDisposable, new()
        {
            using (var client = new TServ())
                return func(client);
        }

        private static void Invoke<TServ>(Action<TServ> action)
            where TServ : IDisposable, new()
        {
            using (var client = new TServ())
                action(client);
        }

        #endregion

        #region Custom

        public static TResult Configuration<TResult>(Func<ConfigurationServiceClient, TResult> func)
        {
            return Invoke(func);
        }

        public static void Configuration(Action<ConfigurationServiceClient> action)
        {
            Invoke(action);
        }

        #endregion
    }
}
