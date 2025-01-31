using Convenience.Migrations;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using t = Convenience.Models.Interfaces.ISharedTools;

namespace SelfStudy.ChumonJissekiReception.Interfaces {

    public interface IDisplay : ISharedTools {
        public static void DisplayData(object inDisplayData) {

            Action<string> w = value => Console.Write(value);
            Action<string> wl = value => w(value+"\n");
            var properties = inDisplayData.GetType().GetProperties();
            int[] lengthArray = new int[properties.Length];

            //Hearder処理サイズ調査
            for (int counter = 0; counter < properties.Length; counter++) {
                if (IsAvairableType(properties[counter])) {
                    lengthArray[counter] = properties[counter].Name.Sum(c => (c > 127 ? 2 : 1));
                }
            }

            //データサイズ調査
            for (int counter = 0; counter < lengthArray.Length; counter++) {
                PropertyInfo aProperty = properties[counter];

                if (IsAvairableType(aProperty)) {
                    string? strValue = SetAnyDataToString(aProperty, inDisplayData);
                    int length = strValue?.Sum(c => (c > 127 ? 2 : 1)) ?? 0;
                    lengthArray[counter] = (length > lengthArray[counter]) ? length : lengthArray[counter];
                }
            }

            //オブジェクト名表示
            string className = inDisplayData.GetType().Name;
            int allColumQty = lengthArray.Sum(c => c) + lengthArray.Count(x => x > 0) + 1;
            wl(new string('-', allColumQty));
            wl("|" + t.PadString(className, (allColumQty - 2) * (-1)) + "|");
            wl(new string('-', allColumQty));

            //ヘッダー表示

            for (int counter = 0; counter < properties.Length; counter++) {
                PropertyInfo aProperty = properties[counter];
                if (IsAvairableType(aProperty)) {
                    if (counter == 0) {
                        Console.Write("|");
                    }
                    int flgLeftOrRight = JudgeValueOnLeftOrRight(aProperty);
                    Console.Write(t.PadString(aProperty.Name, lengthArray[counter] * flgLeftOrRight) + "|");
                }
            }
            w("\n");
            wl(new string('-', allColumQty));
            for (int counter = 0; counter < properties.Length; counter++) {
                PropertyInfo aProperty = properties[counter];
                if (IsAvairableType(aProperty)) {
                    if (counter == 0) {
                        Console.Write("|");
                    }
                    string strValue = SetAnyDataToString(aProperty, inDisplayData);

                    int flgLeftOrRight = JudgeValueOnLeftOrRight(aProperty);
                    Console.Write(t.PadString(strValue, lengthArray[counter] * flgLeftOrRight) + "|");
                }
            }
            w("\n");
            wl(new string('-', allColumQty));

        }

        private static int JudgeValueOnLeftOrRight(PropertyInfo aProperty) {
            if (aProperty.PropertyType == typeof(int) ||
                aProperty.PropertyType == typeof(uint) ||
                aProperty.PropertyType == typeof(decimal) ||
                aProperty.PropertyType == typeof(long) ||
                aProperty.PropertyType == typeof(ulong) ||
                aProperty.PropertyType == typeof(double) ||
                aProperty.PropertyType == typeof(float)
                ) {
                return 1;
            }
            else {
                return -1;
            }
        }

        private static string? SetAnyDataToString(PropertyInfo aProperty, object inDisplayData) {

            string? strValue = default;
            if (IsAvairableType(aProperty)) {
                strValue = aProperty?.GetValue(inDisplayData)?.ToString() ?? string.Empty;
            }
            else {
                return null;
            }

            return strValue;
        }

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