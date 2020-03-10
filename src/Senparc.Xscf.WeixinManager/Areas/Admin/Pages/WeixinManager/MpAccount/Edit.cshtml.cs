using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Senparc.Scf.Service;
using Senparc.Xscf.WeixinManager.Models;
using Senparc.Xscf.WeixinManager.Models.VD.Admin;

namespace Senparc.Xscf.WeixinManager.Areas.Admin.Pages.WeixinManager
{
    public class MpAccount_EditModel : BaseAdminWeixinManagerModel
    {
        [ModelBinder]
        public MpAccountDto MpAccountDto { get; set; }
        private readonly ServiceBase<MpAccount> _mpAccountService;

        public MpAccount_EditModel(Lazy<XscfModuleService> xscfModuleService,
                                   ServiceBase<MpAccount> mpAccountService) : base(xscfModuleService)
        {
            _mpAccountService = mpAccountService;
        }

        public bool IsEdit { get; set; }

        public async Task<IActionResult> OnGetAsync(int id = 0)
        {
            IsEdit = id > 0;
            if (IsEdit)
            {
                var mpAccount = await _mpAccountService.GetObjectAsync(z => z.Id == id);
                if (mpAccount == null)
                {
                    return RenderError("公众号信息不存在！");
                }

                MpAccountDto = _mpAccountService.Mapper.Map<MpAccountDto>(mpAccount);
            }
            else
            {
                MpAccountDto = new MpAccountDto();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id = 0)
        {
            IsEdit = id > 0;
            if (IsEdit)
            {
                var mpAccount = await _mpAccountService.GetObjectAsync(z => z.Id == id);
                if (mpAccount == null)
                {
                    return RenderError("公众号信息不存在！");
                }
                _mpAccountService.Mapper.Map(MpAccountDto, mpAccount);
                _mpAccountService.SaveObject(mpAccount);
            }
            else
            {
                var mpAccount = new MpAccount(MpAccountDto, _mpAccountService.Mapper);
                _mpAccountService.SaveObject(mpAccount);
            }
            return Page();
        }
    }
}
