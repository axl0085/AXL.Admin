using System;

namespace AXL.Infrastructure.Model {

    public class SendEmailDto {
        public string FileUrl { get; set; } = "";
        public string Subject { get; set; }
        public string ToUser { get; set; }
        public string Content { get; set; } = "";
        public string HtmlContent { get; set; }

        /// <summary>
        /// 是否发送给自己
        /// </summary>
        public bool SendMe { get; set; }

        public DateTime AddTime { get; set; }
    }
}