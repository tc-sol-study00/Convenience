using Convenience.Models.DataModels;

namespace SelfStudy.Propaties {

    //インターフェースはメソッドの定義だけの契約

    //普通の書き方
    public interface IPropaties {
        public ChumonJisseki ChumonJisseki { get; set; }
    }
    public class GetterSetterExplanation1 : IPropaties {

        public ChumonJisseki ChumonJisseki { get; set; }

    }

    //長めに書くと

    public class GetterSetterExplanation2 : IPropaties {

        private ChumonJisseki _chumonJisseki = new ChumonJisseki();
        public ChumonJisseki ChumonJisseki {
            get { return _chumonJisseki; }
            set { _chumonJisseki = value; }
        }
    }

    //Javaのような書き方

    public interface IJavaFuumi {
        public ChumonJisseki GetChumonJisseki();
        void SetChumonJisseki(ChumonJisseki value);
    }

    public class GetterSetterExplanation3 : IJavaFuumi {

        private ChumonJisseki _chumonJisseki = new ChumonJisseki();
        public ChumonJisseki GetChumonJisseki() { return _chumonJisseki; }
        public void SetChumonJisseki(ChumonJisseki value) { _chumonJisseki = value; }
    }
}
