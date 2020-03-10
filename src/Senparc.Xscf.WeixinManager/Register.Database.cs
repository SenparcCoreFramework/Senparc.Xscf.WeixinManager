using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Senparc.Xscf.WeixinManager.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Senparc.Xscf.WeixinManager
{
	public partial class Register
	{

        #region IXscfDatabase 接口

        public void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserTag_WeixinUserConfigurationMapping());
            modelBuilder.ApplyConfiguration(new WeixinUserConfigurationMapping());
            modelBuilder.ApplyConfiguration(new UserTagConfigurationMapping());
        }

        public void AddXscfDatabaseModule(IServiceCollection services)
        {
            services.AddScoped<MpAccount>();
            services.AddScoped<MpAccountDto>();
            services.AddScoped<WeixinUser>();
            services.AddScoped<WeixinUserDto>();
        }

        public const string DATABASE_PREFIX = "WeixinManager_";

        public string DatabaseUniquePrefix => DATABASE_PREFIX;

        public Type XscfDatabaseDbContextType => typeof(WeixinSenparcEntities);

        #endregion
    }
}
