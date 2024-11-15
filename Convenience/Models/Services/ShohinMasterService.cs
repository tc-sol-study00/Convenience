using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.ViewModels.ShohinMaster;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using static Convenience.Models.Interfaces.IMasterRegistrationService<Convenience.Models.DataModels.ShohinMaster, Convenience.Models.Services.ShohinMasterService.PostMasterData, Convenience.Models.ViewModels.ShohinMaster.ShohinMasterViewModel>;
using static Convenience.Models.Services.ShohinMasterService;

namespace Convenience.Models.Services {
    public class ShohinMasterService : IMasterRegistrationService<ShohinMaster, PostMasterData, ShohinMasterViewModel> {

        public ConvenienceContext _context { get; set; }

        private readonly IMasterRegistrationService<ShohinMaster, PostMasterData, ShohinMasterViewModel> my;
        public IList<ShohinMaster> KeepMasterDatas { get; set; }

        public IList<PostMasterData> PostedMasterDatas { get; set; }

        public IMasterRegistrationViewModel MasterRegisiationViewModel { get; set; }

        public ShohinMasterService(ConvenienceContext context) {
            _context = context;
            KeepMasterDatas = new List<ShohinMaster>();
            PostedMasterDatas = new List<PostMasterData>();
            MasterRegisiationViewModel = new ShohinMasterViewModel();
            my = this;
        }

        public IList<ShohinMaster> MapFromPostDataToKeepMasterData(IList<PostMasterData> argDatas) {

            IMapper mapper = new MapperConfiguration(cfg => {
                cfg.AddCollectionMappers();
                cfg.CreateMap<PostMasterData, ShohinMaster>()
                .EqualityComparison((src, dest) => src.ShohinId == dest.ShohinId)
                .ForMember(dest => dest.ShiireMasters, opt => opt.Ignore())
                .ForMember(dest => dest.TentoZaiko, opt => opt.Ignore());
            }).CreateMapper();

            var itemsToAdd = argDatas.Where(a => !KeepMasterDatas.Any(cd => cd.ShohinId == a.ShohinId)).ToList();
            foreach (var item in itemsToAdd) {
                _context.Set<ShohinMaster>().Add(mapper.Map<ShohinMaster>(item));
            }
            var itemsToRemove = KeepMasterDatas.Where(cd => !argDatas.Any(a => a.ShohinId == cd.ShohinId)).ToList();
            foreach (var item in itemsToRemove) {
                _context.Set<ShohinMaster>().Remove(item);
            }

            mapper.Map(argDatas, KeepMasterDatas);

            return KeepMasterDatas;
        }

        public IList<PostMasterData> MapFromKeepMasterDataToPostData(IList<ShohinMaster> argDatas) {
            IMapper mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<ShohinMaster, PostMasterData>()
                .EqualityComparison((src, dest) => src.ShohinId == dest.ShohinId)
                .ForMember(dest => dest.DeleteFlag, opt => opt.MapFrom(src => false));
            }).CreateMapper();

            mapper.Map(argDatas, PostedMasterDatas);
            return PostedMasterDatas;
        }

        public IList<ShohinMaster> QueryMasterData() {
            KeepMasterDatas = _context.ShohinMaster.OrderBy(x => x.ShohinId)
                 .ToList();
            return KeepMasterDatas;
        }
        public ShohinMasterViewModel MakeViewModel() {
            var viewModel = this.MasterRegisiationViewModel;
            return (ShohinMasterViewModel)my.DefaultMakeViewModel();
        }

        public ShohinMasterViewModel UpdateMasterData(ShohinMasterViewModel argMasterRegistrationViewModel) {
            return (ShohinMasterViewModel)my.DefaultUpdateMasterData((IMasterRegistrationViewModel)argMasterRegistrationViewModel);
        }

        public IList<PostMasterData> InsertRow(IList<PostMasterData> PostMasterDatas, int index) {
            return my.DefaultInsertRow(PostMasterDatas, index);
        }

        public class PostMasterData : ShohinMaster, IPostMasterData {

            [DisplayName("削除")]
            public bool DeleteFlag { get; set; }
        }

    }
}
