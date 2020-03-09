using Senparc.Scf.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Senparc.Xscf.WeixinManager.Models.VD.Admin
{
    public class BaseAdminWeixinManagerModel : Senparc.Scf.AreaBase.Admin.AdminXscfModulePageModelBase
    {
        public BaseAdminWeixinManagerModel(Lazy<XscfModuleService> xscfModuleService) : base(xscfModuleService)
        {
        }
    }
}
