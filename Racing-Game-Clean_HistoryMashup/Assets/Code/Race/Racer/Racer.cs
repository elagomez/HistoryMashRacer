using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELine
{
	public abstract class Racer : MonoBehaviour
	{
		#region inner types
		public struct PowerupInventoryItem
		{
			public Powerup.PowerupType Type;
			public GameObject Prefab;
		}
		#endregion inner types

		#region inspector members
		[SerializeField]
		protected LayerMask m_racerLayerMask;

		[SerializeField]
		protected LayerMask m_trackLayerMask;

		[SerializeField]
		[HideInInspector]
		private GameObject m_visualsRoot;

		[Header( "Speed" )]
		[SerializeField]
		[Tooltip( "The maximum forward speed of the racer in Meters Per Second." )]
		protected float m_maximumForwardSpeedMPS = 20.0f;

		[SerializeField]
		[Tooltip( "The maximum reverse speed of the racer in Meters Per Second." )]
		private float m_maximumReverseSpeedMPS = 5.0f;

		[SerializeField]
		[Tooltip( "The rate of acceleration in Meters Per Second Per Second." )]
		private float m_acceleration = 0.5f;

		[SerializeField]
		[Tooltip( "The rate of deceleration in Meters Per Second Per Second." )]
		private float m_deceleration = 1.0f;

		[Header( "Steering" )]
		[SerializeField]
		[Tooltip( "The rate of turning based on current speed." )]
		private AnimationCurve m_steeringCurve = AnimationCurve.Linear( 0.0f, 0.0f, 1.0f, 1.0f );

		[SerializeField]
		[Tooltip( "The rate of deceleration in Meters Per Second Per Second at full turn." )]
		private float m_speedDecayRateWhileTurning = 0.15f;

		[SerializeField]
		[Tooltip( "The minimum forward speed to decay to at full turn." )]
		private float m_minForwardSpeedToDecayTo = 10.0f;

		[SerializeField]
		[Tooltip( "The minimum reverse speed to decay to at full turn." )]
		private float m_minReverseSpeedToDecayTo = 5.0f;

		[Header( "Sound Effects" )]
		[SerializeField]
		private AudioSource m_engineSoundSource;
		[SerializeField]
		private float m_enginePitchAtMaxSpeed = 3.0f;

		[Header( "Powerups" )]
		[Range(0, 10)]
		[SerializeField]
		[Tooltip( "The total number of powerups a racer can carry." )]
		private int m_maxPowerups = 4;
		#endregion inspector members

		#region private members
		private Rigidbody m_rigidbody;

		public float m_targetSpeed;
		private float m_actualSpeed;

		private Vector3 m_lastPosition;

		private float m_currentThrottleValue;
		private float m_currentSteeringValue;
		private float m_targetSteeringValue;

		private float m_normalizedSpeed;

		private Vector3 m_gravity;
		private float m_maxGravityMPS;

		private Vector3 m_currentGroundNormal;
		private float m_currentGroundDistance;

		private bool m_isGrounded = true;

		private float m_raceProgress;
		private float m_raceProgressNormalized;

		protected float m_velocityMultiplier = 1.0f;
		protected float m_accelerationMultiplier = 1.0f;

		private List<PowerupInventoryItem> m_powerupInventory = new List<PowerupInventoryItem>();
		private Dictionary<GameObject, Powerup.PowerupType> m_powerupTypeLookup = new Dictionary<GameObject, Powerup.PowerupType>();

		private bool m_powerupOverrideControlActive = false;
		#endregion private members

		#region properties
		public virtual float MaximumForwardSpeedMPS { get { return m_maximumForwardSpeedMPS * m_velocityMultiplier; } }
		public float MaximumReverseSpeedMPS { get { return m_maximumReverseSpeedMPS * m_velocityMultiplier; } }

		public float VelocityMultiplier { get { return m_velocityMultiplier; } set { m_velocityMultiplier = value; } }
		public float AccelerationMultiplier { get { return m_accelerationMultiplier; } set { m_accelerationMultiplier = value; } }

		public float CurrentThrottleValue { get { return m_currentThrottleValue; } }
		public float CurrentSteeringValue { get { return m_currentSteeringValue; } }

		public float CurrentSpeed { get { return m_actualSpeed; } }

		public bool PowerupOverrideControlActive { get { return m_powerupOverrideControlActive; } set { m_powerupOverrideControlActive = value; } }

		public GameObject VisualsRoot { get { return m_visualsRoot; } }

		public List<PowerupInventoryItem> PowerupInventory { get { return m_powerupInventory; } }

		public int MaxPowerups { get { return m_maxPowerups; } }
		#endregion properties

		#region events
		public event Action<Powerup.PowerupType> OnPowerupAcquired;
		public event Action OnPowerupUsed;
		#endregion events

		#region constructors
		protected virtual void Awake()
		{
			m_rigidbody = GetComponent<Rigidbody>();
			m_maxGravityMPS = 10.0f;

			m_currentGroundNormal = transform.up;

			AccelerationMultiplier = 1.0f;
		}

		protected virtual void Start()
		{
			RaceManager.Instance.RegisterRacer( this );
		}
		#endregion constructors

		#region methods
		public void AcquirePowerup( GameObject powerupPrefab )
		{
			Powerup powerup = powerupPrefab.GetComponent<Powerup>();
			m_powerupInventory.Add( new PowerupInventoryItem() { Type = powerup.Type, Prefab = powerupPrefab } );
			m_powerupTypeLookup[powerupPrefab] = powerup.Type;

			if( OnPowerupAcquired != null )
			{
				OnPowerupAcquired( powerup.Type );
			}
		}

		protected void UsePowerup()
		{
			if( m_powerupInventory.Count > 0 )
			{
				PowerupInventoryItem inventoryItem = m_powerupInventory[0];
				m_powerupInventory.RemoveAt( 0 );
				GameObject powerupInstance = Instantiate( inventoryItem.Prefab, transform, false );
				Powerup powerup = powerupInstance.GetComponent<Powerup>();
				powerup.Initialize( this );

				if( OnPowerupUsed != null )
				{
					OnPowerupUsed();
				}
			}
		}

		private void GroundedCheck()
		{
			RaycastHit hitInfo;
			Ray ray = new Ray( transform.position + Vector3.up, -Vector3.up );

			//Raycast down towards the track. 
			if( Physics.Raycast( ray, out hitInfo, 50.0f, m_trackLayerMask ) == true )
			{
				m_currentGroundNormal = hitInfo.normal;
				m_currentGroundDistance = hitInfo.distance - 1.0f;

				if( m_currentGroundDistance < 0.1f )
				{
					m_isGrounded = true;
				}
				else
				{
					m_isGrounded = false;
				}
			}
			else
			{
				m_isGrounded = false;
			}

			if( m_isGrounded == true )
			{
				m_gravity = Physics.gravity * Time.fixedDeltaTime;
			}
			else
			{
				m_gravity += Physics.gravity * Time.fixedDeltaTime;
				m_gravity = Vector3.ClampMagnitude( m_gravity, m_maxGravityMPS );
			}
		}

		void ApplySteering()
		{
			if( m_isGrounded == true )
			{
				if( m_currentSteeringValue != 0 )
				{
					transform.rotation *= CalculateSteerRotation();
				}
			}
		}

		private void AlignToTrack()
		{
			if( m_currentGroundDistance < 0.5f )
			{
				// Project the velocity on a plane defined by the current ground normal.
				Vector3 projectedNormal = ( transform.forward - ( Vector3.Dot( transform.forward, m_currentGroundNormal ) * m_currentGroundNormal ) );
				transform.rotation = Quaternion.Slerp( transform.rotation, Quaternion.LookRotation( projectedNormal, m_currentGroundNormal ), 10.0f * Time.fixedDeltaTime );
			}
		}

		private Quaternion CalculateSteerRotation()
		{
			float steeringAngle = 90.0f * m_currentSteeringValue;
			float steeringAngleDelta = steeringAngle * m_steeringCurve.Evaluate( m_normalizedSpeed ) * Time.fixedDeltaTime;
			return Quaternion.AngleAxis( steeringAngleDelta, transform.up );
		}

		private void ApplyThrottle()
		{
			if( m_isGrounded == true )
			{
				if( m_currentThrottleValue != 0.0f )
				{
					if( Mathf.Abs( m_currentSteeringValue ) < 0.5f )
					{
						if( m_currentThrottleValue > 0.0f )
						{
							m_targetSpeed = Mathf.Lerp( m_targetSpeed, MaximumForwardSpeedMPS, m_acceleration * AccelerationMultiplier * Time.fixedDeltaTime );
						}
						else
						{
							m_targetSpeed = Mathf.Lerp( m_targetSpeed, -MaximumReverseSpeedMPS, m_acceleration * AccelerationMultiplier * Time.fixedDeltaTime );
						}
					}
					else
					{
						if( m_currentThrottleValue > 0.0f )
						{
							m_targetSpeed = Mathf.Lerp( m_targetSpeed, m_minForwardSpeedToDecayTo, m_speedDecayRateWhileTurning * Time.fixedDeltaTime );
						}
						else
						{
							m_targetSpeed = Mathf.Lerp( m_targetSpeed, -m_minReverseSpeedToDecayTo, m_speedDecayRateWhileTurning * Time.fixedDeltaTime );
						}
					}
				}
				else
				{
					m_targetSpeed = Mathf.Lerp( m_targetSpeed, 0, m_deceleration * Time.fixedDeltaTime );
				}
			}
		}

		protected abstract void KartControlInput( out float throttle, out float steer );
		#endregion methods

		#region monobehaviour callbacks
		private void Update()
		{
			float throttle = 0;
			float steer = 0;

			if( m_powerupOverrideControlActive == false )
			{
				KartControlInput( out throttle, out steer );
			}

			m_currentThrottleValue = throttle;
			m_currentSteeringValue = Mathf.Sign( m_actualSpeed ) * steer;

			GroundedCheck();
			AlignToTrack();

			if( Mathf.Sign( m_actualSpeed ) > 0.0f )
			{
				m_normalizedSpeed = Mathf.Abs( m_actualSpeed ) / MaximumForwardSpeedMPS;
			}
			else
			{
				m_normalizedSpeed = Mathf.Abs( m_actualSpeed ) / MaximumReverseSpeedMPS;
			}

			m_engineSoundSource.pitch = Mathf.Max( 0.5f, ( Mathf.Abs( m_actualSpeed ) / m_maximumForwardSpeedMPS ) * m_enginePitchAtMaxSpeed );
		}

		private void FixedUpdate()
		{
			ApplySteering();
			ApplyThrottle();

			m_actualSpeed = ( transform.position - m_lastPosition ).magnitude / Time.deltaTime;
			m_lastPosition = transform.position;

			m_rigidbody.velocity = ( transform.forward * m_targetSpeed ) + m_gravity;
			m_rigidbody.angularVelocity = Vector3.zero;
		}

		private void OnDestroy()
		{
			if( RaceManager.Instance != null )
			{
				RaceManager.Instance.UnregisterRacer( this );
			}
		}
        #endregion monobehaviour callbacks

        public void OnHitByAsteroid(float maxSpeed, float duration)
        {
            debuffSpeed = maxSpeed;
            this.StartCoroutine( HitByAsteroid( duration ) );
        }

        private bool hitByAsteroid = false;
        private float debuffSpeed;
        private IEnumerator HitByAsteroid(float duration)
        {
            hitByAsteroid = true;
            yield return new WaitForSeconds( duration );
            hitByAsteroid = false;
        }
    }
}