using Convenience.Models.DataModels;

namespace SelfStudy.Interfaces {
    internal interface IObjectCopy {

        public static int Flg { get; set; }
        public IList<ChumonJisseki> OriginalChumonJissekis { get; set; }

        public IList<ChumonJisseki> ClonedChumonJissekis { get; set; }

        public Task<IList<ChumonJisseki>> ObjectCopyController();
    }
}
