using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    public class IgnoreTransparentUI : MonoBehaviour
    {
        public float alphaThreshold = 0.1f;

        void Start()
        {
            Image img = GetComponent<Image>();
        
            if (img != null)
            {
                img.alphaHitTestMinimumThreshold = alphaThreshold;
            }
        }
    }
}