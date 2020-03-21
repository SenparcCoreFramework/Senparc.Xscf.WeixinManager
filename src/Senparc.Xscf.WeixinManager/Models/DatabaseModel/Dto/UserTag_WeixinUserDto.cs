﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Senparc.Xscf.WeixinManager.Models
{
    public class UserTag_WeixinUserDto
    {
        public int UserTagId { get; private set; }
        public UserTag_CreateOrUpdateDto UserTag { get; private set; }

        public int WeixinUserId { get; private set; }
        public WeixinUserDto WeixinUser { get; private set; }

        private UserTag_WeixinUserDto() { }
    }
}
