using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Senparc.CO2NET.Extensions;
using Senparc.CO2NET.Trace;
using Senparc.Scf.Core.Models;
using Senparc.Scf.Service;
using Senparc.Scf.Utility;
using Senparc.Xscf.WeixinManager.Models;
using Senparc.Xscf.WeixinManager.Models.VD.Admin;

namespace Senparc.Xscf.WeixinManager.Areas.Admin.WeixinManager
{
    public class WeixinUser_IndexModel : BaseAdminWeixinManagerModel
    {
        public MpAccountDto MpAccountDto { get; set; }
        public PagedList<WeixinUserDto> WeixinUserDtos { get; set; }

        private readonly ServiceBase<Models.MpAccount> _mpAccountService;
        private readonly ServiceBase<Models.WeixinUser> _weixinUserService;
        private readonly ServiceBase<Models.UserTag> _userTagService;
        private int pageCount = 20;

        public WeixinUser_IndexModel(Lazy<XscfModuleService> xscfModuleService,
            ServiceBase<Models.MpAccount> mpAccountService, ServiceBase<Models.WeixinUser> weixinUserService,
            ServiceBase<Models.UserTag> userTagService
            )
            : base(xscfModuleService)
        {
            _mpAccountService = mpAccountService;
            _weixinUserService = weixinUserService;
            _userTagService = userTagService;
        }

        public async Task<IActionResult> OnGetAsync(int mpId = 0, int pageIndex = 1)
        {
            if (mpId > 0)
            {
                var mpAccount = await _mpAccountService.GetObjectAsync(z => z.Id == mpId);
                if (mpAccount == null)
                {
                    return RenderError("���ں����ò����ڣ�" + mpId);
                }
                MpAccountDto = _mpAccountService.Mapper.Map<MpAccountDto>(mpAccount);
            }

            var seh = new Scf.Utility.SenparcExpressionHelper<Models.WeixinUser>();
            seh.ValueCompare.AndAlso(MpAccountDto != null, z => z.MpAccountId == MpAccountDto.Id);
            var where = seh.BuildWhereExpression();
            var result = await _weixinUserService.GetObjectListAsync(pageIndex, pageCount, where, z => z.Id, Scf.Core.Enums.OrderingType.Descending, new[] { nameof(Models.WeixinUser.UserTags_WeixinUsers) });

            ViewData["Test"] = result.FirstOrDefault();
            WeixinUserDtos = new PagedList<WeixinUserDto>(result.Select(z => _mpAccountService.Mapper.Map<WeixinUserDto>(z)).ToList(), result.PageIndex, result.PageCount, result.TotalCount);
            return Page();
        }

        public enum SyncType
        {
            /// <summary>
            /// ֻ����δ��ӵ��û�
            /// </summary>
            add,
            /// <summary>
            /// ����δ��ӵ��û���ͬʱ����������Ϣ��ʱ���ϳ���
            /// </summary>
            all
        }

