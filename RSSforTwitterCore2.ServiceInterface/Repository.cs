using ServiceStack.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Tweetinvi.Models;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using RSSforTwitterCore2.ServiceModel.Readers;
using RSSforTwitterCore2.ServiceModel.Builders;
using RSSforTwitterCore2.ServiceModel.Enums;
using RSSforTwitterCore2.ServiceInterface.CustomTypes;
using ServiceStack;
using ServiceStack.Caching;
using Tweetinvi;
using Tweetinvi.Core.Extensions;
using Tweetinvi.Models.Entities;

namespace RSSforTwitterCore2.ServiceInterface
{
    public class Repository : IRepository
    {
        private readonly int _maxTweets;
        
        private long _maxId;
        private long _sinceId;
        private int _currentPosition;

        private readonly ITwitterReader _reader;
        //выступает в качестве временного хранилища для твитов.
        private readonly FixedSizedQueue<ITweet> _tweetsStorage;
        

        public Repository(ITwitterReader reader, int maxTweets)
        {
            _reader = reader;
            _maxTweets = maxTweets;
            _tweetsStorage = new FixedSizedQueue<ITweet>(200);
        }

        #region Public Async Methods
        public async Task<string> GetRssAsync()
        {
            _currentPosition = 0;
            //получаем твиты из хранилища, если они там есть.
            var storedTweets = ReceiveStoredTweets(_currentPosition, _maxTweets);
            if (storedTweets.Count > 0)
            {
                //устанавливаем идентификаторы MaxId и SinceId среди полученных твитов.
                SetTweetIds(storedTweets);
                //устанавливаем количество полученных твитов из хранилища.
                _currentPosition = storedTweets.Count;
                //создаём rss ленту.
                return new RssBuilder().Generate(storedTweets);
            }

            //если в хранилище твитов не оказалось, получаем твиты по запросу.
            IList<ITweet> receivedTweets =
                await _reader.GetTweetsAsync(_maxId, _maxTweets, Enumerable.Empty<IUrlEntity>(), TweetTypes.Base);

            if (receivedTweets.Count > 0)
            {
                SetTweetIds(receivedTweets);
                //сохраняем твиты в хранилище.
                StoreTweets(receivedTweets);
            }

            _currentPosition = receivedTweets.Count;
            return new RssBuilder().Generate(receivedTweets);
        }
        public async Task<string> GetRssBatchAsync()
        {
            var storedTweets = ReceiveStoredTweets(_currentPosition, _maxTweets);
            if (storedTweets.Count > 0)
            {
                //устанавливаем идентификатор SinceId среди полученных твитов.
                SetTweetSinceId(storedTweets);
                //если из хранилища получено достаточное количество твитов, создаём rss ленту.
                if (storedTweets.Count >= _maxTweets)
                {
                    _currentPosition += storedTweets.Count;
                    return new RssBuilder().Generate(storedTweets);
                }
            }

            //получаем url'ы твитов и передаем их в запрос..
            var urls = ReceiveTweetsUrls();
            //если в хранилище твитов не найдено или их недостаточно, получаем дополнительные твиты по запросу.
            IList<ITweet> receivedTweets = 
                await _reader.GetTweetsAsync(_sinceId, _maxTweets - storedTweets.Count, urls,  TweetTypes.Old);

            if (receivedTweets.Count > 0)
            {
                SetTweetSinceId(receivedTweets);
                StoreTweets(receivedTweets);
            }

            var tweets = storedTweets.Union(receivedTweets);
            _currentPosition += tweets.Count();
            return new RssBuilder().Generate(tweets);
        }
        public async Task<string> GetRssUpdateAsync()
        {
            var urls = ReceiveTweetsUrls();
            IList<ITweet> receivedTweets = 
                await _reader.GetTweetsAsync(_maxId, _maxTweets, urls,  TweetTypes.New);

            if (receivedTweets.Count > 0)
            {
                //устанавливаем идентификатор MaxId среди полученных твитов.
                SetTweetMaxId(receivedTweets);
                StoreTweets(receivedTweets);
            }
            _currentPosition += receivedTweets.Count;
            return new RssBuilder().Generate(receivedTweets);
        }
        #endregion

        #region Private Methods
        private void SetTweetIds(IList<ITweet> tweets)
        {
            if (tweets == null || tweets.Count == 0) return;
            SetTweetMaxId(tweets);
            SetTweetSinceId(tweets);
        }
        private void SetTweetSinceId(IList<ITweet> tweets)
        {
            if (tweets == null || tweets.Count == 0) return;
            _sinceId = tweets.Min(x => x.Id);
        }
        private void SetTweetMaxId(IList<ITweet> tweets)
        {
            if (tweets == null || tweets.Count == 0) return;
            _maxId = tweets.Max(x => x.Id);
        }
        private List<ITweet> ReceiveStoredTweets(int position, int count)
        {
            return _tweetsStorage.Skip(position).Take(count).ToList();
        }
        private void StoreTweets(IEnumerable<ITweet> tweets, bool isLimited = false)
        {
            foreach (var tweet in tweets)
            {
                if (isLimited && _tweetsStorage.Count == _tweetsStorage.Limit) break;
                    _tweetsStorage.Enqueue(tweet);
            }
        }
        private List<IUrlEntity> ReceiveTweetsUrls()
        {
            return _tweetsStorage.SelectMany(x => x.Urls).ToList();
        }
        #endregion
    }
}
