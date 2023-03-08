using AXL.Infrastructure.Attribute;
using AXL.Model.System;
using AXL.Service.System.IService;

namespace AXL.Service.System {

    /// <summary>
    /// 参数配置Service业务层处理
    /// </summary>
    [AppService(ServiceType = typeof(ISysConfigService), ServiceLifetime = LifeTime.Transient)]
    public class SysConfigService : BaseService<SysConfig>, ISysConfigService {

        #region 业务逻辑代码

        public SysConfig GetSysConfigByKey(string key) {
            return Queryable().First(f => f.ConfigKey == key);
        }

        #endregion 业务逻辑代码
    }
}