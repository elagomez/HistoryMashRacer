using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELine
{
	public class Pickup_Powerup : MonoBehaviour
	{
		#region inspector members
		[SerializeField]
		private List<GameObject> m_powerupPrefabs = new List<GameObject>();

		[SerializeField]
		private float m_collectedCooldownDuration = 5.0f;

		[SerializeField]
		private GameObject m_boxVisuals;

		[SerializeField]
		private GameObject m_particleEffect;
		#endregion inspector members

		#region private members
		private int m_randomPowerupIndex;
		private Vector3 m_boxVisualsOriginalPosition;
		private float m_randomOffset;
		private bool m_enabled = true;
		#endregion private members

		#region constructors
		private void Start()
		{
			m_boxVisualsOriginalPosition = m_boxVisuals.transform.position;
			m_randomOffset = UnityEngine.Random.Range( -100.0f, 100.0f );
			m_randomPowerupIndex = UnityEngine.Random.Range( 0, m_powerupPrefabs.Count );
		}
		#endregion constructors

		#region methods
		private IEnumerator PickupCollected()
		{
			m_enabled = false;
			m_boxVisuals.SetActive( false );
			m_particleEffect.SetActive( true );

			yield return new WaitForSeconds( m_collectedCooldownDuration );

			m_randomPowerupIndex = UnityEngine.Random.Range( 0, m_powerupPrefabs.Count );

			m_boxVisuals.SetActive( true );
			m_particleEffect.SetActive( false );
			m_enabled = true;
		}
		#endregion methods

		#region monobehaviour callbacks
		private void Update()
		{
			if( m_enabled == false )
			{
				return;
			}

			m_boxVisuals.transform.position = m_boxVisualsOriginalPosition + new Vector3( 0, 0.25f * Mathf.Sin( ( Time.time + m_randomOffset) * 5.0f ), 0 );
		}

		private void OnTriggerEnter( Collider other )
		{
			if( m_enabled == false )
			{
				return;
			}

			Racer racer = other.GetComponent<Racer>();
			if( racer != null )
			{
				if( racer.PowerupInventory.Count < racer.MaxPowerups )
				{
					GameObject powerupPrefab = m_powerupPrefabs[m_randomPowerupIndex];
					if( powerupPrefab != null )
					{
						racer.AcquirePowerup( powerupPrefab );
					}
				}

				StartCoroutine( PickupCollected() );
			}
		}
		#endregion monobehaviour callbacks
	}
}