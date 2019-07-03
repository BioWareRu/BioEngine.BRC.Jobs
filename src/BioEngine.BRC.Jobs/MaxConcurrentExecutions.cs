using System;
using System.Linq;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Logging;

namespace BioEngine.BRC.Jobs
{
    public class MaxConcurrentExecutions : JobFilterAttribute, IClientFilter
    {
        private readonly ILog _logger = LogProvider.GetLogger(typeof(MaxConcurrentExecutions));
        //private static readonly ILog _logger = new ColouredConsoleLogProvider().GetLogger(nameof(MaxConcurrentExecutions));
        private int MaxConcurrentExecutionsAllowed { get; }

        public MaxConcurrentExecutions(int maxConcurrentExecutionsAllowed)
        {
            if (maxConcurrentExecutionsAllowed <= 0) throw new ArgumentException("maxConcurrentExecutionsAllowed argument value should be greater that zero.");
            MaxConcurrentExecutionsAllowed = maxConcurrentExecutionsAllowed;
        }

        public void OnCreated(CreatedContext filterContext)
        {
        }

        public void OnCreating(CreatingContext filterContext)
        {
            if (filterContext == null) throw new ArgumentNullException(nameof(filterContext));

            var jobName = filterContext.Job.ToString();
            _logger.Info(jobName);
            var processingJobs = filterContext.Storage.GetMonitoringApi().ProcessingJobs(0, 10000);
            if (processingJobs.Count > 0)
            {
                var pj = string.Join(", ", processingJobs.Select(j => j.Key + " : " + j.Value.Job));
                _logger.Info(pj);
            }
            var matchingJobCount = processingJobs.Count(j => j.Value.Job.ToString().Equals(jobName));
            if (matchingJobCount >= MaxConcurrentExecutionsAllowed)
            {
                _logger.Info($"Cancel start attempt for {jobName} because {matchingJobCount.ToString()} jobs are already running");
                filterContext.Canceled = true;
            }
            else
            {
                _logger.Info($"New {jobName} job will be started. Currently Running / Max Allowed: {matchingJobCount.ToString()} / {MaxConcurrentExecutionsAllowed.ToString()}");
            }
        }
    }
}
