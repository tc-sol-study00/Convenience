using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Convenience.Models.ViewModels.Chumon;
using Convenience.Models.ViewModels.Shiire;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using static Convenience.Models.Properties.Message;
using static Convenience.Models.Properties.Shiire;

namespace Convenience.Models.Services {
    /// <summary>
    /// 仕入サービスクラス
    /// </summary>
    public class ShiireService : IShiireService, IDbContext {
        
        //DBコンテキスト
        private readonly ConvenienceContext _context;

        /// <summary>
        /// 仕入クラス用オブジェクト変数
        /// </summary>
        public IShiire shiire { get; set; }

        /// <summary>
        /// 仕入キービューモデル（１枚目の画面用）
        /// </summary>
        public ShiireKeysViewModel ShiireKeysViewModel { get; set; } = new ShiireKeysViewModel();

        /// <summary>
        /// 仕入ビューモデル（２枚目の画面用） 
        /// </summary>
        public ShiireViewModel ShiireViewModel { get; set; } = new ShiireViewModel();

        /// <summary>
        /// コンストラクター　通常用
        /// </summary>
        /// <param name="context">DBコンテキスト</param>
        /// <param name="shiire">仕入クラスＤＩ注入用</param>
        public ShiireService(ConvenienceContext context, IShiire shiire) {
            this._context = context;
            this.shiire = shiire;
        }

        /// <summary>
        /// C#コンソールデバッグ用
        /// </summary>
        public ShiireService() {
            this._context = IDbContext.DbOpen();
        }

        /// <summary>
        /// <para>仕入セッティング</para>
        /// <para>仕入実績データの画面初期表示用（DB更新後の再表示も含む）</para>
        /// </summary>
        /// <param name="inShiireKeysViewModel">注文キービューモデル</param>
        /// <returns>ShiireViewModel 仕入ビューモデル（仕入実績・インクルードされた注文実績・インクルードされた倉庫在庫）</returns>
        /// <remarks>
        /// <para>①現在時間により仕入日セット</para>
        /// <para>②仕入は毎回新規なので、仕入SEQを発番・注文実績から仕入実績を作る</para>
        /// <para>③関係する倉庫在庫を接続（表示用）</para>
        /// </remarks>
        public async Task<ShiireViewModel> ShiireSetting(ShiireKeysViewModel inShiireKeysViewModel) {
            var chumonId = inShiireKeysViewModel.ChumonId;
            DateOnly inShiireDate = DateOnly.FromDateTime(DateTime.Now);
            //仕入SEQ
            uint inSeqByShiireDate = await shiire.NextSeq(chumonId, inShiireDate);
            IList<ShiireJisseki> createdShiireJissekis;
            //新規の場合
            createdShiireJissekis = await shiire.ChumonToShiireJisseki(chumonId, inShiireDate, inSeqByShiireDate);

            //shiireJissekiのSokoZaikoに、実際の倉庫在庫を接続（表示用）
            shiire.ShiireSokoConnection(createdShiireJissekis, _context.SokoZaiko);

            List<ShiireJisseki> listdt = (List<ShiireJisseki>)createdShiireJissekis;
            listdt.Sort((x, y) => {
                int result = (x.ShiireSakiId != y.ShiireSakiId) ? x.ShiireSakiId.CompareTo(y.ShiireSakiId) :
                              (x.ShiirePrdId != y.ShiirePrdId) ? x.ShiirePrdId.CompareTo(y.ShiirePrdId) :
                              x.ShohinId.CompareTo(y.ShohinId);
                return result;
            });

            this.ShiireViewModel=SetShiireModel(0, listdt);

            //DB更新エンティティ数=0、処理表示するための仕入実績データを返却
            return (this.ShiireViewModel);
        }

