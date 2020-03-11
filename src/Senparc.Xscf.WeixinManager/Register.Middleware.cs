using Microsoft.AspNetCore.Builder;
using Senparc.Scf.Core.Config;
using Senparc.Scf.XscfBase;
using System.IO;

namespace Senparc.Xscf.WeixinManager
{
	public partial class Register : IXscfMiddleware  //需要引入中间件的模块
	{
		#region IXscfMiddleware 接口

		public IApplicationBuilder UseMiddleware(IApplicationBuilder app)
		{
			return app;
		}

		#endregion

	}
}
