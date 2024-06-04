using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Properties;
using Convenience.Models.ViewModels.Shiire;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static Convenience.Models.Properties.Shiire;

namespace Convenience.Models.Services {

    public class ShiireService {
        private readonly ConvenienceContext _context;

        private Shiire shiire;

        public ShiireService(ConvenienceContext context) {
            _context = context;
            ShiireClassCreate();
        }

        public ShiireService() {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var contextOptions = new DbContextOptionsBuilder<ConvenienceContext>()
                .UseNpgsql(configuration["ConvenienceContext"])
                .Options;

            _context = new ConvenienceContext(contextOptions);

            ShiireClassCreate();
        }

        private void ShiireClassCreate() {
            shiire = new Shiire(_context);
        }

        public async Task<(int, IList<ShiireJisseki>)> ShiireHandling(string inChumonId) {
            DateOnly inShiireDate = DateOnly.FromDateTime(DateTime.Now);
            //仕入SEQ
            uint inSeqByShiireDate = await shiire.NextSeq(inChumonId, inShiireDate);
            IList<ShiireJisseki> shiireJissekis;
            //新規の場合
            shiireJissekis = await shiire.ChumonToShiireJisseki(inChumonId, inShiireDate, inSeqByShiireDate);

            //shiireJissekiのSokoZaikoに、実際の倉庫在庫を接続（表示用）
            shiire.ShiireSokoConnection(shiireJissekis, _context.SokoZaiko);

            return (0, shiire.Shiirejissekis);
        }

        public async Task<(int, IList<ShiireJisseki>)> ShiireHandling(string inChumonId, DateOnly inShiireDate, uint inSeqByShiireDate, IList<ShiireJisseki> inShiireJissekis) {
            IList<ShiireJisseki> shiireJissekis;

            //既に仕入実績にあるか？
            if (await shiire.ChuumonIdOnShiireJissekiExistingCheck(inChumonId, inShiireDate, inSeqByShiireDate)) {
                shiireJissekis = await shiire.ShiireToShiireJisseki(inChumonId, inShiireDate, inSeqByShiireDate);  //ある場合は仕入実績から仕入実績を作る
            }
            else {
                shiireJissekis = await shiire.ChumonToShiireJisseki(inChumonId, inShiireDate, inSeqByShiireDate);  //ない場合は注文実績から仕入実績を作る
            }
            shiireJissekis = shiireJissekis;

            //引数の注文実績の内容でプロパティを更新する(postデータ取り込み)
            shiireJissekis = shiire.ShiireUpdate(inShiireJissekis);

            bool loopFlg = true;
            ShiireUkeireReturnSet shiirezaikoset=null;
            uint loopCount = 0;
            int entities = 0;

            while (loopFlg) {

                //プロパティの内容から、上記で反映した内容で、注文実績の注文残と倉庫残を調整する
                //在庫の登録はここで行われる（１）
                shiirezaikoset = await shiire.ChuumonZanZaikoSuChousei(inChumonId, shiireJissekis);

                try {
                    //ここにＤＢ保管処理を入れる
                    entities = await ShiireUpdate();
                    loopFlg = false;
                }
                //排他制御エラーの場合
                catch (DbUpdateConcurrencyException ex) {
                    if (ex.Entries.Count() ==1 && ex.Entries.First().Entity is SokoZaiko) {
                        if (loopCount++ > 10) {
                            throw;  //10回トライしてダメなら例外スロー
                        }
                        Thread.Sleep(1000); //１秒待つ
                        //倉庫在庫をデタッチしないと、キャッシュが生きたままなので
                        //（１）の処理で同じデータを取得してしまう為の処置
                        foreach (var item in shiire.SokoZaikos) {
                            _context.Entry(item).State = EntityState.Detached;
                        }
                        //注文残の引き戻し
                        foreach (var item in shiire.Shiirejissekis) {
                            item.ChumonJissekiMeisaii.ChumonZan=
                            _context.Entry(item.ChumonJissekiMeisaii).Property(p => p.ChumonZan).OriginalValue;
                        }
                        loopFlg = true;
                    }
                    else {      //その他排他制御の場合は例外をスローする
                        throw;
                    }
                }
            }

            //shiireJissekiのSokoZaikoに、実際の倉庫在庫を接続（表示用）
            shiire.ShiireSokoConnection(shiirezaikoset.ShiireJissekis, shiirezaikoset.SokoZaikos);

            //表示用部分ビューに結果を返す
            //ShiireViewModel shiireModel = SetShiireModel(entities,shiirezaikoset.ShiireJissekis);

            return (entities, shiire.Shiirejissekis);
        }

        public async Task<ShiireKeysViewModel> SetShiireKeysModel() {
            var shiireKeysModel = new ShiireKeysViewModel {
                ChumonId = null,
                ChumonIdList = (await shiire.ZanAriChumonList())
                .Select(s => new SelectListItem { Value = s.ChumonId, Text = s.ChumonId + ":" + s.ChumonZan.ToString() })
                .ToList()
            };
            return (shiireKeysModel);
        }

        //更新

        public async Task<int> ShiireUpdate() {
            var entitiesx = _context.ChangeTracker.Entries();
            var entities = _context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .Select(e => e.Entity).Count();

            await _context.SaveChangesAsync();

            return (entities);
        }
    }
}