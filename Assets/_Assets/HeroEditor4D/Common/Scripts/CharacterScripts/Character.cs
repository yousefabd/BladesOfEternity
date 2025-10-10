using System;
using System.Collections.Generic;
using System.Linq;
using Assets.HeroEditor4D.Common.Scripts.Collections;
using Assets.HeroEditor4D.Common.Scripts.Data;
using Assets.HeroEditor4D.Common.Scripts.Enums;
using UnityEngine;

namespace Assets.HeroEditor4D.Common.Scripts.CharacterScripts
{
    /// <summary>
    /// Character presentation in editor. Contains sprites, renderers, animation and so on.
    /// </summary>
    public partial class Character : MonoBehaviour
    {
        public SpriteCollection SpriteCollection;

        [Header("Body")]
        public List<Sprite> Ears;
        public Sprite Hair;
        public Sprite HairCropped;
        public Sprite Beard;
        public List<Sprite> Body;
        public Sprite Head;

        [Header("Expressions")]
        public string Expression = "Default";
        public List<Expression> Expressions;

        [Header("Equipment")]
        public Sprite Helmet;
        public Sprite PrimaryWeapon;
        public Sprite SecondaryWeapon;
        public Sprite Cape;
        public Sprite Quiver;
        public Sprite Backpack;
        public List<Sprite> Shield;
        public List<Sprite> Armor;
        public List<Sprite> CompositeWeapon;

        [Header("Accessories")]
        public Sprite Makeup;
        public Sprite Mask;
        public List<Sprite> Earrings;

        [Header("Body renderers")]
        public List<SpriteRenderer> BodyRenderers;
        public SpriteRenderer HeadRenderer;
        public List<SpriteRenderer> EarsRenderers;
        public SpriteRenderer HairRenderer;
        public SpriteRenderer BeardRenderer;
        public SpriteRenderer EyebrowsRenderer;
        public SpriteRenderer EyesRenderer;
        public SpriteRenderer MouthRenderer;

        [Header("Equipment renderers")]
        public SpriteRenderer HelmetRenderer;
        public SpriteRenderer PrimaryWeaponRenderer;
        public SpriteRenderer SecondaryWeaponRenderer;
        public List<SpriteRenderer> ArmorRenderers;
        public List<SpriteRenderer> VestRenderers;
        public List<SpriteRenderer> BracersRenderers;
        public List<SpriteRenderer> LeggingsRenderers;
        //public SpriteRenderer CapeRenderer;
        public SpriteRenderer QuiverRenderer;
        public SpriteRenderer BackpackRenderer;
        public List<SpriteRenderer> ShieldRenderers;
        public List<SpriteRenderer> BowRenderers;
        public List<SpriteRenderer> CrossbowRenderers;

        [Header("Accessories renderers")]
        public SpriteRenderer MakeupRenderer;
        public SpriteRenderer MaskRenderer;
        public List<SpriteRenderer> EarringsRenderers;

        [Header("Materials")]
        public Material DefaultMaterial;
        public Material EyesPaintMaterial;
        public Material EquipmentPaintMaterial;

        [Header("Animation")]
        public WeaponType WeaponType;

        [Header("Meta")]
        public bool HideEars;
        public bool CropHair;

        [Header("Anchors")]
	    public Transform AnchorBody;
	    public Transform AnchorSword;
	    public Transform AnchorBow;
        public Transform AnchorFireMuzzle;

        [Header("Service")]
		public LayerManager LayerManager;

        [Header("Custom")]
        public List<Sprite> Underwear;
        public Color UnderwearColor;
        public bool ShowHelmet = true;

        /// <summary>
        /// Initializes character renderers with selected sprites.
        /// </summary>
        public void Initialize()
        {
            try // Disable try/catch for debugging.
            {
                TryInitialize();
            }
            catch (Exception e)
            {
                Debug.LogWarningFormat("Unable to initialize character {0}: {1}", name, e.Message);
            }
        }

