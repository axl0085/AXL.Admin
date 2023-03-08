using AXL.Admin.WebApi.Extensions;
using AXL.Common;
using AXL.Model;
using AXL.Model.System.Dto;
using AXL.Service.System.IService;
using Microsoft.AspNetCore.Mvc;

namespace AXL.Admin.WebApi.Controllers.System.monitor {

    /// <summary>
    /// 操作日志记录
    /// </summary>
    [Verify]
    [Route("/monitor/operlog")]
    public class SysOperlogController : BaseController {
        private ISysOperLogService sysOperLogService;
        private IWebHostEnvironment WebHostEnvironment;

        public SysOperlogController(ISysOperLogService sysOperLogService, IWebHostEnvironment hostEnvironment) {
            this.sysOperLogService = sysOperLogService;
            WebHostEnvironment = hostEnvironment;
        }

        /// <summary>
        /// 查询操作日志
        /// </summary>
        /// <param name="sysOperLog"></param>
        /// <returns></returns>
        [HttpGet("list")]
        public IActionResult OperList([FromQuery] SysOperLogDto sysOperLog) {
            PagerInfo pagerInfo = new(sysOperLog.PageNum, sysOperLog.PageSize);

            sysOperLog.OperName = !HttpContext.IsAdmin() ? HttpContext.GetName() : sysOperLog.OperName;
            var list = sysOperLogService.SelectOperLogList(sysOperLog, pagerInfo);

            return SUCCESS(list, "MM/dd HH:mm");
        }

        /// <summary>
        /// 删除操作日志
        /// </summary>
        /// <param name="operIds"></param>
        /// <returns></returns>
        [Log(Title = "操作日志", BusinessType = BusinessType.DELETE)]
        [ActionPermissionFilter(Permission = "monitor:operlog:delete")]
        [HttpDelete("{operIds}")]
        public IActionResult Remove(string operIds) {
            if (!HttpContext.IsAdmin()) {
                return ToResponse(ApiResult.Error("操作失败"));
            }
            long[] operIdss = Tools.SpitLongArrary(operIds);
            return SUCCESS(sysOperLogService.DeleteOperLogByIds(operIdss));
        }

        /// <summary>
        /// 清空操作日志
        /// </summary>
        /// <returns></returns>
        [Log(Title = "清空操作日志", BusinessType = BusinessType.CLEAN)]
        [ActionPermissionFilter(Permission = "monitor:operlog:delete")]
        [HttpDelete("clean")]
        public ApiResult ClearOperLog() {
            if (!HttpContext.IsAdmin()) {
                return ApiResult.Error("操作失败");
            }
            sysOperLogService.CleanOperLog();

            return ToJson(1);
        }

        /// <summary>
        /// 导出操作日志
        /// </summary>
        /// <returns></returns>
        [Log(Title = "操作日志", BusinessType = BusinessType.EXPORT)]
        [ActionPermissionFilter(Permission = "monitor:operlog:export")]
        [HttpGet("export")]
        public IActionResult Export([FromQuery] SysOperLogDto sysOperLog) {
            var list = sysOperLogService.SelectOperLogList(sysOperLog, new PagerInfo(1, 10000));
            var result = ExportExcelMini(list.Result, "操作日志", "操作日志");
            return ExportExcel(result.Item2, result.Item1);
        }
    }
}