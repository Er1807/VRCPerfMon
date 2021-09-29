using HarmonyLib;
using MelonLoader;
using System;
using System.Diagnostics;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityPerfMon;
using VRC;
using VRC.Udon;

[assembly: MelonInfo(typeof(PerfMon), "UnityPerfMon", "0.0.1", "Eric")]
[assembly: MelonGame]

namespace UnityPerfMon
{
     //[HarmonyPatch(typeof(MonoBehaviour), "Update")]
    public class PerfMon : MelonMod
    {
        

        public override void OnApplicationStart()
        {


            MelonLogger.Msg("Patching Methods (Pre/Post)");
            HarmonyInstance.Patch(
                    typeof(UdonBehaviour).GetMethod(nameof(UdonBehaviour.PostLateUpdate)),
                    new HarmonyMethod(AccessTools.Method(typeof(Hooker), nameof(Hooker.Prefix))),
                    new HarmonyMethod(AccessTools.Method(typeof(Hooker), nameof(Hooker.Postfix))));
            HarmonyInstance.Patch(
                    typeof(UdonBehaviour).GetMethod(nameof(UdonBehaviour.ManagedUpdate)),
                    new HarmonyMethod(AccessTools.Method(typeof(Hooker), nameof(Hooker.Prefix))),
                    new HarmonyMethod(AccessTools.Method(typeof(Hooker), nameof(Hooker.Postfix))));

            HarmonyInstance.Patch(
                    typeof(UdonBehaviour).GetMethod(nameof(UdonBehaviour.ManagedLateUpdate)),
                    new HarmonyMethod(AccessTools.Method(typeof(Hooker), nameof(Hooker.Prefix))),
                    new HarmonyMethod(AccessTools.Method(typeof(Hooker), nameof(Hooker.Postfix))));

            //HarmonyInstance.Patch(
            //        typeof(DynamicBone).GetMethod(nameof(DynamicBone.Method_Private_Void_Single_Boolean_0)),
            //        new HarmonyMethod(AccessTools.Method(typeof(Hooker), nameof(Hooker.Prefix))),
            //        new HarmonyMethod(AccessTools.Method(typeof(Hooker), nameof(Hooker.Postfix))));



            HarmonyInstance.Patch(
                    typeof(DynamicBoneController).GetMethod(nameof(DynamicBoneController.LateUpdate)),
                    new HarmonyMethod(AccessTools.Method(typeof(Hooker), nameof(Hooker.Prefix))),
                    new HarmonyMethod(AccessTools.Method(typeof(Hooker), nameof(Hooker.Postfix))));

            HarmonyInstance.Patch(
                     typeof(Camera).GetMethod(nameof(Camera.FireOnPreRender), new Type[] { typeof(Camera) }),
                     new HarmonyMethod(AccessTools.Method(typeof(Hooker), nameof(Hooker.PrefixCamera), new Type[] { typeof(Camera) })));


            HarmonyInstance.Patch(
                     typeof(Camera).GetMethod(nameof(Camera.FireOnPostRender), new Type[] { typeof(Camera) }),
                     new HarmonyMethod(AccessTools.Method(typeof(Hooker), nameof(Hooker.PostfixCamera), new Type[] { typeof(Camera) })));



            MelonLogger.Msg("Finished Methods (Pre/Post)");
        }

        
    }
}
