using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELine
{
	public class RaceCamera : MonoBehaviour
	{
		#region inspector members
		[SerializeField]
		private Vector3 m_lookatTargetOffset = new Vector3( 0.0f, 1.0f, 0.0f );

		[SerializeField]
		private Vector3 m_cameraOffsetFromTarget = new Vector3( 0.0f, 2.0f, -4.0f );

		[SerializeField]
		[Range(0.0f, 100.0f)]
		private float m_motionSmoothing = 6.0f;
		#endregion inspector members

		#region private members
		private Transform m_cameraLookAtTransform = null;
		private Quaternion m_currentOrbitRotation;
		private Vector3 m_currentLookatTargetOffset;
		#endregion private members

		#region constructors
		private void Start()
		{
			m_cameraLookAtTransform = Racer_Player.Instance.transform;
			transform.parent = null;
			m_currentLookatTargetOffset = m_lookatTargetOffset;
		}
		#endregion constructors

		#region monobehaviour callbacks
		private void LateUpdate()
		{
			if( m_cameraLookAtTransform == null )
			{
				return;
			}

			m_currentOrbitRotation = Quaternion.Slerp( m_currentOrbitRotation, m_cameraLookAtTransform.rotation, m_motionSmoothing * Time.deltaTime );
			m_currentLookatTargetOffset = Vector3.Lerp( m_currentLookatTargetOffset, m_lookatTargetOffset, m_motionSmoothing * Time.deltaTime );

			Vector3 offsetTarget = m_cameraLookAtTransform.TransformPoint( m_currentLookatTargetOffset );
			transform.position = ( m_currentOrbitRotation * m_cameraOffsetFromTarget ) + offsetTarget;
			transform.rotation = Quaternion.LookRotation( ( offsetTarget - transform.position ).normalized, Vector3.up );
		}
		#endregion monobehaviour callbacks
	}
}