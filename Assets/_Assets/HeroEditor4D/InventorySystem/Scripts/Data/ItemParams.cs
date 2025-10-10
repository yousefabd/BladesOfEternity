using System;
using System.Collections.Generic;
using System.Linq;
using Assets.HeroEditor4D.Common.Scripts.Common;
using Assets.HeroEditor4D.InventorySystem.Scripts.Enums;
using Newtonsoft.Json;

namespace Assets.HeroEditor4D.InventorySystem.Scripts.Data
{
    /// <summary>
    /// Represents generic item params (common for all items).
    /// </summary>
    [Serializable]
    public class ItemParams
    {
	    public string Id;
        public int Level;
        public ItemRarity Rarity;
        public ItemType Type;
        public ItemClass Class;
        public List<ItemTag> Tags = new List<ItemTag>();
        public List<Property> Properties = new List<Property>();
        public int Price;
        public int Weight;
        public ItemMaterial Material;
        public string IconId;
        public string SpriteId;
        public string Meta;

        /// <summary>
        /// Use this property to provide localization values for items.
        /// </summary>
        [JsonIgnore, NonSerialized] public List<LocalizedValue> Localization = new List<LocalizedValue>();

        public char Grade => (char) (65 + Level);

        public Property FindProperty(PropertyId id)
        {
            var target = Properties.SingleOrDefault(i => i.Id == id && i.Element == ElementId.Physic);

            return target;
        }

        public Property FindProperty(PropertyId id, ElementId element)
        {
            var target = Properties.SingleOrDefault(i => i.Id == id && i.Element == element);

            return target;
        }

        public string GetLocalizedName(string language)
        {
            var localized = Localization.SingleOrDefault(i => i.Language == language) ?? Localization.SingleOrDefault(i => i.Language == "English");

            return localized == null ? Id : localized.Value;
        }

        public List<string> MetaToList()
        {
            return Meta.IsEmpty() ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(Meta);
        }

        public ItemParams Copy()
        {
            return new ItemParams
            {
                Id = Id,
                Level = Level,
                Rarity = Rarity,
                Type = Type,
                Class = Class,
                Tags = Tags.ToList(),
                Properties = Properties.Select(i => i.Copy()).ToList(),
                Price = Price,
                Weight = Weight,
                Material = Material,
                IconId = IconId,
                SpriteId = SpriteId,
                Meta = Meta
            };
        }
    }
}