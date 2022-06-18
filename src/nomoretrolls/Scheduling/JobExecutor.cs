using System.Diagnostics;
using nomoretrolls.Telemetry;

namespace nomoretrolls.Scheduling
{
    internal class JobExecutor
    {
        private readonly ITelemetry _telemetry;

        public JobExecutor(ITelemetry telemetry)
        {
            _telemetry = telemetry;
        }

        public async Task<JobExecuteResult> ExecuteJobAsync(JobScheduleInfo job)
        {
            var sw = new Stopwatch();

            try
            {
                sw.Start();

                _telemetry.Event(new TelemetryTraceEvent() { Message = $"Starting job {job.Job.GetType().Name}..." });
                await job.Job.ExecuteAsync();
                _telemetry.Event(new TelemetryTraceEvent() { Message = $"Finished job {job.Job.GetType().Name}." });
                sw.Stop();

                return new JobExecuteResultOk(job.Job, sw.Elapsed);
            }
            catch (Exception ex)
            {
                sw.Stop();
                _telemetry.Event(new TelemetryErrorEvent() { Message = ex.Message } );
                return new JobExecuteResultError(job.Job, sw.Elapsed, ex);
            }
        }
    }
}
