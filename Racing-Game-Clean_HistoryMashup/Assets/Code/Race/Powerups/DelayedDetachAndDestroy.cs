using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELine
{
	public class DelayedDetachAndDestroy : MonoBehaviour
	{
		#region inspector members
		[SerializeField]
		private float m_detachDelay = 1.0f;

		[SerializeField]
		private float m_destroyDelay = 3.0f;
		#endregion inspector members

		#region constructors
		private IEnumerator Start()
		{
			yield return new WaitForSeconds( m_detachDelay );

			transform.parent = null;

			yield return new WaitForSeconds( m_destroyDelay );

			Destroy( gameObject );
		}
		#endregion construcors
	}
}