        public async Task<IActionResult> OnGetSyncUserAsync(int mpId, SyncType syncType)
        {
            var mpAccount = await _mpAccountService.GetObjectAsync(z => z.Id == mpId);
            if (mpAccount == null)
            {
                return RenderError("���ں����ò����ڣ�" + mpId);
            }

            SenparcTrace.SendCustomLog("��ʼ���ں��û�ͬ��", mpAccount.Name);
            //List<WeixinUserDto> weixinUserDtos = new List<WeixinUserDto>();
            string lastOpenId = null;
            List<string> openIds = new List<string>();
            while (true)
            {
                var result = await Senparc.Weixin.MP.AdvancedAPIs.UserApi.GetAsync(mpAccount.AppId, lastOpenId);
                if (result.data != null)
                {
                    SenparcTrace.SendCustomLog("��ȡ��OpenId", $"{result.data.openid.Count} ��");
                    openIds.AddRange(result.data.openid);
                }

                if (result.next_openid.IsNullOrEmpty())
                {
                    break;
                }
                lastOpenId = result.next_openid;
            }

            //����Tag
            var weixinTagDto = new UserTag_CreateOrUpdateDto();
            var allDbUserTags = await _userTagService.GetFullListAsync(z => z.MpAccountId == mpId);
            SenparcTrace.SendCustomLog("��ǰ�Ѿ��洢UserTags", $"{allDbUserTags.Count} ��\r\n{string.Join(",", allDbUserTags.Select(z => z.Name).ToArray())}");

            var tagInfo = await Senparc.Weixin.MP.AdvancedAPIs.UserTagApi.GetAsync(mpAccount.AppId);
            SenparcTrace.SendCustomLog("΢���˺Ŵ洢UerTag", $"{tagInfo.tags.Count} ��\r\n{string.Join(",", tagInfo.tags.Select(z => z.name).ToArray())}");

            //��ӻ���� UserTag
            foreach (var tag in tagInfo.tags)
            {
                var dbUserTag = allDbUserTags.FirstOrDefault(z => z.TagId == tag.id);

                UserTag_CreateOrUpdateDto tagDto;
                tagDto = dbUserTag == null
                    ? _userTagService.Mapper.Map<UserTag_CreateOrUpdateDto>(tag)//������tag
                    : _userTagService.Mapper.Map<UserTag_CreateOrUpdateDto>(dbUserTag)//�����ݿ��ȡ
                    ;

                tagDto.MpAccountId = mpId;
                tagDto.TagId = tag.id;//���Ʋ�һ�£��ֶ����

                var changed = false;

                if (dbUserTag == null)
                {
                    SenparcTrace.SendCustomLog("����UserTag", $"{tagDto.TagId}:{tagDto.Name}");
                    dbUserTag = _userTagService.Mapper.Map<UserTag>(tagDto);
                    //await _userTagService.SaveObjectAsync(dbUserTag);
                    allDbUserTags.Add(dbUserTag);
                    changed = true;
                }
                else
                {
                    SenparcTrace.SendCustomLog("����UserTag", $"{tagDto.TagId}:{tagDto.Name}");
                    changed = dbUserTag.Update(tagDto);
                }

                if (changed)
                {
                    await _userTagService.SaveObjectAsync(dbUserTag);
                }
            }

            //���ɾ���� Tag
            var tobeRemoveTags = allDbUserTags.Where(z => !tagInfo.tags.Exists(t => t.name == z.Name));
            if (tobeRemoveTags.Count() > 0)
            {
                await _userTagService.DeleteAllAsync(tobeRemoveTags);
            }

            var allUsers = await _weixinUserService.GetFullListAsync(z => z.MpAccountId == mpId, null, new[] { nameof(Models.WeixinUser.UserTags_WeixinUsers) });

            ConcurrentBag<Models.WeixinUser> allToSaveWeixinUsers = new ConcurrentBag<Models.WeixinUser>();
            ConcurrentBag<Models.WeixinUser> newWeixinUsers = new ConcurrentBag<Models.WeixinUser>();

            ConcurrentDictionary<int, Task> tasks = new ConcurrentDictionary<int, Task>();
            var maxThreadsCount = 300;//�����ȡ�߳���
            SenparcTrace.SendCustomLog("��ʼͬ���û���Ϣ", $"�� {openIds.Count} ��");
            for (int i = 0; i < openIds.Count; i++)
            {
                var index = i;
                var openId = openIds[index];
                Task task = Task.Run(async () =>
                {
                    var weixinUser = allUsers.FirstOrDefault(z => z.OpenId == openId);
                    if (weixinUser == null || syncType == SyncType.all)
                    {
                        //��Ҫ�������Ը����û�
                        var user = await Senparc.Weixin.MP.AdvancedAPIs.UserApi.InfoAsync(mpAccount.AppId, openId);
                        var weixinUserDto = _weixinUserService.Mapper.Map<WeixinUser_UpdateFromApiDto>(user);
                        weixinUserDto.MpAccountId = mpId;
                        if (weixinUser != null)
                        {
                            //�鿴��������£��������ݽ��жԱ�
                            var oldWeixinUserJson = weixinUserDto.ToJson(false, new Newtonsoft.Json.JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });
                            var newWeixinUserDto = _weixinUserService.Mapper.Map<Models.WeixinUser_UpdateFromApiDto>(weixinUser);
                            var newWeixinUserJson = newWeixinUserDto.ToJson(false, new Newtonsoft.Json.JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });
                            if (oldWeixinUserJson != newWeixinUserJson)
                            {
                                SenparcTrace.SendCustomLog("WeixinUserJson ����", $"�ɣ�{oldWeixinUserJson}\r\n�£�{newWeixinUserJson}");
                                _weixinUserService.Mapper.Map(weixinUserDto, weixinUser);
                            }

                            //��ӻ�ɾ�����˵� Tag
                            foreach (var tag in user.tagid_list)
                            {
                                var userTag = allDbUserTags.FirstOrDefault(z => z.TagId == tag);
                                if (userTag == null)
                                {
                                    SenparcTrace.SendCustomLog("ƥ�䵽δͬ���� TagId", "TagId��" + tag);//��������������
                                }

                                //����δ��ӵ�Tag
                                var userTags_WeixinUsers = weixinUser.UserTags_WeixinUsers.FirstOrDefault(z => /*z.WeixinUserId == weixinUser.Id &&*/ z.UserTagId == userTag.Id);
                                if (userTags_WeixinUsers == null)
                                {
                                    weixinUser.UserTags_WeixinUsers.Add(new UserTag_WeixinUser(weixinUser.Id, tag));
                                }
                            }

                            //������Ҫɾ����Tag
                            var tobeRemoveTagList = weixinUser.UserTags_WeixinUsers.Where(z =>
                                {
                                    var userTag = allDbUserTags.FirstOrDefault(z => z.TagId == z.TagId);
                                    return !user.tagid_list.Contains(userTag.TagId);
                                });

                            foreach (var userTags_WeixinUsers in tobeRemoveTagList)
                            {
                                weixinUser.UserTags_WeixinUsers.Remove(userTags_WeixinUsers);
                            }
                        }
                        else
                        {
                            weixinUser = _weixinUserService.Mapper.Map<Models.WeixinUser>(weixinUserDto);//����
                            newWeixinUsers.Add(weixinUser);
                        }

                        //TODO:����group��Ϣ
                        weixinUser.UpdateTime();
                        allToSaveWeixinUsers.Add(weixinUser);//����
                    }
                });

