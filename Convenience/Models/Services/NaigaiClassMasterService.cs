using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.ViewModels.NaigaiClassMaster;
using System.ComponentModel;

namespace Convenience.Models.Services {
    /// <summary>
    /// 内外区分マスタのサービスクラス
    /// </summary>
    public class NaigaiClassMasterService : INaigaiClassMasterService {

        /// <summary>
        /// データベースコンテキスト
        /// </summary>
        public ConvenienceContext _context { get; set; }

        // 自分自身をインターフェースとして参照
        private readonly INaigaiClassMasterService my;

        /// <summary>
        /// 現在保持しているマスタデータ
        /// </summary>
        public IList<NaigaiClassMaster> KeepMasterDatas { get; set; }

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
        public NaigaiClassMasterService(ConvenienceContext context) {
            _context = context;
            KeepMasterDatas = new List<NaigaiClassMaster>();
            PostedMasterDatas = new List<PostMasterData>();
            MasterRegisiationViewModel = new NaigaiClassMasterViewModel(_context);
            my = this; // 自身をインターフェース型として格納
        }

        /// <summary>
        /// Postデータを保持データにマッピング
        /// </summary>
        /// <param name="argDatas">Postデータリスト</param>
        /// <returns>保持データリスト</returns>
        public IList<NaigaiClassMaster> MapFromPostDataToKeepMasterData(IList<PostMasterData> argDatas) {
            // AutoMapperの設定
            IMapper mapper = new MapperConfiguration(cfg => {
                cfg.AddCollectionMappers(); // コレクションのマッピングを有効化
                cfg.CreateMap<PostMasterData, NaigaiClassMaster>()
                .EqualityComparison((src, dest) => src.NaigaiClass == dest.NaigaiClass) // 主キーで比較
                .ForMember(dest => dest.KaikeiJissekis, opt => opt.Ignore()); // 無関係のプロパティを無視
            }).CreateMapper();

            // 新規アイテムの追加
            var itemsToAdd = argDatas.Where(a => !KeepMasterDatas.Any(cd => cd.NaigaiClass == a.NaigaiClass)).ToList();
            foreach (var item in itemsToAdd) {
                _context.Set<NaigaiClassMaster>().Add(mapper.Map<NaigaiClassMaster>(item));
            }

            // 不要アイテムの削除
            var itemsToRemove = KeepMasterDatas.Where(cd => !argDatas.Any(a => a.NaigaiClass == cd.NaigaiClass)).ToList();
            foreach (var item in itemsToRemove) {
                _context.Set<NaigaiClassMaster>().Remove(item);
            }

            // 保持データを更新
            mapper.Map(argDatas, KeepMasterDatas);

            return KeepMasterDatas;
        }

        /// <summary>
        /// 保持データをPostデータにマッピング
        /// </summary>
        public IList<PostMasterData> MapFromKeepMasterDataToPostData(IList<NaigaiClassMaster> argDatas) {
            IMapper mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<NaigaiClassMaster, PostMasterData>()
                .EqualityComparison((src, dest) => src.NaigaiClass == dest.NaigaiClass) // 主キーで比較
                .ForMember(dest => dest.DeleteFlag, opt => opt.MapFrom(src => false)); // 削除フラグをfalseに設定
            }).CreateMapper();

            // マッピングを実行
            mapper.Map(argDatas, PostedMasterDatas);
            return PostedMasterDatas;
        }

        /// <summary>
        /// データベースからマスタデータを取得
        /// </summary>
        public IList<NaigaiClassMaster> QueryMasterData() {
            KeepMasterDatas = _context.NaigaiClassMaster.OrderBy(x => x.NaigaiClass).ToList();
            return KeepMasterDatas;
        }

        /// <summary>
        /// ビューモデルを作成
        /// </summary>
        public NaigaiClassMasterViewModel MakeViewModel() {
            return (NaigaiClassMasterViewModel)my.DefaultMakeViewModel();
        }

        /// <summary>
        /// ビューモデルを基にマスタデータを更新
        /// </summary>
        public NaigaiClassMasterViewModel UpdateMasterData(NaigaiClassMasterViewModel argMasterRegistrationViewModel) {
            return (NaigaiClassMasterViewModel)my.DefaultUpdateMasterData((IMasterRegistrationViewModel<PostMasterData>)argMasterRegistrationViewModel);
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
        public class PostMasterData : NaigaiClassMaster, IPostMasterData {

            /// <summary>
            /// 削除フラグ
            /// </summary>
            [DisplayName("削除")]
            public bool DeleteFlag { get; set; }
        }
    }
}
