using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELine
{
	public class KartWheel : MonoBehaviour
	{
		#region inspector members
		[SerializeField]
		private bool m_steeringWheel = false;

		[SerializeField]
		private float m_maxTurnAngle = 45.0f;

		[SerializeField]
		private float m_wheelRadius = 1.0f;

		[SerializeField]
		private float m_steerResponseStiffness;
		#endregion inspector members

		#region private members
		private Racer m_racerParent;
		private Quaternion m_originalLocalRotation;

		private float m_smoothedSteering = 0;

		private float m_currentDriveRotation = 0;
		#endregion private members

		#region properties
		#endregion properties

		#region events
		#endregion events

		#region constructors
		private void Awake()
		{
			m_racerParent = GetComponentInParent<Racer>();
			m_originalLocalRotation = transform.localRotation;
		}
		#endregion constructors

		#region methods
		#endregion methods

		#region monobehaviour callbacks
		private void Update()
		{
			transform.localRotation = m_originalLocalRotation;

			if( m_steeringWheel == true )
			{
				float flip = m_racerParent.CurrentThrottleValue >= 0 ? 1 : -1;
				m_smoothedSteering = Mathf.Lerp( m_smoothedSteering, m_racerParent.CurrentSteeringValue * flip, m_steerResponseStiffness * Time.deltaTime );
				transform.localRotation *= Quaternion.AngleAxis( m_maxTurnAngle * m_smoothedSteering, Vector3.up );
			}

			float distanceTraveled = m_racerParent.CurrentSpeed * Time.deltaTime;
			float rotationInRadians = distanceTraveled / m_wheelRadius;
			float rotationInDegrees = rotationInRadians * Mathf.Rad2Deg;
			m_currentDriveRotation += rotationInDegrees;
			transform.localRotation *= Quaternion.AngleAxis( m_currentDriveRotation, Vector3.right );
		}
		#endregion monobehaviour callbacks
	}
}