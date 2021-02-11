using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnlimitedTweaks.Data;
using UnlimitedTweaks.System;

namespace UnlimitedBotCore {
    public class Player {

        public string UserId { get; set; }
        public PlayerData Data { get; set; }
        
        public Player(string userId) {
            UserId = userId;

            LoadData();
        }

        public bool SaveData() {
            try {
                File.WriteAllText(Extensions.GetFilePath(UserId), JsonConvert.SerializeObject(Data));
                return true;
            } catch(Exception e) {
                Console.WriteLine($"{e.Message}\n{e.StackTrace}");
                return false;
            }
        }

        public bool LoadData() {
            var path = Extensions.GetFilePath(UserId);

            if(!Directory.Exists(Extensions.GetDirectoryPath())) {
                Directory.CreateDirectory(Extensions.GetDirectoryPath());
            }

            if(!File.Exists(path)) {
                Data = new PlayerData() { Minutes = 0, Club = "none", Mvp = 0, Escaped = 0 };

                File.WriteAllText(path, JsonConvert.SerializeObject(Data));
                return true;
            }

            try {
                Data = JsonConvert.DeserializeObject<PlayerData>(File.ReadAllText(path));
            } catch(Exception) {
                Data = null;
            }

            return Data != null;
        }

    }
}