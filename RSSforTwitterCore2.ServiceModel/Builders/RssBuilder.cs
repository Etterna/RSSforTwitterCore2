using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tweetinvi.Models;
using Tweetinvi.Models.Entities;
using WilderMinds.RssSyndication;

namespace RSSforTwitterCore2.ServiceModel.Builders
{
    public class RssBuilder: IRssBuilder
    {
        public string Generate(IEnumerable<ITweet> tweets)
        {
            Feed feed = GenerateFeed();

            IEnumerable<Item> feedItems = GenerateItems(tweets);
            feed.Items = feedItems.ToList();
            
            return feed.Serialize();
        }

        #region Private Methods
        private Feed GenerateFeed()
        {
            var feed = new Feed
            {
                Title = "RSS for Twitter",
                Description = "Imported from Twitter",
                Link = new Uri("http://twitter.com")
            };
            
            return feed;
        }
        private IEnumerable<Item> GenerateItems(IEnumerable<ITweet> tweets)
        {
            return tweets.Select(GenerateItem);
        }
        private Item GenerateItem(ITweet tweet)
        {
            //var image = tweet.Entities.Medias.FirstOrDefault(x => x.MediaType == "photo");

            //Компонент RSSSyndication поддерживает только одну ссылку,
            //поэтому в ленту мы передаем первую ссылку из твита.

            //Для вывода нескольких ссылок на твит нужно сделать свою реализацию.
            //Также и с изображениями.

            //Если требуется в дальнейшем могу добавить.
            Uri link = GetTweetLink(tweet);
            
            return new Item
            {
                Title = tweet.Text,
                Body = link.AbsoluteUri,
                Link = link,
                Permalink = link.AbsoluteUri,
                PublishDate = tweet.CreatedAt,
                Author = new Author { Name = tweet.CreatedBy.Name }
            };
        }
        private Uri GetTweetLink(ITweet tweet)
        {
            IUrlEntity url = tweet.Entities.Urls.FirstOrDefault();
            return new Uri(url.URL);
        }
        #endregion
    }
}
