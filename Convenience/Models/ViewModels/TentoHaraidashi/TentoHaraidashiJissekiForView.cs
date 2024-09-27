using Convenience.Models.DataModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace Convenience.Models.ViewModels.TentoHaraidashi {
    public class TentoHaraidashiJissekiForView : TentoHaraidashiJisseki {
        public SokoZaiko SokoZaiko { get; set; }

        public TentoZaiko TentoZaiko { get; set; }
    }
}
