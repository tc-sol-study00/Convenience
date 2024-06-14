using Convenience.Models.DataModels;
using System.Linq.Expressions;
using static Convenience.Models.ViewModels.Zaiko.ZaikoViewModel;

namespace Convenience.Models.Interfaces {
    public interface IZaiko {

        public IQueryable<ZaikoListLine>? SoKoZaikoQueryable { get; set; }

        public IQueryable<ZaikoListLine> CreateSokoZaikoList(string searchKey);
        public IQueryable<ZaikoListLine> AddOrderby(string sortKey, bool descending);
        }
}
