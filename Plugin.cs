using HarmonyLib;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using Newtonsoft.Json;
using NiceMiss.Installers;
using SiraUtil.Zenject;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

namespace NiceMiss
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger log { get; set; }
        internal string mapDataFilePath { get; private set; }

        [Init]
        public Plugin(IPALogger logger, Zenjector zenjector)
        {
            Instance = this;
            log = logger;
            zenjector.Install<NiceMissMenuInstaller>(Location.Menu);
            zenjector.Install<NiceMissGameInstaller>(Location.GameCore);
        }

        [Init]
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Plugin.log?.Debug("Config loaded");

        }

        [OnStart]
        public void OnApplicationStart()
        {
            var harmony = new Harmony("net.kyle1413.nicemiss");
            harmony.PatchAll();
            mapDataFilePath = Path.Combine(Directory.GetCurrentDirectory(), "UserData", "NissMiss.MapData");
            if (File.Exists(mapDataFilePath))
            {
                Dictionary<string, Dictionary<string, NoteTracker.Rating>> temp = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, NoteTracker.Rating>>>(File.ReadAllText(mapDataFilePath));
                if (temp != null)
                {
                    NoteTracker.mapData = temp;
                    log.Info("Loaded NiceMiss Map data");
                }
            }

            SharedCoroutineStarter.instance.StartCoroutine(LoadQuickOutlineMaterials());
        }

        public IEnumerator LoadQuickOutlineMaterials()
        {
            var quickOutlineBundleRequest = AssetBundle.LoadFromStreamAsync(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("NiceMiss.QuickOutline.Resources.outlineBundle"));
            yield return quickOutlineBundleRequest;
            var quickOutlineBundle = quickOutlineBundleRequest.assetBundle;
            if (quickOutlineBundle == null)
            {
                Plugin.log.Error("Failed To load QuickOutline Bundle");
                yield break;

            }
            var fillMatRequest = quickOutlineBundle.LoadAssetAsync<Material>("OutlineFill");
            yield return fillMatRequest;
            Outline.outlineFillMaterialSource = fillMatRequest.asset as Material;
            var maskMatRequest = quickOutlineBundle.LoadAssetAsync<Material>("OutlineMask");
            yield return maskMatRequest;
            Outline.outlineMaskMaterialSource = maskMatRequest.asset as Material;
            Plugin.log.Debug("Loaded QuickOutline Material Assets");
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            File.WriteAllText(mapDataFilePath, JsonConvert.SerializeObject(NoteTracker.mapData));
        }
    }
}
