using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Convenience.Models.ViewModels.Shiire;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static Convenience.Models.Properties.Shiire;

namespace Convenience.Models.Services {

    public class ShiireService : IShiireService {
        private readonly ConvenienceContext _context;

        private readonly IShiire shiire;

        /// <summary>
        /// コンストラクター　通常用
        /// </summary>
        /// <param name="context">DBコンテキスト</param>
        /// <param name="shiire">仕入クラスＤＩ注入用</param>
        public ShiireService(ConvenienceContext context,IShiire shiire) {
            this._context = context;
            this.shiire = shiire; 
            //ShiireClassCreate();
        }

        /*
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
        */

        /*
         * 機能：仕入ハンドリング（新規用）
         *     ：仕入実績データの画面初期表示用（DB更新後の再表示も含む）
         *     ：①現在時間により仕入日セット
         *     ：②仕入は毎回新規なので、仕入SEQを発番・注文実績から仕入実績を作る
         *     ：③関係する倉庫在庫を接続（表示用）
         * 入力：注文コード
         * 出力：仕入実績・インクルードされた注文実績・インクルードされた倉庫在庫
         * 
         */
        public async Task<(int, IList<ShiireJisseki>)> ShiireHandling(string inChumonId) {
            DateOnly inShiireDate = DateOnly.FromDateTime(DateTime.Now);
            //仕入SEQ
            uint inSeqByShiireDate = await shiire.NextSeq(inChumonId, inShiireDate);
            IList<ShiireJisseki> shiireJissekis;
            //新規の場合
            shiireJissekis = await shiire.ChumonToShiireJisseki(inChumonId, inShiireDate, inSeqByShiireDate);

            //shiireJissekiのSokoZaikoに、実際の倉庫在庫を接続（表示用）
            shiire.ShiireSokoConnection(shiireJissekis, _context.SokoZaiko);

            //DB更新エンティティ数=0、処理表示するための仕入実績データを返却
            return (0, shiire.Shiirejissekis);
        }

        /*
         * 機能：仕入ハンドリング（Post後処理・再表示用）
         *     ：仕入実績データPost後の後の再表示用
         * 　　：①仕入実績がある場合は、仕入実績取り込み、ない場合は注文実績から作成
         * 　　：②　①の内容に対し、ポストデータを反映
         * 　　：③注文実績の注文残と倉庫在庫の在庫数を仕入数にあわせ過不足する
         * 　　：④仕入実績DB更新
         * 　　：⑤仕入実績に倉庫在庫を接続しインクルードできるようにする（表示用）
         * 入力：注文コード・仕入日・仕入SEQ、Postされた仕入実績データ
         * 出力：更新エンティティ数・DB更新された仕入実績
         */
        public async Task<(int, IList<ShiireJisseki>)> ShiireHandling(string inChumonId, DateOnly inShiireDate, uint inSeqByShiireDate, IList<ShiireJisseki> inShiireJissekis) {
            IList<ShiireJisseki> shiireJissekis;

            //機能（１）  ：仕入実績データを準備する
            //入力        ：注文実績・仕入日付・仕入SEQ
            //出力        ：仕入実績があれば、DBから仕入実績を読み込む
            //　　        ：仕入実績がなければ、注文実績から仕入実績を作る
            //              ※　出力は仕入クラスの仕入実績プロパティを参照している

            //機能（１－１）：仕入実績が存在しているかチェック
            if (await shiire.ChuumonIdOnShiireJissekiExistingCheck(inChumonId, inShiireDate, inSeqByShiireDate)) {
                //機能（１－１－１）：仕入実績がある場合（更新用）
                shiireJissekis = await shiire.ShiireToShiireJisseki(inChumonId, inShiireDate, inSeqByShiireDate);  
            }
            else {
                //機能（１－１－２）仕入実績がない場合（新規用）
                shiireJissekis = await shiire.ChumonToShiireJisseki(inChumonId, inShiireDate, inSeqByShiireDate);  
            }

            //機能（２） ：（１）で処理した注文実績プロパティを、ポストデータで更新する
            //　         ：入力：ポストされた仕入実績データと、仕入実績プロパティ
            //　         ：出力：ポストデータで更新された仕入実績
            shiireJissekis = shiire.ShiireUpdate(inShiireJissekis);

            //機能（３） ：（２）のポストデータが反映された注文実績プロパティをベースに以下の処理を行う
            //  ・ポストデータ反映後のデータを元に注文実績の注文残と倉庫残を調整する
            //　・DBに保存する

            //初期化
            ShiireUkeireReturnSet shiirezaikoset = null;    //仕入実績・在庫を管理するオブジェクト 
            bool isLoopContinue = true;                     //リトライフラグ→DB更新のトライを続けるかフラグ 
            uint loopCount = 1;                             //リトライ回数を管理する変数
            int entities=0;                                 //SaveChangeしたエンティティ数
            const int reTryMaxCount = 10;                   //リトライする回数
            const int waitTime = 1000;    //1000m秒=1秒     //排他エラー時の再リトライ前の待機時間（単位ミリ秒）

            while (isLoopContinue) {
                //プロパティの内容から、上記で反映した内容で、注文実績の注文残と倉庫残を調整する
                //在庫の登録はここで行われる
                shiirezaikoset = await shiire.ChuumonZanZaikoSuChousei(inChumonId, shiireJissekis);

                try {
                    //ＤＢ保管処理
                    entities = await ShiireUpdate();
                    isLoopContinue = false; //ステートメントまで来たら例外なしなので、リトライフラグをfalseにする
                }
                //排他制御エラーの場合
                catch (DbUpdateConcurrencyException ex) {
                    if (ex.Entries.Count() == 1 && ex.Entries.First().Entity is SokoZaiko) {
                        if (loopCount++ > reTryMaxCount)throw;  //10回トライしてダメなら例外スロー

                        Thread.Sleep(waitTime); //１秒待つ
                        //倉庫在庫をデタッチしないと、キャッシュが生きたままなので
                        //（１）の処理で同じデータを取得してしまう為の処置
                        foreach (var item in shiire.SokoZaikos) {
                            _context.Entry(item).State = EntityState.Detached;
                        }
                        //注文残の引き戻し
                        //処理が失敗しているので、注文残を引き戻す
                        foreach (var item in shiire.Shiirejissekis) {
                            item.ChumonJissekiMeisaii.ChumonZan =
                            _context.Entry(item.ChumonJissekiMeisaii).Property(p => p.ChumonZan).OriginalValue;
                        }
                        //リトライする
                        isLoopContinue = true;
                    }
                    else {      
                        //その他排他制御の場合は例外をスローする
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

        /*
         * 機能：DB更新処理
         * 入力：
         *      想定DB更新エンティティ
         *          ①注文実績＋明細（注文残）
         *          ②仕入実績
         *          ③倉庫在庫
         * 出力：DB更新・更新エンティティ数 
         */
        public async Task<int> ShiireUpdate() {

            //DB更新見込みのエンティティ数を求める→1以上だとなんらか更新されたという意味
            int entities = _context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .Select(e => e.Entity).Count();

            //DB更新
            await _context.SaveChangesAsync();

            //更新エンティティ数返却
            return (entities);
        }
    }
}