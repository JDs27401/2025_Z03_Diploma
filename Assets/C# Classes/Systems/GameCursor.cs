using UnityEngine;
using UnityEngine.EventSystems;

namespace C__Classes.Systems
{
    public class GameCursor : MonoBehaviour
    {
        [Tooltip("Game Crosshair Texture")]
        public Texture2D crosshairTexture; 
    
        [Tooltip("Default Cursor Texture")]
        public Texture2D defaultCursorTexture;

        private Vector2 crosshairHotSpot;
        private Vector2 defaultHotSpot;

        void Start()
        {
            if (crosshairTexture != null)
            {
                crosshairHotSpot = new Vector2(crosshairTexture.width / 2f, crosshairTexture.height / 2f);
            }
            defaultHotSpot = Vector2.zero; 
        }

        void Update()
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                Cursor.SetCursor(defaultCursorTexture, defaultHotSpot, CursorMode.Auto);
            }
            else
            {
                Cursor.SetCursor(crosshairTexture, crosshairHotSpot, CursorMode.Auto);
            }
        }
    }
}