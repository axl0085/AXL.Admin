using AXL.Common;
using JinianNet.JNTemplate;

namespace AXL.Admin.WebApi.Extensions {

    public static class LogoExtension {

        public static void AddLogo(this IServiceCollection services) {
            Console.ForegroundColor = ConsoleColor.Blue;
            var contentTpl = JnHelper.ReadTemplate("", "logo.txt");
            var content = contentTpl?.Render();

            Console.WriteLine(content);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Swagger：http://localhost:8888/swagger/index.html");
        }
    }
}