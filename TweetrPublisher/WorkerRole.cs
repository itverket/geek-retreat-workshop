using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.Azure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Storage;

namespace TweetrPublisher
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent _runCompleteEvent = new ManualResetEvent(false);
        private CloudStorageAccount _storageAccount;
        private CloudQueueClient _queueClient;
        private CloudQueue _tweetQueue;
        int _tweetsPerMinute;
        private TweetFactory _tweetFactory;

        public override bool OnStart()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            _tweetFactory = new TweetFactory(CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("TweetrConnectionString")));
            SetTweetPublishRate();
            InitializeTweetDestinationQueue();

            ServicePointManager.DefaultConnectionLimit = 12;
            RoleEnvironment.Changed += RoleEnvironment_Changed;

            bool result = base.OnStart();
            Trace.TraceInformation("TweetrPublisher has been started");
            return result;
        }
        public override void Run()
        {
            Trace.TraceInformation("TweetrPublisher is running");
            try
            {
                TableContinuationToken token = null;
                var cancellationToken = _cancellationTokenSource.Token;
                while (!cancellationToken.IsCancellationRequested)
                {
                    var tweetQuerySegment = _tweetFactory.Get1000Tweets(ref token);
                    foreach (var tweetTableEntity in tweetQuerySegment.Results)
                    {
                        var tweetJson = tweetTableEntity.TweetJson;
                        _tweetQueue.AddMessage(new CloudQueueMessage(tweetJson));
                        Trace.TraceInformation("Published tweet with id {1} to queue, {0} seconds until next.",
                                                60000 / (_tweetsPerMinute * 1000), tweetTableEntity.Tweet.IdString);
                        Thread.Sleep(60000 / _tweetsPerMinute);
                    }
                }
            }
            finally
            {
                _runCompleteEvent.Set();
            }
        }

        private void RoleEnvironment_Changed(object sender, RoleEnvironmentChangedEventArgs e)
        {
            var configChanges = e.Changes
                .OfType<RoleEnvironmentConfigurationSettingChange>()
                .ToList();

            if (!configChanges.Any())
                return;

            if (configChanges.Any(c => c.ConfigurationSettingName == "TweetsPerMinute"))
            {
                Trace.TraceInformation("Role service configuration changed, updating the publish rate for role.");
                SetTweetPublishRate();
            }
        }

        private void SetTweetPublishRate()
        {
            _tweetsPerMinute = Convert.ToInt32(CloudConfigurationManager.GetSetting("TweetsPerMinute"));
        }

        private void InitializeTweetDestinationQueue()
        {
            _queueClient = _storageAccount.CreateCloudQueueClient();
            _tweetQueue = _queueClient.GetQueueReference("tweetsqueue");
            _tweetQueue.CreateIfNotExists();
        }

        public override void OnStop()
        {
            Trace.TraceInformation("TweetrPublisher is stopping");

            this._cancellationTokenSource.Cancel();
            this._runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("TweetrPublisher has stopped");
        }
    }
}
