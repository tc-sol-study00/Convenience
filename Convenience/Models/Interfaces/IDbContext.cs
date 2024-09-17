using Convenience.Data;
using Microsoft.EntityFrameworkCore;

namespace Convenience.Models.Interfaces {
    public interface IDbContext {
        private const string ConfigrationFileName = "appsettings.json"; 
        private const string KeyWordInAppConfig = "ConvenienceContext";

        protected static ConvenienceContext DbOpen() {
            //DBコンテクスト用接続子読み込み
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile(ConfigrationFileName, optional: true, reloadOnChange: true)
                .Build();
            //DBコンテクスト作成
            var contextOptions = new DbContextOptionsBuilder<ConvenienceContext>()
                .UseNpgsql(configuration[KeyWordInAppConfig])
                .Options;
            return (new ConvenienceContext(contextOptions));
        }
    }
}
