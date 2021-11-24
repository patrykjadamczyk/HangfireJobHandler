using System.Threading.Tasks;

namespace HangfireJobHandler
{
    public interface IJobHandler
    {
        Task DeleteJobFromQueueAsync(string jobId);
        Task<bool> TryEnqueueJobAsync(string jobId);
    }
}
