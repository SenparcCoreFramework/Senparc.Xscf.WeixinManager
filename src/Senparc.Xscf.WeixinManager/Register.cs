﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Senparc.CO2NET.RegisterServices;
using Senparc.Scf.Core.Enums;
using Senparc.Scf.Core.Models;
using Senparc.Scf.Service;
using Senparc.Scf.XscfBase;
using Senparc.Weixin.MP.Containers;
using Senparc.Xscf.WeixinManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Senparc.Xscf.WeixinManager
{
    public partial class Register : XscfRegisterBase, IXscfRegister //注册 XSCF 基础模块接口（必须）
    {
        #region IXscfRegister 接口

        public override string Name => "Senparc.Xscf.WeixinManager";

        public override string Uid => "EB84CB21-AC22-406E-0001-000000000001";


        public override string Version => "0.2.1-beta1";


        public override string MenuName => "微信管理";


        public override string Icon => "fa fa-weixin";


        public override string Description => @"SCF 模块：盛派官方发布的微信管理后台
使用此插件可以在 SCF 中快速集成微信公众号、小程序的部分基础管理功能，欢迎大家一起扩展！
微信 SDK 基于 Senparc.Weixin SDK 开发。";

        public override IList<Type> Functions => new Type[] { };


        public override IServiceCollection AddXscfModule(IServiceCollection services, IConfiguration configuration)
        {
            //services.AddScoped<PostModel>(ServiceProvider =>
            //{
            //    //根据条件生成不同的PostModel


            //});

            return base.AddXscfModule(services, configuration);//如果重写此方法，必须调用基类方法
        }

        public async Task InstallOrUpdateAsync(IServiceProvider serviceProvider, InstallOrUpdate installOrUpdate)
        {
            //更新数据库
            await base.MigrateDatabaseAsync<WeixinSenparcEntities>(serviceProvider);
        }

        public async Task UninstallAsync(IServiceProvider serviceProvider, Func<Task> unsinstallFunc)
        {
            //TODO:可以在基础模块里给出选项是否删除

            WeixinSenparcEntities mySenparcEntities = serviceProvider.GetService<WeixinSenparcEntities>();

            //指定需要删除的数据实体

            //注意：这里作为演示，删除了所有的表，实际操作过程中，请谨慎操作，并且按照删除顺序对实体进行排序！
            var dropTableKeys = EntitySetKeys.GetEntitySetInfo(this.XscfDatabaseDbContextType).Keys.ToList();

            //按照删除顺序排序
            var types = new[] { typeof(UserTag_WeixinUser), typeof(UserTag), typeof(WeixinUser), typeof(MpAccount) };
            types.ToList().AddRange(dropTableKeys);
            types = types.Distinct().ToArray();
            await base.DropTablesAsync(serviceProvider, mySenparcEntities, types);

            await base.UninstallAsync(serviceProvider, unsinstallFunc).ConfigureAwait(false);
        }

        private List<MpAccount> _allMpAccounts = null;

        private List<MpAccount> GetAllMpAccounts(IServiceProvider serviceProvider)
        {
            try
            {
                if (_allMpAccounts == null)
                {
                    var mpAccountService = serviceProvider.GetService<ServiceBase<MpAccount>>();
                    _allMpAccounts = mpAccountService.GetFullList(z => z.AppId != null && z.AppId.Length > 0, z => z.Id, OrderingType.Ascending);
                }
                return _allMpAccounts;
            }
            catch
            {
                return new List<MpAccount>();
            }

        }

        public override IApplicationBuilder UseXscfModule(IApplicationBuilder app, IRegisterService registerService)
        {
            try
            {
                //未安装数据库表的情况下可能会出错，因此需要try
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    var allMpAccount = GetAllMpAccounts(scope.ServiceProvider);

                    //批量自动注册公众号
                    foreach (var mpAccount in allMpAccount)
                    {
                        Task.Factory.StartNew(async () =>
                        {
                            await AccessTokenContainer.RegisterAsync(mpAccount.AppId, mpAccount.AppSecret, $"{mpAccount.Name}-{mpAccount.Id}");
                        });
                    }
                }
            }
            catch
            {
            }

            return base.UseXscfModule(app, registerService);
        }

        #endregion
    }
}
