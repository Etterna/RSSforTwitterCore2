using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using ServiceStack;

namespace RSSforTwitterCore2.ServiceModel
{
    [DataContract]
    [Route("/rssUpdate")]
    public class GetRssUpdate : IReturn<GetRssUpdateResponse> { }

    [DataContract]
    public class GetRssUpdateResponse : IHasResponseStatus
    {
        public ResponseStatus ResponseStatus { get; set; }
    }
}
