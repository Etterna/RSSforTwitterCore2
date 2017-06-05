using System;
using ServiceStack;
using RSSforTwitterCore2.ServiceModel.Readers;
using RSSforTwitterCore2.ServiceModel;
using RSSforTwitterCore2.ServiceModel.Builders;
using System.Threading.Tasks;
using System.Collections.Generic;
using Tweetinvi.Models;

namespace RSSforTwitterCore2.ServiceInterface
{
    public class RssService: Service
    {
        public IRepository Repository { get; set; }

        [AddHeader(ContentType = "application/rss+xml")]
        public object Get(GetRssFeed request)
        {
            return Repository.GetRssAsync();
        }

        [AddHeader(ContentType = "application/xml")]
        public object Get(GetRssUpdate request)
        {
            return Repository.GetRssUpdateAsync();
        }
        [AddHeader(ContentType = "application/xml")]
        public object Get(GetRssBatch request)
        {
            return Repository.GetRssBatchAsync();
        }
    }
}
