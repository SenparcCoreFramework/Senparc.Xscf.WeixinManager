using Senparc.Scf.Core.Models;
using Senparc.Scf.Service;
using Senparc.Xscf.WeixinManager.Models;
using Senparc.Xscf.WeixinManager.Models.VD.Admin;
using System;
using System.Linq;
using System.Threading.Tasks;

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
        }
    }
}
