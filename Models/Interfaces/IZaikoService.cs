using Convenience.Models.DataModels;

namespace Convenience.Models.Interfaces {
    public interface IZaikoService {
        public Task<IList<SokoZaiko>> KeyInput(string key, bool inDescendig);
    }
}
