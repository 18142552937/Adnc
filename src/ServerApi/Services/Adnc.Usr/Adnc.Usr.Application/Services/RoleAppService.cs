﻿using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Adnc.Usr.Application.Contracts.Dtos;
using Adnc.Infra.Common.Extensions;
using Adnc.Usr.Core.Entities;
using Adnc.Usr.Core.Services;
using Adnc.Core.Shared.IRepositories;
using Adnc.Infra.Common.Helper;
using Adnc.Application.Shared.Services;
using Adnc.Application.Shared.Dtos;
using Adnc.Usr.Application.Contracts.Services;

namespace Adnc.Usr.Application.Services
{
    public class RoleAppService : AbstractAppService, IRoleAppService
    {
        private readonly IEfRepository<SysRole> _roleRepository;
        private readonly IEfRepository<SysUser> _userRepository;
        private readonly UsrManager _usrManager;
        private readonly CacheService _cacheService;

        public RoleAppService(IEfRepository<SysRole> roleRepository,
            IEfRepository<SysUser> userRepository,
            UsrManager usrManager,
            CacheService cacheService)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _usrManager = usrManager;
            _cacheService = cacheService;
        }

        public async Task<AppSrvResult<long>> CreateAsync(RoleCreationDto input)
        {
            var isExists = (await _cacheService.GetAllRolesFromCacheAsync()).Where(x => x.Name == input.Name).Any();
            if (isExists)
                return Problem(HttpStatusCode.BadRequest, "该角色名称已经存在");

            var role = Mapper.Map<SysRole>(input);
            role.Id = IdGenerater.GetNextId();
            await _roleRepository.InsertAsync(role);

            return role.Id;
        }

        public async Task<AppSrvResult> UpdateAsync(long id, RoleUpdationDto input)
        {
            var isExists = (await _cacheService.GetAllRolesFromCacheAsync()).Where(x => x.Name == input.Name && x.Id != id).Any();
            if (isExists)
                return Problem(HttpStatusCode.BadRequest, "该角色名称已经存在");

            var role = Mapper.Map<SysRole>(input);

            role.Id = id;

            await _roleRepository.UpdateAsync(role, UpdatingProps<SysRole>(x => x.Name, x => x.Tips, x => x.Ordinal));

            return AppSrvResult();
        }

        public async Task<AppSrvResult> DeleteAsync(long id)
        {
            if (id == 1600000000010)
                return Problem(HttpStatusCode.Forbidden, "禁止删除初始角色");

            if (await _userRepository.AnyAsync(x => x.RoleIds == id.ToString()))
                return Problem(HttpStatusCode.Forbidden, "有用户使用该角色，禁止删除");

            await _roleRepository.DeleteAsync(id);

            return AppSrvResult();
        }

        public async Task<AppSrvResult> SetPermissonsAsync(RoleSetPermissonsDto input)
        {
            if (input.RoleId == 1600000000010)
                return Problem(HttpStatusCode.Forbidden, "禁止设置初始角色");

            await _usrManager.SetRolePermissonAsync(input.RoleId, input.Permissions);

            return AppSrvResult();
        }

        public async Task<RoleTreeDto> GetRoleTreeListByUserIdAsync(long userId)
        {
            RoleTreeDto result = null;
            IEnumerable<ZTreeNodeDto<long, dynamic>> treeNodes = null;

            var user = await _cacheService.GetUserValidateInfoFromCacheAsync(userId) ;

            if (user == null)
                return null;

            var roles =await _cacheService.GetAllRolesFromCacheAsync();
            var roleIds = user.RoleIds?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => long.Parse(x)) ?? new List<long>();
            if (roles.Any())
            {
                treeNodes = roles.Select(x => new ZTreeNodeDto<long, dynamic>
                {
                    Id = x.Id,
                    PID = x.Pid.HasValue ? x.Pid.Value : 0,
                    Name = x.Name,
                    Open = x.Pid.HasValue && x.Pid.Value > 0 ? false : true,
                    Checked = roleIds.Contains(x.Id)
                });

                result = new RoleTreeDto
                {
                    TreeData = treeNodes.Select(x => new Node<long>
                    {
                        Id = x.Id,
                        PID = x.PID,
                        Name = x.Name,
                        Checked = x.Checked
                    }),
                    CheckedIds = treeNodes.Where(x => x.Checked).Select(x => x.Id)
                };
            }

            return result;
        }

        public async Task<PageModelDto<RoleDto>> GetPagedAsync(RolePagedSearchDto search)
        {
            Expression<Func<SysRole, bool>> whereCondition = x => true;
            if (search.RoleName.IsNotNullOrWhiteSpace())
            {
                whereCondition = whereCondition.And(x => x.Name.Contains(search.RoleName));
            }

            var pagedModel = await _roleRepository.PagedAsync(search.PageIndex, search.PageSize, whereCondition, x => x.Ordinal, true);

            return Mapper.Map<PageModelDto<RoleDto>>(pagedModel);
        }
    }
}
