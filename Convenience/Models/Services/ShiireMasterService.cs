using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.ViewModels.ShiireMaster;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using static Convenience.Models.Interfaces.IMasterRegistrationService<Convenience.Models.DataModels.ShiireMaster, Convenience.Models.Services.ShiireMasterService.PostMasterData, Convenience.Models.ViewModels.ShiireMaster.ShiireMasterViewModel>;
using static Convenience.Models.Services.ShiireMasterService;

namespace Convenience.Models.Services {
    /// <summary>
    /// 仕入マスタの管理サービスクラス
    /// </summary>
    public class ShiireMasterService : IMasterRegistrationService<ShiireMaster, PostMasterData, ShiireMasterViewModel> {

        /// <summary>
        /// データベースコンテキスト
        /// </summary>
        public ConvenienceContext _context { get; set; }

        // 自分自身をインターフェースとして参照
        private readonly IMasterRegistrationService<ShiireMaster, PostMasterData, ShiireMasterViewModel> my;

        /// <summary>
        /// 現在保持しているマスタデータ
        /// </summary>
        public IList<ShiireMaster> KeepMasterDatas { get; set; }

        /// <summary>
        /// Postされたマスタデータ
        /// </summary>
        public IList<PostMasterData> PostedMasterDatas { get; set; }

        /// <summary>
        /// ビューモデル
        /// </summary>
        public IMasterRegistrationViewModel MasterRegisiationViewModel { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">データベースコンテキスト</param>
        public ShiireMasterService(ConvenienceContext context) {
            _context = context;
            KeepMasterDatas = new List<ShiireMaster>();
            PostedMasterDatas = new List<PostMasterData>();
            MasterRegisiationViewModel = new ShiireMasterViewModel(_context);
            my = this; // 自身をインターフェース型として格納
        }

        /// <summary>
        /// Postデータを保持データにマッピング
        /// </summary>
        /// <param name="argDatas">Postデータリスト</param>
        /// <returns>保持データリスト</returns>
        public IList<ShiireMaster> MapFromPostDataToKeepMasterData(IList<PostMasterData> argDatas) {
            // AutoMapperの設定
            IMapper mapper = new MapperConfiguration(cfg => {
                cfg.AddCollectionMappers(); // コレクションのマッピングを有効化
                cfg.CreateMap<PostMasterData, ShiireMaster>()
                .EqualityComparison((src, dest) =>
                    src.ShiireSakiId == dest.ShiireSakiId &&
                    src.ShiirePrdId == dest.ShiirePrdId &&
                    src.ShohinId == dest.ShohinId) // 主キーで比較
                .ForMember(dest => dest.ShohinMaster, opt => opt.Ignore()) // ナビゲーションプロパティを無視
                .ForMember(dest => dest.ShiireSakiMaster, opt => opt.Ignore())
                .ForMember(dest => dest.ChumonJissekiMeisaiis, opt => opt.Ignore())
                .ForMember(dest => dest.SokoZaiko, opt => opt.Ignore())
                .ForMember(dest => dest.TentoHaraidashiJissekis, opt => opt.Ignore());
            }).CreateMapper();

            // 新規アイテムの追加
            var itemsToAdd = argDatas.Where(a =>
                !KeepMasterDatas.Any(cd =>
                    cd.ShiireSakiId == a.ShiireSakiId &&
                    cd.ShiirePrdId == a.ShiirePrdId &&
                    cd.ShohinId == a.ShohinId)).ToList();
            foreach (var item in itemsToAdd) {
                _context.Set<ShiireMaster>().Add(mapper.Map<ShiireMaster>(item));
            }

            // 不要アイテムの削除
            var itemsToRemove = KeepMasterDatas.Where(cd =>
                !argDatas.Any(a =>
                    a.ShiireSakiId == cd.ShiireSakiId &&
                    a.ShiirePrdId == cd.ShiirePrdId &&
                    a.ShohinId == cd.ShohinId)).ToList();
            foreach (var item in itemsToRemove) {
                _context.Set<ShiireMaster>().Remove(item);
            }

            // 保持データを更新
            mapper.Map(argDatas, KeepMasterDatas);

            return KeepMasterDatas;
        }

        /// <summary>
        /// 保持データをPostデータにマッピング
        /// </summary>
        public IList<PostMasterData> MapFromKeepMasterDataToPostData(IList<ShiireMaster> argDatas) {
            IMapper mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<ShiireMaster, PostMasterData>()
                .EqualityComparison((src, dest) => src.ShohinId == dest.ShohinId) // 商品IDで比較
                .ForMember(dest => dest.DeleteFlag, opt => opt.MapFrom(src => false)); // 削除フラグをfalseに設定
            }).CreateMapper();

            // マッピングを実行
            mapper.Map(argDatas, PostedMasterDatas);
            return PostedMasterDatas;
        }

        /// <summary>
        /// データベースから仕入マスタデータを取得
        /// </summary>
        public IList<ShiireMaster> QueryMasterData() {
            KeepMasterDatas = _context.ShiireMaster.OrderBy(x => x.ShiireSakiId)
                .ThenBy(x => x.ShiirePrdId)
                .ThenBy(x => x.ShohinId)
                .Include(x => x.ShiireSakiMaster) // ナビゲーションプロパティをロード
                .ToList();
            return KeepMasterDatas;
        }

        /// <summary>
        /// ビューモデルを作成
        /// </summary>
        public ShiireMasterViewModel MakeViewModel() {
            return (ShiireMasterViewModel)my.DefaultMakeViewModel();
        }

        /// <summary>
        /// ビューモデルを基にマスタデータを更新
        /// </summary>
        public ShiireMasterViewModel UpdateMasterData(ShiireMasterViewModel argMasterRegistrationViewModel) {
            return (ShiireMasterViewModel)my.DefaultUpdateMasterData((IMasterRegistrationViewModel)argMasterRegistrationViewModel);
        }

        /// <summary>
        /// 新しい行を挿入
        /// </summary>
        public IList<PostMasterData> InsertRow(IList<PostMasterData> PostMasterDatas, int index) {
            return my.DefaultInsertRow(PostMasterDatas, index);
        }

        /// <summary>
        /// Postデータ用クラス
        /// </summary>
        public class PostMasterData : ShiireMaster, IPostMasterData {

            /// <summary>
            /// 削除フラグ
            /// </summary>
            [DisplayName("削除")]
            public bool DeleteFlag { get; set; }
        }

    }
}
