using System;
using System.Collections.Generic;
using System.Linq;
using Scriptable_Objects;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// What the player is waearing, one item per clothing type.
    /// Matching a passport only reads Worn. Ntohing here is drawn on the player.
    /// </summary>
    public class PlayerOutfit : MonoBehaviour
    {
        [SerializeField] private ItemDefinition[] startingOutfit;
        
        private readonly Dictionary<ClothingType, ItemDefinition> _worn = new Dictionary<ClothingType, ItemDefinition>();

        /// <summary>
        /// The items currently worn. Slots that were never filled are absent.
        /// </summary>
        public IEnumerable<ItemDefinition> Worn => _worn.Values;

        private void Awake()
        {
            if (startingOutfit == null)
                return;

            for (int i = 0; i < startingOutfit.Length; i++)
                Wear(startingOutfit[i]);
        }

        /// <summary>
        /// Puts the item in its clothing slot and returns whatever was there before.
        /// A null return means the slot was empty.
        /// </summary>
        public ItemDefinition Wear(ItemDefinition item)
        {
            if (item == null)
                return null;

            _worn.TryGetValue(item.type, out ItemDefinition previous);
            _worn[item.type] = item;
            Debug.Log($"Wearing {string.Join(", ", _worn.Values.Select(worn => $"{worn.color}"))}");
            return previous;
        }
    }
}