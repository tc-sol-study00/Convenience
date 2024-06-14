using Convenience.Models.DataModels;
using static Convenience.Models.ViewModels.Zaiko.ZaikoViewModel;

namespace Convenience.Models.Interfaces {
    public interface IZaikoService {
        public Task<IList<ZaikoListLine>> KeyInput(KeyEventRec[] keydata, SelecteWhereItem[] selecteWhereItemArray);
    }
}
