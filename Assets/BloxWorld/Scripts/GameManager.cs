using System;
using System.Collections.Generic;
using UnityEngine;

namespace BloxWorld
{
    [DefaultExecutionOrder(-900)]
    [RequireComponent(typeof(SaveManager))]
    public sealed class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField] PetData[] petCatalog = Array.Empty<PetData>();
        readonly Dictionary<string, PetData> catalog = new Dictionary<string, PetData>();
        SaveManager saves;
        public SaveData State => saves.Data;
        public event Action StateChanged;
        public event Action<bool> PersistenceCompleted;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            saves = GetComponent<SaveManager>();
            DontDestroyOnLoad(gameObject);
            foreach (var pet in petCatalog)
            {
                if (pet == null || string.IsNullOrWhiteSpace(pet.id))
                    throw new InvalidOperationException("Pet catalog contains an empty ID.");
                if (catalog.ContainsKey(pet.id)) throw new InvalidOperationException("Duplicate pet ID: " + pet.id);
                catalog.Add(pet.id, pet);
            }
        }

        public PetData GetPet(string id) => id != null && catalog.TryGetValue(id, out var pet) ? pet : null;
        public bool AddCoins(long amount)
        {
            if (amount <= 0 || amount > long.MaxValue - State.coins) return false;
            State.coins += amount;
            Commit();
            return true;
        }
        public bool TryPurchaseBuilding(string id, long price)
        {
            if (string.IsNullOrWhiteSpace(id) || price < 0 || State.coins < price || State.buildings.Contains(id)) return false;
            State.coins -= price;
            State.buildings.Add(id);
            Commit();
            return true;
        }
        public OwnedPet GrantPet(string definitionId, PetElement element = PetElement.None)
        {
            var definition = GetPet(definitionId);
            if (definition == null) return null;
            var pet = new OwnedPet
            {
                instanceId = Guid.NewGuid().ToString("N"), definitionId = definitionId,
                element = element == PetElement.None ? definition.element : element,
                equipped = State.pets.FindAll(p => p.equipped).Count < State.petSlots
            };
            State.pets.Add(pet);
            if (!State.unlockedPetIds.Contains(definitionId)) State.unlockedPetIds.Add(definitionId);
            Commit();
            return pet;
        }
        public bool SetEquipped(string instanceId, bool equipped)
        {
            var pet = State.pets.Find(p => p.instanceId == instanceId);
            if (pet == null || pet.equipped == equipped) return false;
            if (equipped && State.pets.FindAll(p => p.equipped).Count >= State.petSlots) return false;
            pet.equipped = equipped;
            Commit();
            return true;
        }
        public void SetCalmMode(bool enabled) { State.calmMode = enabled; Commit(); }
        public void SaveAvatar(AvatarData avatar)
        {
            if (avatar == null) throw new ArgumentNullException(nameof(avatar));
            State.avatar = JsonUtility.FromJson<AvatarData>(JsonUtility.ToJson(avatar));
            Commit();
        }
        void Commit()
        {
            bool persisted = saves.SaveNow();
            StateChanged?.Invoke();
            PersistenceCompleted?.Invoke(persisted);
        }
        void OnDestroy() { if (Instance == this) Instance = null; }
    }
}
