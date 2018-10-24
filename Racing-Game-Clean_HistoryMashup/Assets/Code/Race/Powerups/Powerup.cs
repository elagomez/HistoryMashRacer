using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELine
{
	public abstract class Powerup : MonoBehaviour
	{
		#region inner types
		public enum PowerupType
		{
			SpeedBoost,
			OilSlick,
			Projectile,
            Portal,
            Boulder,
            Asteroid,
            HoneyPot
		}
		#endregion inner types

		#region private members
		private Racer m_owner;
		#endregion private members

		#region properties
		public Racer Owner { get { return m_owner; } }
		public abstract PowerupType Type { get; }
		#endregion properties

		#region constructors
		public virtual void Initialize( Racer racer )
		{
			m_owner = racer;
		}
		#endregion constructors
	}
}