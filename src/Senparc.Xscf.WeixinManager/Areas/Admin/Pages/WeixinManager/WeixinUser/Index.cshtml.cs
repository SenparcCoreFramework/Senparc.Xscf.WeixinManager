using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Senparc.CO2NET.Extensions;
using Senparc.Scf.Core.Models;
using Senparc.Scf.Service;
using Senparc.Scf.Utility;
using Senparc.Xscf.WeixinManager.Models;
using Senparc.Xscf.WeixinManager.Models.VD.Admin;

namespace Senparc.Xscf.WeixinManager.Areas.Admin.WeixinManager
{
    public class WeixinUser_IndexModel : BaseAdminWeixinManagerModel
    {
        public MpAccountDto MpAccountDto { get; set; }
        public PagedList<WeixinUserDto> WeixinUserDtos { get; set; }

        private readonly ServiceBase<Models.MpAccount> _mpAccountService;
        private readonly ServiceBase<Models.WeixinUser> _weixinUserService;
        private int pageCount = 20;

        public WeixinUser_IndexModel(Lazy<XscfModuleService> xscfModuleService,
            ServiceBase<Models.MpAccount> mpAccountService, ServiceBase<Models.WeixinUser> weixinUserService)
            : base(xscfModuleService)
        {
            _mpAccountService = mpAccountService;
            _weixinUserService = weixinUserService;
        }

        public async Task<IActionResult> OnGetAsync(int mpId = 0, int pageIndex = 1)
        {
            if (mpId > 0)
            {
                var mpAccount = await _mpAccountService.GetObjectAsync(z => z.Id == mpId);
                if (mpAccount == null)
                {
                    return RenderError("公众号配置不存在：" + mpId);
                }
                MpAccountDto = _mpAccountService.Mapper.Map<MpAccountDto>(mpAccount);
            }

            var seh = new Scf.Utility.SenparcExpressionHelper<Models.WeixinUser>();
            seh.ValueCompare.AndAlso(MpAccountDto != null, z => z.MpAccountId == MpAccountDto.Id);
            var where = seh.BuildWhereExpression();
            var result = await _weixinUserService.GetObjectListAsync(pageIndex, pageCount, where, z => z.Id, Scf.Core.Enums.OrderingType.Descending);
            WeixinUserDtos = new PagedList<WeixinUserDto>(result.Select(z => _mpAccountService.Mapper.Map<WeixinUserDto>(z)).ToList(), result.PageIndex, result.PageCount, result.TotalCount);
            return Page();
        }

        public async Task<IActionResult> OnGetSyncUserAsync(int mpId)
        {
            var mpAccount = await _mpAccountService.GetObjectAsync(z => z.Id == mpId);
            if (mpAccount == null)
            {
                return RenderError("公众号配置不存在：" + mpId);
            }

            //List<WeixinUserDto> weixinUserDtos = new List<WeixinUserDto>();
            string lastOpenId = null;
            while (true)
            {
                var result = await Senparc.Weixin.MP.AdvancedAPIs.UserApi.GetAsync(mpAccount.AppId, lastOpenId);
                if (result.data!=null)
                {
                    foreach (var openId in result.data.openid)
                    {
                        var user = await Senparc.Weixin.MP.AdvancedAPIs.UserApi.InfoAsync(mpAccount.AppId, openId);
                        var weixinUser = await _weixinUserService.GetObjectAsync(z => z.OpenId == openId);
                        var weixinUserDto = _weixinUserService.Mapper.Map<WeixinUser_UpdateFromApiDto>(user);
                        weixinUserDto.MpAccountId = mpId;
                        if (weixinUser != null)
                        {
                            //TODO:判断并更新 特定的DTO对象
                            //weixinUserDto.Id = weixinUser.Id;
                        }
                        else
                        {
                            //TODO:更新group和tag信息
                            weixinUser = _weixinUserService.Mapper.Map<Models.WeixinUser>(weixinUserDto);
                            await _weixinUserService.SaveObjectAsync(weixinUser);
                        }
                    }
                }
               
                if (result.next_openid.IsNullOrEmpty())
                {
                    break;
                }
                lastOpenId = result.next_openid;
            }
            base.SetMessager(Scf.Core.Enums.MessageType.success, "更新成功！");
            return RedirectToPage("./Index", new { uid = Uid, mpId = mpId });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int[] ids)
        {
            var mpId = 0;
            foreach (var id in ids)
            {
                var weixinUser = await _weixinUserService.GetObjectAsync(z => z.Id == id);
                if (weixinUser != null)
                {
                    mpId = weixinUser.MpAccountId;
                    await _weixinUserService.DeleteObjectAsync(weixinUser);
                }
            }
            return RedirectToPage("./Index",new { Uid, mpId });
        }
    }
}
