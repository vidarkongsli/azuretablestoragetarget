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
            SaveChanges();
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
