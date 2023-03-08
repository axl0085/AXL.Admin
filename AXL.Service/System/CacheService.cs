using AXL.Common.Cache;
using System.Collections.Generic;

namespace AXL.Service.System {

    public class CacheService {

        #region 用户权限 缓存

        public static List<string> GetUserPerms(string key) {
            return (List<string>)CacheHelper.GetCache(key);
        }

        public static void SetUserPerms(string key, object data) {
            CacheHelper.SetCache(key, data);
        }

        public static void RemoveUserPerms(string key) {
            CacheHelper.Remove(key);
        }

        #endregion 用户权限 缓存
    }
}