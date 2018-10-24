using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELine
{
	public class PowerupUIController : MonoBehaviour
	{
		#region inspector members
		[SerializeField]
		private GameObject m_speedBostIconPrefab;

		[SerializeField]
		private GameObject m_oilSlickIconPrefab;

		[SerializeField]
		private GameObject m_projectileIconPrefab;

        [SerializeField]
        private GameObject m_portalIconPrefab;

        [SerializeField]
        private GameObject m_boulderIconPrefab;

        [SerializeField]
        private GameObject m_asteroidIconPrefab;

        [SerializeField]
        private GameObject m_honeyPotIconPrefab;

        #endregion inspector members

        #region private members
        private List<GameObject> m_powerupIconInstances = new List<GameObject>();
		#endregion private members

		#region constructors
		private void Start()
		{
			if( Racer_Player.Instance != null )
			{
				Racer_Player.Instance.OnPowerupAcquired += PowerupAcquiredHandler;
				Racer_Player.Instance.OnPowerupUsed += PowerupUsedHandler;
			}
		}
		#endregion constructors

		#region methods
		private void PowerupAcquiredHandler( Powerup.PowerupType type )
		{
			switch( type )
			{
				case Powerup.PowerupType.SpeedBoost:
					m_powerupIconInstances.Add( Instantiate( m_speedBostIconPrefab, transform ) );
					break;
				case Powerup.PowerupType.OilSlick:
					m_powerupIconInstances.Add( Instantiate( m_oilSlickIconPrefab, transform ) );
					break;
				case Powerup.PowerupType.Projectile:
					m_powerupIconInstances.Add( Instantiate( m_projectileIconPrefab, transform ) );
					break;
                case Powerup.PowerupType.Portal:
                    m_powerupIconInstances.Add( Instantiate( m_portalIconPrefab, transform ) );
                    break;
                case Powerup.PowerupType.Boulder:
                    m_powerupIconInstances.Add( Instantiate( m_boulderIconPrefab, transform ) );
                    break;
                case Powerup.PowerupType.Asteroid:
                    m_powerupIconInstances.Add( Instantiate( m_asteroidIconPrefab, transform ) );
                    break;
                case Powerup.PowerupType.HoneyPot:
                    m_powerupIconInstances.Add( Instantiate( m_honeyPotIconPrefab, transform ) );
                    break;
			}
		}

		private void PowerupUsedHandler()
		{
			Destroy( m_powerupIconInstances[0] );
			m_powerupIconInstances.RemoveAt( 0 );
		}
		#endregion methods
	}
}