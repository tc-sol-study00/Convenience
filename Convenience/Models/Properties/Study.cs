using Convenience.Data;
using Convenience.Models.DataModels;
using Microsoft.EntityFrameworkCore;

namespace Convenience.Models.Properties {
    public class Study {

        private readonly ConvenienceContext _context;        //更新されて良いの？

        public Study(ConvenienceContext context) {
            this._context = context;
        }

        //Aさん作成メソッド
        public ChumonJissekiMeisai? Method1(ChumonJissekiMeisai argDataModels) {

            _ = argDataModels?.ChumonId ?? argDataModels?.ShiireSakiId ?? argDataModels?.ShiirePrdId 
                ?? argDataModels?.ShohinId ?? throw new Exception("引数エラー");

            ChumonJissekiMeisai? queriedChumonJissekiMeisai = _context.ChumonJissekiMeisai.AsNoTracking()
                .Where(cjm => cjm.ChumonId == argDataModels.ChumonId &&             //NULLを想定していない
                              cjm.ShiireSakiId == argDataModels.ShiireSakiId &&
                              cjm.ShiirePrdId == argDataModels.ShiirePrdId &&
                              cjm.ShohinId == argDataModels.ShohinId
                 )
                .Include(cjm => cjm.ShiireMaster)
                .ThenInclude(sm => (sm!.ShohinMaster))                                 //NULLになるけど・・・
                .FirstOrDefault();

            ChumonJissekiMeisai? result;
            if (queriedChumonJissekiMeisai is not null) {
                queriedChumonJissekiMeisai.ChumonSu = argDataModels.ChumonSu;
                ChumonJissekiMeisai updatedChumonJissekiMeisai = queriedChumonJissekiMeisai;
                result = updatedChumonJissekiMeisai;
            }
            else {
                result = null;
            }

            //result = updatedChumonJisseki ?? new ChumonJissekiMeisai();           //警告



            List<ChumonJissekiMeisai> chumonJissekiMeisais = _context.ChumonJissekiMeisai.AsNoTracking()
                .Where(cjm => cjm.ChumonId == argDataModels.ChumonId &&
                              cjm.ShiireSakiId == argDataModels.ShiireSakiId &&
                              cjm.ShiirePrdId == argDataModels.ShiirePrdId &&
                              cjm.ShohinId == argDataModels.ShohinId
                 )
                .Include(cjm => cjm.ShiireMaster)
                .ThenInclude(sm => sm!.ShohinMaster)
                .ToList();

            if (chumonJissekiMeisais.Count > 0) {
                result = chumonJissekiMeisais[0];   //警告がでない
            }
            return result;
        }

        //Bさん作成メソッド
        public decimal? MainMethod() {
            ChumonJissekiMeisai postedChumonJissekiMeisai = new ();

            ChumonJissekiMeisai? updatedChumonJissekiMeisai = Method1(postedChumonJissekiMeisai); //DataModel内のメンバーにはNULLを渡している→Abend
            if (updatedChumonJissekiMeisai is not null) {
                var res = updatedChumonJissekiMeisai.ChumonZan;  //Bさんは、aにはなにかデータが入ると思っていて、aにはnullが入っているとAbendする
                return res;
            }
            else {
                return null;
            }
        }
    }
    public class DataModel {
        public string ChumonId { get; set; }        = string.Empty;
        public string ShiireSakiId { get; set; }    = string.Empty;
        public string ShiirePrdId { get; set; }     = string.Empty;
        public string ShohinId { get; set; }        = string.Empty;

        public decimal ChumonSu { get; set; }           = default;
    }
}
