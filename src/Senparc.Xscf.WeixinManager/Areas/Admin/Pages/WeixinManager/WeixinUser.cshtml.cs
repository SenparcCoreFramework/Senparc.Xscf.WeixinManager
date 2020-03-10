using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Senparc.Scf.Service;
using Senparc.Xscf.WeixinManager.Models.VD.Admin;

namespace Senparc.Xscf.WeixinManager.Areas.Admin.WeixinManager
{
    public class WeixinUserModel : BaseAdminWeixinManagerModel
    {
        public WeixinUserModel(Lazy<XscfModuleService> xscfModuleService) : base(xscfModuleService)
        {
        }

        public void OnGet()
        {
        }
    }
}
