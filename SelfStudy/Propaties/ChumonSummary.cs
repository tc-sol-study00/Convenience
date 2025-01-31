using Convenience.Data;
using Convenience.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

using _t = Convenience.Models.Interfaces.ISharedTools;

namespace SelfStudy.Propaties {
    public class ChumonSummary : IDisposable, ISharedTools {

        private readonly ConvenienceContext _context;
        

        private bool _disposed = false;
        private bool _myselfcontext = false;

        public ChumonSummary() : this(IDbContext.DbOpen()) {
            _myselfcontext = true;
        }
        public ChumonSummary(ConvenienceContext context) {
            _context = context;
        }
        ~ChumonSummary() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if ((!_disposed) && _myselfcontext) {
                if (disposing) {
                    //マネージドリソース解放を書く
                    _context?.Dispose();
                }
                //アンマネージドリソース解放を書く

                //
                _disposed = true;
            }
        }

        public class SummarizedListItem : IYYYYMM {
            public string ShiireSakiId { get; set; }
            public string? ShiireSakiKaisya { get; set; }
            public int Year { get; set; }
            public int Month { get; set; }
            public decimal ChumonSu { get; set; }
            public decimal ChumonZan { get; set; }
        }

        public interface IYYYYMM {
            public int Year { get; set; }
            public int Month { get; set; }
            public string YYYYMM => new DateOnly(Year, Month, 1).ToString("yyyyMM");
        }

        public IEnumerable<SummarizedListItem> ChumonSummaryList() {
            return _context.ChumonJissekiMeisai
                .AsNoTracking()
                .Include(x => x.ChumonJisseki)
                .ThenInclude(x => x.ShiireSakiMaster)
                .GroupBy(x => new { x.ShiireSakiId, x.ChumonJisseki!.ChumonDate.Year, x.ChumonJisseki.ChumonDate.Month })
                .Select(x => new SummarizedListItem { 
                    ShiireSakiId = x.Key.ShiireSakiId,
                    ShiireSakiKaisya=x.Max(x => x.ChumonJisseki!.ShiireSakiMaster!.ShiireSakiKaisya??string.Empty),
                    Year = x.Key.Year, 
                    Month = x.Key.Month, 
                    ChumonSu = x.Sum(x => x.ChumonSu), 
                    ChumonZan = x.Sum(x => x.ChumonZan) 
                })
                ;
        }

        static Action<string> wLine= strdata => Console.WriteLine(strdata);

        public void MakeChumonSummary() {
            var header=$"{_t.PadString("仕入先",-10)}:{_t.PadString("仕入先会社", -14)}:{_t.PadString("年月",-6)}:{_t.PadString("注文数",8)}:{_t.PadString("注文残",8)}";
            var boader=$"{_t.PadString("", -10,'-')}:{_t.PadString("", -14,'-')}:{_t.PadString("", -6,'-')}:{_t.PadString("", 8,'-')}:{_t.PadString("", 8,'-')}";

            bool firstFlg = true;
            foreach (var aChumonList in ChumonSummaryList().ToList()) {
                if (firstFlg) {
                    wLine("\n"+boader+"\n"+header+"\n"+boader);
                    firstFlg = false;
                }

                IYYYYMM aDataWIthYYYYMM = aChumonList as IYYYYMM;

                var outdata = $"{_t.PadString(aChumonList.ShiireSakiId,-10)}:{_t.PadString(aChumonList.ShiireSakiKaisya,-14)}:{_t.PadString(aDataWIthYYYYMM.YYYYMM,-6)}:{aChumonList.ChumonSu,8}:{aChumonList.ChumonZan,8}";
                wLine(outdata);
            }
            wLine(boader);
        }
    }
}
