using Convenience.Models.DataModels;
using Convenience.Models.ViewModels.Chumon;
using Convenience.Models.Properties;
using static Convenience.Models.Properties.Message;
using Convenience.Data;

namespace Convenience.Models.Interfaces {

    public interface IChumonService {
        public IChumon chumon { get; set; }

        //注文クラスのオブジェクト変数
        public ChumonViewModel ChumonSetting(string inShiireSakiId, DateOnly inChumonDate);

        //②注文作成が問い合わせかハンドリングする
        //inChumonDate（注文日付）に何も入っていない場合は、注文作成(ChumonSakusei)を呼ぶ
        //inChumonDate（注文日付）に何かはいっていたら、注文問い合わせ(ChumonToiasase)を呼ぶが、もし注文問い合わせの結果が０件であれば、注文作成(ChumonSakusei)をコールする

        public (ChumonJisseki, int, bool, ErrDef) ChumonCommit(ChumonJisseki inChumonJisseki);

        //①chumon.ChumonUpdate(inChumonJisseki)を実行する
        //②注文実績と注文実績明細をSaveChangeする
        //③戻り値のintは、更新されたエンティティを戻す
        //上記の戻し方
        //var entities = _context.ChangeTracker.Entries()
        //.Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
        //.Select(e => e.Entity).Count();
        //return(entities)

    }

}