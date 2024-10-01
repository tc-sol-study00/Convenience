using Convenience.Models.DataModels;

namespace Convenience.Models.Interfaces {
    public interface ITentoHaraidashi {

        public TentoHaraidashiJisseki TentoHaraidashiJisseki { get; set; }

        public Task<TentoHaraidashiHeader> TentoHaraidashiSakusei(DateTime argCurrentDateTime);

    }
}
