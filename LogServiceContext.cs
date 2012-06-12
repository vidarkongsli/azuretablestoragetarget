using System.Data.Services.Client;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace Demo.NLog.Azure
{
    public class LogServiceContext : TableServiceContext
    {
        public LogServiceContext(string baseAddress, StorageCredentials credentials) : base(baseAddress, credentials) { }
        internal void Log(LogEntry logEntry)
        {
            AddObject("LogEntries", logEntry);
            BeginSaveChanges(SaveChangesOptions.Batch, null, null);
        }

        public IQueryable<LogEntry> LogEntries
        {
            get
            {
                return CreateQuery<LogEntry>("LogEntries");
            }
        }
    }
}
