using Senparc.Scf.Core.Models.DataBaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Senparc.Xscf.WeixinManager.Models
{
    public class MpAccountDto : DtoBase
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; private set; }
        [Required]
        [MaxLength(100)]
        public string AppId { get; private set; }
        [Required]
        [MaxLength(100)]
        public string AppSecret { get; private set; }
        [Required]
        [MaxLength(500)]
        public string Token { get; private set; }
        [MaxLength(500)]
        public string EncodingAESKey { get; private set; }

        private MpAccountDto() { }

    }
}
