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
            //Авторизуемся по ключам.
            Auth.SetUserCredentials(customerKey, customerSecret, accessToken, accessTokenSecret);
            //Получаем пользователя.
            _user = User.GetAuthenticatedUser();
        }

        public async Task<IList<ITweet>> GetTweetsAsync(long id, int maxTweets, IEnumerable<IUrlEntity> uniqueUrls,
            TweetTypes type)
        {
            //Создаем параметры запроса для твитов.
            var htParams = CreateHomeTimelineParameters(id, maxTweets, type);
            //Отправляем запрос на получение твитов.
            var homeTimeline = await _user.GetHomeTimelineAsync(htParams);
            //Если никаких твитов не получено, возвращается пустой список.
            if (homeTimeline == null || !homeTimeline.Any())
                return new List<ITweet>();
            //Удаляем ссылки из текста твита.
            ClearTweetsTitleFromUrls(homeTimeline);
            //Фильтруем твиты по наличию ссылок и уникальным ссылкам.
            //Если твит А содержит ссылки а,б,в, в то время как, твит Б содержит ссылки в и г,
            //у твита Б будет удалена только ссылка в.
            return FilterTweetsByUrls(homeTimeline, uniqueUrls).ToList();
        }

        #region Private Static Methods
        private static IEnumerable<ITweet> FilterTweetsByUrls(IEnumerable<ITweet> homeTimeline, IEnumerable<IUrlEntity> uniqueUrls)
        {
            //Получаем твиты со ссылками.
            var tweetsWithUrls = homeTimeline.Where(x => x.Urls.Count > 0).ToList();
            //Удаляем из твитов дублирующиеся ссылки.
            tweetsWithUrls.ForEach(x => x.Urls.RemoveAll(uniqueUrls.Contains));
            //Удаляем твиты у которых после фильтрации не осталось ссылок.
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