﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Adnc.Usr.Application.Contracts.Dtos;
using Adnc.Application.Shared.Interceptors;
using Adnc.Application.Shared.Services;
using Adnc.Infra.Caching.Interceptor;
using Adnc.Usr.Application.Contracts.Consts;

namespace Adnc.Usr.Application.Contracts.Services
{
    /// <summary>
    /// 菜单/权限服务
    /// </summary>
    public interface IMenuAppService : IAppService
    {
        /// <summary>
        /// 新增菜单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [OpsLog(LogName = "新增菜单")]
        [CachingEvict(CacheKeys = new[] { CachingConsts.MenuListCacheKey, CachingConsts.MenuTreeListCacheKey, CachingConsts.MenuRelationCacheKey, CachingConsts.MenuCodesCacheKey })]
        Task<AppSrvResult<long>> CreateAsync(MenuCreationDto input);

        /// <summary>
        /// 修改菜单
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [OpsLog(LogName = "修改菜单")]
        [CachingEvict(CacheKeys = new[] { CachingConsts.MenuListCacheKey, CachingConsts.MenuTreeListCacheKey,CachingConsts.MenuRelationCacheKey, CachingConsts.MenuCodesCacheKey })]
        Task<AppSrvResult> UpdateAsync(long id,MenuUpdationDto input);

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OpsLog(LogName = "删除菜单")]
        [CachingEvict(CacheKeys = new[] { CachingConsts.MenuListCacheKey, CachingConsts.MenuTreeListCacheKey, CachingConsts.MenuRelationCacheKey, CachingConsts.MenuCodesCacheKey })]
        Task<AppSrvResult> DeleteAsync(long id);

        /// <summary>
        /// 获取菜单列表
        /// </summary>
        /// <returns></returns>
        [CachingAble(CacheKey = CachingConsts.MenuTreeListCacheKey)]
        Task<List<MenuNodeDto>> GetlistAsync();

        /// <summary>
        /// 获取左侧路由菜单
        /// </summary>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        Task<List<MenuRouterDto>> GetMenusForRouterAsync(IEnumerable<long> roleIds);

        /// <summary>
        /// 获取指定角色的菜单
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<MenuTreeDto> GetMenuTreeListByRoleIdAsync(long roleId);
    }
}
