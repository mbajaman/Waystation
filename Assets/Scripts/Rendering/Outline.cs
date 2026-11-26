using UnityEngine;

namespace Rendering
{
    /// <summary>
    /// Toggles the Outline rendering layer on renderers so the outline feature draws them.
    /// </summary>
    public static class Outline
    {
        static uint _mask;
        static bool _resolved;

        /// <summary>
        /// Turns the outline on or off for every renderer in the list.
        /// </summary>
        public static void Set(Renderer[] renderers, bool on)
        {
            if (renderers == null)
                return;

            uint mask = Mask;
            if (mask == 0)
                return;

            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (renderer == null)
                    continue;

                uint bits = renderer.renderingLayerMask;
                bits = on ? bits | mask : bits & ~mask;
                renderer.renderingLayerMask = bits;
            }
        }

        // Cached so focus changes don't look the name up every time.
        static uint Mask
        {
            get
            {
                if (_resolved)
                    return _mask;

                _mask = RenderingLayerMask.GetMask("Outline");
                _resolved = true;
                if (_mask == 0)
                    Debug.LogError("No rendering layer named 'Outline'. Add one under Tags and Layers > Rendering Layers.");
                return _mask;
            }
        }
    }
}
