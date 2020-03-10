using Senparc.Scf.Repository;
using Senparc.Scf.Service;
using Senparc.Xscf.WeixinManager.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Senparc.Xscf.WeixinManager.Services
{
    public class MpAccountService : ServiceBase<MpAccount>,IServiceBase<MpAccount>
    {
        public MpAccountService(IRepositoryBase<MpAccount> repo, IServiceProvider serviceProvider) : base(repo, serviceProvider)
        {
        }
    }
}
