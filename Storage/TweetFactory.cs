using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Storage.Azure.Table.Entities.Twitter;

namespace Storage
{
    public class TweetFactory
    {
        private readonly CloudTable _tweetsTable;

        public TweetFactory(CloudStorageAccount cloudStorageAccount)
        {
            var tableClient = cloudStorageAccount.CreateCloudTableClient();
            _tweetsTable = tableClient.GetTableReference("tweetsoldestfirst");
            _tweetsTable.CreateIfNotExists();
        }

        public TableQuerySegment<TweetTableEntity> Get1000Tweets(ref TableContinuationToken token)
        {
            var query = new TableQuery();
            EntityResolver<TweetTableEntity> tweetTableEntityResolver = (pk, rk, ts, props, etag) =>
            {
                var resolvedEntity = new TweetTableEntity
                {
                    PartitionKey = pk,
                    RowKey = rk,
                    Timestamp = ts,
                    ETag = etag,
                };
                resolvedEntity.ReadEntity(props, null);
                return resolvedEntity;
            };
            var segment = _tweetsTable.ExecuteQuerySegmented(query, tweetTableEntityResolver, token);
            token = segment.ContinuationToken;
            return segment;
        }

    }
}