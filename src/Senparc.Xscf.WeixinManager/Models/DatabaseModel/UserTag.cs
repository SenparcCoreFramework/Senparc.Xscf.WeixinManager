using Senparc.Scf.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Senparc.Xscf.WeixinManager.Models
{
    [Table(Register.DATABASE_PREFIX + nameof(UserTag))]//必须添加前缀，防止全系统中发生冲突
    [Serializable]
    public class UserTag : EntityBase<int>
    {
        public string Name { get; private set; }
        public int Count { get; private set; }

        private UserTag() { }

        public IList<UserTag_WeixinUser> UserTags_WeixinUsers { get; private set; }
    }
}
