using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Services.Client;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using NLog;
using NLog.Common;
using NLog.Targets;

namespace Demo.NLog.Azure
{
    [Target("AzureStorage")]
    public class AzureStorageTarget : Target
    {
        private LogServiceContext _ctx;
        private string _tableEndpoint;

        [Required]
        public string TableStorageConnectionStringName { get; set; }

        protected override void InitializeTarget()
        {
            base.InitializeTarget();
            var cloudStorageAccount =
                CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue(TableStorageConnectionStringName));
            _tableEndpoint = cloudStorageAccount.TableEndpoint.AbsoluteUri;
            CloudTableClient.CreateTablesFromModel(typeof(LogServiceContext), _tableEndpoint, cloudStorageAccount.Credentials);
            _ctx = new LogServiceContext(cloudStorageAccount.TableEndpoint.AbsoluteUri, cloudStorageAccount.Credentials);
        }

        protected override void Write(LogEventInfo loggingEvent)
        {
            Action doWriteToLog = () =>
            {
                try
                {
                    _ctx.Log(new LogEntry
                    {
                        RoleInstance = RoleEnvironment.CurrentRoleInstance.Id,
                        DeploymentId = RoleEnvironment.DeploymentId,
                        Timestamp = loggingEvent.TimeStamp,
                        Message = loggingEvent.FormattedMessage,
                        Level = loggingEvent.Level.Name,
                        LoggerName = loggingEvent.LoggerName,
                        StackTrace = loggingEvent.StackTrace != null ? loggingEvent.StackTrace.ToString() : null
                    });
                }
                catch (DataServiceRequestException e)
                {
                    InternalLogger.Error(string.Format("{0}: Could not write log entry to {1}: {2}",
                        GetType().AssemblyQualifiedName, _tableEndpoint, e.Message), e);
                }
            };
            doWriteToLog.BeginInvoke(null, null);
        }
    }
}
