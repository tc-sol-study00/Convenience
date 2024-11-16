using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Properties;
using Convenience.Models.ViewModels.ShiireMaster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using static Convenience.Models.Properties.Message;
using System.Linq.Dynamic.Core;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Convenience.Models.Interfaces {
    public interface IMasterRegistrationService<TKeepMasterData, TPostMasterData, TMasterRegistrationViewModel> : ISharedTools {

        public ConvenienceContext _context { get; set; }

        /// <summary>
        /// 更新先マスタデータ
        /// </summary>
        public IList<TKeepMasterData> KeepMasterDatas { get; set; }

        /// <summary>
        /// Postされるマスタデータ
        /// </summary>
        public IList<TPostMasterData> PostedMasterDatas { get; set; }

        /// <summary>
        /// ビューモデル
        /// </summary>
        public IMasterRegistrationViewModel MasterRegisiationViewModel { get; set; }

        /// <summary>
        /// マスタデータ問い合わせ
        /// </summary>
        /// <returns></returns>
        public IList<TKeepMasterData> QueryMasterData();

        public TMasterRegistrationViewModel MakeViewModel();

        public TMasterRegistrationViewModel UpdateMasterData(TMasterRegistrationViewModel arg3);

        public IList<TPostMasterData> InsertRow(IList<TPostMasterData> PostMasterDatas, int index);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="indatas"></param>
        /// <returns></returns>
        public IMasterRegistrationViewModel DefaultMakeViewModel() {
            // マスタデータ問い合わせ
            KeepMasterDatas = QueryMasterData();

            //表示用データ(Post用データ）セット
            PostedMasterDatas = MapFromKeepMasterDataToPostData(KeepMasterDatas);

            //ビューモデルセット
            //MasterRegisiationViewModel = argMasterRegistrationViewModel;
            MasterRegisiationViewModel.PostMasterDatas = PostedMasterDatas;

            return MasterRegisiationViewModel;
        }

        public IMasterRegistrationViewModel DefaultUpdateMasterData(IMasterRegistrationViewModel argMasterRegistrationViewModel) {
            // マスタデータ問い合わせ
            KeepMasterDatas = QueryMasterData();

            //削除データチェック
            IList<TPostMasterData> postMasterDatas = argMasterRegistrationViewModel.PostMasterDatas;

            IList<TPostMasterData> remainPostMasterData = DeleteData<TPostMasterData>(postMasterDatas.Cast<IPostMasterData>().ToList());

            KeepMasterDatas = MapFromPostDataToKeepMasterData(remainPostMasterData);

            var entities = _context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
            .Select(e => e.Entity).Count();

            _context.SaveChanges();

            ErrDef errCd = ErrDef.DataValid;
            bool IsValid = true;

            // マスタデータ問い合わせ
            KeepMasterDatas = QueryMasterData();

            //表示用データ(Post用データ）セット
            PostedMasterDatas = MapFromKeepMasterDataToPostData(KeepMasterDatas);

            //ビューモデルセット
            //MasterRegisiationViewModel = argMasterRegistrationViewModel;
            MasterRegisiationViewModel.PostMasterDatas = PostedMasterDatas;
            MasterRegisiationViewModel.IsNormal = IsValid;
            MasterRegisiationViewModel.Remark = errCd == ErrDef.DataValid && entities > 0 || errCd != ErrDef.DataValid
                    ? new Message().SetMessage(ErrDef.NormalUpdate)?.MessageText
                    : null;

            return MasterRegisiationViewModel;

        }

        private IList<T> DeleteData<T>(IEnumerable<IPostMasterData> indatas) {
            var remainPostMasterData = indatas.Cast<IPostMasterData>()
                .Where(x => !x.DeleteFlag)
                .OfType<T>()
                .ToList();

            return remainPostMasterData;
        }

        public IList<TPostMasterData> DefaultInsertRow(IList<TPostMasterData> PostMasterDatas, int index) {
            PostMasterDatas.Insert(index + 1, PostMasterDatas[index]);
            return PostMasterDatas;
        }


        public IList<TKeepMasterData> MapFromPostDataToKeepMasterData(IList<TPostMasterData> argDatas);

        public IList<TPostMasterData> MapFromKeepMasterDataToPostData(IList<TKeepMasterData> argDatas);

        public interface IPostMasterData : IDeleteFlag {

        }
        public interface IDeleteFlag {
            public bool DeleteFlag { get; set; }
        }
        public interface IMasterRegistrationViewModel {
            public IList<TPostMasterData> PostMasterDatas { get; set; }

            /// <summary>
            /// 処理が正常がどうか（正常=true)
            /// </summary>
            public bool? IsNormal { get; set; }
            /// <summary>
            /// 処理結果（ＤＢ反映結果）表示内容
            /// </summary>
            public string? Remark { get; set; }
        }

        public interface IMasterRegistrationSelectList {
            public ConvenienceContext _context { get; set; }

            public IList<SelectListItem> SetSelectList<T>() where T : class,ISelectList, new() {

                T attr = new T();

                IQueryable<T> query = _context.Set<T>();

                string orderByName=attr.OrderKey.First();
                query = query.OrderBy(orderByName);

                foreach (string orderby in attr.OrderKey.Skip(1)) {
                        ((IOrderedQueryable<T>)query).ThenBy(orderby);
                }

                IQueryable<SelectListItem> result = query.Select(x => new SelectListItem {
                    Value = x.Value,
                    Text = $"{x.Value}:{x.Text}"
                });

                return result.ToList();
            }

        }

    }
    public interface ISelectList {
        public string Value { get; }
        public string Text { get; }
        public string[] OrderKey { get; } 
    }
}
