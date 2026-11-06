using System;
using System.Collections.Generic;
using UnityEngine;

namespace BloxWorld
{
    [Serializable]
    public sealed class OwnedPet
    {
        public string instanceId;
        public string definitionId;
        public PetElement element;
        public int level = 1;
        public bool equipped;
        public string parentA;
        public string parentB;
    }

    [Serializable]
    public sealed class AvatarData
    {
        public Color head = new Color(1f, 0.8f, 0.35f);
        public Color shirt = new Color(0.15f, 0.5f, 1f);
        public Color trousers = new Color(0.2f, 0.25f, 0.45f);
        public bool hat;
        public bool wings;
        public bool trail;
        public int face;
    }

    [Serializable]
    public sealed class SaveData
    {
        public int version = 1;
        public long coins;
        public int treadmillLevel = 1;
        public int habitatLevel = 1;
        public int speedLevel = 1;
        public int strengthLevel = 1;
        public int sizeLevel = 1;
        public int auraLevel;
        public int petSlots = 3;
        public bool calmMode = true;
        public AvatarData avatar = new AvatarData();
        public List<OwnedPet> pets = new List<OwnedPet>();
        public List<string> unlockedPetIds = new List<string>();
        public List<string> buildings = new List<string>();
        public List<string> eggs = new List<string>();

        public void Normalize()
        {
            coins = Math.Max(0, coins);
            treadmillLevel = Mathf.Clamp(treadmillLevel, 1, 4);
            habitatLevel = Math.Max(1, habitatLevel);
            speedLevel = Math.Max(1, speedLevel);
            strengthLevel = Math.Max(1, strengthLevel);
            sizeLevel = Math.Max(1, sizeLevel);
            auraLevel = Math.Max(0, auraLevel);
            petSlots = Mathf.Clamp(petSlots, 3, 8);
            avatar = avatar ?? new AvatarData();
            pets = pets ?? new List<OwnedPet>();
            unlockedPetIds = unlockedPetIds ?? new List<string>();
            buildings = buildings ?? new List<string>();
            eggs = eggs ?? new List<string>();
            pets.RemoveAll(p => p == null || string.IsNullOrWhiteSpace(p.definitionId));
            var ids = new HashSet<string>();
            int equippedCount = 0;
            foreach (var pet in pets)
            {
                if (string.IsNullOrWhiteSpace(pet.instanceId) || !ids.Add(pet.instanceId))
                {
                    pet.instanceId = Guid.NewGuid().ToString("N");
                    ids.Add(pet.instanceId);
                }
                pet.level = Math.Max(1, pet.level);
                if (pet.equipped && ++equippedCount > petSlots) pet.equipped = false;
            }
        }
    }
}
