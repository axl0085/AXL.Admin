using Microsoft.AspNetCore.Mvc;

namespace AXL.Admin.WebApi.Controllers.System.monitor {

    [Verify]
    [Route("monitor/online")]
    public class SysUserOnlineController : BaseController {

        [HttpGet("list")]
        public IActionResult Index() {
            return SUCCESS(null);
        }
    }
}