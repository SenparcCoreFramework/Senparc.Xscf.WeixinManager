using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Senparc.CO2NET.Extensions;
using Senparc.Scf.Service;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.MP.Entities;
using Senparc.Xscf.WeixinManager.Models;
using Senparc.Xscf.WeixinManager.Models.VD.Admin;

namespace Senparc.Xscf.WeixinManager.Areas.Admin.WeixinManager
{
    public class IndexModel : BaseAdminWeixinManagerModel
    {
        public List<MpAccountDto> MpAccountDtos { get; set; }
        public int RegisteredMpAccountCount { get; set; }
        public int WeixinUserCount { get; set; }

        public List<AccessTokenBag> AccessTokenBags { get; set; }

        private readonly ServiceBase<Models.MpAccount> _mpAccountService;
        private readonly ServiceBase<Models.WeixinUser> _weixinUserService;
        public IndexModel(Lazy<XscfModuleService> xscfModuleService,
            ServiceBase<Models.MpAccount> mpAccountService,
            ServiceBase<Models.WeixinUser> weixinUserService
            ) : base(xscfModuleService)
        {
            _mpAccountService = mpAccountService;
            _weixinUserService = weixinUserService;
        }

        public async Task OnGetAsync()
        {
            var allMpAccounts = await _mpAccountService.GetFullListAsync(z => true);

            MpAccountDtos = allMpAccounts.Select(z => _mpAccountService.Mapper.Map<MpAccountDto>(z)).ToList();

            AccessTokenBags = new List<AccessTokenBag>();
            RegisteredMpAccountCount = 0;
            foreach (var mpAccount in allMpAccounts)
            {
                if (await AccessTokenContainer.CheckRegisteredAsync(mpAccount.AppId))
                {
                    var bag = await AccessTokenContainer.TryGetItemAsync(mpAccount.AppId);
                    if (bag.AccessTokenResult != null && !bag.AccessTokenResult.access_token.IsNullOrEmpty())
                    {
                        RegisteredMpAccountCount++;
                    }
                    AccessTokenBags.Add(bag);
                }
            }

            WeixinUserCount = await _weixinUserService.GetCountAsync(z => true);
        }
    }
}
