using AXL.Admin.WebApi.Extensions;
using AXL.Admin.WebApi.Framework;
using AXL.Common.Cache;
using AXL.Model.System;
using AXL.Model.System.Dto;
using AXL.Service.System;
using AXL.Service.System.IService;
using Hei.Captcha;
using IPTools.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UAParser;

namespace AXL.Admin.WebApi.Controllers.System {

    /// <summary>
    /// 登录
    /// </summary>
    [Route("[controller]/[action]")]
    [ApiController]
    public class SysLoginController : BaseController {
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger("LoginController");
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ISysUserService sysUserService;
        private readonly ISysMenuService sysMenuService;
        private readonly ISysLoginService sysLoginService;
        private readonly ISysPermissionService permissionService;
        private readonly SecurityCodeHelper SecurityCodeHelper;
        private readonly ISysConfigService sysConfigService;
        private readonly ISysRoleService roleService;
        private readonly OptionsSetting jwtSettings;

        public SysLoginController(
            IHttpContextAccessor contextAccessor,
            ISysMenuService sysMenuService,
            ISysUserService sysUserService,
            ISysLoginService sysLoginService,
            ISysPermissionService permissionService,
            ISysConfigService configService,
            ISysRoleService sysRoleService,
            SecurityCodeHelper captcha,
            IOptions<OptionsSetting> jwtSettings) {
            httpContextAccessor = contextAccessor;
            SecurityCodeHelper = captcha;
            this.sysMenuService = sysMenuService;
            this.sysUserService = sysUserService;
            this.sysLoginService = sysLoginService;
            this.permissionService = permissionService;
            sysConfigService = configService;
            roleService = sysRoleService;
            this.jwtSettings = jwtSettings.Value;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="loginBody">登录对象</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Login([FromBody] LoginBodyDto loginBody) {
            if (loginBody == null) { throw new CustomException("请求参数错误"); }
            loginBody.LoginIP = HttpContext.GetClientUserIp();
            SysConfig sysConfig = sysConfigService.GetSysConfigByKey("sys.account.captchaOnOff");
            if (sysConfig?.ConfigValue != "off" && CacheHelper.Get(loginBody.Uuid) is string str && !str.ToLower().Equals(loginBody.Code.ToLower())) {
                return ToResponse(ResultCode.CAPTCHA_ERROR, "验证码错误");
            }

            var user = sysLoginService.Login(loginBody, RecordLogInfo(httpContextAccessor.HttpContext!));

            List<SysRole> roles = roleService.SelectUserRoleListByUserId(user.UserId);
            //权限集合 eg *:*:*,system:user:list
            List<string> permissions = permissionService.GetMenuPermission(user);

            LoginUser loginUser = new(user, roles, permissions);
            CacheService.SetUserPerms(GlobalConstant.UserPermKEY + user.UserId, permissions);
            return SUCCESS(JwtUtil.GenerateJwtToken(JwtUtil.AddClaims(loginUser), jwtSettings.JwtSettings));
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <returns></returns>
        [Log(Title = "注销")]
        [HttpPost]
        public IActionResult LogOut() {
            //Task.Run(async () =>
            //{
            //    //注销登录的用户，相当于ASP.NET中的FormsAuthentication.SignOut
            //    await HttpContext.SignOutAsync();
            //}).Wait();
            var userid = HttpContext.GetUId();
            var name = HttpContext.GetName();

            CacheService.RemoveUserPerms(GlobalConstant.UserPermKEY + userid);
            return SUCCESS(new { name, id = userid });
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        [Verify]
        [HttpGet]
        public IActionResult GetUserInfo() {
            long userid = HttpContext.GetUId();
            var user = sysUserService.SelectUserById(userid);

            //前端校验按钮权限使用
            //角色集合 eg: admin,yunying,common
            List<string> roles = permissionService.GetRolePermission(user);
            //权限集合 eg *:*:*,system:user:list
            List<string> permissions = permissionService.GetMenuPermission(user);
            user.WelcomeContent = GlobalConstant.WelcomeMessages[new Random().Next(0, GlobalConstant.WelcomeMessages.Length)];
            return SUCCESS(new { user, roles, permissions });
        }

        /// <summary>
        /// 获取路由信息
        /// </summary>
        /// <returns></returns>
        [Verify]
        [HttpGet]
        public IActionResult GetRouters() {
            long uid = HttpContext.GetUId();
            var menus = sysMenuService.SelectMenuTreeByUserId(uid);

            return ToResponse(ToJson(1, sysMenuService.BuildMenus(menus)));
        }

        /// <summary>
        /// 生成图片验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResult CaptchaImage() {
            string uuid = Guid.NewGuid().ToString().Replace("-", "");

            SysConfig sysConfig = sysConfigService.GetSysConfigByKey("sys.account.captchaOnOff");
            var captchaOff = sysConfig?.ConfigValue ?? "0";

            var length = AppSettings.GetAppConfig("CaptchaOptions:length", 4);
            var code = SecurityCodeHelper.GetRandomEnDigitalText(length);
            byte[] imgByte = GenerateCaptcha(captchaOff, code);
            string base64Str = Convert.ToBase64String(imgByte);
            CacheHelper.SetCache(uuid, code);
            var obj = new { captchaOff, uuid, img = base64Str };// File(stream, "image/png")

            return ToJson(1, obj);
        }

        /// <summary>
        /// 生成图片验证码
        /// </summary>
        /// <param name="captchaOff"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        private byte[] GenerateCaptcha(string captchaOff, string code) {
            byte[] imgByte;
            if (captchaOff == "1") {
                imgByte = SecurityCodeHelper.GetGifEnDigitalCodeByte(code);//动态gif数字字母
            }
            else if (captchaOff == "2") {
                imgByte = SecurityCodeHelper.GetGifBubbleCodeByte(code);//动态gif泡泡
            }
            else if (captchaOff == "3") {
                imgByte = SecurityCodeHelper.GetBubbleCodeByte(code);//泡泡
            }
            else {
                imgByte = SecurityCodeHelper.GetEnDigitalCodeByte(code);//英文字母加数字
            }

            return imgByte;
        }

        /// <summary>
        /// 记录用户登陆信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private SysLogininfor RecordLogInfo(HttpContext context) {
            var ipAddr = context.GetClientUserIp();
            var ip_info = IpTool.Search(ipAddr);
            ClientInfo clientInfo = context.GetClientInfo();
            SysLogininfor sysLogininfor = new() {
                Browser = clientInfo.ToString(),
                Os = clientInfo.OS.ToString(),
                Ipaddr = ipAddr,
                UserName = context.GetName(),
                LoginLocation = ip_info?.Province + "-" + ip_info?.City
            };

            return sysLogininfor;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Log(Title = "注册", BusinessType = Infrastructure.Enums.BusinessType.INSERT)]
        public IActionResult Register([FromBody] RegisterDto dto) {
            SysConfig config = sysConfigService.GetSysConfigByKey("sys.account.register");
            if (config?.ConfigValue != "true") {
                return ToResponse(ResultCode.CUSTOM_ERROR, "当前系统没有开启注册功能！");
            }
            SysConfig sysConfig = sysConfigService.GetSysConfigByKey("sys.account.captchaOnOff");
            if (sysConfig?.ConfigValue != "off" && CacheHelper.Get(dto.Uuid) is string str && !str.ToLower().Equals(dto.Code.ToLower())) {
                return ToResponse(ResultCode.CAPTCHA_ERROR, "验证码错误");
            }
            if (UserConstants.NOT_UNIQUE.Equals(sysUserService.CheckUserNameUnique(dto.Username))) {
                return ToResponse(ResultCode.CUSTOM_ERROR, $"保存用户{dto.Username}失败，注册账号已存在");
            }
            SysUser user = sysUserService.Register(dto);
            if (user.UserId > 0) {
                return SUCCESS(user);
            }
            return ToResponse(ResultCode.CUSTOM_ERROR, "注册失败，请联系管理员");
        }
    }
}