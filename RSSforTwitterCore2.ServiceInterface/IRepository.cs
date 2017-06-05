using RSSforTwitterCore2.ServiceModel.Readers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RSSforTwitterCore2.ServiceInterface
{
    public interface IRepository
    {
        Task<string> GetRssAsync();
        Task<string> GetRssBatchAsync();
        Task<string> GetRssUpdateAsync();
    }
}
