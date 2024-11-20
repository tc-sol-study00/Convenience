using AutoMapper;
using Convenience.Controllers;
using Convenience.Models.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;

namespace Convenience.Models.Properties {
    /// <summary>
    /// CSVファイルを作る
    /// </summary>
    public class ConvertObjectToCsv : IConvertObjectToCsv {
        private const string strEncoding = "shift-jis"; //エンコードコード
        private const string tmpDir = "tmpfiles";       //wwwroot直下のCSVファイルを作成するディレクトリ

        private readonly Encoding encoding;             //エンコード
        private CsvConfiguration configuration;         //Csvコンフィグ
        private IWebHostEnvironment _environment;       //WWWRootを取り出すインスタンス

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="environment"></param>
        public ConvertObjectToCsv(IWebHostEnvironment environment) {
            _environment = environment;                     //WWWRootを取り出すインスタンス
            (configuration, encoding) = CsvConfig();        //Csvコンフィグ
        }

        /// <summary>
        /// Csvコンフィグ
        /// </summary>
        /// <returns>Csvコンフィグ・エンコード</returns>
        public (CsvConfiguration config, Encoding encording) CsvConfig() {

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var encoding = Encoding.GetEncoding(strEncoding);

            var config = new CsvConfiguration(CultureInfo.CurrentCulture) {
                AllowComments = true,                         // コメントを許可するか(default:false)
                Comment = '#',                                // コメントとして扱う行を示す文字(default:#)
                HasHeaderRecord = true,                       // ヘッダー行の有無(default:true)
                Delimiter = ",",                              // 区切り文字(default:#)
                Encoding = encoding,                          // 文字コード指定(default:UTF-8)
                IgnoreBlankLines = true,                      // 空行を無視するかどうか(default:true)
                TrimOptions = TrimOptions.Trim,               // 値のトリムオプション
                Quote = '"',                                  // 値を囲む文字(default:'"')
                ShouldQuote = args =>                           // 出力時に値をQuoteで指定した文字で囲むかどうか
                {
                    // 数値型の値はクォートしない
                    if (double.TryParse(args.Field, out _)) {
                        return false;
                    }

                    // その他のデータ型はクォートする
                    return true;
                },
            };

            return (config, encoding);

        }

        /// <summary>
        /// CSVへ変換
        /// </summary>
        /// <typeparam name="T1">CSV化したいモデル</typeparam>
        /// <typeparam name="T2">CSVMappingモデル</typeparam>
        /// <param name="modeldatas">変換元モデル</param>
        /// <param name="filename">CSVマッピングモデル</param>
        /// <returns>作られたファイル名（フルパス）</returns>
        public string ConvertToCSV<T1, T2>(IEnumerable<T1> modeldatas, string filename)
            where T1 : class
            where T2 : class {

            /*
             * ファイル名形成（フルパス）
             */
            var wwwRootPath = _environment.WebRootPath;
            var filePath = Path.Combine(wwwRootPath, tmpDir, filename);

            /*
             * ライターストリーム
             */
            using var stream = new StreamWriter(filePath, false, encoding);
            using var csv = new CsvWriter(stream, configuration);

            // AutoMapperの設定
            IMapper mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<T1, T2>();
            }).CreateMapper();

            //CSVMappingモデルに変換
            IEnumerable<T2> csvRecords = mapper.Map<IEnumerable<T2>>(modeldatas);

            // CSV出力
            csv.WriteRecords(csvRecords);

