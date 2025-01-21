using Convenience.Data;
using Convenience.Models.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq.Expressions;

namespace Convenience.Models.Properties {
    public class SelectList : ISelectList, ISharedTools {

        private readonly ConvenienceContext _context;

        public string Value { get; }
        public string Text { get; }
        public string[] OrderKey { get; }

        public SelectList(ConvenienceContext context) {
            _context = context;
        }
        public IQueryable<T1> GenerateList<T1, T2>(Expression<Func<T1, T2>> orderExpression) where T1 : class, ISelectList {
            return ISharedTools.IsExistCheck(orderExpression) ? _context.Set<T1>().OrderBy(orderExpression) : _context.Set<T1>();
        }

    }
}
