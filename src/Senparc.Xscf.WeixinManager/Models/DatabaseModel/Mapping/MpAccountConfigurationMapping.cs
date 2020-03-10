using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Senparc.Scf.Core.Models.DataBaseModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Senparc.Xscf.WeixinManager.Models
{
    public class MpAccountConfigurationMapping : ConfigurationMappingWithIdBase<MpAccount, int>
    {
        public override void Configure(EntityTypeBuilder<MpAccount> builder)
        {
            //builder.HasMany(z => z.WeixinUsers).WithOne(z => z.MpAccount);
        }
    }
}
