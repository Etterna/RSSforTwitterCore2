using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using ServiceStack;

namespace RSSforTwitterCore2.ServiceModel
{
    [DataContract]
    [Route("/rssNextBatch")]
    public class GetRssBatch : IReturn<GetRssBatchResponse> { }

    [DataContract]
    public class GetRssBatchResponse : IHasResponseStatus
    {
        public ResponseStatus ResponseStatus { get; set; }
    }
}