                tasks[index] = task;

                if (index % maxThreadsCount == 0 || index == openIds.Count - 1)
                {
                    //ֻ����N���̣߳�����ȴ�
                    Task.WaitAll(tasks.Values.ToArray());
                    //��¼�쳣
                    foreach (var item in tasks.Values.Where(z => z.Exception != null))
                    {
                        SenparcTrace.BaseExceptionLog(item.Exception);
                    }
                    //�������ɵ��߳�
                    tasks.Clear();
                }
            }

            SenparcTrace.SendCustomLog("�����û���Ϣ", $"{allToSaveWeixinUsers.Count} ��");
            await _weixinUserService.SaveObjectListAsync(allToSaveWeixinUsers);

            base.SetMessager(Scf.Core.Enums.MessageType.success, "���³ɹ���");
            return RedirectToPage("./Index", new { uid = Uid, mpId = mpId });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int[] ids)
        {
            var mpId = 0;
            foreach (var id in ids)
            {
                var weixinUser = await _weixinUserService.GetObjectAsync(z => z.Id == id);
                if (weixinUser != null)
                {
                    mpId = weixinUser.MpAccountId;
                    await _weixinUserService.DeleteObjectAsync(weixinUser);
                }
            }
            return RedirectToPage("./Index", new { Uid, mpId });
        }
    }
}
