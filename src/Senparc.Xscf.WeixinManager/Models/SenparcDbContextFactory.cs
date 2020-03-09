using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Senparc.CO2NET.Cache;
using Senparc.CO2NET.RegisterServices;
using Senparc.Scf.Core.Config;
using Senparc.Scf.Core.Models;
using Senparc.Scf.XscfBase.Database;
using System;
using System.IO;

namespace Senparc.Xscf.WeixinManager
{
    /// <summary>
    /// 设计时 DbContext 创建（仅在开发时创建 Code-First 的数据库 Migration 使用，在生产环境不会执行）
    /// </summary>
    public class SenparcDbContextFactory : SenparcDesignTimeDbContextFactoryBase<WeixinSenparcEntities, Register>
    {

        public SenparcDbContextFactory()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddMemoryCache();//使用本地缓需要添加
            services.Add(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));//使用 Memcached 或 Logger 需要添加

            //Senparc.CO2NET 全局注册（必须）
            services.AddSenparcGlobalServices(new  Configuration());

            services.AddMemoryCache();
            Senparc.CO2NET.SenparcDI.GlobalServiceCollection = services;
        }

        /// <summary>
        /// 用于寻找 App_Data 文件夹，从而找到数据库连接字符串配置信息
        /// </summary>
        public override string RootDictionaryPath => Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\"/*项目根目录*/);
    }
}
