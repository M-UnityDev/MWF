using System; 
using UnityEngine; 
using UnityEngine.InputSystem; 
using Unity.Burst; 
using UnityEngine.SceneManagement;
namespace Game.Input
{
    [BurstCompile] public class InputHandler : MonoBehaviour
    {
        public static event Action<InputActionMap> OnMapChanged;
        public static PlayerControl Inputs;
        private void Awake()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            Inputs = new PlayerControl();
            ToggleActionMap(Inputs.Player);
        }
        private void OnSceneUnloaded(Scene current) => Inputs.Disable();
        public static void ToggleActionMap(InputActionMap Map)
        {
            if (Map.enabled)
                return;
            Inputs.Disable();
            Map.Enable();
            OnMapChanged?.Invoke(Map);
        }
    }
}