using ELine.CustomAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELine
{
	[ExecuteInEditMode]
	[RequireComponent( typeof( LightProbeGroup ) )]
	public class SplineLightProbeUtility : MonoBehaviour
	{
		#region inspector members
		[SerializeField]
		private Spline m_spline;

		[SerializeField]
		private int m_splineLengthSegmentCount = 100;

		[SerializeField]
		[Range(1, 10)]
		private int m_horizontalCount = 2;

		[SerializeField]
		private float m_horisontalSpan = 2.0f;

		[SerializeField]
		private float m_offsetFromSpline = 1.0f;

		[SerializeField]
		[Uneditable( UneditableAttribute.Effective.Always )]
		private int m_lightprobeCount = 0;
		#endregion inspector members

		#region private members
		private LightProbeGroup m_lightProbeGroup;
		private List<Vector3> m_probePositions = new List<Vector3>();
		#endregion private members

		#region constructors
		private void Start()
		{
			UpdateLightProbes();
		}
		#endregion construcors

		#region methods
		private void UpdateLightProbes()
		{ 
			if( m_lightProbeGroup == null )
			{
				m_lightProbeGroup = GetComponent<LightProbeGroup>();
			}

			transform.position = Vector3.zero;

			m_probePositions.Clear();

			if( m_spline != null )
			{
				for( int i = 0; i < m_splineLengthSegmentCount; i++ )
				{
					float splineT = (float)i / (float)m_splineLengthSegmentCount;
					Vector3 splinePosition = m_spline.GetPositionOnSpline( splineT );
					Vector3 splineForward = m_spline.GetTangentToSpline( splineT ).normalized;
					Vector3 splineRight = Vector3.Cross( Vector3.up, splineForward ).normalized;
					Vector3 splineUp = Vector3.Cross( splineForward, splineRight ).normalized;

					Debug.DrawLine( splinePosition, splinePosition + splineForward, Color.blue, 10.0f );
					Debug.DrawLine( splinePosition, splinePosition + splineRight, Color.red, 10.0f );
					Debug.DrawLine( splinePosition, splinePosition + splineUp, Color.green, 10.0f );


					Vector3 startPosition = splinePosition + ( splineRight * -m_horisontalSpan * 0.5f );
					float horizontalIncrement = m_horisontalSpan / m_horizontalCount;

					for( int j = 0; j < m_horizontalCount; j++ )
					{
						m_probePositions.Add( startPosition + ( splineRight * horizontalIncrement * (j + 0.5f) ) + ( splineUp * m_offsetFromSpline ) );
					}
				}

				m_lightProbeGroup.probePositions = m_probePositions.ToArray();

				m_lightprobeCount = m_probePositions.Count;
				m_probePositions.Clear();
			}
		}

		private void OnValidate()
		{
			UpdateLightProbes();
		}
		#endregion methods

		#region monobehaviour callbacks
		#endregion monobehaviour callbacks
	}
}