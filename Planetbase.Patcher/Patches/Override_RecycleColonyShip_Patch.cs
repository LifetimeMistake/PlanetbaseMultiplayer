using Harmony;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using PlanetbaseMultiplayer.SharedLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetbaseMultiplayer.Patcher.Patches
{
    [HarmonyPatch(typeof(ColonyShip), "recycle")]
    class Override_RecycleColonyShip_Patch
    {
        static bool Prefix(ColonyShip __instance, ResourceContainer ___mResourceContainer)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            List<ResourceUpdateData> extracted = new List<ResourceUpdateData>();
            List<ResourceConstructionData> created = new List<ResourceConstructionData>();
            foreach (Resource resource in ___mResourceContainer.getResources())
            {
                extracted.Add(new ResourceUpdateData(resource.getId(), ResourceAction.Extract, (Vector3_Serializable)(__instance.getPosition() + MathUtil.randFlatVector(5f).Rounded() * 1.2f),
                    new Quaternion_Serializable(), Location.Exterior));
            }
            ResourceType metalType = TypeList<ResourceType, ResourceTypeList>.find<Metal>();
            ResourceType bioplasticType = TypeList<ResourceType, ResourceTypeList>.find<Bioplastic>();
            for (int i = 0; i < 15; i++)
            {
                created.Add(new ResourceConstructionData(metalType.GetType().Name, ResourceSubtype.None, (Vector3_Serializable)(__instance.getPosition() + MathUtil.randFlatVector(5f).Rounded() * 1.2f),
                    new Quaternion_Serializable(), Location.Exterior, false));
            }
            for (int i = 0; i < 10; i++)
            {
                created.Add(new ResourceConstructionData(bioplasticType.GetType().Name, ResourceSubtype.None, (Vector3_Serializable)(__instance.getPosition() + MathUtil.randFlatVector(5f).Rounded() * 1.2f),
                    new Quaternion_Serializable(), Location.Exterior, false));
            }
            Globals.LocalClient.OnColonyShipRecycled_Locally(__instance, created.ToArray(), extracted.ToArray());
            return false;
        }
    }
}
