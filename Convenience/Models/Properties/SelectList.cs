using Convenience.Data;
using Convenience.Models.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq.Expressions;

namespace Convenience.Models.Properties {
    public class SelectList : ISharedTools {

        private readonly ConvenienceContext _context;

        public SelectList(ConvenienceContext context) {
            _context = context;
        }

        /// <summary>
        /// リストデータを取得する
        /// </summary>
        /// <typeparam name="T1">取り出すエンティティ</typeparam>
        /// <typeparam name="T2">OrdebByから戻る値</typeparam>
        /// <param name="orderExpression">OrderByのラムダ式</param>
        /// <returns>処理されたエンティティ</returns>
        public IQueryable<T1> GenerateList<T1, T2>(Expression<Func<T1, T2>> orderExpression) where T1 : class, ISelectList {
            return ISharedTools.IsExistCheck(orderExpression) ? _context.Set<T1>().OrderBy(orderExpression) : _context.Set<T1>();
        }

    }
}
