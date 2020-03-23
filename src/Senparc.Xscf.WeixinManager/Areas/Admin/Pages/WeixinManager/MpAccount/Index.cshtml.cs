using Microsoft.AspNetCore.Mvc;
using Senparc.Scf.Core.Models;
using Senparc.Scf.Service;
using Senparc.Weixin.MP.Containers;
using Senparc.Xscf.WeixinManager.Models;
using Senparc.Xscf.WeixinManager.Models.VD.Admin;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Senparc.Xscf.WeixinManager.Areas.Admin.WeixinManager
{
    public class MpAccount_IndexModel : BaseAdminWeixinManagerModel
    {
        public PagedList<MpAccountDto> MpAccountDtos { get; set; }

        private readonly ServiceBase<Models.MpAccount> _mpAccountService;
        private int pageCount = 20;


        public MpAccount_IndexModel(Lazy<XscfModuleService> xscfModuleService, ServiceBase<Models.MpAccount> mpAccountService) : base(xscfModuleService)
        {
            _mpAccountService = mpAccountService;
        }

        public async Task OnGetAsync(int pageIndex = 1)
        {
            var result = await _mpAccountService.GetObjectListAsync(pageIndex, pageCount, z => true, z => z.Id, Scf.Core.Enums.OrderingType.Descending);
            MpAccountDtos = new PagedList<MpAccountDto>(result.Select(z => _mpAccountService.Mapper.Map<MpAccountDto>(z)).ToList(), result.PageIndex, result.PageCount, result.TotalCount);
            
            //测试，将用户加入某个组
            //await Senparc.Weixin.MP.AdvancedAPIs.UserTagApi.BatchTaggingAsync(MpAccountDtos[0].AppId, 2, new System.Collections.Generic.List<string> { "oxRg0uLsnpHjb8o93uVnwMK_WAVw" });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int[] ids)
        {
            foreach (var id in ids)
            {
                var mpAccount = await _mpAccountService.GetObjectAsync(z => z.Id == id);
                if (mpAccount != null)
                {
                    await _mpAccountService.DeleteObjectAsync(mpAccount);
                    await AccessTokenContainer.RemoveFromCacheAsync(mpAccount.AppId);//清除注册状态
                }
            }
            return RedirectToPage("./Index",new { Uid });
        }
    }
}

