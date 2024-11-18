using Convenience.Data;
using Convenience.Models.Properties;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static Convenience.Models.Properties.Message;
using System.Linq.Dynamic.Core;

namespace Convenience.Models.Interfaces {
    /// <summary>
    /// マスター登録サービスインターフェース
    /// </summary>
    /// <typeparam name="TKeepMasterData">更新されるマスタデータの型</typeparam>
    /// <typeparam name="TPostMasterData">Postされるデータの型</typeparam>
    /// <typeparam name="TMasterRegistrationViewModel"> 登録用ビューモデルの型</typeparam>
    public abstract class MasterRegistrationServiceBase<TKeepMasterData, TPostMasterData, TMasterRegistrationViewModel> : ISharedTools {

        /// <summary>
        /// データベースコンテキスト
        /// </summary>
        public ConvenienceContext _context { get; set; }

        /// <summary>
        /// 更新されるマスタデータ
        /// </summary>
        public IList<TKeepMasterData> KeepMasterDatas { get; set; }

        /// <summary>
        /// Postされるデータ
        /// </summary>
        public IList<TPostMasterData> PostedMasterDatas { get; set; }

        /// <summary>
        /// 登録用ビューモデル
        /// </summary>
        public IMasterRegistrationViewModel<TPostMasterData> MasterRegisiationViewModel { get; set; }

        /// <summary>
        /// データベースからマスタデータを問い合わせる抽象メソッド
        /// </summary>
        /// <returns>取得されたマスタデータ</returns>
        public abstract IList<TKeepMasterData> QueryMasterData();

        /// <summary>
        /// ビューモデルを作成
        /// </summary>
        /// <returns>生成されたビューモデル</returns>
        public abstract TMasterRegistrationViewModel MakeViewModel();

        /// <summary>
        /// ビューモデルを基にマスタデータを更新
        /// </summary>
        /// <param name="arg3">更新対象のビューモデル</param>
        /// <returns>更新されたビューモデル</returns>
        public abstract TMasterRegistrationViewModel UpdateMasterData(TMasterRegistrationViewModel arg3);

        /// <summary>
        /// 新しい行を挿入
        /// </summary>
        /// <param name="PostMasterDatas">現在の投稿データリスト</param>
        /// <param name="index">挿入位置のインデックス</param>
        /// <returns>更新後のデータリスト</returns>
        public abstract IList<TPostMasterData> InsertRow(IList<TPostMasterData> PostMasterDatas, int index);

        /// <summary>
        /// デフォルトのビューモデル作成処理
        /// </summary>
        /// <returns>生成されたビューモデル</returns>
        public IMasterRegistrationViewModel<TPostMasterData> DefaultMakeViewModel() {
            // データベースからマスタデータを取得
            KeepMasterDatas = QueryMasterData();

            // マスタデータをPost用データに変換
            PostedMasterDatas = MapFromKeepMasterDataToPostData(KeepMasterDatas);

            // ビューモデルにセット
            MasterRegisiationViewModel.PostMasterDatas = PostedMasterDatas;

            return MasterRegisiationViewModel;
        }

        /// <summary>
        /// デフォルトの更新処理
        /// </summary>
        public IMasterRegistrationViewModel<TPostMasterData> DefaultUpdateMasterData(IMasterRegistrationViewModel<TPostMasterData> argMasterRegistrationViewModel) {
            // データベースから再度マスタデータを取得
            KeepMasterDatas = QueryMasterData();

            // 削除フラグのチェックと除外処理
            IList<TPostMasterData> postMasterDatas = argMasterRegistrationViewModel.PostMasterDatas;
            IList<TPostMasterData> remainPostMasterData = DeleteData<TPostMasterData>(postMasterDatas.Cast<IPostMasterData>().ToList());

            // 残ったデータを保持データに変換
            KeepMasterDatas = MapFromPostDataToKeepMasterData(remainPostMasterData);

            // 変更の保存
            var entities = _context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
                .Select(e => e.Entity).Count();
            _context.SaveChanges();

            // 結果の設定
            ErrDef errCd = ErrDef.DataValid;
            bool IsValid = true;

            // 最新のデータを取得
            KeepMasterDatas = QueryMasterData();
            PostedMasterDatas = MapFromKeepMasterDataToPostData(KeepMasterDatas);

            // ビューモデルを更新
            MasterRegisiationViewModel.PostMasterDatas = PostedMasterDatas;
            MasterRegisiationViewModel.IsNormal = IsValid;
            MasterRegisiationViewModel.Remark = (errCd == ErrDef.DataValid && entities > 0 || errCd != ErrDef.DataValid)
                ? new Message().SetMessage(ErrDef.NormalUpdate)?.MessageText
                : null;

            return MasterRegisiationViewModel;
        }

        /// <summary>
        /// 削除フラグが立っていないデータをフィルタリング
        /// </summary>
        private IList<T> DeleteData<T>(IEnumerable<IPostMasterData> indatas) {
            return indatas.Cast<IPostMasterData>()
                .Where(x => !x.DeleteFlag)
                .OfType<T>()
                .ToList();
        }

        /// <summary>
        /// 新しい行を挿入するデフォルトの処理
        /// </summary>
        public IList<TPostMasterData> DefaultInsertRow(IList<TPostMasterData> PostMasterDatas, int index) {
            PostMasterDatas.Insert(index + 1, PostMasterDatas[index]);
            return PostMasterDatas;
        }

        /// <summary>
        /// Postデータから更新用データモデルへの変換（実装強制用）
        /// </summary>
        public abstract IList<TKeepMasterData> MapFromPostDataToKeepMasterData(IList<TPostMasterData> argDatas);

        /// <summary>
        /// 更新用データモデルからPostデータへの変換（実装強制用）
        /// </summary>
        public abstract IList<TPostMasterData> MapFromKeepMasterDataToPostData(IList<TKeepMasterData> argDatas);




        /// <summary>
        /// 選択リスト作成のためのインターフェース
        /// </summary>
        public abstract class IMasterRegistrationSelectList {
            public ConvenienceContext _context { get; }

            /// <summary>
            /// 任意の型Tの選択リストを作成
            /// </summary>
            public IList<SelectListItem> SetSelectList<T>() where T : class, ISelectList, new() {
                T attr = new T();
                IQueryable<T> query = _context.Set<T>();

                // ソートキーで並び替え
                string orderByName = attr.OrderKey.First();
                query = query.OrderBy(orderByName);

                foreach (string orderby in attr.OrderKey.Skip(1)) {
                    ((IOrderedQueryable<T>)query).ThenBy(orderby);
                }

                // 選択リスト作成
                IQueryable<SelectListItem> result = query.Select(x => new SelectListItem {
                    Value = x.Value,
                    Text = $"{x.Value}:{x.Text}"
                });

                return result.ToList();
            }
        }
    }
}