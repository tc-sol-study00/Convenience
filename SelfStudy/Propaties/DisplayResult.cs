using Convenience.Models.Interfaces;
using System.Reflection;

using t = Convenience.Models.Interfaces.ISharedTools;

namespace SelfStudy.ChumonJissekiReception.Interfaces {

    public class DisplayResult : ISharedTools {

        private int[] lengthArray;

        private static Action<string> wl = value => Console.WriteLine(value);
        private static Action<string> w = value => Console.Write(value);

        public DisplayResult() {
            lengthArray = new int[] { };
        }

        //リスト形式のデータを表示する
        public void DisplayData(IEnumerable<object> inDisplayDatas, string inAttribute = "") {
            foreach (var aDisplayData in inDisplayDatas) {
                SetDisplayWidth(aDisplayData);
            }

            bool isNeedHeader = true;
            foreach (var aDisplayData in inDisplayDatas) {
                Display(aDisplayData, isNeedHeader, inAttribute);
                isNeedHeader = false;
            }
        }

        //単体のデータを表示する
        public void DisplayData(object inDisplayData, string inAttribute = "") {
            SetDisplayWidth(inDisplayData);
            Display(inDisplayData, true, inAttribute);
        }

        //データとカラム名から最大の横の長さをカラム毎に求める

        private void SetDisplayWidth(object inDisplayData) {

            Action<string> w = value => Console.Write(value);
            Action<string> wl = value => w(value + "\n");

            //引数のオブジェクトのプロパティを求める
            var properties = inDisplayData.GetType().GetProperties();
            //各カラムの文字サイズをセットするための配列を作成
            lengthArray = new int[properties.Length];

            //カラム名文字サイズ調査
            for (int counter = 0; counter < properties.Length; counter++) {
                PropertyInfo aProperty = properties[counter];
                if (IsAvairableType(aProperty)) {
                    lengthArray[counter] = IsAvairableType(properties[counter]) ? properties[counter].Name.Sum(c => (c > 127 ? 2 : 1)) : 1;
                }
            }

            //データ文字サイズ調査
            for (int counter = 0; counter < lengthArray.Length; counter++) {
                PropertyInfo aProperty = properties[counter];

                if (IsAvairableType(aProperty)) {
                    string strValue = SetAnyDataToString(aProperty, inDisplayData);
                    int length = strValue.Sum(c => (c > 127 ? 2 : 1));
                    lengthArray[counter] = (length > lengthArray[counter]) ? length : lengthArray[counter];
                }
            }
        }

        //データ表示
        private void Display(object inDisplayData, bool isNeedHearder = true, string inAttribute = "") {

            var properties = inDisplayData.GetType().GetProperties();
            int allColumQty = lengthArray.Sum(c => c) + lengthArray.Count(x => x > 0) + 1;

            if (isNeedHearder) {
                //オブジェクト名表示
                string className = inDisplayData.GetType().Name;
                string headerName = className + "(" + inAttribute + ")";

                int allColumQtyHeader;
                if (headerName.Length > (allColumQty - 2)) {
                    allColumQtyHeader = headerName.Length + 2;
                }
                else {
                    allColumQtyHeader = allColumQty;
                }

                wl(new string('-', allColumQtyHeader));
                wl("|" + t.PadString(headerName, (allColumQtyHeader - 2) * (-1)) + "|");
                wl(new string('-', allColumQtyHeader));

                //ヘッダー表示

                for (int counter = 0; counter < properties.Length; counter++) {
                    PropertyInfo aProperty = properties[counter];
                    if (counter == 0) {
                        Console.Write("|");
                    }
                    if (IsAvairableType(aProperty)) {
                        int flgLeftOrRight = JudgeValueOnLeftOrRight(aProperty);
                        w(t.PadString(aProperty.Name, lengthArray[counter] * flgLeftOrRight) + "|");
                    }
                }
                w("\n");
                wl(new string('-', allColumQty));
            }

            for (int counter = 0; counter < properties.Length; counter++) {

                PropertyInfo aProperty = properties[counter];

                if (counter == 0) {
                    w("|");
                }
                if (IsAvairableType(aProperty)) {   //表示対象か

                    string strValue = SetAnyDataToString(aProperty, inDisplayData);

                    int flgLeftOrRight = JudgeValueOnLeftOrRight(aProperty);
                    w(t.PadString(strValue, lengthArray[counter] * flgLeftOrRight) + "|");
                }
            }
            w("\n");
            wl(new string('-', allColumQty));
        }



        //文字に変換する
        private static string SetAnyDataToString(PropertyInfo aProperty, object inDisplayData) {

            return IsAvairableType(aProperty) ?
                aProperty?.GetValue(inDisplayData)?.ToString() ?? string.Empty : string.Empty;
        }

        //数値型の判断（数値型の場合、1（右寄せ）。それ以外（文字列や日付・時間系）なら-1（左寄せ）
        private static int JudgeValueOnLeftOrRight(PropertyInfo aProperty) {
            return aProperty.PropertyType == typeof(int) ||
                aProperty.PropertyType == typeof(uint) ||
                aProperty.PropertyType == typeof(decimal) ||
                aProperty.PropertyType == typeof(long) ||
                aProperty.PropertyType == typeof(ulong) ||
                aProperty.PropertyType == typeof(double) ||
                aProperty.PropertyType == typeof(float) ? 1 : -1;
        }

        //表示対象タイプの判断
        private static bool IsAvairableType(PropertyInfo aProperty) {
            return aProperty.PropertyType == typeof(int) ||
                aProperty.PropertyType == typeof(uint) ||
                aProperty.PropertyType == typeof(decimal) ||
                aProperty.PropertyType == typeof(string) ||
                aProperty.PropertyType == typeof(DateOnly) ||
                aProperty.PropertyType == typeof(DateTime) ||
                aProperty.PropertyType == typeof(long) ||
                aProperty.PropertyType == typeof(ulong) ||
                aProperty.PropertyType == typeof(double) ||
                aProperty.PropertyType == typeof(float) ||
                aProperty.PropertyType == typeof(bool) ||
                aProperty.PropertyType == typeof(Guid) ||
                aProperty.PropertyType == typeof(byte) ||
                aProperty.PropertyType == typeof(char);
        }
    }
}