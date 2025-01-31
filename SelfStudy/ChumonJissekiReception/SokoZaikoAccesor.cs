using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using SelfStudy.ChumonJissekiReception.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfStudy.ChumonJissekiReception {
    /// <summary>
    /// 倉庫在庫サクセサ
    /// </summary>
    public class SokoZaikoAccesor : ISokoZaikoAccesor {

        private readonly ConvenienceContext _context;
        public SokoZaiko? SokoZaiko { get; set; }
        public SokoZaikoAccesor(ConvenienceContext context) {
            _context = context;
        }

        public SokoZaikoAccesor() : this(IDbContext.DbOpen()) {
        }
        /// <summary>
        /// 倉庫在庫取得
        /// </summary>
        /// <param name="inShiireSakiId">仕入先コード</param>
        /// <param name="inShiirePrdId">仕入商品コード</param>
        /// <param name="inShohinId">商品コード</param>
        /// <returns>倉庫在庫（既存）</returns>
        public SokoZaiko? GetSokoZaiko(string inShiireSakiId, string inShiirePrdId, string inShohinId) {
            this.SokoZaiko = _context.SokoZaiko
                .Where( sz => sz.ShiireSakiId == inShiireSakiId && 
                        sz.ShiirePrdId == inShiirePrdId && 
                        sz.ShohinId == inShohinId )
                .FirstOrDefault();
            return this.SokoZaiko;
        }

        /// <summary>
        /// 倉庫在庫作成
        /// </summary>
        /// <param name="inShiireJisseki">仕入実績</param>
        /// <returns>倉庫在庫（新規）</returns>
        public SokoZaiko CreateSokoZaiko(string inShiireSakiId, string inShiirePrdId, string inShohinId) {
            this.SokoZaiko = new SokoZaiko() {
                ShiireSakiId = inShiireSakiId,
                ShiirePrdId = inShiirePrdId,
                ShohinId = inShohinId,
                SokoZaikoCaseSu = 0,
                SokoZaikoSu = 0,
                LastShiireDate = DateOnly.FromDateTime(DateTime.Today),
            };

            //DBに追加準備
            _context.Add(this.SokoZaiko);

            return this.SokoZaiko;
        }

    }
}
