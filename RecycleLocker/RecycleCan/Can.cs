using System.Collections.Generic;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using UnityEngine;
#if SUBNAUTICA
using Ingredient = CraftData.Ingredient;
#endif

namespace RecycleCan
{
    public class Can
    {
        public static TechType techType = Buildablecan.Info.TechType;
        public static void Patch()
        {
            Buildablecan.Register();
        }
    }

    public static class Buildablecan
    {
        public static Texture2D HorizontalWallLockersTexture = RamuneLib.Utils.ImageUtils.GetTexture("door");
        public static Texture2D HorizontalWallLockersspec = RamuneLib.Utils.ImageUtils.GetTexture("door");
       // public static Texture2D HorizontalWallLockersnorm = RamuneLib.Utils.ImageUtils.GetTexture("submarine_locker_01_normal");
        public static Texture2D HorizontalLockersTexture = RamuneLib.Utils.ImageUtils.GetTexture("export");
        public static Texture2D HorizontalLockersspec = RamuneLib.Utils.ImageUtils.GetTexture("export");
      //  public static Texture2D HorizontalLockersnorm = RamuneLib.Utils.ImageUtils.GetTexture("submarine_locker_01_normal");
        public static PrefabInfo Info { get; } = PrefabInfo.WithTechType("RCan", "RecycleLocker", "Scientists from Alterra Converted a SmallLocker to recycle items", "English")
            .WithIcon(RamuneLib.Utils.ImageUtils.GetSprite(TechType.SmallLocker));

        public static void Register()
        {
            CustomPrefab prefab = new CustomPrefab(Info);
            CloneTemplate curtainsClone = new CloneTemplate(Info, TechType.SmallLocker);

            curtainsClone.ModifyPrefab += obj =>
            {   
                var triggerCull = obj.GetComponentInChildren<TriggerCull>();
                GameObject.DestroyImmediate(triggerCull);
                var label = obj.FindChild("Label");
                    GameObject.DestroyImmediate(label);
                var renderer1 = obj.FindChild("model").FindChild("submarine_locker_02").GetComponent<MeshRenderer>();
                foreach (var m in renderer1.materials)
                {
                    m.mainTexture = Buildablecan.HorizontalLockersTexture;
                    m.SetTexture("_SpecTex", HorizontalLockersTexture);
                    m.SetTexture("_Illum", HorizontalLockersTexture);
                   // m.SetTexture("_BumpMap", HorizontalWallLockersnorm);

                }
                var renderer = obj.FindChild("model").FindChild("submarine_locker_02").FindChild("submarine_locker_02_door").GetComponent<MeshRenderer>();
                foreach (var m in renderer.materials)
                {
                    m.mainTexture = Buildablecan.HorizontalWallLockersTexture;
                    m.SetTexture("_SpecTex", HorizontalWallLockersTexture);
                    m.SetTexture("_Illum", HorizontalWallLockersTexture);
                   // m.SetTexture("_BumpMap", HorizontalWallLockersnorm);

                }
                obj.AddComponent<RecycleLockerHandler>();
            };


            prefab.SetUnlock(TechType.PrecursorIonCrystal);
            prefab.SetGameObject(curtainsClone);
            prefab.SetPdaGroupCategory(TechGroup.InteriorModules, TechCategory.InteriorModule).SetBuildable();
            prefab.SetRecipe(new RecipeData()
            {
                craftAmount = 0,
                Ingredients = new List<CraftData.Ingredient>()
                {
                    new CraftData.Ingredient(TechType.PrecursorIonCrystal, 1),
                    new CraftData.Ingredient(TechType.TitaniumIngot, 1),
                    new CraftData.Ingredient(TechType.UraniniteCrystal, 1),
                    new CraftData.Ingredient(TechType.LaserCutter, 1)
                },
            });
                
            prefab.Register();

            
        }
    }
}
