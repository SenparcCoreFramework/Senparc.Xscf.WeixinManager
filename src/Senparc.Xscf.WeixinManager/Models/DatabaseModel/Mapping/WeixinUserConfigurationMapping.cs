using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Senparc.Scf.Core.Models.DataBaseModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Senparc.Xscf.WeixinManager.Models
{
    public class WeixinUserConfigurationMapping : ConfigurationMappingWithIdBase<WeixinUser, int>
    {
        public override void Configure(EntityTypeBuilder<WeixinUser> builder)
        {
            builder.HasOne(z => z.MpAccount)
                .WithMany(z => z.WeixinUsers)
                .HasForeignKey(z => z.MpAccountId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName($"FK__{nameof(WeixinUser)}__MpAccountId");
        }
    }
}
