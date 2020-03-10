using Senparc.Scf.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Senparc.Xscf.WeixinManager.Models
{
    //关于 EF 多对多的做法：https://www.entityframeworktutorial.net/efcore/configure-many-to-many-relationship-in-ef-core.aspx

    /// <summary>
    /// UserTag - WeixinUser 多对多关联表
    /// </summary>

    [Table(Register.DATABASE_PREFIX + nameof(UserTag_WeixinUser))]//必须添加前缀，防止全系统中发生冲突
    [Serializable]
    public class UserTag_WeixinUser : EntityBase
    {
        public int UserTagId { get; private set; }
        public UserTag UserTag { get; private set; }

        public int WeixinUserId { get; private set; }
        public WeixinUser WeixinUser { get; private set; }
    }
}
