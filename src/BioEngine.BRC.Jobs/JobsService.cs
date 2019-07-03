using System.Threading;
using System.Threading.Tasks;
using BioEngine.Extra.IPB.Comments;
using Hangfire;
using Microsoft.Extensions.Hosting;

namespace BioEngine.BRC.Jobs
{
    public class JobsService : IHostedService
    {
        private readonly IRecurringJobManager _recurringJobManager;

        public JobsService(IRecurringJobManager recurringJobManager)
        {
            _recurringJobManager = recurringJobManager;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _recurringJobManager.AddOrUpdate<IPBJobsRunner>("sync new comments",
                runner => runner.SyncNewCommentsAsync(), "*/5 * * * *");
            _recurringJobManager.AddOrUpdate<IPBJobsRunner>("sync all comments",
                runner => runner.SyncAllCommentsAsync(), "0 0 * * *");
            return Task.CompletedTask;
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    public class IPBJobsRunner
    {
        private readonly IPBCommentsSynchronizer _commentsSynchronizer;

        public IPBJobsRunner(IPBCommentsSynchronizer commentsSynchronizer)
        {
            _commentsSynchronizer = commentsSynchronizer;
        }

        [MaxConcurrentExecutions(1)]
        public Task SyncAllCommentsAsync()
        {
            return _commentsSynchronizer.SyncAllAsync();
        }

        [MaxConcurrentExecutions(1)]
        public Task SyncNewCommentsAsync()
        {
            return _commentsSynchronizer.SyncNewAsync();
        }
    }
}
