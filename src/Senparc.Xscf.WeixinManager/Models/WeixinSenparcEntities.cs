using Microsoft.EntityFrameworkCore;
using Senparc.Scf.Core.Models;
using Senparc.Scf.XscfBase;
using Senparc.Scf.XscfBase.Database;
using Senparc.Xscf.WeixinManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.Xscf.WeixinManager
{
    public class WeixinSenparcEntities : XscfDatabaseDbContext
    {
        public DbSet<MpAccount> MpAccounts { get; set; }
        public DbSet<WeixinUser> WeixinUsers { get; set; }
        public DbSet<UserTag> UserTags { get; set; }
        public DbSet<UserTag_WeixinUser> UserTags_WeixinUsers { get; set; }

        public WeixinSenparcEntities(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

        public override IXscfDatabase XscfDatabaseRegister => new Register();
    }
}
