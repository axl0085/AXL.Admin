using AXL.Model.System;
using System.Collections.Generic;

namespace AXL.Service.System.IService {

    public interface ISysPermissionService {

        public List<string> GetRolePermission(SysUser user);

        public List<string> GetMenuPermission(SysUser user);
    }
}