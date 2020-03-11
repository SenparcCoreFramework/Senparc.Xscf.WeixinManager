﻿using AutoMapper;
using Senparc.Scf.Core.Models;
using Senparc.Scf.Core.Models.DataBaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Senparc.Xscf.WeixinManager.Models
{

    public class MpAccountDto : MpAccount_CreateOrUpdateDto
    {
        //[IgnoreMap]
        public int Id { get; set; }
    }

    public class MpAccount_CreateOrUpdateDto : DtoBase
    {
        [MaxLength(200)]
        public string Logo { get; set; }
        [Required]
        [MaxLength(100)]
        [Display(Name = "公众号名称")]
        public string Name { get; set; }
        [Required]
        [MaxLength(100)]
        public string AppId { get; set; }
        [Required]
        [MaxLength(100)]
        public string AppSecret { get; set; }
        [Required]
        [MaxLength(500)]
        public string Token { get; set; }
        [MaxLength(500)]
        public string EncodingAESKey { get; set; }

        public IList<WeixinUser> WeixinUsers { get; set; }
    }
}
