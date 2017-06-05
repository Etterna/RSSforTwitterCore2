using RSSforTwitterCore2.ServiceModel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetinvi.Models;
using Tweetinvi.Models.Entities;

namespace RSSforTwitterCore2.ServiceModel.Readers
{
    public interface ITwitterReader
    {
        Task<IList<ITweet>> GetTweetsAsync(long id, int maxTweets, IEnumerable<IUrlEntity> uniqueUrls, TweetTypes type);
    }
}