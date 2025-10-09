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
