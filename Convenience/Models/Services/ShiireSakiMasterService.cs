using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.ViewModels.ShiireSakiMaster;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static Convenience.Models.Interfaces.IMasterRegistrationService<Convenience.Models.DataModels.ShiireSakiMaster, Convenience.Models.Services.ShiireSakiMasterService.PostMasterData, Convenience.Models.ViewModels.ShiireSakiMaster.ShiireSakiMasterViewModel>;
using static Convenience.Models.Services.ShiireSakiMasterService;
using Microsoft.EntityFrameworkCore;

namespace Convenience.Models.Services {
    public class ShiireSakiMasterService : IMasterRegistrationService<ShiireSakiMaster, PostMasterData, ShiireSakiMasterViewModel> {

        public ConvenienceContext _context { get; set; }

        private readonly IMasterRegistrationService<ShiireSakiMaster, PostMasterData, ShiireSakiMasterViewModel> my;
        public IList<ShiireSakiMaster> KeepMasterDatas { get; set; }

        public IList<PostMasterData> PostedMasterDatas { get; set; }

        public IMasterRegistrationViewModel MasterRegisiationViewModel { get; set; }

        public ShiireSakiMasterService(ConvenienceContext context) {
            _context = context;
            KeepMasterDatas = new List<ShiireSakiMaster>();
            PostedMasterDatas = new List<PostMasterData>();
            MasterRegisiationViewModel = new ShiireSakiMasterViewModel(_context);
            my = this;
        }

        public IList<ShiireSakiMaster> MapFromPostDataToKeepMasterData(IList<PostMasterData> argDatas) {

            IMapper mapper = new MapperConfiguration(cfg => {
                cfg.AddCollectionMappers();
                cfg.CreateMap<PostMasterData, ShiireSakiMaster>()
                .EqualityComparison((src, dest) => src.ShiireSakiId == dest.ShiireSakiId)
                .ForMember(dest => dest.ShireMasters, opt => opt.Ignore())
                .ForMember(dest => dest.ChumonJissekis, opt => opt.Ignore())
                ;
            }).CreateMapper();

            var itemsToAdd = argDatas.Where(a => !KeepMasterDatas.Any(cd => cd.ShiireSakiId == a.ShiireSakiId)).ToList();
            foreach (var item in itemsToAdd) {
                _context.Set<ShiireSakiMaster>().Add(mapper.Map<ShiireSakiMaster>(item));
            }
            var itemsToRemove = KeepMasterDatas.Where(cd => !argDatas.Any(a => a.ShiireSakiId == cd.ShiireSakiId)).ToList();
            foreach (var item in itemsToRemove) {
                _context.Set<ShiireSakiMaster>().Remove(item);
            }

            mapper.Map(argDatas, KeepMasterDatas);

            return KeepMasterDatas;
        }

        public IList<PostMasterData> MapFromKeepMasterDataToPostData(IList<ShiireSakiMaster> argDatas) {
            IMapper mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<ShiireSakiMaster, PostMasterData>()
                .EqualityComparison((src, dest) => src.ShiireSakiId == dest.ShiireSakiId)
                .ForMember(dest => dest.DeleteFlag, opt => opt.MapFrom(src => false));
            }).CreateMapper();

            mapper.Map(argDatas, PostedMasterDatas);
            return PostedMasterDatas;
        }

        public IList<ShiireSakiMaster> QueryMasterData() {
            KeepMasterDatas = _context.ShiireSakiMaster.OrderBy(x => x.ShiireSakiId)
                 .ToList();
            return KeepMasterDatas;
        }
        public ShiireSakiMasterViewModel MakeViewModel() {
            return (ShiireSakiMasterViewModel)my.DefaultMakeViewModel();
        }

        public ShiireSakiMasterViewModel UpdateMasterData(ShiireSakiMasterViewModel argMasterRegistrationViewModel) {
            return (ShiireSakiMasterViewModel)my.DefaultUpdateMasterData((IMasterRegistrationViewModel)argMasterRegistrationViewModel);
        }

        public IList<PostMasterData> InsertRow(IList<PostMasterData> PostMasterDatas, int index) {
            return my.DefaultInsertRow(PostMasterDatas, index);
        }

        public class PostMasterData : ShiireSakiMaster, IPostMasterData {

            [DisplayName("削除")]
            public bool DeleteFlag { get; set; }
        }

    }
}
