using System;
using System.Collections.Generic;
using System.Linq;
using Assets.HeroEditor4D.Common.Scripts.Collections;
using Assets.HeroEditor4D.Common.Scripts.Common;
using Assets.HeroEditor4D.Common.Scripts.Data;
using UnityEngine;

namespace Assets.HeroEditor4D.Common.Scripts.CharacterScripts
{
    public class AvatarSetup : MonoBehaviour
    {
        public List<SpriteCollection> SpriteCollections;
        public SpriteRenderer Head;
        public SpriteRenderer Hair;
        public List<SpriteRenderer> Ears;
        public SpriteRenderer Eyes;
        public SpriteRenderer Eyebrows;
        public SpriteRenderer Mouth;
        public SpriteRenderer Beard;
        public SpriteRenderer Helmet;

        public void Initialize(CharacterAppearance appearance, string helmetId)
        {
            if (SpriteCollections.Count == 0) throw new Exception("Please set sprite collections for avatar setup.");

            var ears = FindItemSprite(i => i.Ears, appearance.Ears).Sprites[1];

            Head.sprite = FindItemSprite(i => i.Body, appearance.Body).Sprites.Single(i => i.name == "FrontHead");
            Head.color = Ears[0].color = Ears[1].color = appearance.BodyColor;

            ItemSprite hair = null;

            if (appearance.Hair.IsEmpty())
            {
                Hair.enabled = false;
            }
            else
            {
                hair = FindItemSprite(i => i.Hair, appearance.Hair);
                Hair.enabled = true;
                Hair.sprite = hair.Sprites[1];
                Hair.color = hair.Tags.Contains("NoPaint") ? (Color32) Color.white : appearance.HairColor;
            }

            Beard.sprite = appearance.Beard.IsEmpty() ? null : FindItemSprite(i => i.Beard, appearance.Beard).Sprite;
            Beard.color = appearance.BeardColor;
            Eyes.sprite = FindItemSprite(i => i.Eyes, appearance.Eyes).Sprite;
            Eyes.color = appearance.EyesColor;

            if (appearance.Eyebrows.IsEmpty())
            {
                Eyebrows.enabled = false;
            }
            else
            {
                Eyebrows.enabled = true;
                Eyebrows.sprite = FindItemSprite(i => i.Eyebrows, appearance.Eyebrows).Sprite;
            }

            Mouth.sprite = FindItemSprite(i => i.Mouth, appearance.Mouth).Sprite;
            Mouth.transform.localPosition = new Vector3(0, appearance.Type == 0 ?  -0.1f : 0.25f);

            if (helmetId == null)
            {
                var hideEars = hair != null && hair.Tags.Contains("HideEars");

                Helmet.enabled = false;
                Ears.ForEach(j => { j.sprite = ears; j.enabled = !hideEars; });
            }
            else
            {
                Helmet.enabled = true;

                var helmet = FindItemSprite(i => i.Armor, helmetId);
                var showEars = helmet.Tags.Contains("ShowEars");

                Helmet.sprite = helmet.Sprites.Single(i => i.name == "FrontHead");
                Ears.ForEach(j => { j.sprite = ears; j.enabled = showEars; });

                if (!appearance.Hair.IsEmpty() && !helmet.Tags.Contains("FullHair"))
                {
                    Hair.sprite = FindItemSprite(i => i.Hair, "Common.Basic.Hair.Default").Sprites[1];
                    Hair.enabled = Hair.sprite != null;
                }
            }

            Ears[0].transform.localPosition = appearance.Type == 0 ? new Vector3(-1f, 0.5f) : new Vector3(-0.9f, 0.7f);
            Ears[1].transform.localPosition = appearance.Type == 0 ? new Vector3(1f, 0.5f) : new Vector3(0.9f, 0.7f);
        }

        private ItemSprite FindItemSprite(Func<SpriteCollection, IEnumerable<ItemSprite>> selector, string id)
        {
            foreach (var collection in SpriteCollections)
            {
                var sprite = selector(collection).FirstOrDefault(i => i.Id == id);

                if (sprite != null) return sprite;
            }

            throw new Exception($"Can't find {id}.");
        }
    }
}