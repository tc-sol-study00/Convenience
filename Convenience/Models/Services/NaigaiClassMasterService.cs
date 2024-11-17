using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.ViewModels.NaigaiClassMaster;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static Convenience.Models.Interfaces.IMasterRegistrationService<Convenience.Models.DataModels.NaigaiClassMaster, Convenience.Models.Services.NaigaiClassMasterService.PostMasterData, Convenience.Models.ViewModels.NaigaiClassMaster.NaigaiClassMasterViewModel>;
using static Convenience.Models.Services.NaigaiClassMasterService;
using Microsoft.EntityFrameworkCore;

namespace Convenience.Models.Services {
    public class NaigaiClassMasterService : IMasterRegistrationService<NaigaiClassMaster, PostMasterData, NaigaiClassMasterViewModel> {

        public ConvenienceContext _context { get; set; }

        private readonly IMasterRegistrationService<NaigaiClassMaster, PostMasterData, NaigaiClassMasterViewModel> my;
        public IList<NaigaiClassMaster> KeepMasterDatas { get; set; }

        public IList<PostMasterData> PostedMasterDatas { get; set; }

        public IMasterRegistrationViewModel MasterRegisiationViewModel { get; set; }

        public NaigaiClassMasterService(ConvenienceContext context) {
            _context = context;
            KeepMasterDatas = new List<NaigaiClassMaster>();
            PostedMasterDatas = new List<PostMasterData>();
            MasterRegisiationViewModel = new NaigaiClassMasterViewModel(_context);
            my = this;
        }

        public IList<NaigaiClassMaster> MapFromPostDataToKeepMasterData(IList<PostMasterData> argDatas) {

            IMapper mapper = new MapperConfiguration(cfg => {
                cfg.AddCollectionMappers();
                cfg.CreateMap<PostMasterData, NaigaiClassMaster>()
                .EqualityComparison((src, dest) => src.NaigaiClass == dest.NaigaiClass)
                .ForMember(dest => dest.KaikeiJissekis, opt => opt.Ignore())
                ;
            }).CreateMapper();

            var itemsToAdd = argDatas.Where(a => !KeepMasterDatas.Any(cd => cd.NaigaiClass == a.NaigaiClass)).ToList();
            foreach (var item in itemsToAdd) {
                _context.Set<NaigaiClassMaster>().Add(mapper.Map<NaigaiClassMaster>(item));
            }
            var itemsToRemove = KeepMasterDatas.Where(cd => !argDatas.Any(a => a.NaigaiClass == cd.NaigaiClass)).ToList();
            foreach (var item in itemsToRemove) {
                _context.Set<NaigaiClassMaster>().Remove(item);
            }

            mapper.Map(argDatas, KeepMasterDatas);

            return KeepMasterDatas;
        }

        public IList<PostMasterData> MapFromKeepMasterDataToPostData(IList<NaigaiClassMaster> argDatas) {
            IMapper mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<NaigaiClassMaster, PostMasterData>()
                .EqualityComparison((src, dest) => src.NaigaiClass == dest.NaigaiClass)
                .ForMember(dest => dest.DeleteFlag, opt => opt.MapFrom(src => false));
            }).CreateMapper();

            mapper.Map(argDatas, PostedMasterDatas);
            return PostedMasterDatas;
        }

        public IList<NaigaiClassMaster> QueryMasterData() {
            KeepMasterDatas = _context.NaigaiClassMaster.OrderBy(x => x.NaigaiClass)
                 .ToList();
            return KeepMasterDatas;
        }
        public NaigaiClassMasterViewModel MakeViewModel() {
            return (NaigaiClassMasterViewModel)my.DefaultMakeViewModel();
        }

        public NaigaiClassMasterViewModel UpdateMasterData(NaigaiClassMasterViewModel argMasterRegistrationViewModel) {
            return (NaigaiClassMasterViewModel)my.DefaultUpdateMasterData((IMasterRegistrationViewModel)argMasterRegistrationViewModel);
        }

        public IList<PostMasterData> InsertRow(IList<PostMasterData> PostMasterDatas, int index) {
            return my.DefaultInsertRow(PostMasterDatas, index);
        }

        public class PostMasterData : NaigaiClassMaster, IPostMasterData {

            [DisplayName("削除")]
            public bool DeleteFlag { get; set; }
        }

    }
}
