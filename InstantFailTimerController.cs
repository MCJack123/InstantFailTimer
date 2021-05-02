using System.Linq;
using UnityEngine;
using BS_Utils.Utilities;

namespace InstantFailTimer {
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.
    /// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
    /// </summary>
    public class InstantFailTimerController : MonoBehaviour {
        public static InstantFailTimerController Instance { get; private set; }

        public void OnGameSceneActive() {
            BSEvents.noteWasMissed += OnNoteWasMissed;
            BSEvents.levelCleared += OnLevelCleared;
        }

        private void OnNoteWasMissed(NoteData data, int score) {
            Plugin.Log.Debug(data.time.ToString());
            if (data.time < Configuration.PluginConfig.Instance.failTime) {
                Resources.FindObjectsOfTypeAll<StandardLevelGameplayManager>().LastOrDefault()?.HandleGameEnergyDidReach0();
                Resources.FindObjectsOfTypeAll<MissionLevelGameplayManager>().LastOrDefault()?.HandleGameEnergyDidReach0();
            }
            BSEvents.noteWasMissed -= OnNoteWasMissed;
        }

        private void OnLevelCleared(StandardLevelScenesTransitionSetupDataSO data, LevelCompletionResults res) {
            BSEvents.noteWasMissed -= OnNoteWasMissed;
            BSEvents.levelCleared -= OnLevelCleared;
        }

        // These methods are automatically called by Unity, you should remove any you aren't using.
        #region Monobehaviour Messages
        /// <summary>
        /// Only ever called once, mainly used to initialize variables.
        /// </summary>
        private void Awake() {
            // For this particular MonoBehaviour, we only want one instance to exist at any time, so store a reference to it in a static property
            //   and destroy any that are created while one already exists.
            if (Instance != null) {
                Plugin.Log?.Warn($"Instance of {GetType().Name} already exists, destroying.");
                GameObject.DestroyImmediate(this);
                return;
            }
            GameObject.DontDestroyOnLoad(this); // Don't destroy this object on scene changes
            Instance = this;
            Plugin.Log?.Debug($"{name}: Awake()");
        }

        /// <summary>
        /// Called when the script is being destroyed.
        /// </summary>
        private void OnDestroy() {
            Plugin.Log?.Debug($"{name}: OnDestroy()");
            if (Instance == this)
                Instance = null; // This MonoBehaviour is being destroyed, so set the static instance property to null.

        }
        #endregion
    }
}
