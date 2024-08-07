﻿using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Convenience.Models.Services;
using Convenience.Models.ViewModels.Shiire;
using Microsoft.AspNetCore.Mvc;
using static Convenience.Models.Properties.Message;

namespace Convenience.Controllers {

    public class ShiireController : Controller {
        private readonly ConvenienceContext _context;

        private readonly IShiireService shiireService;

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="context">DBコンテキスト</param>
        /// <param name="shiireService">仕入サービスクラスＤＩ注入用</param>
        public ShiireController(ConvenienceContext context,IShiireService shiireService) {
            this._context = context;
            this.shiireService = shiireService;
            //shiireService = new ShiireService(_context);
        }

        public async Task<IActionResult> ShiireKeyInput() {
            ShiireKeysViewModel keymodel = await shiireService.SetShiireKeysModel();
            return View(keymodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShiireKeyInput(ShiireKeysViewModel inKeysModel) {
            string inChumonId = inKeysModel.ChumonId;
            (int, IList<ShiireJisseki>) resultTuple = await shiireService.ShiireHandling(inChumonId);

            //resultTuple.Item2 = resultTuple.Item2.OrderBy(s => new { s.ShiireSakiId, s.ShiirePrdId, s.ShohinId }).ToList();

            List<ShiireJisseki> listdt = (List<ShiireJisseki>)resultTuple.Item2;
            listdt.Sort((x, y) => {
                int result = (x.ShiireSakiId != y.ShiireSakiId) ? x.ShiireSakiId.CompareTo(y.ShiireSakiId) :
                              (x.ShiirePrdId != y.ShiirePrdId) ? x.ShiirePrdId.CompareTo(y.ShiirePrdId) :
                              x.ShohinId.CompareTo(y.ShohinId);
                return result;
            });

            ShiireViewModel shiireViewModel = SetShiireModel(resultTuple.Item1, listdt);

            ViewBag.HandlingFlg = "FirstDisplay";
            return View("Shiire", shiireViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Shiire(ShiireViewModel inShiireViewModel) {
            ModelState.Clear();

            string inChumonId = inShiireViewModel.ChumonId;
            DateOnly inShiireDate = inShiireViewModel.ShiireDate;
            uint inSeqByShiireDate = inShiireViewModel.SeqByShiireDate;
            IList<ShiireJisseki> inShiireJissekis = inShiireViewModel.ShiireJissekis;

            var resultTuple = await shiireService.ShiireHandling(inChumonId, inShiireDate, inSeqByShiireDate, inShiireJissekis);

            List<ShiireJisseki> listdt = (List<ShiireJisseki>)resultTuple.Item2;

            listdt.Sort((x, y) => {
                int result = (x.ShiireSakiId != y.ShiireSakiId) ? x.ShiireSakiId.CompareTo(y.ShiireSakiId) :
                              (x.ShiirePrdId != y.ShiirePrdId) ? x.ShiirePrdId.CompareTo(y.ShiirePrdId) :
                              x.ShohinId.CompareTo(y.ShohinId);
                return result;
            });

            ShiireViewModel shiireViewModel = SetShiireModel(resultTuple.Item1, listdt);

            return View("Shiire", shiireViewModel);
        }

        public ShiireViewModel SetShiireModel(int entities, IList<ShiireJisseki> inshiireJissekis) {
            ShiireJisseki shiireJisseki = inshiireJissekis.FirstOrDefault();

            ShiireViewModel shiireViewModel = new ShiireViewModel {
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
            ViewBag.HandlingFlg = "SecondDisplay";
            return (shiireViewModel);
        }
    }
}