using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELine
{
	public class Powerup_Projectile : Powerup
	{
		#region inspector members
		[SerializeField]
		private LayerMask m_trackLayerMask;

		[SerializeField]
		private float m_velocity = 20.0f;

		[SerializeField]
		private AnimationCurve m_impactedRacerSpinoutCurve;

		[SerializeField]
		private GameObject m_projectileVisuals;

		[SerializeField]
		private GameObject m_impactParticleEffect;
		#endregion inspector members

		#region private members
		private Rigidbody m_rigidbody;
		private bool m_enabled = false;
		#endregion private members

		#region properties
		public override PowerupType Type { get { return PowerupType.Projectile; } }
		#endregion properties

		#region constructors
		public override void Initialize( Racer racer )
		{
			base.Initialize( racer );
			m_rigidbody = GetComponent<Rigidbody>();

			transform.parent = null;
			transform.position += new Vector3( 0, 0.5f, 0 );

			m_rigidbody.velocity = transform.forward * (m_velocity + racer.CurrentSpeed);
			m_rigidbody.angularVelocity = Vector3.zero;

			m_enabled = true;
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

		private IEnumerator TimedDestroy()
		{
			yield return new WaitForSeconds( 3.0f );
			Destroy( gameObject );
		}

		private void DoImpact()
		{
			m_enabled = false;
			m_rigidbody.velocity = Vector3.zero;
			m_rigidbody.angularVelocity = Vector3.zero;

			m_projectileVisuals.SetActive( false );
			m_impactParticleEffect.SetActive( true );
		}
		#endregion methods

		#region monobehaviour callbacks
		private void FixedUpdate()
		{
			float velocityMagnitude = m_rigidbody.velocity.magnitude;

			RaycastHit hit;
			if( Physics.Raycast( transform.position, Vector3.down, out hit, 5.0f, m_trackLayerMask, QueryTriggerInteraction.Ignore ) )
			{
				m_rigidbody.MovePosition( hit.point + new Vector3( 0, 0.5f, 0 ) );
			}
		}

		private void OnTriggerEnter( Collider other )
		{
			if( m_enabled == false || other.isTrigger == true )
			{
				return;
			}

			Racer racer = other.GetComponent<Racer>();
			if( racer != null && racer.PowerupOverrideControlActive == false )
			{
				if( racer != Owner )
				{
					DoImpact();
					StartCoroutine( SpinOutRacer( racer ) );
				}
			}
			else
			{
				DoImpact();
				StartCoroutine( TimedDestroy() );
			}
		}
		#endregion monobehaviour callbacks
	}
}