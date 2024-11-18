using AutoMapper; // オブジェクトマッピングライブラリ
using AutoMapper.EquivalencyExpression; // AutoMapperのコレクション比較拡張
using Convenience.Data; // データベースコンテキスト
using Convenience.Models.DataModels; // データモデル
using Convenience.Models.Interfaces; // インターフェース
using Convenience.Models.ViewModels.ShohinMaster; // ビューモデル
using System.ComponentModel; // データ注釈属性

namespace Convenience.Models.Services {
    /// <summary>
    /// 商品マスタを管理するサービスクラス
    /// </summary>
    public class ShohinMasterService : IShohinMasterService {

        /// <summary>
        /// データベースコンテキスト
        /// </summary>
        public ConvenienceContext _context { get; set; }

        // 自身をインターフェース型として保持
        private readonly IShohinMasterService my;

        /// <summary>
        /// 現在保持しているマスタデータ
        /// </summary>
        public IList<ShohinMaster> KeepMasterDatas { get; set; }

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
        public ShohinMasterService(ConvenienceContext context) {
            _context = context;
            KeepMasterDatas = new List<ShohinMaster>(); // 初期化
            PostedMasterDatas = new List<PostMasterData>(); // 初期化
            MasterRegisiationViewModel = new ShohinMasterViewModel(); // ビューモデルの初期化
            my = this; // 自身をインターフェース型として格納
        }

        /// <summary>
        /// Postデータを保持データにマッピング
        /// </summary>
        /// <param name="argDatas">Postデータリスト</param>
        /// <returns>保持データリスト</returns>
        public IList<ShohinMaster> MapFromPostDataToKeepMasterData(IList<PostMasterData> argDatas) {

            // AutoMapperの設定
            IMapper mapper = new MapperConfiguration(cfg => {
                cfg.AddCollectionMappers(); // コレクションのマッピングを有効化
                cfg.CreateMap<PostMasterData, ShohinMaster>()
                .EqualityComparison((src, dest) => src.ShohinId == dest.ShohinId) // 主キーで比較
                .ForMember(dest => dest.ShiireMasters, opt => opt.Ignore()) // ナビゲーションプロパティを無視
                .ForMember(dest => dest.TentoZaiko, opt => opt.Ignore()); // 店舗在庫も無視
            }).CreateMapper();

            // 新規アイテムを追加
            var itemsToAdd = argDatas.Where(a =>
                !KeepMasterDatas.Any(cd => cd.ShohinId == a.ShohinId)).ToList();
            foreach (var item in itemsToAdd) {
                _context.Set<ShohinMaster>().Add(mapper.Map<ShohinMaster>(item));
            }

            // 不要なアイテムを削除
            var itemsToRemove = KeepMasterDatas.Where(cd =>
                !argDatas.Any(a => a.ShohinId == cd.ShohinId)).ToList();
            foreach (var item in itemsToRemove) {
                _context.Set<ShohinMaster>().Remove(item);
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
        public IList<PostMasterData> MapFromKeepMasterDataToPostData(IList<ShohinMaster> argDatas) {
            // AutoMapperの設定
            IMapper mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<ShohinMaster, PostMasterData>()
                .EqualityComparison((src, dest) => src.ShohinId == dest.ShohinId) // 主キーで比較
                .ForMember(dest => dest.DeleteFlag, opt => opt.MapFrom(src => false)); // 削除フラグをfalseに設定
            }).CreateMapper();

            // マッピングを実行
            mapper.Map(argDatas, PostedMasterDatas);
            return PostedMasterDatas;
        }

        /// <summary>
        /// データベースから商品マスタデータを取得
        /// </summary>
        /// <returns>保持データリスト</returns>
        public IList<ShohinMaster> QueryMasterData() {
            KeepMasterDatas = _context.ShohinMaster
                .OrderBy(x => x.ShohinId) // 商品IDで並び替え
                .ToList();
            return KeepMasterDatas;
        }

        /// <summary>
        /// ビューモデルを作成
        /// </summary>
        /// <returns>ビューモデル</returns>
        public ShohinMasterViewModel MakeViewModel() {
            var viewModel = this.MasterRegisiationViewModel;
            return (ShohinMasterViewModel)my.DefaultMakeViewModel();
        }

        /// <summary>
        /// ビューモデルを基にマスタデータを更新
        /// </summary>
        /// <param name="argMasterRegistrationViewModel">更新用ビューモデル</param>
        /// <returns>更新後のビューモデル</returns>
        public ShohinMasterViewModel UpdateMasterData(ShohinMasterViewModel argMasterRegistrationViewModel) {
            return (ShohinMasterViewModel)my.DefaultUpdateMasterData((IMasterRegistrationViewModel<PostMasterData>)argMasterRegistrationViewModel);
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
        public class PostMasterData : ShohinMaster, IPostMasterData {
            /// <summary>
            /// 削除フラグ
            /// </summary>
            [DisplayName("削除")]
            public bool DeleteFlag { get; set; }
        }
    }
}
