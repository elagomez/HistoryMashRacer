using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELine
{
	public class Racer_Player : Racer
	{
		#region private members
		private static Racer_Player s_instance;
		private bool m_firePressed = false;
		#endregion private members

		#region properties
		public static Racer_Player Instance { get { return s_instance; } }
		#endregion properties

		#region constructors
		protected override void Awake()
		{
			base.Awake();
			s_instance = this;
		}
		#endregion constructors

		#region monobehaviour callbacks
		protected override void KartControlInput( out float throttle, out float steer )
		{
			steer = Input.GetAxis( "Steer" );
			throttle = Input.GetAxis( "Throttle" );

			if( Input.GetButtonDown( "Fire" ) == true || ( Input.GetAxis("Fire") > 0.5f && m_firePressed == false ) )
			{
				m_firePressed = true;
				UsePowerup();
			}

			if( Input.GetAxis( "Fire" ) < 0.5f && m_firePressed == true )
			{
				m_firePressed = false;
			}
		}
		#endregion monobehaviour callbacks
	}
}