		/// <summary>
		/// Initializes character renderers with selected sprites.
		/// </summary>
		private void TryInitialize()
        {
            var expressionNames = Expressions.Select(i => i.Name).ToList();

            if (!expressionNames.Contains("Default") || !expressionNames.Contains("Angry") || !expressionNames.Contains("Dead"))
            {
                throw new Exception("Character must have at least 3 basic expressions: Default, Angry and Dead.");
            }

            if (ShowHelmet)
            {
                MapSprites(EarsRenderers, HideEars ? null : Ears);
                SetSprite(HairRenderer, Hair == null ? null : CropHair ? HairCropped : Hair);
                SetSprite(HelmetRenderer, Helmet);
			}
            else
            {
                MapSprites(EarsRenderers, Ears);
                SetSprite(HairRenderer, Hair);
                SetSprite(HelmetRenderer, null);
			}

            SetExpression(Expression);
			SetSprite(BeardRenderer, Beard);
			MapSprites(BodyRenderers, Body);
            SetSprite(HeadRenderer, Head);
            MapSprites(ArmorRenderers, Armor);
            //CapeRenderer.sprite = Cape;
            SetSprite(QuiverRenderer, Quiver);
            SetSprite(BackpackRenderer, Backpack);
            MapSprites(ShieldRenderers, Shield);
			SetSprite(MakeupRenderer, Makeup);
            SetSprite(MaskRenderer, Mask);
            MapSprites(EarringsRenderers, Earrings);

            SetSprite(PrimaryWeaponRenderer, null);
            SetSprite(SecondaryWeaponRenderer, null);
            BowRenderers.ForEach(i => SetSprite(i, null));
            CrossbowRenderers.ForEach(i => SetSprite(i, null));

            switch (WeaponType)
            {
                case WeaponType.Melee1H:
                case WeaponType.Melee2H:
                case WeaponType.Firearm1H:
                case WeaponType.Firearm2H:
                case WeaponType.Throwable:
                    SetSprite(PrimaryWeaponRenderer, PrimaryWeapon);
                    break;
                case WeaponType.Paired:
                    SetSprite(PrimaryWeaponRenderer, PrimaryWeapon);
                    SetSprite(SecondaryWeaponRenderer, SecondaryWeapon);
                    break;
                case WeaponType.Bow:
                    MapSprites(BowRenderers, CompositeWeapon);
                    break;
                case WeaponType.Crossbow:
                    MapSprites(CrossbowRenderers, CompositeWeapon);
                    break;
                default:
                    throw new NotImplementedException(WeaponType.ToString());
            }

            ApplyMaterials();
		}

        private static void SetSprite(SpriteRenderer spriteRenderer, Sprite sprite)
        {
            if (spriteRenderer == null) return;

            spriteRenderer.sprite = sprite;
            spriteRenderer.enabled = sprite != null;
        }

		private void MapSprites(List<SpriteRenderer> spriteRenderers, List<Sprite> sprites)
        {
            spriteRenderers.ForEach(i => MapSprite(i, sprites));
        }

        private void MapSprite(SpriteRenderer spriteRenderer, List<Sprite> sprites)
        {
            SetSprite(spriteRenderer, sprites == null ? null : spriteRenderer.GetComponent<SpriteMapping>().FindSprite(sprites));
        }

        private void ApplyMaterials()
        {
            var renderers = ArmorRenderers.ToList();

            renderers.Add(HairRenderer);
            renderers.Add(PrimaryWeaponRenderer);
            renderers.Add(SecondaryWeaponRenderer);
            renderers.ForEach(i => i.sharedMaterial = i.color == Color.white ? DefaultMaterial : EquipmentPaintMaterial);
        }

        public void CopyFrom(Character character)
        {
            if (character == null) throw new ArgumentNullException(nameof(character), "Can't copy from empty character!");

            Body = character.Body.ToList();
            Ears = character.Ears.ToList();
            Hair = character.Hair;
            Expression = character.Expression;
            Expressions = character.Expressions.ToList();
            Beard = character.Beard;

            Helmet = character.Helmet;
            Armor = character.Armor.ToList();
            PrimaryWeapon = character.PrimaryWeapon;
            SecondaryWeapon = character.SecondaryWeapon;
            Cape = character.Cape;
            Quiver = character.Quiver;
            Backpack = character.Backpack;
            Shield = character.Shield.ToList();
            CompositeWeapon = character.CompositeWeapon.ToList();

            Makeup = character.Makeup;
            Mask = character.Mask;
            Earrings = character.Earrings.ToList();

            foreach (var target in GetComponentsInChildren<SpriteRenderer>(true).Where(i => i.sprite != null))
            {
                foreach (var source in character.GetComponentsInChildren<SpriteRenderer>(true))
                {
                    if (target.name == source.name && target.transform.parent.name == source.transform.parent.name)
                    {
                        target.color = source.color;
                        break;
                    }
                }
            }

            WeaponType = character.WeaponType;
            HideEars = character.HideEars;
            CropHair = character.CropHair;
            Initialize();
            SetExpression("Default");
        }
    }
}