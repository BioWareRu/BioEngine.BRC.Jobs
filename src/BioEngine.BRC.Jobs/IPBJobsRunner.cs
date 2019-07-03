using System.Threading.Tasks;
using BioEngine.Extra.IPB.Comments;

namespace BioEngine.BRC.Jobs
{
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