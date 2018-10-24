using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELine
{
	public class Powerup_OilSlick : Powerup
	{
		#region inspector members
		[SerializeField]
		private LayerMask m_trackLayerMask;

		[SerializeField]
		private AnimationCurve m_impactedRacerSpinoutCurve;

		[SerializeField]
		private GameObject m_oilSlickVisuals;

		[SerializeField]
		private GameObject m_impactParticleEffect;
		#endregion inspector members

		#region private members
		private bool m_enabled = false;
		private bool m_justPlaced = true;
		#endregion private members

		#region properties
		public override PowerupType Type { get { return PowerupType.OilSlick; } }
		#endregion properties

		#region constructors
		public override void Initialize( Racer racer )
		{
			base.Initialize( racer );
			transform.parent = null;

			//Raycast down towards the track and place oil slick on track. 
			RaycastHit hitInfo;
			if( Physics.Raycast( transform.position + Vector3.up, -Vector3.up, out hitInfo, 5.0f, m_trackLayerMask ) == true )
			{
				transform.position = hitInfo.point + new Vector3( 0, 0.05f, 0 );
				transform.rotation = Quaternion.LookRotation(hitInfo.normal);
			}
			else
			{
				Destroy( gameObject );
			}

			m_enabled = true;
			m_justPlaced = true;

			StartCoroutine( JustPlaced() );
		}
		#endregion constructors

		#region methods
		private IEnumerator SpinOutRacer( Racer racer )
		{
			racer.PowerupOverrideControlActive = true;

			float duration = m_impactedRacerSpinoutCurve.keys[m_impactedRacerSpinoutCurve.keys.Length - 1].time;

			for( float elapsed = 0; elapsed < duration; elapsed += Time.deltaTime )
			{
				float progress = elapsed / duration;
				racer.VisualsRoot.transform.localRotation = Quaternion.AngleAxis( m_impactedRacerSpinoutCurve.Evaluate( progress ) * 360.0f, Vector3.up );
				yield return null;
			}

			racer.VisualsRoot.transform.localRotation = Quaternion.identity;

			racer.PowerupOverrideControlActive = false;

			Destroy( gameObject );
		}

		private IEnumerator JustPlaced()
		{
			yield return new WaitForSeconds( 1.0f );
			m_justPlaced = false;
		}

		private void SlickHit()
		{
			m_enabled = false;

			m_oilSlickVisuals.SetActive( false );
			m_impactParticleEffect.SetActive( true );
		}
		#endregion methods

		#region monobehaviour callbacks
		private void OnTriggerEnter( Collider other )
		{
			if( m_enabled == false || other.isTrigger == true )
			{
				return;
			}

			Racer racer = other.GetComponent<Racer>();
			if( racer != null && racer.PowerupOverrideControlActive == false )
			{
				if( racer == Owner  && m_justPlaced == true)
				{
					return;
				}
				
				SlickHit();
				StartCoroutine( SpinOutRacer( racer ) );
			}
		}
		#endregion monobehaviour callbacks
	}
}