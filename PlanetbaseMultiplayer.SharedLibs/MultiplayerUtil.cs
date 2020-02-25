using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
		public static Selectable FindSelectableFromId(int selectableId)
		{
			Selectable selectable = null;
			selectable = Construction.find(selectableId);
			if (selectable != null) return selectable;
			selectable = Character.find(selectableId);
			if (selectable != null) return selectable;
			selectable = Resource.find(selectableId);
			if (selectable != null) return selectable;
			selectable = Ship.find(selectableId);
			if (selectable != null) return selectable;
			return selectable;
		}
    }
}
