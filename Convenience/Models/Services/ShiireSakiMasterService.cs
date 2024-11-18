using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.ViewModels.ShiireSakiMaster;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace Convenience.Models.Services {
    /// <summary>
    /// 仕入先マスタを管理するサービスクラス
    /// </summary>
    public class ShiireSakiMasterService : IShiireSakiMasterService {

        /// <summary>
        /// データベースコンテキスト
        /// </summary>
        public ConvenienceContext _context { get; set; }

        // 自身をインターフェース型として保持
        private readonly IShiireSakiMasterService my;

        /// <summary>
        /// 現在保持しているマスタデータ
        /// </summary>
        public IList<ShiireSakiMaster> KeepMasterDatas { get; set; }

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
        public ShiireSakiMasterService(ConvenienceContext context) {
            _context = context;
            KeepMasterDatas = new List<ShiireSakiMaster>();
            PostedMasterDatas = new List<PostMasterData>();
            MasterRegisiationViewModel = new ShiireSakiMasterViewModel(_context);
            my = this; // 自身をインターフェース型として格納
        }

        /// <summary>
        /// Postデータを保持データにマッピング
        /// </summary>
        /// <param name="argDatas">Postデータリスト</param>
        /// <returns>保持データリスト</returns>
        public IList<ShiireSakiMaster> MapFromPostDataToKeepMasterData(IList<PostMasterData> argDatas) {

            // AutoMapperの設定
            IMapper mapper = new MapperConfiguration(cfg => {
                cfg.AddCollectionMappers(); // コレクションのマッピングを有効化
                cfg.CreateMap<PostMasterData, ShiireSakiMaster>()
                .EqualityComparison((src, dest) => src.ShiireSakiId == dest.ShiireSakiId) // 主キーで比較
                .ForMember(dest => dest.ShireMasters, opt => opt.Ignore()) // ナビゲーションプロパティを無視
                .ForMember(dest => dest.ChumonJissekis, opt => opt.Ignore());
            }).CreateMapper();

            // 新規アイテムを追加
            var itemsToAdd = argDatas.Where(a =>
                !KeepMasterDatas.Any(cd => cd.ShiireSakiId == a.ShiireSakiId)).ToList();
            foreach (var item in itemsToAdd) {
                _context.Set<ShiireSakiMaster>().Add(mapper.Map<ShiireSakiMaster>(item));
            }

            // 不要なアイテムを削除
            var itemsToRemove = KeepMasterDatas.Where(cd =>
                !argDatas.Any(a => a.ShiireSakiId == cd.ShiireSakiId)).ToList();
            foreach (var item in itemsToRemove) {
                _context.Set<ShiireSakiMaster>().Remove(item);
            }

            // 保持データを更新
            mapper.Map(argDatas, KeepMasterDatas);

            return KeepMasterDatas;
        }

        /// <summary>
        /// 保持データをPostデータにマッピング
        /// </summary>
        /// <param name="argDatas">保持データリスト</param>
        /// <returns>Postデータリスト</returns>
        public IList<PostMasterData> MapFromKeepMasterDataToPostData(IList<ShiireSakiMaster> argDatas) {
            // AutoMapperの設定
            IMapper mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<ShiireSakiMaster, PostMasterData>()
                .EqualityComparison((src, dest) => src.ShiireSakiId == dest.ShiireSakiId) // 主キーで比較
                .ForMember(dest => dest.DeleteFlag, opt => opt.MapFrom(src => false)); // 削除フラグをfalseに設定
            }).CreateMapper();

            // マッピングを実行
            mapper.Map(argDatas, PostedMasterDatas);
            return PostedMasterDatas;
        }

        /// <summary>
        /// データベースから仕入先マスタデータを取得
        /// </summary>
        /// <returns>保持データリスト</returns>
        public async Task<IList<ShiireSakiMaster>> QueryMasterData() {
            KeepMasterDatas = await _context.ShiireSakiMaster
                .OrderBy(x => x.ShiireSakiId) // 仕入先IDで並び替え
                .ToListAsync();
            return KeepMasterDatas;
        }

        /// <summary>
        /// ビューモデルを作成
        /// </summary>
        /// <returns>ビューモデル</returns>
        public async Task<ShiireSakiMasterViewModel> MakeViewModel() {
            return (ShiireSakiMasterViewModel) await my.DefaultMakeViewModel();
        }

        /// <summary>
        /// ビューモデルを基にマスタデータを更新
        /// </summary>
        /// <param name="argMasterRegistrationViewModel">更新用ビューモデル</param>
        /// <returns>更新後のビューモデル</returns>
        public async Task<ShiireSakiMasterViewModel> UpdateMasterData(ShiireSakiMasterViewModel argMasterRegistrationViewModel) {
            return (ShiireSakiMasterViewModel)await my.DefaultUpdateMasterData((IMasterRegistrationViewModel<PostMasterData>)argMasterRegistrationViewModel);
        }

        /// <summary>
        /// Postデータに新しい行を挿入
        /// </summary>
        /// <param name="PostMasterDatas">Postデータリスト</param>
        /// <param name="index">挿入位置</param>
        /// <returns>更新後のPostデータリスト</returns>
        public IList<PostMasterData> InsertRow(IList<PostMasterData> PostMasterDatas, int index) {
            return my.DefaultInsertRow(PostMasterDatas, index);
        }

        /// <summary>
        /// Postデータ用の内部クラス
        /// </summary>
        public class PostMasterData : ShiireSakiMaster, IPostMasterData {
            /// <summary>
            /// 削除フラグ
            /// </summary>
            [DisplayName("削除")]
            public bool DeleteFlag { get; set; }
        }
    }
}
