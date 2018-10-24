using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELine
{
	public class Racer_AI : Racer
	{
		#region inspector members
		[Header( "AI Settings" )]
		[SerializeField]
		[Tooltip("The amount of randomization that affects steering.")]
		private AnimationCurve m_steeringRandomization;

		[SerializeField]
		[Tooltip( "The amount of randomization that affects throttling." )]
		private AnimationCurve m_throttleRandomization;

		[SerializeField]
		[Tooltip( "The distance ahead of the racer on the track spline that the racer attempts to steer to." )]
		private float m_splineLookAheadDistance = 4.0f;

		[SerializeField]
		[Tooltip( "The maximum amount of time the racer will hold onto the current powerup." )]
		private float m_powerupMaxHoldTime = 5.0f;

		[SerializeField]
		[Tooltip( "The amount of time the racer will wait between powerup uses." )]
		private float m_powerupUseCooldown = 4.0f;

		[SerializeField]
		[Tooltip( "A curve determining the strength of max speed adjustment to keep the ai racer from falling to far behind or ahead of the player." )]
		private AnimationCurve m_rubberBandCurve = AnimationCurve.Linear( 1.0f, 1.0f, 0.0f, 0.0f );

		[SerializeField]
		[Tooltip( "The distance range of rubberbanding effect." )]
		private Vector2 m_rubberBandingDistanceRange = new Vector2( 10.0f, 30.0f );
		#endregion inspector members

		#region private members
		private float m_timeSinceUsedPowerupLast = 0;
		private float m_rubberBandingMultiplier = 1.0f;

		private float m_randomizationOffset;
		#endregion private members

		#region properties
		public override float MaximumForwardSpeedMPS { get { return Mathf.Clamp( m_maximumForwardSpeedMPS * m_velocityMultiplier * m_rubberBandingMultiplier, 0.001f, 1000.0f ); } }
		#endregion properties

		protected override void Start()
		{
			base.Start();
			m_steeringRandomization.postWrapMode = WrapMode.Loop;
			m_throttleRandomization.postWrapMode = WrapMode.Loop;

			m_randomizationOffset = UnityEngine.Random.Range(0.0f, 10.0f);
		}

		#region methods
		private void ApplyRubberBanding()
		{
			float playerProgress = RaceManager.Instance.GetRacerProgressDistance( Racer_Player.Instance );
			float aiProgress = RaceManager.Instance.GetRacerProgressDistance( this );
			
			float progressDifference = playerProgress - aiProgress;
			
			float progressDifferenceNormalised =  Mathf.Clamp01( ( Mathf.Abs( progressDifference ) - m_rubberBandingDistanceRange.x ) / m_rubberBandingDistanceRange.y );

			m_rubberBandingMultiplier = 1.0f + ( Mathf.Sign( progressDifference ) * m_rubberBandCurve.Evaluate( progressDifferenceNormalised ) );
		}

		private void UsePowerups()
		{
			m_timeSinceUsedPowerupLast += Time.deltaTime;
			bool maxHoldTimeExpired = m_timeSinceUsedPowerupLast >= m_powerupMaxHoldTime;
			bool cooldownExpired = m_timeSinceUsedPowerupLast >= m_powerupUseCooldown;

			if( PowerupInventory.Count > 0 && cooldownExpired )
			{
				switch( PowerupInventory[0].Type )
				{
					case Powerup.PowerupType.SpeedBoost:
						UsePowerup();
						m_timeSinceUsedPowerupLast = 0.0f;
						break;
					case Powerup.PowerupType.OilSlick:
						if( Physics.Raycast( transform.position + ( Vector3.up * 0.5f ), -transform.forward, 50.0f, m_racerLayerMask, QueryTriggerInteraction.Ignore ) || maxHoldTimeExpired )
						{
							UsePowerup();
							m_timeSinceUsedPowerupLast = 0.0f;
						}
						break;
					case Powerup.PowerupType.Projectile:
						if( Physics.Raycast( transform.position + ( Vector3.up * 0.5f ), transform.forward, 50.0f, m_racerLayerMask, QueryTriggerInteraction.Ignore ) || maxHoldTimeExpired )
						{
							UsePowerup();
							m_timeSinceUsedPowerupLast = 0.0f;
						}
						break;
				}
			}
			else if( PowerupInventory.Count == 0 )
			{
				m_timeSinceUsedPowerupLast = 0.0f;
			}
		}
		#endregion methods

		#region monobehaviour callbacks
		protected override void KartControlInput( out float throttle, out float steer )
		{ 
			float currentSplineT = RaceManager.Instance.SplineGuide.GetClosestPointParam( transform.position, 5 );
			Vector3 splineDirectionNormal = RaceManager.Instance.SplineGuide.GetTangentToSpline( currentSplineT ).normalized;

			float targetSplineT = RaceManager.Instance.SplineGuide.GetClosestPointParam( transform.position + ( splineDirectionNormal * m_splineLookAheadDistance ), 5 );
			Vector3 targetSplinePosition = RaceManager.Instance.SplineGuide.GetPositionOnSpline( targetSplineT );
			Vector3 racerToTargetSplinePositionDir = ( targetSplinePosition - transform.position ).normalized;

			bool correctDirection = Vector3.Dot( transform.forward, splineDirectionNormal ) > 0;

			throttle = correctDirection ? 1 : -1;
			ApplyRubberBanding();
			throttle = Mathf.Clamp( throttle + m_throttleRandomization.Evaluate( Time.time + m_randomizationOffset ), -1.0f, 1.0f ); 

			steer = correctDirection ? 1 : -1;
			steer *= Vector3.Dot( racerToTargetSplinePositionDir, transform.right );

			steer = Mathf.Clamp( steer + m_steeringRandomization.Evaluate( Time.time + m_randomizationOffset ), -1.0f, 1.0f );

			UsePowerups();
		}
		#endregion monobehaviour callbacks
	}
}