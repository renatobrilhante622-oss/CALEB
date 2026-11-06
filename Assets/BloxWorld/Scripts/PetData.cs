using UnityEngine;

namespace BloxWorld
{
    public enum PetElement { None, Fire, Ice, Crystal, Candy, Robot, Hero }

    [CreateAssetMenu(menuName = "BloxWorld/Pet", fileName = "Pet")]
    public sealed class PetData : ScriptableObject
    {
        [Tooltip("Permanent unique ID. Never rename after releasing a save.")]
        public string id = "dog";
        public string displayName = "Cachorrinho";
        public Sprite icon;
        public Color bodyColor = new Color(0.8f, 0.5f, 0.2f);
        [Min(0)] public int coinsPerSecond = 1;
        [Min(1)] public int attack = 1;
        public PetElement element;
        public bool isBoss;
    }
}
