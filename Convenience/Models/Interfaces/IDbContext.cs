using Convenience.Data;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Microsoft.EntityFrameworkCore;

namespace Convenience.Models.Interfaces {
    /// <summary>
    /// DB接続用インターフェース（c#コンソールデバッグ用）
    /// </summary>
    public interface IDbContext {
        private const string ConfigrationFileName = "appsettings.json"; 
        private const string KeyWordInAppConfig = "ConnectionStrings:ConvenienceContext";

        /// <summary>
        /// PostgreSQL DBオープン
        /// </summary>
        /// <returns></returns>
        protected static ConvenienceContext DbOpen() {
            //DBコンテクスト用接続子読み込み
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile(ConfigrationFileName, optional: true, reloadOnChange: true)
                .Build();

            //DBコンテクスト作成
            var contextOptions = new DbContextOptionsBuilder<ConvenienceContext>()
                .UseNpgsql(configuration[KeyWordInAppConfig])
                .LogTo(Console.WriteLine, LogLevel.Information)
                .Options;
            return new ConvenienceContext(contextOptions);
        }
    }
}

/*使い方
（１）本インターフェースの実装
（２）以下のコンストラクター追加（例）

        /// <summary>
        /// コンストラクタ（ＡＳＰ用）
        /// </summary>
        /// <param name="context">DBコンテキスト</param>
        public Chumon(ConvenienceContext context) {
            _context = context;
        }

        /// <summary>
        /// 注文クラスデバッグ用
        /// </summary>
        public Chumon() {
            _context = IDbContext.DbOpen();
        }
*/
