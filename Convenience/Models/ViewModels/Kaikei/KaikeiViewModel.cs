﻿using Convenience.Data;
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
        /// 店頭払出実績
        /// </summary>
        /// 
        [DisplayName("会計日時＋コード")]
        public string KaikeiDateAndId { get; set; }

        public KaikeiJissekiForAdd KaikeiJissekiforAdd { get; set; }
        public IList<SelectListItem> ShohinList { get; set; }

        public KaikeiHeader KaikeiHeader { get; set; }
        public bool? IsNormal { get; set; }
        public string? Remark { get; set; } = string.Empty;

        public IList<SelectListItem> KaikeiHeaderList { get; set; }

        public KaikeiViewModel(ConvenienceContext context) {
            ConvenienceContext _context = context;
            KaikeiJissekiforAdd = new KaikeiJissekiForAdd(_context);
            ShohinList = new List<SelectListItem>();
            KaikeiDateAndId = string.Empty;
            KaikeiHeader = new KaikeiHeader();
            KaikeiHeaderList = new List<SelectListItem>();
        }
        public KaikeiViewModel() {
            KaikeiJissekiforAdd = new KaikeiJissekiForAdd();
            ShohinList = new List<SelectListItem>();
            KaikeiDateAndId = string.Empty;
            KaikeiHeader = new KaikeiHeader();
            KaikeiHeaderList = new List<SelectListItem>();
        }
    }
    public class KaikeiJissekiForAdd : IKaikeiJissekiForAdd {

        [Column("shohin_code")]
        [DisplayName("商品コード")]
        public string? ShohinId { get; set; }
        public string? ShohinName { get; set; }
        public decimal UriageSu { get; set; }
        public decimal UriageKingaku { get; set; }
        public string NaigaiClass { get; set; }
        public ShohinMaster? ShohinMaster { get; set; }
        public IEnumerable<SelectListItem> NaigaiClassListItems { get; set; }

        private void Initial() {
            this.UriageSu = 0;
            this.UriageKingaku = 0;
        }
        public KaikeiJissekiForAdd() {
            this.NaigaiClassListItems = new List<SelectListItem>();
            this.NaigaiClass = "0";
            Initial();
        }
        public KaikeiJissekiForAdd(ConvenienceContext context) {
            ConvenienceContext _context = context;
            this.NaigaiClassListItems =
                _context.NaigaiClassMaster.AsNoTracking().OrderBy(x => x.NaigaiClass)
                .Select(x => new SelectListItem() { Text = $"{x.NaigaiClass}:{x.NaigaiClassName}", Value = x.NaigaiClass })
                .ToList();
            this.NaigaiClass = "0";
            Initial();
        }
    }



    

    /// <summary>
    /// 店頭払出キー入力に使うリストの内容
    /// </summary>
    public class UriageDateTimeAndIdMatching {

        public DateTime UriageDatetime { get; set; } = default;
        public string? UriageDatetimeId { get; set; } = default;

        public UriageDateTimeAndIdMatching(DateTime UriageDatetime, string UriageDatetimeId) {
            this.UriageDatetime = UriageDatetime;
            this.UriageDatetimeId = UriageDatetimeId;
        }
        public UriageDateTimeAndIdMatching() {
        }
    }
}