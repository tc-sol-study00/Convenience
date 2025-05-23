using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debug {
    public interface ITools {

        public bool IsExist<T>(T argData);
        public bool IsExistProc<T>(T argData) {
            if (typeof(T) == typeof(string)) {
                if (argData != null) {
                    return true;
                }
                else {
                    return false;
                }
            }
            else if (typeof(T) == typeof(int)) {
                if ((int)(object)argData > default(int)) {
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                return false;
            }
        }
    }

    public class Tsukau : ITools {

        public bool IsExist<T>(T argData) {
            return IsExistProc(argData);
        }
        public void M1() {

            IsExist("123");

        }
    }
}
