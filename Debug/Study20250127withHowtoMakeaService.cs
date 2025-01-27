using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Convenience.Models.ViewModels.Chumon;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debug {
    internal class Study20250127withHowtoMakeaService : IDbContext {

        public ChumonViewModel ChumonViewModel { get; set; }

        public readonly ConvenienceContext _context;
        public readonly IChumon _chumon;

        public Study20250127withHowtoMakeaService() {
            _context = IDbContext.DbOpen();
            _chumon = new Chumon();
        }

        public async Task<ChumonKeysViewModel> SetChumonKeysViewModel() {
            return new ChumonKeysViewModel() {
                ShiireSakiId = null,
                ChumonDate = DateOnly.FromDateTime(DateTime.Now),
                ShiireSakiList = await _chumon.ShiireSakiList(x => x.ShiireSakiId)
                    .Select(x => new SelectListItem() {
                        Value = x.ShiireSakiId,
                        Text = $"{x.ShiireSakiId}:{x.ShiireSakiKaisya}"
                    }).ToListAsync()
            };
        }

        public async Task<ChumonViewModel> ChumonSetting(ChumonKeysViewModel inChumonKeysViewModel) {
            string inShiireSakiId = inChumonKeysViewModel.ShiireSakiId;
            DateOnly inChumonDate = inChumonKeysViewModel.ChumonDate;

            ChumonJisseki chumonJisseki = await _chumon.ChumonToiawase(inShiireSakiId, inChumonDate);

            if (chumonJisseki == null) {
                chumonJisseki = await _chumon.ChumonSakusei(inShiireSakiId, inChumonDate);
            }

            return new ChumonViewModel() {
                ChumonJisseki = chumonJisseki,
                IsNormal = true,
                Remark = string.Empty
            };
        }

        public async Task<ChumonViewModel> ChumonUpdate(ChumonViewModel inChumonViewModel) {
            string inShiireSakiId = inChumonViewModel.ChumonJisseki.ShiireSakiId;
            DateOnly inChumonDate = inChumonViewModel.ChumonJisseki.ChumonDate;

            ChumonKeysViewModel aChumonKeysViewModel = new ChumonKeysViewModel() {
                ShiireSakiId = inShiireSakiId,
                ChumonDate = inChumonDate,
            };
            ChumonViewModel = await ChumonSetting(aChumonKeysViewModel);

            _chumon.ChumonJisseki = ChumonViewModel.ChumonJisseki;

            ChumonJisseki chumonJisseki = await _chumon.ChumonUpdate(inChumonViewModel.ChumonJisseki);

            int entitues = await _chumon.ChumonSaveChanges();
            (bool isNormal, string remark) = entitues > 0 ? (true, "更新しました") : (true, string.Empty);

            return new ChumonViewModel() {
                ChumonJisseki = chumonJisseki,
                IsNormal = isNormal,
                Remark = remark
            };

        }
        public async Task<ChumonViewModel> ChumonUpdate(ChumonViewModel inChumonViewModel) {
        }
        }
}
