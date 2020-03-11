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
        public class AccessTokenData
        {
            public int Id { get; set; }
            public string AppId { get; set; }
            public int TotalSeconds { get; set; }
            public double LeftPercent { get; set; }
            public string Status { get; set; }
        }

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

        public async Task<IActionResult> OnGetAccessTokenStatusAsync(int[] ids)
        {
            var data = new List<AccessTokenData>();
            var allMpAccounts = await _mpAccountService.GetFullListAsync(z => true);
            foreach (var id in ids)
            {
                var mpAccount = allMpAccounts.FirstOrDefault(z => z.Id == id);
                if (mpAccount == null)
                {
                    continue;
                }
                var appId = mpAccount.AppId;
                string status = null;
                double leftSeconds = 0;
                AccessTokenBag bag = null;
                if (await AccessTokenContainer.CheckRegisteredAsync(appId))
                {
                    bag = await AccessTokenContainer.TryGetItemAsync(appId);
                    if (bag.AccessTokenResult != null && !bag.AccessTokenResult.access_token.IsNullOrEmpty())
                    {
                        leftSeconds = (bag.AccessTokenExpireTime - SystemTime.Now).TotalSeconds;
                        if (leftSeconds > 0)
                        {
                            status = "有效";
                        }
                        else
                        {
                            leftSeconds = 0;
                            status = "已过期";
                        }
                    }
                    else
                    {
                        status = "无效";
                    }

                }
                else
                {
                    status = "未注册";
                }
                data.Add(new AccessTokenData()
                {
                    AppId = appId,
                    LeftPercent = bag?.AccessTokenResult != null ? Math.Round(leftSeconds / bag.AccessTokenResult.expires_in, 1) : 0,
                    TotalSeconds = bag?.AccessTokenResult.expires_in ?? 0,
                });
            }
            return new JsonResult(data);
        }
    }
}
