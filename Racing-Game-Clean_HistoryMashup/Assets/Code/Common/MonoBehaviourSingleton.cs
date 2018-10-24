using UnityEngine;

namespace ELine
{
	public abstract class MonoBehaviourSingleton<T> : MonoBehaviour
	where T : MonoBehaviour
	{
		#region members
		private static T _instance;
		#endregion members

		#region properties
		public static T Instance { get { return _instance; } }
		#endregion properties

		#region constructors
		protected virtual void Awake()
		{
			if( _instance != null && _instance != this )
			{
				Debug.LogFormat( "{0} is being destroyed because a singelton reference was already set!", this.name );
				DestroyInstance( this );
				return;
			}

			_instance = this.GetComponent<T>();
		}

		private void DestroyInstance( MonoBehaviour monobehaviour )
		{
			if( Application.isPlaying == false )
			{
				DestroyImmediate( monobehaviour );
			}
			else
			{
				Destroy( monobehaviour );
			}
		}
		#endregion constructors

		#region methods
		protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        protected virtual void OnApplicationQuit()
        {
            if (_instance != this)
                return;
        }
        #endregion methods
    }
}
