using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELine
{
	public class Powerup_SpeedBoost : Powerup
	{
		#region inspector members
		[SerializeField]
		private float m_velocityMultiplier;
		[SerializeField]
		private float m_accelerationMultiplier;
		[SerializeField]
		private float m_duration = 3.0f;
		#endregion inspector members

		#region properties
		public override PowerupType Type { get { return PowerupType.SpeedBoost; } }
		#endregion properties

		#region constructors
		public override void Initialize( Racer racer )
		{
			base.Initialize( racer );

			StartCoroutine( ApplySpeedBoost() );
		}
		#endregion constructors

		#region methods
		private IEnumerator ApplySpeedBoost()
		{
			float velocityAdjustment = m_velocityMultiplier - 1.0f;
			float accelerationAdjustment = m_accelerationMultiplier - 1.0f;

			Owner.VelocityMultiplier += velocityAdjustment;
			Owner.AccelerationMultiplier += accelerationAdjustment;
			
			yield return new WaitForSeconds( m_duration );

			Owner.VelocityMultiplier -= velocityAdjustment;
			Owner.AccelerationMultiplier -= accelerationAdjustment;

			Destroy( gameObject );
		}
		#endregion methods
	}
}