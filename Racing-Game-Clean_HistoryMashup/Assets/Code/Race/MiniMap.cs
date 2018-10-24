using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ELine
{
	public class MiniMap : MonoBehaviour
	{
		#region inspector members
		[SerializeField]
		private GameObject m_racerIconPrefab;

		[SerializeField]
		private Color m_playerColor;

		[SerializeField]
		private Color m_aiColor;
		#endregion inspector members

		#region private members
		private RectTransform m_containerRectTransform;
		private Dictionary<Racer, RectTransform> m_racerIconRectTransformLookup = new Dictionary<Racer, RectTransform>();
		private Vector2 m_minimapScaleFactor;
		private Vector2 m_minimapPositionOffset;
		#endregion private members

		#region constructors
		private IEnumerator Start()
		{
			yield return null;

			m_containerRectTransform = GetComponent<RectTransform>();

			for( int i = 0; i < RaceManager.Instance.ActiveRacers.Count; i++ )
			{
				GameObject racerIconInstance = Instantiate( m_racerIconPrefab, m_containerRectTransform );
				Image icon = racerIconInstance.GetComponent<Image>();
				RectTransform rectTransform = racerIconInstance.GetComponent<RectTransform>();

				icon.color = ( RaceManager.Instance.ActiveRacers[i] is Racer_Player ) ? m_playerColor : m_aiColor;
				m_racerIconRectTransformLookup[RaceManager.Instance.ActiveRacers[i]] = rectTransform;
			}

			m_minimapScaleFactor = new Vector2( m_containerRectTransform.sizeDelta.x / 1000.0f, m_containerRectTransform.sizeDelta.y / 1000.0f );
			m_minimapPositionOffset = new Vector2( -m_containerRectTransform.sizeDelta.x * 0.5f, -m_containerRectTransform.sizeDelta.y * 0.5f );
		}
		#endregion constructors

		#region methods
		private void Update()
		{
			foreach( var racer in m_racerIconRectTransformLookup.Keys )
			{
				m_racerIconRectTransformLookup[racer].localPosition = new Vector3( racer.transform.position.x * m_minimapScaleFactor.x + m_minimapPositionOffset.x, racer.transform.position.z * m_minimapScaleFactor.y + m_minimapPositionOffset.y, 0 );
			}
		}
		#endregion methods
	}
}