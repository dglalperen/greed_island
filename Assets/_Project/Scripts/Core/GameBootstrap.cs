using UnityEngine;
using UnityEngine.SceneManagement;

namespace GreedIsland.Core
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private string sceneToLoad = GameConstants.PrototypeSceneName;
        [SerializeField] private bool useSingleLoad = true;

        private void Start()
        {
            TryLoadPrototype();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void BootstrapOnSceneLoaded()
        {
            var activeScene = SceneManager.GetActiveScene();
            if (activeScene.name != GameConstants.BootstrapSceneName)
            {
                return;
            }

            SceneManager.LoadSceneAsync(GameConstants.PrototypeSceneName, LoadSceneMode.Single);
        }

        private void TryLoadPrototype()
        {
            if (SceneManager.GetActiveScene().name != GameConstants.BootstrapSceneName)
            {
                return;
            }

            var mode = useSingleLoad ? LoadSceneMode.Single : LoadSceneMode.Additive;
            SceneManager.LoadSceneAsync(sceneToLoad, mode);
        }
    }
}
