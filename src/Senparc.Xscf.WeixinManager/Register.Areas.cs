using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Senparc.Scf.Core.Areas;
using System;
using System.Collections.Generic;
using System.Text;

namespace Senparc.Xscf.WeixinManager
{
	public partial class Register
	{
		#region IAreaRegister 接口

		public string HomeUrl => "/Admin/WeixinManager/Index";

		public List<AreaPageMenuItem> AareaPageMenuItems => new List<AreaPageMenuItem>() {
			 new AreaPageMenuItem(GetAreaUrl("/Admin/WeixinManager/Index"),"首页","fa fa-laptop"),
			 new AreaPageMenuItem(GetAreaUrl("/Admin/WeixinManager/MpAccount"),"公众号管理","fa fa-users"),
			 new AreaPageMenuItem(GetAreaUrl("/Admin/WeixinManager/WeixinUser"),"用户管理","fa fa-comments"),
		};

		public IMvcBuilder AuthorizeConfig(IMvcBuilder builder, IWebHostEnvironment env)
		{
			return builder;
		}

		#endregion
	}
}
