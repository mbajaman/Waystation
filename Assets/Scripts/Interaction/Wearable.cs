using System;
using Player;
using Scriptable_Objects;
using UnityEngine;

namespace Interaction
{
    /// <summary>
    /// A clothing prop the player puts on. The worn item is only data on PlayerOutfit.
    /// If that slot was full, thjis object becoms the item that came off.
    /// </summary>
    public class Wearable : MonoBehaviour, IInteractable
    {
        [SerializeField] private ItemDefinition item;

        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        /// <summary>
        /// Text the Interactor shows, for example "Wear Red Hat".
        /// </summary>
        public string Prompt => item != null ? $"Wear {item.displayName}" : "Wear";

        private void Awake()
        {
            ApplyTint();
        }

        /// <summary>
        /// True when this prop has an item and the player has an outfit to put it on.
        /// </summary>
        public bool CanInteract(Interactor interactor)
        {
            if (item == null)
                return false;

            return interactor.GetComponentInParent<PlayerOutfit>() != null;
        }

        /// <summary>
        /// Puts this item on the player. Removes this object when the slot was empty.
        /// </summary>
        public void Interact(Interactor interactor)
        {
            PlayerOutfit outfit = interactor.GetComponentInParent<PlayerOutfit>();
            if (outfit == null || item == null)
                return;

            ItemDefinition previous = outfit.Wear(item);
            if (previous == null)
            {
                Destroy(gameObject);
                return;
            }

            item = previous;
            ApplyTint();
        }

        private void ApplyTint()
        {
            if (item == null)
                return;

            var block = new MaterialPropertyBlock();
            block.SetColor(BaseColorId, ColorOf(item.color));
            
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
                renderers[i].SetPropertyBlock(block);
        }

        private static Color ColorOf(ItemColor color)
        {
            switch (color)
            {
                case ItemColor.Red: return Color.red;
                case ItemColor.Blue: return Color.blue;
                case ItemColor.Green: return Color.green;
                case ItemColor.Yellow: return Color.yellow;
                default: return Color.white;
            }
        }
    }
}