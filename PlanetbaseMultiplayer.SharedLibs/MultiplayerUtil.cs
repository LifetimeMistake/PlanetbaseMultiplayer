using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetbaseMultiplayer.SharedLibs
{
    public static class MultiplayerUtil
    {
        public static ResourceSubtype GetDefaultResourceSubtype(ResourceType resourceType)
        {
			ResourceSubtype subtype = ResourceSubtype.None;
			if (resourceType == ResourceTypeList.VegetablesInstance)
			{
				subtype = ResourceSubtype.Tomatoes;
			}
			else if (resourceType == ResourceTypeList.MealInstance)
			{
				subtype = ResourceSubtype.Basic;
			}
			else if (resourceType == ResourceTypeList.VitromeatInstance)
			{
				subtype = ResourceSubtype.Chicken;
			}
			return subtype;
		}
    }
}
