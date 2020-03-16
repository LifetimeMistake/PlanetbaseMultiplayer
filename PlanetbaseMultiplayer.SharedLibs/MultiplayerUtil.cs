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
			selectable = ConstructionComponent.find(selectableId);
			if (selectable != null) return selectable;
			return selectable;
		}

		public static List<int> GetAllIds()
		{
			List<int> ids = new List<int>();
			if(Construction.mConstructions != null)
			foreach (Construction construction in Construction.mConstructions)
				ids.Add(construction.getId());
			if (ConstructionComponent.mComponents != null)
				foreach (ConstructionComponent component in ConstructionComponent.mComponents)
				ids.Add(component.getId());
			if (Resource.mResources != null)
				foreach (Resource resource in Resource.mResources)
				ids.Add(resource.getId());
			if (Character.mCharacters != null)
				foreach (Character character in Character.mCharacters)
				ids.Add(character.getId());
			if (Ship.mShips != null)
				foreach (Ship ship in Ship.mShips)
				ids.Add(ship.getId());
			return ids;
		}
    }
}
