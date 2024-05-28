using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Convenience.Models.Properties {

    public class Chumon : IChumon, IDbContext {
        /*
         * 注文クラス
         */

        /*
         * プロパティ
         */

        //注文実績（データモデル）
        public ChumonJisseki ChumonJisseki { get; set; }

        /*
         * 共通変数
         */

        //DBコンテキスト
        private readonly ConvenienceContext _context;

        /*
         * コンストラクタ
         */

        public Chumon(ConvenienceContext context) {
            _context = context;
        }

        //注文クラスデバッグ用
        public Chumon() {
            _context = IDbContext.DbOpen();
        }

        public void Chumon2() {
            //_context = IDbContext.DbOpen();
        }


        //注文作成
        public ChumonJisseki ChumonSakusei(string inShireSakiId, DateOnly inChumonDate) {
            /*
             * 注文作成（新規）
             *  引数　  仕入先コード
             *  戻り値　注文実績
             *
             *  仕入先より注文実績データ（親）を生成する
             *  注文実績明細データ（子）を仕入マスタを元に作成する
             *  注文実績データ（親）と注文実績明細データ（子）を連結する
             *  注文実績（プラス注文実績明細）を戻り値とする
             */

            //引数チェック（仕入先コード有無）
            if (_context.ShiireSakiMaster.Find(inShireSakiId) == null) {
                throw new Exception("仕入先ＩＤエラー");
            }
            //仕入先より注文実績データ（親）を生成する(a)

            ChumonJisseki = new ChumonJisseki {
                ChumonId = ChumonIdHatsuban(inChumonDate,_context),              //注文コード発番
                ShiireSakiId = inShireSakiId,                       //仕入先コード（引数より）
                ChumonDate = inChumonDate                           //注文日付
            };

            //注文実績明細データ（子）を作るために仕入マスタを読み込む(b)
            var shiireMasters = _context.ShireMaster
                .Where(shiire => shiire.ShiireSakiId == inShireSakiId)
                .Include(shiire => shiire.ShiireSakiMaster)
                .Include(shiire => shiire.ShohinMaster)
                .OrderBy(shiire => shiire.ShohinId);


            ChumonJisseki.ChumonJissekiMeisais = new List<ChumonJissekiMeisai>() { };

            //(b)のデータから注文実績明細を作成する
            foreach (var shiire in shiireMasters) {

                shiire.ShohinMaster.ShiireMasters = null;
                //shiire.ShiireSakiMaster.ShireMasters = null;

                var meisai = new ChumonJissekiMeisai {
                    ChumonId = ChumonJisseki.ChumonId,
                    ShiireSakiId = ChumonJisseki.ShiireSakiId,  //仕入先コードを注文実績からセット(aより)
                    ShiirePrdId = shiire.ShiirePrdId,           //仕入商品コードのセット(bより）
                    ShohinId = shiire.ShohinId,                 //仕入マスタから商品コード（bより）
                    ChumonSu = 0,                               //初期値として注文数０をセット
                    ChumonZan = 0,                              //初期値として注文残０をセット
                    ShiireMaster = shiire                       //仕入マスタに対するリレーション情報のセット
                };
                ChumonJisseki.ChumonJissekiMeisais.Add(meisai);
            }

            //_context.ChumonJisseki.Attach(ChumonJisseki);
            //注文実績（プラス注文実績明細）を戻り値とする
            return ChumonJisseki;
        }

        public ChumonJisseki ChumonToiawase(string inShireSakiId, DateOnly inChumonDate) {
            /*
             * 注文更新用問い合わせ
             *  引数　  仕入先コード、注文日
             *  戻り値　注文実績
             *
             *  ①注文実績＋注文実績＋仕入マスタ＋商品マスタ検索
             *  ②戻り値を注文実績＋注文実績明細とする
             */
            //①注文実績＋注文実績＋仕入マスタ＋商品マスタ検索
            ChumonJisseki = _context.ChumonJisseki
                     .Where(c => c.ShiireSakiId == inShireSakiId && c.ChumonDate == inChumonDate)
                     .Include(cm => cm.ChumonJissekiMeisais)
                     .ThenInclude(shi => shi.ShiireMaster)
                     .ThenInclude(sho => sho.ShohinMaster)
                     .FirstOrDefault();
            //②戻り値を注文実績＋注文実績明細とする
            return (ChumonJisseki);
        }

        private string ChumonIdHatsuban(DateOnly InTheDate,ConvenienceContext _context) {
            /*
             * 注文コード発番
             *  引数　  なし
             *  戻り値　注文コード
             *
             *  注文コード書式例）：20240129-001(yyyyMMdd-001～999）
             */
            uint seqNumber;
            string dateArea;
            //今日の日付
            dateArea = InTheDate == null ? DateTime.Today.ToString("yyyyMMdd"):dateArea = InTheDate.ToString("yyyyMMdd");

            //今日の日付からすでに今日の分の注文コードがないか調べる
            var chumonid = _context.ChumonJisseki
                .Where(x => x.ChumonId.StartsWith(dateArea))
                .Max(x => x.ChumonId);

            seqNumber = string.IsNullOrEmpty(chumonid) ? 1 //今日、注文コード起こすのが初めての場合
                      : uint.Parse(chumonid.Substring(9, 3)) + 1;
            // 上記以外の場合、 //注文コードの右３桁の数値を求め＋１にする

            ////３桁の数値が999以内（ＯＫ） それを超過するとnull
            ///

            return seqNumber <= 999 ? $"{dateArea}-{seqNumber:000}" : null;  // 999以上はNULLセット
        }

        public ChumonJisseki ChumonUpdate2(ChumonJisseki inChumonJisseki) {

            //注文実績明細のpostデータを反映する
            //ベース　メンバーの注文実績内の注文実績明細
            //postデータで注文実績データは更新かけない仕様だから、反映処理不要

            foreach (ChumonJissekiMeisai existedChumonJissekiMeisai in ChumonJisseki.ChumonJissekiMeisais) {

                //メンバーの内容と、引数の内容（Postされた内容）を照合して、注文数と注文残を上乗せしている処理
                ChumonJissekiMeisai? postedChumonJissekiMeisais = inChumonJisseki.ChumonJissekiMeisais
                    .Where(x => x.ChumonId == existedChumonJissekiMeisai.ChumonId && x.ShiireSakiId == existedChumonJissekiMeisai.ShiireSakiId &&
                    x.ShiirePrdId == existedChumonJissekiMeisai.ShiirePrdId && x.ShohinId == existedChumonJissekiMeisai.ShohinId).FirstOrDefault();

                if (postedChumonJissekiMeisais != null) {　//←念為
                    existedChumonJissekiMeisai.ChumonSu = postedChumonJissekiMeisais.ChumonSu;
                    //以下は、注文数を調整された時に注文残をバランスをとるため時、ごちゃごちゃ書いた
                    existedChumonJissekiMeisai.ChumonZan += postedChumonJissekiMeisais.ChumonSu - existedChumonJissekiMeisai.ChumonSu;
                }

            }
            //注文実績がまだＤＢに登録されてなければＤＢに対して追加
            if (_context.ChumonJisseki.Where(x => x.ChumonId == inChumonJisseki.ChumonId).FirstOrDefault() == null) {
                _context.ChumonJisseki.Add(ChumonJisseki);  //注文実績明細は、注文実績にぶら下がっているので同タイミングで挿入される
            }
            //登録されている場合は、更新なのでそのままで良い

            //注文実績の中の注文実績明細の中身を変更しているので、そのまま注文実績を戻り値としている

            //ここではsavechangesは実行せず、サービスで書けるがデバッグのときはかけてよい（そうしないと、ＤＢ更新の確認ができないから）
            return (ChumonJisseki);
        }

        public ChumonJisseki ChumonUpdate(ChumonJisseki inChumonJisseki) {
            /*
             * 注文実績＋注文明細更新
             *  引数　  注文実績
             *  戻り値　注文実績
             */

            ChumonJisseki existedChumonJisseki; //DBにすでに登録されている場合の移送先

            //プロパティ注文実績に引数の注文実績をセットする
            ChumonJisseki = inChumonJisseki;

            //注文実績を読む
            existedChumonJisseki = _context.ChumonJisseki
                .Include(e => e.ChumonJissekiMeisais.OrderBy(x => x.ShiirePrdId))
                .FirstOrDefault(e => e.ChumonId == ChumonJisseki.ChumonId);

            if (existedChumonJisseki != null) {  //注文実績がある場合

                //引数で渡された注文実績データを現プロパティに反映する
                var config = new MapperConfiguration(cfg => {
                    cfg.AddCollectionMappers();
                    cfg.CreateMap<ChumonJisseki, ChumonJisseki>()
                    .EqualityComparison((odto, o) => odto.ChumonId == o.ChumonId);
                    cfg.CreateMap<ChumonJissekiMeisai, ChumonJissekiMeisai>()
                    .EqualityComparison((odto, o) => odto.ChumonId == o.ChumonId && odto.ShiireSakiId == o.ShiireSakiId && odto.ShiirePrdId == o.ShiirePrdId && odto.ShohinId == o.ShohinId)
                    .BeforeMap((src, dest) => src.LastChumonSu = dest.ChumonSu)
                    .ForMember(dest => dest.ChumonZan, opt => opt.MapFrom(src => src.ChumonZan + src.ChumonSu - src.LastChumonSu))
                    .ForMember(dest => dest.ChumonJisseki, opt => opt.Ignore());
                });
                //引数で渡された注文実績をDBから読み込んだ注文実績に上書きする
                var mapper = new Mapper(config);
                mapper.Map(ChumonJisseki, existedChumonJisseki);

                ChumonJisseki = existedChumonJisseki;
            }
            else {   //注文実績がない場合、引数で渡された注文実績をDBにレコード追加する
                foreach (var item in ChumonJisseki.ChumonJissekiMeisais) {
                    item.ChumonZan = item.ChumonSu;
                }
                _context.ChumonJisseki.Add(ChumonJisseki);
            }
            //注文実績＋注文実績明細を戻り値とする
            return ChumonJisseki;
        }
    }
}