using CsvHelper.Configuration;
using System.Text;

namespace Convenience.Models.Interfaces {
    /// <summary>
    /// CSVファイルを作る用インターフェース
    /// </summary>
    public interface IConvertObjectToCsv
        {
        /// <summary>
        /// Csvコンフィグ
        /// </summary>
        /// <returns>Csvコンフィグ・エンコード</returns>
        public (CsvConfiguration config, Encoding encording) CsvConfig();

        /// <summary>
        /// CSVへ変換
        /// </summary>
        /// <typeparam name="T1">CSV化したいモデル</typeparam>
        /// <typeparam name="T2">CSVMappingモデル</typeparam>
        /// <param name="arg1">変換元モデル</param>
        /// <param name="arg2">CSVマッピングモデル</param>
        /// <returns>作られたファイル名（フルパス）</returns>
        public string ConvertToCSV<T1, T2>(IEnumerable<T1> arg1, string arg2)
            where T1 : class
            where T2 : class;

        /// <summary>
        /// ファイル名を取得する
        /// </summary>
        /// <param name="arg1">コントローラー名</param>
        /// <returns>ファイル名</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public string GetFileName(string arg1);
    }
}
