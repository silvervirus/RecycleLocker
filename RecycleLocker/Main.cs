using System;
using BepInEx;

namespace RecycleCan
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Main: BaseUnityPlugin
    {
        public const String PLUGIN_GUID = "SN.RecycleLocker";
        public const String PLUGIN_NAME = "SNRecycleCan";
        public const String PLUGIN_VERSION = "1.0.0";
        public void Awake()
        {
            RecyclotronDataLoader.LoadBannedItems();
          Can.Patch();
          Console.WriteLine("[RecycleLocker] Successfully patched.");
            
        }
    }
}