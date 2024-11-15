using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.ViewModels.ShiireMaster;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static Convenience.Models.Interfaces.IMasterRegistrationService<Convenience.Models.DataModels.ShiireMaster, Convenience.Models.Services.ShiireMasterService.PostMasterData, Convenience.Models.ViewModels.ShiireMaster.ShiireMasterViewModel>;
using static Convenience.Models.Services.ShiireMasterService;
using Microsoft.EntityFrameworkCore;

namespace Convenience.Models.Services {
    public class ShiireMasterService : IMasterRegistrationService<ShiireMaster, PostMasterData, ShiireMasterViewModel> {

        public ConvenienceContext _context { get; set; }

        private readonly IMasterRegistrationService<ShiireMaster, PostMasterData, ShiireMasterViewModel> my;
        public IList<ShiireMaster> KeepMasterDatas { get; set; }

        public IList<PostMasterData> PostedMasterDatas { get; set; }

        public IMasterRegistrationViewModel MasterRegisiationViewModel { get; set; }

        public ShiireMasterService(ConvenienceContext context) {
            _context = context;
            KeepMasterDatas = new List<ShiireMaster>();
            PostedMasterDatas = new List<PostMasterData>();
            MasterRegisiationViewModel = new ShiireMasterViewModel(_context);
            my = this;
        }

        public IList<ShiireMaster> MapFromPostDataToKeepMasterData(IList<PostMasterData> argDatas) {

            IMapper mapper = new MapperConfiguration(cfg => {
                cfg.AddCollectionMappers();
                cfg.CreateMap<PostMasterData, ShiireMaster>()
                .EqualityComparison((src, dest) => src.ShiireSakiId == dest.ShiireSakiId && src.ShiirePrdId == dest.ShiirePrdId && src.ShohinId == dest.ShohinId)
                .ForMember(dest => dest.ShohinMaster, opt => opt.Ignore())
                .ForMember(dest => dest.ShiireSakiMaster, opt => opt.Ignore())
                .ForMember(dest => dest.ChumonJissekiMeisaiis, opt => opt.Ignore())
                .ForMember(dest => dest.SokoZaiko, opt => opt.Ignore())
                .ForMember(dest => dest.TentoHaraidashiJissekis, opt => opt.Ignore())
                ;
            }).CreateMapper();

            var itemsToAdd = argDatas.Where(a => !KeepMasterDatas.Any(cd => cd.ShiireSakiId == a.ShiireSakiId && cd.ShiirePrdId == a.ShiirePrdId && cd.ShohinId == a.ShohinId)).ToList();
            foreach (var item in itemsToAdd) {
                _context.Set<ShiireMaster>().Add(mapper.Map<ShiireMaster>(item));
            }
            var itemsToRemove = KeepMasterDatas.Where(cd => !argDatas.Any(a => a.ShiireSakiId == cd.ShiireSakiId && a.ShiirePrdId == cd.ShiirePrdId && a.ShohinId == cd.ShohinId)).ToList();
            foreach (var item in itemsToRemove) {
                _context.Set<ShiireMaster>().Remove(item);
            }

            mapper.Map(argDatas, KeepMasterDatas);

            return KeepMasterDatas;
        }

        public IList<PostMasterData> MapFromKeepMasterDataToPostData(IList<ShiireMaster> argDatas) {
            IMapper mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<ShiireMaster, PostMasterData>()
                .EqualityComparison((src, dest) => src.ShohinId == dest.ShohinId)
                .ForMember(dest => dest.DeleteFlag, opt => opt.MapFrom(src => false));
            }).CreateMapper();

            mapper.Map(argDatas, PostedMasterDatas);
            return PostedMasterDatas;
        }

        public IList<ShiireMaster> QueryMasterData() {
            KeepMasterDatas = _context.ShiireMaster.OrderBy(x => x.ShiireSakiId).ThenBy(x => x.ShiirePrdId).ThenBy(x => x.ShohinId)
                 .Include(x => x.ShiireSakiMaster)
                 .ToList();
            return KeepMasterDatas;
        }
        public ShiireMasterViewModel MakeViewModel() {
            return (ShiireMasterViewModel)my.DefaultMakeViewModel();
        }

        public ShiireMasterViewModel UpdateMasterData(ShiireMasterViewModel argMasterRegistrationViewModel) {
            return (ShiireMasterViewModel)my.DefaultUpdateMasterData((IMasterRegistrationViewModel)argMasterRegistrationViewModel);
        }

        public IList<PostMasterData> InsertRow(IList<PostMasterData> PostMasterDatas, int index) {
            return my.DefaultInsertRow(PostMasterDatas, index);
        }

        public class PostMasterData : ShiireMaster, IPostMasterData {

            [DisplayName("削除")]
            public bool DeleteFlag { get; set; }
        }

    }
}
