using Convenience.Models.DataModels;
using static Convenience.Models.Properties.Shiire;

namespace Convenience.Models.Interfaces {
    public interface IShiire {

        /*
         * プロパティ
         */

        //仕入実績
        public IList<ShiireJisseki> Shiirejissekis { get; set; } //Include ChuumonJissekiMeisai
        //倉庫在庫     
        public IList<SokoZaiko> SokoZaikos { get; set; }

        /*
         *  Method群 
         */

        //仕入実績に既に存在しているかチェックし、新規・既存の切り分け
        public bool ChuumonIdOnShiireJissekiExistingCheck(string inChumonId, DateOnly inShiireDate, uint inSeqByShiireDate);

        /*
         *  初期表示のために利用する想定
         */

        //仕入SEQの発番（注文IDに対し、新規用）
        public uint NextSeq(string inChumonId, DateOnly inShiireDate);

        // 仕入実績作成（注文IDに対し、新規用）
        // すでに注文されているので、それを仕入実績の初期データとして作成する
        public IList<ShiireJisseki> ChumonToShiireJisseki(string inChumonId, DateOnly inShiireDate, uint inSeqByShiireDate);

        //  仕入実績読み込み（注文IDに対し、既存用）
        public IList<ShiireJisseki> ShiireToShiireJisseki(string inChumonId, DateOnly inShiireDate, uint inSeqByShiireDate);

        //倉庫在庫を仕入データに接続する
        public void ShiireSokoConnection(IList<ShiireJisseki> inShiireJissekis, IEnumerable<SokoZaiko> indata);

        //  注文残・在庫数量調整
        public ShiireUkeireReturnSet ChuumonZanZaikoSuChousei(string inChumonId, IList<ShiireJisseki> inShiireJissekis);

        /*
         *  Post後で利用想定
         */

        // 仕入データPost内容の反映
        public IList<ShiireJisseki> ShiireUpdate(IList<ShiireJisseki> inShiireJissekis);

        /*
         *  注文残があるものリスト 
         */

        //注文残がある注文のリスト化（仕入する前に、一覧を出す用）
        public IList<ChumonList> ZanAriChumonList();

    }
}

