using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ELine
{
	public class RaceManager : MonoBehaviourSingleton<RaceManager>
	{
		#region inspector members
		[SerializeField]
		private Spline m_splineGuide;

		[SerializeField]
		private int m_lapCount = 3;

		[Header("UI")]
		[SerializeField]
		private GameObject m_raceUIRoot;

		[SerializeField]
		private Text m_placementText;

		[SerializeField]
		private Text m_lapText;

		[SerializeField]
		private GameObject m_playerWonUI;

		[SerializeField]
		private GameObject m_playerLostUI;

		[SerializeField]
		private GameObject m_pauseMenuUI;

        [SerializeField]
        private GameObject m_countdownLabel;

        [SerializeField]
        [Range(.5f, 3f)]
        private float m_countdownSeconds = 1f;
        #endregion inspector members



        #region private members
        private List<Racer> m_activeRacers = new List<Racer>();
		private Dictionary<Racer, int> m_racerCurrentLapLookup = new Dictionary<Racer, int>();
		private Dictionary<Racer, float> m_racerLapProgressLookup = new Dictionary<Racer, float>();

		private Racer m_playerRacer;
		private bool m_raceFinished = false;

		private string[] m_positionStrings = new string[4] { "st", "nd", "rd", "th" };
		#endregion private members

		#region properties
		public Spline SplineGuide { get { return m_splineGuide; } }
		public List<Racer> ActiveRacers { get { return m_activeRacers; } }
        public Racer PlayerRacer { get { return m_playerRacer; } }
		#endregion properties

		#region events
		#endregion events

		#region constructors
		#endregion constructors

		#region methods
		public void RegisterRacer( Racer racer )
		{
			if( m_activeRacers.Contains( racer ) == false )
			{
				m_activeRacers.Add( racer );
				m_racerCurrentLapLookup.Add( racer, 1 );
				m_racerLapProgressLookup.Add( racer, 0 );
			}

			if( racer is Racer_Player )
			{
				m_playerRacer = racer;
			}
		}

		public void UnregisterRacer( Racer racer )
		{
			if( m_activeRacers.Contains( racer ) == true )
			{
				m_activeRacers.Remove( racer );
				m_racerCurrentLapLookup.Remove( racer );
				m_racerLapProgressLookup.Remove( racer );
			}
		}

		private void TrackRacerLapProgress( Racer racer )
		{
			float lapT = m_splineGuide.GetClosestPointParam( racer.transform.position, 5 );
			if( lapT - m_racerLapProgressLookup[racer] < 0.1f )
			{
				if( m_racerLapProgressLookup[racer] > 0.9f && lapT < 0.1f )
				{
					m_racerCurrentLapLookup[racer]++;
					m_racerLapProgressLookup[racer] = lapT;
				}
				else
				{
					m_racerLapProgressLookup[racer] = lapT;
				}
			}
		}

		public float GetRacerProgressDistance( Racer racer )
		{
			if( m_activeRacers.Contains( racer ) == true )
			{
				float progress = ( ( m_racerCurrentLapLookup[racer] - 1) * m_splineGuide.Length ) + ( m_racerLapProgressLookup[racer] * m_splineGuide.Length );
				return Mathf.Clamp( progress, 0, m_lapCount * m_splineGuide.Length );
			}

			return 0;
		}

		public float GetRacerProgressNormalized( Racer racer )
		{
			if( m_activeRacers.Contains( racer ) == true )
			{
				return Mathf.Clamp01( GetRacerProgressDistance( racer ) / ( m_splineGuide.Length * m_lapCount ) );
			}

			return 0;
		}

		private string GetRacerPlacementString( Racer racer )
		{
			int placementIndex = 1;

			for( int i = 0; i < m_activeRacers.Count; i++ )
			{
				if( racer == m_activeRacers[i] )
				{
					continue;
				}

				if( GetRacerProgressDistance( m_activeRacers[i] ) > GetRacerProgressDistance( racer ) )
				{
					placementIndex++;
				}
			}

			// string for anything from 4 to 20 has 'th'
			return string.Format( "<b>{0}</b><size=50>{1}</size>", placementIndex, m_positionStrings[Mathf.Clamp(placementIndex, 1, 4) - 1] );
		}

		private void RaceFinished( Racer winner )
		{
			Time.timeScale = 0.0f;

			m_raceUIRoot.SetActive( false );

			if( winner is Racer_Player )
			{
				m_playerWonUI.SetActive( true );
			}
			else
			{
				m_playerLostUI.SetActive( true );
			}
		}

		public void RestartRace()
		{
            Debug.Log( "Beginning Race Restart" );
            Time.timeScale = 1.0f;
			SceneManager.LoadScene( SceneManager.GetActiveScene().name );
            Debug.Log( "Done restarting race" );
		}

		public void ExitGame()
		{
            Debug.Log( "Loading main menu" );
            Time.timeScale = 1f;
            SceneManager.LoadSceneAsync( 0, LoadSceneMode.Single );
            Debug.Log( "Finished loading main menu" );
        }

        IEnumerator DoCountdown()
        {
            Text countdownText = m_countdownLabel.GetComponent<Text>();
            Racer_AI[] aiRacerScripts = GameObject.FindObjectsOfType<Racer_AI>();
            Racer_Player[] playerRacerScripts = GameObject.FindObjectsOfType<Racer_Player>();
            foreach (Racer_AI item in aiRacerScripts)
            {
                item.enabled = false;
            }
            foreach (Racer_Player item in playerRacerScripts)
            {
                item.enabled = false;
            }
            countdownText.text = "3";
            yield return new WaitForSeconds( m_countdownSeconds );
            countdownText.text = "2";
            yield return new WaitForSeconds( m_countdownSeconds );
            countdownText.text = "1";
            yield return new WaitForSeconds( m_countdownSeconds );
            countdownText.text = "Go!";
            foreach (Racer_AI item in aiRacerScripts)
            {
                item.enabled = true;
            }
            foreach (Racer_Player item in playerRacerScripts)
            {
                item.enabled = true;
            }
            yield return new WaitForSeconds( m_countdownSeconds * 2f);
            countdownText.enabled = false;
        }
        #endregion methods

        #region monobehaviour callbacks
        private void OnEnable()
        {
            StartCoroutine( "DoCountdown" );
        }

        private void Update()
		{

            if ( m_raceFinished == true )
			{
				return;
			}

			// Pause logic
			if( Input.GetKeyDown( KeyCode.Escape ) == true )
			{
                Debug.Log( "Set pausemenuUi to active" );
                m_pauseMenuUI.SetActive( !m_pauseMenuUI.activeSelf );
                Debug.Log( "Set Race UI Root to active" );
				m_raceUIRoot.SetActive( !m_pauseMenuUI.activeSelf );
                Debug.Log( "Set timescale to zero" );
				Time.timeScale = m_pauseMenuUI.activeSelf ? 0.0f : 1.0f;
			}

			// Has a racer won the race?
			for( int i = 0; i < m_activeRacers.Count; i++ )
			{
				TrackRacerLapProgress( m_activeRacers[i] );
				if( m_racerCurrentLapLookup[m_activeRacers[i]] > m_lapCount )
				{
					m_raceFinished = true;
					RaceFinished( m_activeRacers[i] );
					return;
				}
			}

			// Update UI
			if( m_playerRacer != null )
			{
				m_placementText.text = GetRacerPlacementString( m_playerRacer );
				m_lapText.text = string.Format( "Lap: {0} / {1}", m_racerCurrentLapLookup[m_playerRacer], m_lapCount );
			}
		}
		#endregion monobehaviour callbacks
	}
}