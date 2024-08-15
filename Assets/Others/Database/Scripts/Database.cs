using ItemSpace;
using UnityEngine;

namespace DatabaseSpace
{
    public static class Database
    {
        public static Item[] Items { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            Items = Resources.LoadAll<Item>("Items/");
        }
    }   
}
