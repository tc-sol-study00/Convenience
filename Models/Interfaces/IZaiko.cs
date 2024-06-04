using Convenience.Models.DataModels;
using System.Linq.Expressions;

namespace Convenience.Models.Interfaces {
    public interface IZaiko {

        public IList<SokoZaiko> SokoZaikos { get; set; }

        public Task<IList<SokoZaiko>> CreateSokoZaikoList<TKey>(Expression<Func<SokoZaiko, TKey>> sortKey, bool descending);
    }
}
