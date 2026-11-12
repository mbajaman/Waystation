using System.Collections.Generic;
using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "Waystation/Item Definition", order = 0)]
    public class ItemDefinition : ScriptableObject
    {
        public string displayName = "Give me a name";
        public Sprite icon;
        public ClothingType type;
        // enum of set colors for now
        public ItemColor color;
    }
}