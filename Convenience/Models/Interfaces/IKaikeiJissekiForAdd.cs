using Convenience.Models.DataModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Convenience.Models.Interfaces {

    /// <summary>
    /// <para>画面上の会計実績追加用のデータ構造インターフェース化</para>
    /// <para>このインターフェースは、画面からの追加や、消費税に関する内外区分による消費税計算で使われる</para>
    /// <para>実装先：KaikeiJissekiForAdd、KaikeiJisseki(DTO)　</para>
    /// </summary>
    public interface IKaikeiJissekiForAdd {
        public string? ShohinId { get; set; }
        public decimal UriageSu { get; set; }
        public decimal UriageKingaku { get; set; }
        public string NaigaiClass { get; set; }
        public ShohinMaster? ShohinMaster { get; set; }
    }
}
