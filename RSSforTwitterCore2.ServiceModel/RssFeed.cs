using ServiceStack;
using System;
using System.Runtime.Serialization;

namespace RSSforTwitterCore2.ServiceModel
{
    [DataContract]
    [Route("/rss")]
    public class GetRssFeed : IReturn<GetRssFeedResponse> { }

    [DataContract]
    public class GetRssFeedResponse: IHasResponseStatus
    {
        public ResponseStatus ResponseStatus { get; set; }
    }
}
