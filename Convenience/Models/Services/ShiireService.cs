using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Convenience.Models.Properties.Config;
using Convenience.Models.ViewModels.Shiire;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static Convenience.Models.Properties.Config.Message;
using static Convenience.Models.Properties.Shiire;

namespace Convenience.Models.Services {
    /// <summary>
    /// 仕入サービスクラス
    /// </summary>
    public class ShiireService : IShiireService, IDbContext, IDisposable {
        
        //DBコンテキスト
        //private readonly ConvenienceContext _context;

        /// <summary>
        /// 仕入クラス用オブジェクト変数
        /// </summary>
        private readonly IShiire _shiire;

        /// <summary>
        /// 仕入キービューモデル（１枚目の画面用）
        /// </summary>
        public ShiireKeysViewModel ShiireKeysViewModel { get; set; } = new ShiireKeysViewModel();

        /// <summary>
        /// 仕入ビューモデル（２枚目の画面用） 
        /// </summary>
        public ShiireViewModel ShiireViewModel { get; set; } = new ShiireViewModel();

        private bool _disposed = false;
        private bool _createdComposition = false;

        /// <summary>
        /// コンストラクター　通常用
        /// </summary>
        /// <param name="context">DBコンテキスト</param>
        /// <param name="shiire">仕入クラスＤＩ注入用</param>
        public ShiireService(IShiire shiire) {
            _shiire = shiire;
        }

        /// <summary>
        /// C#コンソールデバッグ用
        /// </summary>
        public ShiireService() {
            //this._context = ((IDbContext)this).DbOpen();
            _shiire = new Shiire(IDbContext.DbOpen());
        }

        /// <summary>
        ///デトラクタ（アンマネージドリソース開放用）
        /// </summary>
        ~ShiireService() {
            Dispose(false);
        }

        /// <summary>
        /// ファイナライザ
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// ファイナライザ（オーバーライド可）
        /// </summary>
        protected virtual void Dispose(bool disposing) {
            if ((!_disposed) && _createdComposition) {
                if (disposing) {
                    //マネージドリソース解放を書く
                    _shiire?.Dispose();
                }
                //アンマネージドリソース解放を書く

                //複数回実行しないように
                _disposed = true;
            }
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
            var chumonId = inShiireKeysViewModel.ChumonId??throw new ArgumentException("注文コードがセットされていません");
            DateOnly inShiireDate = DateOnly.FromDateTime(DateTime.Now);
            //仕入SEQ
            uint inSeqByShiireDate = await _shiire.NextSeq(chumonId, inShiireDate);
            IList<ShiireJisseki> createdShiireJissekis;
            //新規の場合
            createdShiireJissekis = await _shiire.ChumonToShiireJisseki(chumonId, inShiireDate, inSeqByShiireDate);

            //shiireJissekiのSokoZaikoに、実際の倉庫在庫を接続（表示用）
            _shiire.ShiireSokoConnection(createdShiireJissekis, _shiire.GetSokoZaiko());

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
            if (await _shiire.ChuumonIdOnShiireJissekiExistingCheck(chumonId, shiireDate, seqByShiireDate)) {
                //機能（１－１－１）：仕入実績がある場合（更新用）
                shiireJissekis = await _shiire.ShiireToShiireJisseki(chumonId, shiireDate, seqByShiireDate);
            }
            else {
                //機能（１－１－２）仕入実績がない場合（新規用）
                shiireJissekis = await _shiire.ChumonToShiireJisseki(chumonId, shiireDate, seqByShiireDate);
            }

            //機能（２） ：（１）で処理した注文実績プロパティを、ポストデータで更新する
            //　         ：入力：ポストされた仕入実績データと、仕入実績プロパティ
            //　         ：出力：ポストデータで更新された仕入実績
            shiireJissekis = _shiire.ShiireUpdate(inShiireJissekis);

            //機能（３） ：（２）のポストデータが反映された注文実績プロパティをベースに以下の処理を行う
            //  ・ポストデータ反映後のデータを元に注文実績の注文残と倉庫残を調整する
            //　・DBに保存する

            //初期化
            //ShiireUkeireReturnSet? shiirezaikoset = null;   //仕入実績・在庫を管理するオブジェクト 
            int entities = 0;                                 //SaveChangeしたエンティティ数

            //プロパティの内容から、上記で反映した内容で、注文実績の注文残と倉庫残を調整する
            //在庫の登録はここで行われる
            IList<ShiireJisseki> adjustedShiireJissekis;
            IList<SokoZaiko> adjustedSokoZaiko;

            (adjustedShiireJissekis, adjustedSokoZaiko) =await _shiire.ChuumonZanZaikoSuChousei(chumonId, shiireJissekis);

            // 仕入実績・注文残・倉庫在庫を更新する
            (entities,bool isRetryAtSaveDB) = await _shiire.ShiireSaveChanges();

            //savechange時、リトライがあったら倉庫在庫を再更新しているので、再セット
            if(isRetryAtSaveDB) adjustedSokoZaiko = _shiire.SokoZaikos;

            //shiireJissekiのSokoZaikoに、実際の倉庫在庫を接続（表示用）
            _shiire.ShiireSokoConnection(adjustedShiireJissekis, adjustedSokoZaiko);
            
            List<ShiireJisseki> listdt = (List<ShiireJisseki>)adjustedShiireJissekis;
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
                ChumonIdList = (await _shiire.ZanAriChumonList())
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
            ShiireJisseki shiireJisseki = inshiireJissekis?.FirstOrDefault()
                ??throw new ArgumentException("仕入実績がセットされていません");

            this.ShiireViewModel = new ShiireViewModel {
                ChumonId = shiireJisseki.ChumonId
                ,
                ChumonDate = shiireJisseki.ChumonJissekiMeisaii.ChumonJisseki?.ChumonDate ?? DateOnly.MinValue
                ,
                ShiireDate = shiireJisseki.ShiireDate
                ,
                SeqByShiireDate = shiireJisseki.SeqByShiireDate
                ,
                ShiireSakiId = shiireJisseki.ShiireSakiId
                ,
                ShiireSakiKaisya = shiireJisseki.ChumonJissekiMeisaii.ShiireMaster?.ShiireSakiMaster?.ShiireSakiKaisya ?? string.Empty
                ,
                ShiireJissekis = inshiireJissekis
                ,
                IsNormal = true //正常終了
                ,
                Remark = entities != 0 ? new Message().SetMessage(ErrDef.NormalUpdate)?.MessageText??string.Empty : string.Empty
            };
            return (this.ShiireViewModel);
        }

    }
}