        /// <summary>
        /// <para>仕入データをDBに書き込む・注文残の調整・倉庫在庫への反映（Post後処理・再表示用）</para>
        /// <para>仕入実績データPost後の後の再表示用</para>
        /// </summary>
        /// <param name="inShiireViewModel">仕入ビューモデル（注文コード・仕入日・仕入SEQ、Postされた仕入実績データ）</param>
        /// <returns>仕入ビューモデル（更新エンティティ数・DB更新された仕入実績）</returns>
        /// <remarks>
        /// <para>①仕入実績がある場合は、仕入実績取り込み、ない場合は注文実績から作成</para>
        /// <para>②　①の内容に対し、ポストデータを反映</para>
        /// <para>③注文実績の注文残と倉庫在庫の在庫数を仕入数にあわせ過不足する</para>
        /// <para>④仕入実績DB更新</para>
        /// <para>⑤仕入実績に倉庫在庫を接続しインクルードできるようにする（表示用）</para>
        /// </remarks>
        public async Task<ShiireViewModel> ShiireCommit(ShiireViewModel inShiireViewModel) {

            var chumonId = inShiireViewModel.ChumonId;
            var shiireDate=inShiireViewModel.ShiireDate;
            var seqByShiireDate = inShiireViewModel.SeqByShiireDate;
            var inShiireJissekis = inShiireViewModel.ShiireJissekis;

            IList<ShiireJisseki> shiireJissekis;

            //機能（１）  ：仕入実績データを準備する
            //入力        ：注文実績・仕入日付・仕入SEQ
            //出力        ：仕入実績があれば、DBから仕入実績を読み込む
            //　　        ：仕入実績がなければ、注文実績から仕入実績を作る
            //              ※　出力は仕入クラスの仕入実績プロパティを参照している

            //機能（１－１）：仕入実績が存在しているかチェック
            if (await shiire.ChuumonIdOnShiireJissekiExistingCheck(chumonId, shiireDate, seqByShiireDate)) {
                //機能（１－１－１）：仕入実績がある場合（更新用）
                shiireJissekis = await shiire.ShiireToShiireJisseki(chumonId, shiireDate, seqByShiireDate);
            }
            else {
                //機能（１－１－２）仕入実績がない場合（新規用）
                shiireJissekis = await shiire.ChumonToShiireJisseki(chumonId, shiireDate, seqByShiireDate);
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
            int entities = 0;                                 //SaveChangeしたエンティティ数
            const int reTryMaxCount = 10;                   //リトライする回数
            const int waitTime = 1000;    //1000m秒=1秒     //排他エラー時の再リトライ前の待機時間（単位ミリ秒）

            while (isLoopContinue) {
                //プロパティの内容から、上記で反映した内容で、注文実績の注文残と倉庫残を調整する
                //在庫の登録はここで行われる
                shiirezaikoset = await shiire.ChuumonZanZaikoSuChousei(chumonId, shiireJissekis);

                try {
                    //ＤＢ保管処理

                    //DB更新見込みのエンティティ数を求める→1以上だとなんらか更新されたという意味
                    entities = _context.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                    .Select(e => e.Entity).Count();

                    //DB更新
                    await _context.SaveChangesAsync();

                    isLoopContinue = false; //ステートメントまで来たら例外なしなので、リトライフラグをfalseにする
                }
                //排他制御エラーの場合
                catch (DbUpdateConcurrencyException ex) {
                    if (ex.Entries.Count() == 1 && ex.Entries.First().Entity is SokoZaiko) {
                        if (loopCount++ > reTryMaxCount) throw;  //10回トライしてダメなら例外スロー

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

            List<ShiireJisseki> listdt = (List<ShiireJisseki>)shiirezaikoset.ShiireJissekis;
            listdt.Sort((x, y) => {
                int result = (x.ShiireSakiId != y.ShiireSakiId) ? x.ShiireSakiId.CompareTo(y.ShiireSakiId) :
                              (x.ShiirePrdId != y.ShiirePrdId) ? x.ShiirePrdId.CompareTo(y.ShiirePrdId) :
                              x.ShohinId.CompareTo(y.ShohinId);
                return result;
            });

            //表示用部分ビューに結果を返す
            this.ShiireViewModel = SetShiireModel(entities, listdt);
            return (this.ShiireViewModel);
        }

        /// <summary>
        /// 仕入キーモデル設定（仕入画面１枚目用）
        /// </summary>
        /// <returns>ShiireKeysViewModel 仕入キービューモデル</returns>
        public async Task<ShiireKeysViewModel> SetShiireKeysModel() {
            var shiireKeysModel = new ShiireKeysViewModel {
                ChumonId = null,
                ChumonIdList = (await shiire.ZanAriChumonList())
                .Select(s => new SelectListItem { Value = s.ChumonId, Text = s.ChumonId + ":" + s.ChumonZan.ToString() })
                .ToList()
            };
            return (shiireKeysModel);
        }

        /// <summary>
        /// 仕入ビューモデル設定
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="inshiireJissekis"></param>
        /// <returns></returns>
        private ShiireViewModel SetShiireModel(int entities, IList<ShiireJisseki> inshiireJissekis) {
            ShiireJisseki shiireJisseki = inshiireJissekis.FirstOrDefault();

            this.ShiireViewModel = new ShiireViewModel {
                ChumonId = shiireJisseki.ChumonId
                ,
                ChumonDate = shiireJisseki.ChumonJissekiMeisaii.ChumonJisseki.ChumonDate
                ,
                ShiireDate = shiireJisseki.ShiireDate
                ,
                SeqByShiireDate = shiireJisseki.SeqByShiireDate
                ,
                ShiireSakiId = shiireJisseki.ShiireSakiId
                ,
                ShiireSakiKaisya = shiireJisseki.ChumonJissekiMeisaii.ShiireMaster.ShiireSakiMaster.ShiireSakiKaisya
                ,
                ShiireJissekis = inshiireJissekis
                ,
                IsNormal = true //正常終了
                ,
                Remark = entities != 0 ? new Message().SetMessage(ErrDef.NormalUpdate).MessageText : null
            };
            return (this.ShiireViewModel);
        }

    }
}