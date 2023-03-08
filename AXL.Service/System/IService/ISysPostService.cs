using AXL.Model.System;
using System.Collections.Generic;

namespace AXL.Service.System.IService {

    public interface ISysPostService : IBaseService<SysPost> {

        string CheckPostNameUnique(SysPost sysPost);

        string CheckPostCodeUnique(SysPost sysPost);

        List<SysPost> GetAll();
    }
}