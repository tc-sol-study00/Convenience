using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net.Sockets;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace TestProject1 {
    [TestClass]
    public sealed class Test1 {
        [TestMethod]
        public async Task NorMalCaseForNew() {
            //
            Chumon chumon = new Chumon();
            ChumonJisseki chumonJisseki = await chumon.ChumonSakusei("A000000001", new DateOnly(2025, 02, 19));

            ChumonJisseki expectedChumonJisseki = new ChumonJisseki {
                ChumonId = "20250219-001",
                ChumonDate = new DateOnly(2025, 2, 19),
                ShiireSakiId = "A000000001"
            };

            List<ChumonJissekiMeisai> expectedChumonJissekiMeisais =
                new List<ChumonJissekiMeisai>() {
                    new ChumonJissekiMeisai() {
                        ChumonId = "20250219-001",
                        ShiireSakiId = "A000000001",
                        ShiirePrdId = "0000000001",
                        ShohinId = "0000000001",
                        ChumonSu = 0,
                        ChumonZan = 0,
                        LastChumonSu = null,
                    },
                    new ChumonJissekiMeisai() {
                        ChumonId = "20250219-001",
                        ShiireSakiId = "A000000001",
                        ShiirePrdId = "0000000002",
                        ShohinId = "0000000002",
                        ChumonSu = 0,
                        ChumonZan = 0,
                        LastChumonSu = null,
                    }
                };

            Assert.IsNotNull(chumonJisseki, "ChumonJisseki is  null.");
            Assert.AreEqual(expectedChumonJisseki.ChumonId, chumonJisseki.ChumonId);
            Assert.AreEqual(expectedChumonJisseki.ChumonDate, chumonJisseki.ChumonDate);
            Assert.AreEqual(expectedChumonJisseki.ShiireSakiId, chumonJisseki.ShiireSakiId);

            Assert.IsNotNull(chumonJisseki.ChumonJissekiMeisais, "ChumonJissekiMeisai is  null.");

            int i = 0;
            foreach (var aMeisai in chumonJisseki.ChumonJissekiMeisais) {
                Assert.AreEqual(expectedChumonJissekiMeisais[i].ChumonId, aMeisai.ChumonId);
                Assert.AreEqual(expectedChumonJissekiMeisais[i].ShiireSakiId, aMeisai.ShiireSakiId);
                Assert.AreEqual(expectedChumonJissekiMeisais[i].ShiirePrdId, aMeisai.ShiirePrdId);
                Assert.AreEqual(expectedChumonJissekiMeisais[i].ShohinId, aMeisai.ShohinId);
                Assert.AreEqual(expectedChumonJissekiMeisais[i].ChumonSu, aMeisai.ChumonSu);
                Assert.AreEqual(expectedChumonJissekiMeisais[i].ChumonZan, aMeisai.ChumonZan);
                Assert.AreEqual(expectedChumonJissekiMeisais[i].LastChumonSu, aMeisai.LastChumonSu);
                i++;
            }
        }

        [TestMethod]
        public async Task NorMalCaseForNewAlready() {
            Chumon chumon = new Chumon();
            ChumonJisseki chumonJisseki = await chumon.ChumonSakusei("A000000001", new DateOnly(2025, 01, 27));

            ChumonJisseki expectedChumonJisseki = new ChumonJisseki {
                ChumonId = "20250127-004",
                ChumonDate = new DateOnly(2025, 1, 27),
                ShiireSakiId = "A000000001"
            };

            List<ChumonJissekiMeisai> expectedChumonJissekiMeisais =
                new List<ChumonJissekiMeisai>() {
                    new ChumonJissekiMeisai() {
                        ChumonId = "20250127-004",
                        ShiireSakiId = "A000000001",
                        ShiirePrdId = "0000000001",
                        ShohinId = "0000000001",
                        ChumonSu = 0,
                        ChumonZan = 0,
                        LastChumonSu = null,
                    },
                    new ChumonJissekiMeisai() {
                        ChumonId = "20250127-004",
                        ShiireSakiId = "A000000001",
                        ShiirePrdId = "0000000002",
                        ShohinId = "0000000002",
                        ChumonSu = 0,
                        ChumonZan = 0,
                        LastChumonSu = null,
                    }

                };

            Assert.IsNotNull(chumonJisseki, "ChumonJisseki is  null.");
            Assert.AreEqual(expectedChumonJisseki.ChumonId, chumonJisseki.ChumonId);
            Assert.AreEqual(expectedChumonJisseki.ChumonDate, chumonJisseki.ChumonDate);
            Assert.AreEqual(expectedChumonJisseki.ShiireSakiId, chumonJisseki.ShiireSakiId);

            Assert.IsNotNull(chumonJisseki.ChumonJissekiMeisais, "ChumonJissekiMeisai is  null.");

            int i = 0;
            foreach (var aMeisai in chumonJisseki.ChumonJissekiMeisais) {
                Assert.AreEqual(expectedChumonJissekiMeisais[i].ChumonId, aMeisai.ChumonId);
                Assert.AreEqual(expectedChumonJissekiMeisais[i].ShiireSakiId, aMeisai.ShiireSakiId);
                Assert.AreEqual(expectedChumonJissekiMeisais[i].ShiirePrdId, aMeisai.ShiirePrdId);
                Assert.AreEqual(expectedChumonJissekiMeisais[i].ShohinId, aMeisai.ShohinId);
                Assert.AreEqual(expectedChumonJissekiMeisais[i].ChumonSu, aMeisai.ChumonSu);
                Assert.AreEqual(expectedChumonJissekiMeisais[i].ChumonZan, aMeisai.ChumonZan);
                Assert.AreEqual(expectedChumonJissekiMeisais[i].LastChumonSu, aMeisai.LastChumonSu);
                i++;
            }
        }
        [TestMethod]
        public async Task ExceptionCheck() {
            Chumon chumon = new Chumon();
            await Assert.ThrowsExceptionAsync<ArgumentException>(
            async () => await chumon.ChumonSakusei(null, new DateOnly(2025, 02, 19)),
            "ShiireSakiId が null の場合に ArgumentException が発生することを期待"
        );
            await Assert.ThrowsExceptionAsync<ArgumentException>(
            async () => await chumon.ChumonSakusei("A000000001", new DateOnly(1,1,1)),
            "注文日が null の場合に ArgumentException が発生することを期待"
        );
            await Assert.ThrowsExceptionAsync<NoDataFoundException>(
                async () => await chumon.ChumonSakusei("A000000004", new DateOnly(2025, 02, 19)),
                "仕入マスタがない場合に、ArgumentException が発生することを期待"
        );

        }

        [TestMethod]
        public async Task Moq() {
            var dummyData = new List<ChumonJisseki>
            {
                new ChumonJisseki { ChumonId = "20250219-001", ShiireSakiId = "A000000001" },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<ChumonJisseki>>();

            // IQueryable をまとめて設定
            mockSet.As<IQueryable<ChumonJisseki>>().Setup(m => m.Provider).Returns(dummyData.Provider);
            mockSet.As<IQueryable<ChumonJisseki>>().Setup(m => m.Expression).Returns(dummyData.Expression);
            mockSet.As<IQueryable<ChumonJisseki>>().Setup(m => m.ElementType).Returns(dummyData.ElementType);
            mockSet.As<IQueryable<ChumonJisseki>>().Setup(m => m.GetEnumerator()).Returns(dummyData.GetEnumerator());

            // IAsyncQueryProvider の設定
            var mockAsyncProvider = new Mock<IAsyncQueryProvider>();
            mockAsyncProvider.Setup(m => m.CreateQuery<ChumonJisseki>(It.IsAny<Expression>())).Returns(dummyData);
            mockSet.As<IQueryable<ChumonJisseki>>().Setup(m => m.Provider).Returns(mockAsyncProvider.Object);

            // ToListAsync() で呼ばれる ExecuteAsync をセットアップ
            //mockAsyncProvider.Setup(m => m.ExecuteAsync<It.IsAnyType>(It.IsAny<Expression>(), It.IsAny<CancellationToken>()))
            //    .Returns(Task.FromResult(dummyData.ToList())); // dummyData を Task<List<T>> で包んで返す

            mockSet.As<IQueryable<ChumonJisseki>>().Setup(m => m.Provider).Returns(mockAsyncProvider.Object);


            var mockContext = new Mock<ConvenienceContext>();
            mockContext.Setup(c => c.ChumonJisseki).Returns(mockSet.Object);

            //Chumon chumon = new Chumon(mockContext.Object);

            //var x = await chumon.ChumonSakusei("A000000001", new DateOnly(2025, 02, 19));

            var xx = await mockContext.Object.ChumonJisseki.ToListAsync();
        }
    }
}
