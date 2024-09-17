using Convenience.Models.DataModels;
using Convenience.Models.ViewModels.Shiire;

namespace Convenience.Models.Interfaces {
    public interface IShiireService {
        public Task<(int, IList<ShiireJisseki>)> ShiireHandling(string inChumonId);
        public Task<(int, IList<ShiireJisseki>)> ShiireHandling(string inChumonId, DateOnly inShiireDate, uint inSeqByShiireDate, IList<ShiireJisseki> inShiireJissekis);
        public Task<ShiireKeysViewModel> SetShiireKeysModel();
        public Task<int> ShiireUpdate();
    }
}
