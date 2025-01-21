using Convenience.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Convenience.Models.Interfaces {
    /// <summary>
    /// 選択リストアイテムのインターフェース
    /// </summary>
    public interface ISelectList : ISharedTools {
        public string Value { get; }
        public string Text { get; }
        public string[] OrderKey { get; }


    }
}