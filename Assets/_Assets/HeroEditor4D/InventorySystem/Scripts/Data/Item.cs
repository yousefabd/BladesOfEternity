using System;
using Assets.HeroEditor4D.Common.Scripts.Data;
using Assets.HeroEditor4D.InventorySystem.Scripts.Enums;
using Newtonsoft.Json;

namespace Assets.HeroEditor4D.InventorySystem.Scripts.Data
{
    /// <summary>
    /// Represents item object for storing with game profile (please note, that item params are stored separately in params database).
    /// </summary>
    [Serializable]
    public class Item
    {
        public string Id; // Id is not unique. Use Hash to compare items!
        public Modifier Modifier;

        #if TAP_HEROES

        public ProtectedInt Count;

        #else

        public int Count;

        #endif

        public Item()
        {
        }

        public Item(string id, int count = 1)
        {
            Id = id;
            Count = count;
        }

        public Item(string id, Modifier modifier, int count = 1)
        {
            Id = id;
            Count = count;
            Modifier = modifier;
        }

        public Item Clone()
        {
            return new Item(Id, Modifier, Count);
        }

        [JsonIgnore] public ItemParams Params => ItemCollection.Active.GetItemParams(this);
        [JsonIgnore] public ItemSprite Sprite => ItemCollection.Active.GetItemSprite(this);
        [JsonIgnore] public ItemIcon Icon => ItemCollection.Active.GetItemIcon(this);

        [JsonIgnore] public int Hash => $"{Id}.{Modifier?.Id}.{Modifier?.Level}".GetHashCode();
        [JsonIgnore] public bool IsModified => Modifier != null && Modifier.Id != ItemModifier.None;
        [JsonIgnore] public bool IsEquipment => Params.Type == ItemType.Helmet || Params.Type == ItemType.Armor || Params.Type == ItemType.Vest || Params.Type == ItemType.Bracers || Params.Type == ItemType.Leggings || Params.Type == ItemType.Weapon || Params.Type == ItemType.Shield;
        [JsonIgnore] public bool IsArmor => Params.Type == ItemType.Helmet || Params.Type == ItemType.Armor || Params.Type == ItemType.Vest || Params.Type == ItemType.Bracers || Params.Type == ItemType.Leggings;
        [JsonIgnore] public bool IsWeapon => Params.Type == ItemType.Weapon;
        [JsonIgnore] public bool IsShield => Params.Type == ItemType.Shield;
        [JsonIgnore] public bool IsDagger => Params.Class == ItemClass.Dagger;
        [JsonIgnore] public bool IsSword => Params.Class == ItemClass.Sword;
        [JsonIgnore] public bool IsAxe => Params.Class == ItemClass.Axe;
        [JsonIgnore] public bool IsPickaxe => Params.Class == ItemClass.Pickaxe;
        [JsonIgnore] public bool IsWand => Params.Class == ItemClass.Wand;
        [JsonIgnore] public bool IsBlunt => Params.Class == ItemClass.Blunt;
        [JsonIgnore] public bool IsLance => Params.Class == ItemClass.Lance;
        [JsonIgnore] public bool IsMelee => Params.Type == ItemType.Weapon && Params.Class != ItemClass.Bow && Params.Class != ItemClass.Firearm;
        [JsonIgnore] public bool IsBow => Params.Class == ItemClass.Bow;
        [JsonIgnore] public bool IsFirearm => Params.Class == ItemClass.Firearm;
        [JsonIgnore] public bool IsOneHanded => !IsTwoHanded;
        [JsonIgnore] public bool IsTwoHanded => Params.Class == ItemClass.Bow || Params.Tags.Contains(ItemTag.TwoHanded);
    }
}