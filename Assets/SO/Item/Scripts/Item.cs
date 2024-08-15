using UnityEngine;

namespace ItemSpace
{
    [CreateAssetMenu(menuName = "Game/Item")]
    public class Item : ScriptableObject
    {
        public int Value;
        public Sprite Sprite;
    }  
}

