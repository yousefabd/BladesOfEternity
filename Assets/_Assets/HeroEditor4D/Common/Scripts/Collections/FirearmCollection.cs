using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.HeroEditor4D.Common.Scripts.Collections
{
    /// <summary>
    /// Global object that automatically grabs all required images.
    /// </summary>
    [CreateAssetMenu(fileName = "FirearmCollection", menuName = "HeroEditor4D/FirearmCollection")]
    public class FirearmCollection : ScriptableObject
    {
        public string Id;
        public List<FirearmParams> FirearmParams;

        public static Dictionary<string, FirearmCollection> Instances = new Dictionary<string, FirearmCollection>();

        public void OnEnable()
        {
            if (!Instances.ContainsKey(Id))
            {
                Instances.Add(Id, this);
            }
        }
    }

    [Serializable]
    public class FirearmParams
    {
        public string Name;
        public ParticleSystem FireMuzzlePrefab;
        public AudioClip ShotSound;
        public AudioClip ReloadSound;
    }
}