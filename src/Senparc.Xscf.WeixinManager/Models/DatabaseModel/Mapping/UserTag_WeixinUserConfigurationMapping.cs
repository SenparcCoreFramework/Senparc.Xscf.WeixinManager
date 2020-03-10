using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Senparc.Scf.Core.Models.DataBaseModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Senparc.Xscf.WeixinManager.Models
{
    public class UserTag_WeixinUserConfigurationMapping : ConfigurationMappingWithIdBase<UserTag_WeixinUser>
    {
        public override void Configure(EntityTypeBuilder<UserTag_WeixinUser> builder)
        {
            base.Configure(builder);

            builder.HasKey(uw => new { uw.UserTagId, uw.MpAccountId });
        }
    }
}