            return filePath;
        }

        public static Dictionary<string, string> FilenameDic = new Dictionary<string, string>
        {
            { nameof(ShiireJissekiController), "ShiireJisseki_{yyyyMMddHHmmss}.csv" },
            { nameof(ZaikoController), "SokoZaiko_{yyyyMMddHHmmss}.csv" },
            { nameof(TentoZaikoController), "TentoZaiko_{yyyyMMddHHmmss}.csv" },
            { nameof(KaikeiJissekiController), "KaikeiJisseki_{yyyyMMddHHmmss}.csv" },
            { nameof(ChumonJissekiController), "ChumonJisseki_{yyyyMMddHHmmss}.csv" },
        };

        /// <summary>
        /// ファイル名を取得する
        /// </summary>
        /// <param name="keyword">コントローラー名</param>
        /// <returns>ファイル名</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public string GetFileName(string keyword) {
            if (FilenameDic.ContainsKey(keyword)) {
                string yyyyMMddHHmmss = DateTime.Now.ToString("yyyyMMddHHmmss");
                string pickedData = FilenameDic[keyword];

                return pickedData.Replace("{yyyyMMddHHmmss}", yyyyMMddHHmmss);
            }

            throw new KeyNotFoundException($"Keyword '{keyword}' not found in FilenameDic.");
        }

    }

    /// <summary>
    /// 各CSVフォーマットの詰め合わせ
    /// </summary>
    public static class CSVMapping {

        /// <summary>
        /// 仕入実績画面用
        /// </summary>
        public record ShiireJissekiCSV {
            [Name("注文コード")]
            [Index(1)]
            public string ChumonId { get; set; }

            [Name("仕入日付")]
            [Index(2)]
            public DateOnly ShiireDate { get; set; }

            [Name("仕入SEQ")]
            [Index(3)]
            public uint SeqByShiireDate { get; set; }

            [Name("仕入日時")]
            [Index(4)]
            public DateTime ShiireDateTime { get; set; }

            [Name("仕入先コード")]
            [Index(5)]
            public string ShiireSakiId { get; set; }

            [Name("仕入先会社")]
            [Index(6)]
            public string ShiireSakiKaisya { get; set; }

            [Name("仕入商品コード")]
            [Index(7)]
            public string ShiirePrdId { get; set; }

            [Name("仕入商品名称")]
            [Index(8)]
            public string ShiirePrdName { get; set; }

            [Name("商品コード")]
            [Index(9)]
            public string ShohinId { get; set; }

            [Name("商品名称")]
            [Index(10)]
            public string ShohinName { get; set; }

            [Name("納入数")]
            [Index(11)]
            public decimal NonyuSu { get; set; }
        }

        /// <summary>
        /// 倉庫在庫画面用
        /// </summary>
        public record SokoZaikoCSV {
            [Name("仕入先コード")]
            [Index(1)]
            public string ShiireSakiId { get; set; }
            [Name("仕入商品コード")]
            [Index(2)]
            public string ShiirePrdId { get; set; }
            [Name("商品コード")]
            [Index(3)]
            public string ShohinId { get; set; }
            [Name("商品名称")]
            [Index(4)]
            public string? ShohinName { get; set; }
            [Name("注文残")]
            [Index(5)]
            public decimal? ChumonZan { get; set; }
            [Name("仕入単位在庫数")]
            [Index(6)]
            public decimal SokoZaikoCaseSu { get; set; }
            [Name("在庫数")]
            [Index(7)]
            public decimal SokoZaikoSu { get; set; }
            [Name("直近仕入日")]
            [Index(8)]
            public DateOnly LastShiireDate { get; set; }
            [Name("直近払出日")]
            [Index(9)]
            public DateOnly? LastDeliveryDate { get; set; }
        }
        /// <summary>
        /// 店頭在庫画面用
        /// </summary>
        public record TentoZaIkoCSV {
            [Name("商品コード")]
            [Index(1)]
            public string? ShohinId { get; set; }

            [Name("商品名称")]
            [Index(2)]
            public string? ShohinName { get; set; }

            [Name("店頭在庫数")]
            [Index(3)]
            public decimal ZaikoSu { get; set; }

            [Name("直近仕入日時")]
            [Index(4)]
            public DateOnly LastShireDateTime { get; set; }

            [Name("直近払出日時")]
            [Index(5)]
            public DateTime? LastHaraidashiDate { get; set; }

            [Name("直近売上日時")]
            [Index(6)]
            public DateTime? LastUriageDatetime { get; set; }
        }
        /// <summary>
        /// 会計実績画面用
        /// </summary>
        public record KaikeiJissekiCSV {

            [Name("売上日時")]
            [Index(1)]
            public DateTime UriageDatetime { get; set; }

            [Name("商品コード")]
            [Index(2)]
            public string? ShohinId { get; set; }

            [Name("商品名称")]
            [Index(3)]
            public string ShohinName { get; set; }

            [Name("売上数量")]
            [Index(4)]
            public decimal UriageSu { get; set; }

            [Name("売上金額")]
            [Index(5)]
            public decimal UriageKingaku { get; set; }

            [Name("税込金額")]
            [Index(6)]
            public decimal ZeikomiKingaku { get; set; }

            [Name("商品単価")]
            [Index(7)]
            public decimal ShohinTanka { get; set; }

            [Name("消費税率")]
            [Index(8)]
            public decimal ShohiZeiritsu { get; set; }

            [Name("内外区分")]
            [Index(9)]
            public string NaigaiClass { get; set; }

            [Name("内外区分名称")]
            [Index(10)]
            public string NaigaiClassName { get; set; }

        }
        /// <summary>
        /// 注文実績画面用
        /// </summary>
        public record ChumonJissekiCSV {
            [Name("注文コード")]
            [Index(1)]
            public string ChumonId { get; set; }
            [Name("仕入先コード")]
            [Index(2)]
            public string ShiireSakiId { get; set; }
            [Name("仕入先会社")]
            [Index(3)]
            public string ShiireSakiKaisya { get; set; }
            [Name("仕入商品コード")]
            [Index(4)]
            public string ShiirePrdId { get; set; }
            [Name("仕入商品名称")]
            [Index(5)]
            public string ShiirePrdName { get; set; }
            [Name("商品コード")]
            [Index(6)]
            public string ShohinId { get; set; }
            [Name("商品名称")]
            [Index(7)]
            public string ShohinName { get; set; }
            [Name("注文数")]
            [Index(8)]
            public decimal ChumonSu { get; set; }
            [Name("仕入済数")]
            [Index(9)]
            public decimal ShiireZumiSu { get; set; }
            [Name("注文残")]
            [Index(10)]
            public decimal ChumonZan { get; set; }
            [Name("注文日")]
            [Index(11)]
            public DateOnly ChumonDate { get; set; }
            [Name("注文金額")]
            [Index(12)]
            public decimal ChumonKingaku { get; set; }
            [Name("仕入済金額")]
            [Index(13)]
            public decimal ShiireZumiKingaku { get; set; }
            [Name("注文残金額")]
            [Index(14)]
            public decimal ChumonZanKingaku { get; set; }
        }
    }
}

