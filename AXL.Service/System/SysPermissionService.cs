using AXL.Infrastructure;
using AXL.Infrastructure.Attribute;
using AXL.Model.System;
using AXL.Service.System.IService;
using System;
using System.Collections.Generic;

namespace AXL.Service.System {

    /// <summary>
    /// 角色权限
    /// </summary>
    [AppService(ServiceType = typeof(ISysPermissionService), ServiceLifetime = LifeTime.Transient)]
    public class SysPermissionService : ISysPermissionService {
        private readonly ISysRoleService SysRoleService;
        private readonly ISysMenuService SysMenuService;

        public SysPermissionService(
            ISysRoleService sysRoleService,
            ISysMenuService sysMenuService) {
            SysRoleService = sysRoleService;
            SysMenuService = sysMenuService;
        }

        /// <summary>
        /// 获取角色数据权限
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns>角色权限信息</returns>
        public List<string> GetRolePermission(SysUser user) {
            List<string> roles = new List<string>();
            // 管理员拥有所有权限
            if (user.IsAdmin()) {
                roles.Add("admin");
            }
            else {
                roles.AddRange(SysRoleService.SelectUserRoleKeys(user.UserId));
            }
            return roles;
        }

        /// <summary>
        /// 获取菜单数据权限
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns>菜单权限信息</returns>
        public List<string> GetMenuPermission(SysUser user) {
            List<string> perms = new();
            // 管理员拥有所有权限
            if (user.IsAdmin() || GetRolePermission(user).Exists(f => f.Equals(GlobalConstant.AdminRole))) {
                perms.Add(GlobalConstant.AdminPerm);
            }
            else {
                perms.AddRange(SysMenuService.SelectMenuPermsByUserId(user.UserId));
            }
            return perms;
        }
    }
}