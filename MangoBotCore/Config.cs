using System;
using System.Collections.Generic;
using System.Text;

namespace UnlimitedBotCore {
    public class Config {
        public string Token { get; set; } = "";
        public string Prefix { get; set; } = "^";
        public string Game { get; set; } = "";
        public string BotOwner { get; set; } = "";
        public string AppealURL { get; set; } = "http://appeal.unlimitedscp.com";
    }
}
