using AXL.Model.System;
using System.Collections.Generic;

namespace AXL.Service.System.IService {

    /// <summary>
    /// 通知公告表service接口
    ///
    /// @author zr
    /// @date 2021-12-15
    /// </summary>
    public interface ISysNoticeService : IBaseService<SysNotice> {

        List<SysNotice> GetSysNotices();
    }
}