using HarmonyLib;
using MelonLoader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib;
using UnityEngine;
using UnityEngine.XR;
using VRC;
using VRC.Udon;

namespace UnityPerfMon
{
    public class Hooker
    {
        class Item
        {

            DateTime current { get; set; } = DateTime.Now;
            int framecount { get; set; } = Time.frameCount;
            string type { get; set; }
            string user { get; set; }
            string objectname { get; set; }
            TimeSpan TimeTook { get; set; }

            public Item(string type, string user, string objectname, TimeSpan timeTook)
            {
                this.type = type;
                this.user = user;
                this.objectname = objectname;
                TimeTook = timeTook;
            }

            public override string ToString()
            {
                Dictionary<string, System.Object> dic = new Dictionary<string, System.Object>() {
                    {"current",current },
                    {"framecount",framecount },
                    {"type",type },
                    {"user",user },
                    {"objectname",objectname },
                    {"TimeTook",TimeTook.Ticks*100 }
                };
                return "{ \"index\":{} }\n"+
                    JsonConvert.SerializeObject(dic, Formatting.None)+"\n";
            }
        }

        static HttpClient client = new HttpClient();
        static List<Item> items = new List<Item>();
        static string url = "http://localhost:9200/stats/_bulk";
        public static void Prefix(MonoBehaviour __instance, out Stopwatch __state)
        {
            __state = Stopwatch.StartNew();
            //MelonLogger.Msg("Pre: " + __instance.GetType().Name);
        }

        public static void Postfix(Behaviour __instance, Stopwatch __state)
        {
            if (__instance == null || __state == null) return;
            __state.Stop();
            //save

            if (items.Count > 5000)
                {
                    StringBuilder st = new StringBuilder();

                    foreach (var item in items)
                    {
                        st.Append(item);
                    }
                    var data = new StringContent(st.ToString(), Encoding.UTF8, "application/json");

                    client.PostAsync(url, data);
                    items.Clear();
            }


            if (__instance is UdonBehaviour)
            {
                items.Add(new Item("UdonBehaviour", "", __instance.name, __state.Elapsed));
            }
            if (__instance is DynamicBoneController)
            {
                string name = __instance.transform.root.gameObject.GetComponent<VRCPlayer>()?.prop_String_0;
                items.Add(new Item("DynamicBoneController", name, __instance.name, __state.Elapsed));
            }

            if (__instance is Camera)
            {
                string name = __instance.transform.root.gameObject.GetComponent<VRCPlayer>()?.prop_String_0;
                items.Add(new Item("Camera", name, __instance.name, __state.Elapsed));          
            }
            //MelonLogger.Msg("Post: " +  __instance.GetType().Name);

        }

        static Dictionary<int, Stopwatch> cameras = new Dictionary<int, Stopwatch>();

        public static void PrefixCamera(Camera cam)
        {
            cameras.Add(cam.gameObject.GetInstanceID(), Stopwatch.StartNew());
        }
        public static void PostfixCamera(Camera cam)
        {
            if (!cameras.ContainsKey(cam.gameObject.GetInstanceID())) return;
            Postfix(cam, cameras[cam.gameObject.GetInstanceID()]);
            cameras.Remove(cam.gameObject.GetInstanceID());
        }
    }
}
