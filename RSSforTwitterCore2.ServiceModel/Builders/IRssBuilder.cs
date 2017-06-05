using System;
using System.Collections.Generic;
using System.Text;
using Tweetinvi.Models;

namespace RSSforTwitterCore2.ServiceModel.Builders
{
    public interface IRssBuilder
    {
        string Generate(IEnumerable<ITweet> tweets);
    }
}
