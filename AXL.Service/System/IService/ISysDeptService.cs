﻿using AXL.Model.System;
using AXL.Model.System.Vo;
using System.Collections.Generic;

namespace AXL.Service.System.IService {

    public interface ISysDeptService : IBaseService<SysDept> {

        List<SysDept> GetSysDepts(SysDept dept);

        string CheckDeptNameUnique(SysDept dept);

        int InsertDept(SysDept dept);

        int UpdateDept(SysDept dept);

        void UpdateDeptChildren(long deptId, string newAncestors, string oldAncestors);

        List<SysDept> GetChildrenDepts(List<SysDept> depts, long deptId);

        List<SysDept> BuildDeptTree(List<SysDept> depts);

        List<TreeSelectVo> BuildDeptTreeSelect(List<SysDept> depts);

        List<SysRoleDept> SelectRoleDeptByRoleId(long roleId);

        List<long> SelectRoleDepts(long roleId);

        bool DeleteRoleDeptByRoleId(long roleId);

        int InsertRoleDepts(SysRole role);
    }

    public interface ISysRoleDeptService : IBaseService<SysRoleDept> {

        List<SysRoleDept> SelectRoleDeptByRoleId(long roleId);
    }
}