using Convenience.Models.Interfaces;
using System.Reflection;

namespace SelfStudy.ChumonJissekiReception.Interfaces {

    public class DisplayResult {

        /// <summary>
        /// カラム毎の幅数管理用配列
        /// </summary>
        private int[] lengthArray;

        /// <summary>
        /// 表示用Delegate
        /// </summary>
        private static Action<string> wl = value => Console.WriteLine(value);
        private static Action<string> w = value => Console.Write(value);

        /// <summary>
        /// 初期化
        /// </summary>
        public DisplayResult() {
            lengthArray = new int[] { };
        }

        /// <summary>
        /// リスト形式のデータを表示する
        /// </summary>
        /// <param name="inDisplayDatas">表示するオブジェクト(Collection型）</param>
        /// <param name="inAttribute">表示するオブジェクト名の横に沿える文字列</param>
        public void DisplayData(IEnumerable<object> inDisplayDatas, string inAttribute = "") {

            /*
             * 全行を対象に最大横幅を各カラム毎に求める
             */

            foreach (var aDisplayData in inDisplayDatas) SetDisplayWidth(aDisplayData);

            /*
             * 上記の最大横幅を元に、プロパティと全行のデータを表示する
             */

            bool isNeedHeader = true;
            foreach (var data in inDisplayDatas) {
                Display(data, isNeedHeader, inAttribute);
                isNeedHeader &= false;
            }
        }

        /// <summary>
        /// 単体のデータを表示する
        /// </summary>
        /// <param name="inDisplayData">表示するオブジェクト</param>
        /// <param name="inAttribute">表示するオブジェクト名の横に添える文字列</param>
        public void DisplayData(object inDisplayData, string inAttribute = "") {

            /*
             * 最大横幅を各カラム毎に求める
             */
            
            SetDisplayWidth(inDisplayData);
            
            /*
             * 上記の最大横幅を元に、プロパティとデータを表示する
             */
            
            Display(inDisplayData, true, inAttribute);
        }

        /// <summary>
        /// 最大横幅を各カラム毎に求める
        /// </summary>
        /// <param name="inDisplayData">対象オブジェクト</param>

        private void SetDisplayWidth(object inDisplayData) {

            //引数のオブジェクトのプロパティを求める
            var properties = inDisplayData.GetType().GetProperties();
            //各カラムの文字サイズをセットするための配列を作成
            lengthArray = new int[properties.Length];

            /*
             * カラム名・データの文字サイズ調査
             */

            for (int counter = 0; counter < properties.Length; counter++) {
                PropertyInfo aProperty = properties[counter];

                //表示対象のプロパティか？
                if (!IsAvairableType(aProperty)) continue;

                /*
                 * 表示対象のプロパティの文字サイズチェック
                 */

                // カラム名の文字サイズ
                lengthArray[counter] = aProperty.Name.Sum(c => (c > 127 ? 2 : 1));

                // データの文字サイズ
                int length = SetAnyDataToString(aProperty, inDisplayData).Sum(c => (c > 127 ? 2 : 1));
                lengthArray[counter] = Math.Max(length, lengthArray[counter]);
            }
        }

