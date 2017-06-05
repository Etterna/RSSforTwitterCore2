using Funq;
using RSSforTwitterCore2.ServiceInterface;
using ServiceStack;
using ServiceStack.Configuration;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RSSforTwitterCore2.ServiceModel.Readers;
using ServiceStack.Mvc;

namespace RSSforTwitterCore2
{
    public class AppHost : AppHostBase
    {
        public AppHost() : base("RSS Service", typeof(RssService).GetAssembly()) {
        }

        public override void Configure(Container container)
        {
            var appSettings = new AppSettings();
            SetConfig(new HostConfig { DefaultRedirectPath = "/", DebugMode = true });

            var twitterReader = new TwitterReader(
                appSettings.GetString("CustomerKey"),
                appSettings.GetString("CustomerSecret"),
                appSettings.GetString("AccessToken"),
                appSettings.GetString("AccessTokenSecret"));
            
            container.Register<ServiceInterface.IRepository>(c => new Repository(twitterReader,
                appSettings.Get<int>("MaxTweets")));

            Plugins.Add(new RazorFormat());
            Plugins.Add(new CorsFeature());
        }
    }
}
