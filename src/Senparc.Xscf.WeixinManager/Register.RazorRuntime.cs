using Senparc.Scf.Core.Config;
using Senparc.Scf.XscfBase;
using System.IO;

namespace Senparc.Xscf.WeixinManager
{
	public partial class Register : IXscfRazorRuntimeCompilation  //需要使用 RazorRuntimeCompilation，在开发环境下实时更新 Razor Page
	{
		#region IXscfRazorRuntimeCompilation 接口

		public string LibraryPath => Path.Combine(SiteConfig.WebRootPath, "..", "..", "..", "Senparc.Xscf.WeixinManager", "src", "Senparc.Xscf.WeixinManager");

		#endregion
	}
}
