using Microsoft.EntityFrameworkCore;
using Senparc.Scf.XscfBase;
using Senparc.Scf.XscfBase.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace Senparc.Xscf.WeixinManager
{
    public class WeixinSenparcEntities : XscfDatabaseDbContext
    {
        public WeixinSenparcEntities(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

        public override IXscfDatabase XscfDatabaseRegister => new Register();
    }
}