        /// <summary>
        /// 最大横幅を元に、プロパティと全行のデータを表示する
        /// </summary>
        /// <param name="inDisplayData">対象オブジェクト</param>
        /// <param name="isNeedHeader">カラム情報を出したい場合はtrue</param>
        /// <param name="inAttribute">表示するオブジェクト名の横に添える文字列</param>
        private void Display(object inDisplayData, bool isNeedHeader = true, string inAttribute = "") {
            var properties = inDisplayData.GetType().GetProperties();
            var validProperties = properties.Where(IsAvairableType).ToArray();
            int allColumQty = lengthArray.Sum() + validProperties.Length + 1;

            /*
             * カラム情報表示
             */
            
            if (isNeedHeader) { //カラム情報を出すかの判断
                
                /*
                 * オブジェクト名表示
                 */

                string headerName = $"{inDisplayData.GetType().Name}({inAttribute})";
                int allColumQtyHeader = Math.Max(headerName.Length + 2, allColumQty);
                wl(new string('-', allColumQtyHeader));
                wl($"|{PadString(headerName, -(allColumQtyHeader - 2))}|");
                wl(new string('-', allColumQtyHeader));

                /*
                 * ヘッダー表示
                 */

                w("|");
                foreach (var prop in validProperties) {
                    w(PadString(prop.Name, lengthArray[Array.IndexOf(properties, prop)] * JudgeValueOnLeftOrRight(prop)) + "|");
                }
                w("\n");
                wl(new string('-', allColumQty));
            }

            /*
             * データ表示
             */

            w("|");
            foreach (var prop in validProperties) {
                w(PadString(SetAnyDataToString(prop, inDisplayData), lengthArray[Array.IndexOf(properties, prop)] * JudgeValueOnLeftOrRight(prop)) + "|");
            }
            w("\n");
            wl(new string('-', allColumQty));
        }

        /// <summary>
        /// 文字列に変換する
        /// </summary>
        /// <param name="aProperty">プロパティ情報</param>
        /// <param name="inDisplayData">対象オブジェクト</param>
        /// <returns>文字列に変換されたオブジェクト</returns>
        private static string SetAnyDataToString(PropertyInfo aProperty, object inDisplayData) =>
            IsAvairableType(aProperty) ? aProperty.GetValue(inDisplayData)?.ToString() ?? string.Empty : string.Empty;

        /// <summary>
        /// 数値タイプグループ HashSetは重複不可、データ登録順番の保証なし、だけど高速 
        /// </summary>
        private static readonly HashSet<Type> NumericTypes = new()
        {
            typeof(int), typeof(uint), typeof(decimal), typeof(long), typeof(ulong),
            typeof(double), typeof(float)
        };

        /// <summary>
        /// 表示対象タイプグループ 
        /// <remark>オブジェクト変数などは対象外</remark>
        /// </summary>
        private static readonly HashSet<Type> AvailableTypes = new()
        {
            typeof(int), typeof(uint), typeof(decimal), typeof(string),
            typeof(DateOnly), typeof(DateTime), typeof(long), typeof(ulong),
            typeof(double), typeof(float), typeof(bool), typeof(Guid),
            typeof(byte), typeof(char)
        };

        /// <summary>
        /// 数値型の判断（数値型の場合、1（右寄せ）。それ以外（文字列や日付・時間系）なら-1（左寄せ）
        /// </summary>
        /// <param name="aProperty">プロパティ情報</param>
        /// <returns>数値型ならtrue</returns>
        private static int JudgeValueOnLeftOrRight(PropertyInfo aProperty) =>
            NumericTypes.Contains(aProperty.PropertyType) ? 1 : -1;

        /// <summary>
        /// 表示対象タイプの判断
        /// </summary>
        /// <param name="aProperty">プロパティ情報</param>
        /// <returns>表示対象ならtrue</returns>
        private static bool IsAvairableType(PropertyInfo aProperty) =>
            AvailableTypes.Contains(aProperty.PropertyType);

        /// <summary>
        /// 右寄せ・左寄せ・パディング処理
        /// </summary>
        /// <param name="input">表示用データ</param>
        /// <param name="length">表示する幅数（英数字系は１、２バイト文字系は2として数える）　左寄せの場合、マイナス数値</param>
        /// <param name="padchar">パディング用文字</param>
        /// <returns></returns>
        public static string PadString(string? input, int length, char padchar = ' ') {
            input ??= string.Empty;
            int byteLength = input.Sum(c => (c > 127 ? 2 : 1));
            return length >= 0
                ? input.PadLeft(Math.Abs(length) - byteLength + input.Length, padchar)
                : input.PadRight(Math.Abs(length) - byteLength + input.Length, padchar);
        }
    }
}