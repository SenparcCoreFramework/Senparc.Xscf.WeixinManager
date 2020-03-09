﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Senparc.Scf.Core.Areas;
using Senparc.Scf.Core.Config;
using Senparc.Scf.Core.Enums;
using Senparc.Scf.XscfBase;
using Senparc.Xscf.WeixinManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Senparc.Xscf.WeixinManager
{
    public class Register : XscfRegisterBase,
        IXscfRegister, //注册 XSCF 基础模块接口（必须）
        IAreaRegister, //注册 XSCF 页面接口（按需选用）
        IXscfDatabase,  //注册 XSCF 模块数据库（按需选用）
        IXscfRazorRuntimeCompilation  //需要使用 RazorRuntimeCompilation，在开发环境下实时更新 Razor Page
    {
        #region IXscfRegister 接口

        public override string Name => "Senparc.Xscf.WeixinManager";

        public override string Uid => "EB84CB21-AC22-406E-0001-000000000001";


        public override string Version => "0.1.1-beta1";


        public override string MenuName => "微信管理";


        public override string Icon => "fa fa-weixin";


        public override string Description => @"SCF 模块：盛派官方发布的微信管理后台
使用此插件可以在 SCF 中快速集成微信公众号、小程序的部分基础管理功能，欢迎大家一起扩展！
微信 SDK 基于 Senparc.Weixin SDK 开发。";

        public override IList<Type> Functions => new Type[] { };


        public IServiceCollection AddXscfModule(IServiceCollection services)
        {
            return services;
        }

        public async Task InstallOrUpdateAsync(IServiceProvider serviceProvider, InstallOrUpdate installOrUpdate)
        {
            //更新数据库
            await base.MigrateDatabaseAsync<WeixinSenparcEntities>(serviceProvider);
        }

        public async Task UninstallAsync(IServiceProvider serviceProvider, Func<Task> unsinstallFunc)
        {
            //TODO:可以在基础模块里给出选项是否删除
        }

        #endregion


        #region IAreaRegister 接口

        public string HomeUrl => "/Admin/WeixinManager/Index";

        public List<AreaPageMenuItem> AareaPageMenuItems => new List<AreaPageMenuItem>() {
             new AreaPageMenuItem(GetAreaUrl("/Admin/WeixinManager/Index"),"首页","fa fa-laptop"),
             new AreaPageMenuItem(GetAreaUrl("/Admin/WeixinManager/User"),"用户列表","fa fa-users"),
        };

        public IMvcBuilder AuthorizeConfig(IMvcBuilder builder, IWebHostEnvironment env)
        {
            return builder;
        }

        #endregion

        #region IXscfDatabase 接口

        public void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        public void AddXscfDatabaseModule(IServiceCollection services)
        {
            services.AddScoped<MpAccount>();
            services.AddScoped<MpAccountDto>();
        }

        public const string DATABASE_PREFIX = "WeixinManager_";

        public string DatabaseUniquePrefix => DATABASE_PREFIX;

        public Type XscfDatabaseDbContextType => typeof(WeixinSenparcEntities);

        #endregion

        #region IXscfRazorRuntimeCompilation 接口
        
        public string LibraryPath => Path.Combine(SiteConfig.WebRootPath, "..", "..", "..", "Senparc.Xscf.WeixinManager", "src", "Senparc.Xscf.WeixinManager");

        #endregion
    }
}