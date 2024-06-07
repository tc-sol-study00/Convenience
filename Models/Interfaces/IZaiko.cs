using Convenience.Models.DataModels;
using System.Linq.Expressions;
using static Convenience.Models.ViewModels.Zaiko.ZaikoViewModel;

namespace Convenience.Models.Interfaces {
    public interface IZaiko {

        public IList<SokoZaiko> SokoZaikos { get; set; }

        public Task<IList<ZaikoListLine>> CreateSokoZaikoList<TSource,TKey>(Expression<Func<TSource, TKey>> sortKey, bool descending);
    }
}
