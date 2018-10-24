using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;

namespace ELine
{
    public class MainMenu : MonoBehaviour
    {
        #region InspectorMembers
        [SerializeField]
        private GameObject m_selectedLevelName;

        [SerializeField]
        private GameObject m_selectedLevelVideo;

        [SerializeField]
        private List<string> m_levelNames = new List<string>();

        [SerializeField]
        private List<VideoClip> m_previewVideos = new List<VideoClip>();

        [Header("Camera Settings")]
        [SerializeField]
        private GameObject m_sceneCamera;

        [SerializeField]
        private GameObject m_dummyCamera;

        [SerializeField]
        private GameObject m_mainMenuCanvas;

        [SerializeField]
        [Range(.5f, 5f)]
        private float m_cameraMoveSpeed = 1f;
        #endregion InspectorMembers

        #region PrivateMembers
        private int m_curSceneIndex;
        private int m_sceneCount;
        private Text curLevelName;
        private VideoPlayer curLevelVideo;
        private Vector3 m_cameraMenuPosition = new Vector3( 484f, 7.41f, 342f );
        private Quaternion m_cameraMenuRotation;
        #endregion PrivateMembers

        #region Methods
        public void Awake()
        {
            m_curSceneIndex = 1;
            m_sceneCount = m_levelNames.Count;
            curLevelName = m_selectedLevelName.GetComponent<Text>();
            curLevelVideo = m_selectedLevelVideo.GetComponent<VideoPlayer>();
            UpdateSelectedLevel();
            m_sceneCamera.transform.SetPositionAndRotation( m_cameraMenuPosition, m_sceneCamera.transform.rotation );
            m_cameraMenuRotation = m_sceneCamera.transform.rotation;
        }


        public void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
            #if UNITY_STANDALONE && !UNITY_EDITOR
                Application.Quit();
            #endif
        }

        public void NextGame()
        {
            m_curSceneIndex += 1;
            if (m_curSceneIndex > m_sceneCount)
            {
                m_curSceneIndex = 1;
            }
            UpdateSelectedLevel();
        }

        public void PrevGame()
        {
            m_curSceneIndex -= 1;
            if (m_curSceneIndex < 1)
            {
                m_curSceneIndex = m_sceneCount;
            }
            UpdateSelectedLevel();
        }

        public void UpdateSelectedLevel()
        {
            curLevelName.text = m_levelNames[m_curSceneIndex - 1];
            curLevelVideo.clip = m_previewVideos[m_curSceneIndex - 1];
        }

        public void LoadSelectedLevel()
        {
            m_mainMenuCanvas.GetComponent<Canvas>().enabled = false;
            StopCoroutine( "MoveCamera" );
            StartCoroutine( "MoveCamera" );
            SceneManager.LoadSceneAsync( m_curSceneIndex, LoadSceneMode.Single );
        }
        #endregion Methods

        #region Coroutines
        IEnumerator MoveCamera()
        {
            float percentJourney = 0f;
            float startTime;
            float journeyLength;
            float distCovered;
            startTime = Time.time;
            journeyLength = Vector3.Distance( m_cameraMenuPosition, m_dummyCamera.transform.position );
            while (percentJourney < 1f)
            {
                distCovered = ( Time.time - startTime ) * m_cameraMoveSpeed;
                percentJourney = distCovered / journeyLength;
                m_sceneCamera.transform.position = Vector3.Lerp(m_cameraMenuPosition, m_dummyCamera.transform.position, percentJourney);
                m_sceneCamera.transform.rotation = Quaternion.Lerp( m_cameraMenuRotation, m_dummyCamera.transform.rotation, percentJourney );
                yield return new WaitForEndOfFrame();
            }
        }
        #endregion Coroutines
    }
}
