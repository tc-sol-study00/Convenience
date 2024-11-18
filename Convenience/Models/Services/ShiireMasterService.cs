using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.ViewModels.ShiireMaster;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace Convenience.Models.Services {
    /// <summary>
    /// 仕入マスタの管理サービスクラス
    /// </summary>
    public class ShiireMasterService : IShiireMasterService {

        /// <summary>
        /// データベースコンテキスト
        /// </summary>
        public ConvenienceContext _context { get; set; }

        // 自分自身をインターフェースとして参照
        private readonly IShiireMasterService my;

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
        public IMasterRegistrationViewModel<PostMasterData> MasterRegisiationViewModel { get; set; }

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
        public async Task<IList<ShiireMaster>> QueryMasterData() {
            KeepMasterDatas = await _context.ShiireMaster.OrderBy(x => x.ShiireSakiId)
                .ThenBy(x => x.ShiirePrdId)
                .ThenBy(x => x.ShohinId)
                .Include(x => x.ShiireSakiMaster) // ナビゲーションプロパティをロード
                .ToListAsync();
            return KeepMasterDatas;
        }

        /// <summary>
        /// ビューモデルを作成
        /// </summary>
        public async Task<ShiireMasterViewModel> MakeViewModel() {
            var viewmodel=(ShiireMasterViewModel)await my.DefaultMakeViewModel();
            await viewmodel.InitialAsync(); //リストボックスセット
            return (ShiireMasterViewModel)viewmodel;
        }

        /// <summary>
        /// ビューモデルを基にマスタデータを更新
        /// </summary>
        public async Task<ShiireMasterViewModel> UpdateMasterData(ShiireMasterViewModel argMasterRegistrationViewModel) {
            var viewmodel=(ShiireMasterViewModel)await my.DefaultUpdateMasterData((IMasterRegistrationViewModel<PostMasterData>)argMasterRegistrationViewModel);
            await viewmodel.InitialAsync(); //リストボックスセット
            return (ShiireMasterViewModel)viewmodel;
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
