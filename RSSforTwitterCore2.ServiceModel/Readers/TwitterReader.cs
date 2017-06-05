using RSSforTwitterCore2.ServiceModel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Core.Extensions;
using Tweetinvi.Models;
using Tweetinvi.Models.Entities;
using Tweetinvi.Parameters;

namespace RSSforTwitterCore2.ServiceModel.Readers
{
    public class TwitterReader : ITwitterReader
    {
        private readonly IAuthenticatedUser _user;

        public TwitterReader(string customerKey, string customerSecret, string accessToken, string accessTokenSecret)
        {
            //������������ �� ������.
            Auth.SetUserCredentials(customerKey, customerSecret, accessToken, accessTokenSecret);
            //�������� ������������.
            _user = User.GetAuthenticatedUser();
        }

        public async Task<IList<ITweet>> GetTweetsAsync(long id, int maxTweets, IEnumerable<IUrlEntity> uniqueUrls,
            TweetTypes type)
        {
            //������� ��������� ������� ��� ������.
            var htParams = CreateHomeTimelineParameters(id, maxTweets, type);
            //���������� ������ �� ��������� ������.
            var homeTimeline = await _user.GetHomeTimelineAsync(htParams);
            //���� ������� ������ �� ��������, ������������ ������ ������.
            if (homeTimeline == null || !homeTimeline.Any())
                return new List<ITweet>();
            //������� ������ �� ������ �����.
            ClearTweetsTitleFromUrls(homeTimeline);
            //��������� ����� �� ������� ������ � ���������� �������.
            //���� ���� � �������� ������ �,�,�, � �� ����� ���, ���� � �������� ������ � � �,
            //� ����� � ����� ������� ������ ������ �.
            return FilterTweetsByUrls(homeTimeline, uniqueUrls).ToList();
        }

        #region Private Static Methods
        private static IEnumerable<ITweet> FilterTweetsByUrls(IEnumerable<ITweet> homeTimeline, IEnumerable<IUrlEntity> uniqueUrls)
        {
            //�������� ����� �� ��������.
            var tweetsWithUrls = homeTimeline.Where(x => x.Urls.Count > 0).ToList();
            //������� �� ������ ������������� ������.
            tweetsWithUrls.ForEach(x => x.Urls.RemoveAll(uniqueUrls.Contains));
            //������� ����� � ������� ����� ���������� �� �������� ������.
            tweetsWithUrls.RemoveAll(x => x.Urls.Count == 0);

            return tweetsWithUrls;
        }
        private static void ClearTweetsTitleFromUrls(IEnumerable<ITweet> tweets)
        {
            tweets.ForEach(x => 
                x.Text = x.Urls.Aggregate(x.Text, (current, u) => 
                    current.Replace(u.URL, string.Empty)));
        }
        private static HomeTimelineParameters CreateHomeTimelineParameters(long id, int maxTweets, TweetTypes type)
        {
            var htParams = new HomeTimelineParameters
            {
                ExcludeReplies = true,
                MaximumNumberOfTweetsToRetrieve = maxTweets
            };

            switch (type)
            {
                case TweetTypes.Base:
                    if (id > 0) htParams.SinceId = id;
                    break;
                case TweetTypes.New:
                    htParams.SinceId = id;
                    break;
                case TweetTypes.Old:
                    htParams.MaxId = id - 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return htParams;
        }
        #endregion
    }
}