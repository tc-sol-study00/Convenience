using AutoMapper;
using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SelfStudy.Interfaces;

namespace SelfStudy.Propaties {

    //(1) Newしてもらう
    public class ObjectCopy : IObjectCopy, IDbContext {
        //(2)名前をつける（属性）＝プロパティ
        private ConvenienceContext _context;
        private enum CopyMethod {
            ObjectCopyExec,
            ObjectCopyExecByAutoMapper,
            ObjectCopyExecByLINQ,
            ObjectCopyExecByJson,
        }
        public IList<ChumonJisseki> OriginalChumonJissekis { get; set; }

        public IList<ChumonJisseki> ClonedChumonJissekis { get; set; }
        public static int Flg { get; set; }

        //(3)初期化（コンストラクタ）
        //戻り値がない
        public ObjectCopy(int argFlg) :this(SetDbContext(),argFlg) {
        }

        public ObjectCopy(ConvenienceContext context,int argFlg){
            _context = context;
            OriginalChumonJissekis = new List<ChumonJisseki>();
            ClonedChumonJissekis = new List<ChumonJisseki>();
            Flg = argFlg;
        }

        private static ConvenienceContext SetDbContext() {
            return IDbContext.DbOpen();
        }

        /// <summary>
        /// メイン処理
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<IList<ChumonJisseki>> ObjectCopyController() {

            CopyMethod processSelectFlg = CopyMethod.ObjectCopyExecByJson;

            OriginalChumonJissekis = GetObject();

            ClonedChumonJissekis = processSelectFlg switch {
                CopyMethod.ObjectCopyExec => ObjectCopyExec(OriginalChumonJissekis),
                CopyMethod.ObjectCopyExecByAutoMapper => ObjectCopyExecByAutoMapper(OriginalChumonJissekis),
                CopyMethod.ObjectCopyExecByLINQ => ObjectCopyExecByLINQ(OriginalChumonJissekis),
                CopyMethod.ObjectCopyExecByJson => ObjectCopyExecByJson(OriginalChumonJissekis),
                _ => throw new ArgumentException("Invalid process flag")
            };

            Flg += 10;
            return ClonedChumonJissekis;
        }

        //(4)できること
        //Y=f(x1,x2,x3)

        /// <summary>
        /// DB読み込み
        /// </summary>
        /// <returns></returns>
        private IList<ChumonJisseki> GetObject() {
            var originalChumonJissekis = _context.ChumonJisseki
                .AsNoTracking()
                .Include(x => x.ChumonJissekiMeisais)
                .ToList();
            return originalChumonJissekis;
        }

        /// <summary>
        /// 手作りクーロン化
        /// </summary>
        /// <param name="argChumonJissekis"></param>
        /// <returns></returns>
        private static IList<ChumonJisseki> ObjectCopyExec(IList<ChumonJisseki> argChumonJissekis) {

            //OriginalChumonJissekis = argChumonJisseki;
            //取得した注文実績をコピーする

            var clonedChumonJissekis = new List<ChumonJisseki>();

            foreach (var aOriginalChumonJisseki in argChumonJissekis) {

                ChumonJisseki createdChumonJisseki = new ChumonJisseki() {
                    ChumonId = aOriginalChumonJisseki.ChumonId,
                    ShiireSakiId = aOriginalChumonJisseki.ShiireSakiId,
                    ChumonDate = aOriginalChumonJisseki.ChumonDate,
                };

                IList<ChumonJissekiMeisai> createdChumonJissekiMeisais = new List<ChumonJissekiMeisai>();

                foreach (var aOriginalChumonJissekiMeisais in aOriginalChumonJisseki.ChumonJissekiMeisais) {

                    ChumonJissekiMeisai createdChumonJissekiMeisai = new ChumonJissekiMeisai() {
                        ChumonId = aOriginalChumonJissekiMeisais.ChumonId,
                        ShiireSakiId = aOriginalChumonJissekiMeisais.ShiireSakiId,
                        ShiirePrdId = aOriginalChumonJissekiMeisais.ShiirePrdId,
                        ShohinId = aOriginalChumonJissekiMeisais.ShohinId,
                        ChumonSu = aOriginalChumonJissekiMeisais.ChumonSu,
                        ChumonZan = aOriginalChumonJissekiMeisais.ChumonZan,
                        LastChumonSu = aOriginalChumonJissekiMeisais.LastChumonSu,
                    };
                    createdChumonJissekiMeisais.Add(createdChumonJissekiMeisai);
                }

                createdChumonJisseki.ChumonJissekiMeisais = createdChumonJissekiMeisais;
                clonedChumonJissekis.Add(createdChumonJisseki);
            }
            return clonedChumonJissekis;
        }

        /// <summary>
        /// AutoMapperでクーロン化
        /// </summary>
        /// <param name="argChumonJissekis"></param>
        /// <returns></returns>
        private static IList<ChumonJisseki> ObjectCopyExecByAutoMapper(IList<ChumonJisseki> argChumonJissekis) {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<ChumonJisseki, ChumonJisseki>();
                cfg.CreateMap<ChumonJissekiMeisai, ChumonJissekiMeisai>();
            });
            // IMapperのインスタンスを作成
            IMapper mapper = config.CreateMapper();
            var clonedChumonJissekis = mapper.Map<IList<ChumonJisseki>>(argChumonJissekis);

            return clonedChumonJissekis;
        }

        /// <summary>
        /// LINQでクーロン化
        /// </summary>
        /// <param name="argChumonJissekis"></param>
        /// <returns></returns>
        private static IList<ChumonJisseki> ObjectCopyExecByLINQ(IList<ChumonJisseki> argChumonJissekis) {
            IList<ChumonJisseki> clonedChumonJissekis = argChumonJissekis.Select(x => new ChumonJisseki {
                ChumonId = x.ChumonId,
                ShiireSakiId = x.ShiireSakiId,
                ChumonDate = x.ChumonDate,
                ChumonJissekiMeisais = x.ChumonJissekiMeisais?.Select(y => new ChumonJissekiMeisai {
                    ChumonId = y.ChumonId,
                    ShiireSakiId = y.ShiireSakiId,
                    ShiirePrdId = y.ShiirePrdId,
                    ShohinId = y.ShohinId,
                    ChumonSu = y.ChumonSu,
                    ChumonZan = y.ChumonZan,
                    LastChumonSu = y.LastChumonSu
                }).ToList()
            }).ToList();

            return clonedChumonJissekis;
        }

        /// <summary>
        /// JSONシリアル・デシリアルでクーロン化
        /// </summary>
        /// <param name="argChumonJissekis"></param>
        /// <returns></returns>
        /// <exception cref="NoDataFoundException"></exception>
        private static IList<ChumonJisseki> ObjectCopyExecByJson(IList<ChumonJisseki> argChumonJissekis) {
            //シリアル化
            string serialedChumonJisseki
                = JsonConvert.SerializeObject(argChumonJissekis, Formatting.Indented,
                new JsonSerializerSettings {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

            //デシリアル化
            IList<ChumonJisseki> clonedChumonJissekis
                = JsonConvert.DeserializeObject<IList<ChumonJisseki>>(serialedChumonJisseki)
                ?? throw new NoDataFoundException("データなし");

            return clonedChumonJissekis;
        }
    }
}