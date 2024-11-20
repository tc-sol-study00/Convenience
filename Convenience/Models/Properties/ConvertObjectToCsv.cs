using AutoMapper;
using Convenience.Controllers;
using Convenience.Models.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
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
        private readonly CsvConfiguration configuration;         //Csvコンフィグ
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
            if (FilenameDic.TryGetValue(keyword, out string? pickedData)) {
                string yyyyMMddHHmmss = DateTime.Now.ToString("yyyyMMddHHmmss");
                return pickedData.Replace("{yyyyMMddHHmmss}", yyyyMMddHHmmss);
            }

            throw new KeyNotFoundException($"Keyword '{keyword}' not found in FilenameDic.");
        }

    }
}

