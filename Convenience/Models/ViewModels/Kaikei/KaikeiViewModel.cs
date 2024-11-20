using Convenience.Data;
using Convenience.Migrations;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Convenience.Models.ViewModels.Kaikei {

    /// <summary>
    /// 注文明細ビューモデル
    /// </summary>
    public class KaikeiViewModel {

        /// <summary>
        /// 会計日時＋コード
        /// </summary>
        [DisplayName("会計日時＋コード")]
        public string KaikeiDateAndId { get; set; }

        /// <summary>
        /// 会計１レコード追加入力用
        /// </summary>
        public KaikeiJissekiForAdd KaikeiJissekiforAdd { get; set; }
        /// <summary>
        /// 商品一覧
        /// </summary>
        public IList<SelectListItem> ShohinList { get; set; }

        /// <summary>
        /// 会計実績
        /// </summary>
        public KaikeiHeader KaikeiHeader { get; set; }

        /// <summary>
        /// DB反映処理結果
        /// </summary>
        public bool? IsNormal { get; set; }
        /// <summary>
        /// DB反映処理結果
        /// </summary>
        public string? Remark { get; set; } = string.Empty;

        /// <summary>
        /// 会計実績一覧表示用
        /// </summary>
        public IList<SelectListItem> KaikeiHeaderList { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <remarks>商品一覧もセットする</remarks>
        /// <param name="context">DBコンテキスト</param>
        public KaikeiViewModel(ConvenienceContext context) {
            ConvenienceContext _context = context;
            KaikeiJissekiforAdd = new KaikeiJissekiForAdd(_context);
            ShohinList = new List<SelectListItem>();
            KaikeiDateAndId = string.Empty;
            KaikeiHeader = new KaikeiHeader();
            KaikeiHeaderList = new List<SelectListItem>();
        }

        /// <summary>
        /// コンストラクタ（こちらは利用されていない）
        /// </summary>
        public KaikeiViewModel() {
            KaikeiJissekiforAdd = new KaikeiJissekiForAdd();
            ShohinList = new List<SelectListItem>();
            KaikeiDateAndId = string.Empty;
            KaikeiHeader = new KaikeiHeader();
            KaikeiHeaderList = new List<SelectListItem>();
        }
    }
    /// <summary>
    /// 会計するときに一商品づつ追加できるようにするためのモデル
    /// </summary>
    public class KaikeiJissekiForAdd : IKaikeiJissekiForAdd {

        /// <summary>
        /// 商品コード
        /// </summary>
        [Column("shohin_code")]
        [DisplayName("商品コード")]
        public string? ShohinId { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string? ShohinName { get; set; }
        /// <summary>
        /// 売り上げ数
        /// </summary>
        public decimal UriageSu { get; set; }
        /// <summary>
        /// 売上金額
        /// </summary>
        public decimal UriageKingaku { get; set; }
        /// <summary>
        /// 内外区分
        /// </summary>
        public string NaigaiClass { get; set; }
        /// <summary>
        /// 売上日時
        /// </summary>
        public DateTime UriageDatetime { get; set; }
        /// <summary>
        /// 商品マスタ
        /// </summary>
        public DataModels.ShohinMaster? ShohinMaster { get; set; }
        /// <summary>
        /// 内外区分セレクトリスト
        /// </summary>
        public IEnumerable<SelectListItem> NaigaiClassListItems { get; set; }

        /// <summary>
        /// 共通初期化（売上数・売上金額）
        /// </summary>
        private void Initial() {
            this.UriageSu = 0;
            this.UriageKingaku = 0;
            this.NaigaiClass = "0";
        }
        /// <summary>
        /// コンストラクタ（基本）
        /// </summary>
        public KaikeiJissekiForAdd() {
            this.NaigaiClassListItems = new List<SelectListItem>();
            Initial();
        }
        /// <summary>
        /// コンストラクタ（内外区分マスタ検索機能付き）
        /// </summary>
        /// <param name="context"></param>
        public KaikeiJissekiForAdd(ConvenienceContext context) {
            ConvenienceContext _context = context;
            this.NaigaiClassListItems =
                _context.NaigaiClassMaster.AsNoTracking().OrderBy(x => x.NaigaiClass)
                .Select(x => new SelectListItem() { Text = $"{x.NaigaiClass}:{x.NaigaiClassName}", Value = x.NaigaiClass })
                .ToList();
            Initial();
        }
    }
    

    /// <summary>
    /// 店頭払出キー入力に使うリストの内容
    /// </summary>
    public class UriageDateTimeAndIdMatching {

        /// <summary>
        /// 売上日時
        /// </summary>
        public DateTime UriageDatetime { get; set; } = default;
        /// <summary>
        /// 売上日時コード
        /// </summary>
        public string? UriageDatetimeId { get; set; } = default;

        /// <summary>
        /// コンストラクター（初期データセット）
        /// </summary>
        /// <param name="UriageDatetime"></param>
        /// <param name="UriageDatetimeId"></param>
        public UriageDateTimeAndIdMatching(DateTime UriageDatetime, string UriageDatetimeId) {
            this.UriageDatetime = UriageDatetime;
            this.UriageDatetimeId = UriageDatetimeId;
        }
        /// <summary>
        /// コンストラクター（基本）
        /// </summary>
        public UriageDateTimeAndIdMatching() {
        }
    }
}