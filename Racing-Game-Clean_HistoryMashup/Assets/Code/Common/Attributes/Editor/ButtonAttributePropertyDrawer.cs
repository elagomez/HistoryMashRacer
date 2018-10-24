using UnityEditor;
using UnityEngine;
using ELine.CustomAttributes;
using System.Reflection;

namespace ELine
{
#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(ButtonAttribute))]
    public class ButtonAttributePropertyDrawer : PropertyDrawer
    {
		private MethodInfo _eventMethodInfo = null;

		public override void OnGUI( Rect position, SerializedProperty prop, GUIContent label )
		{
			ButtonAttribute buttonAttribute = (ButtonAttribute)attribute;
			Rect buttonRect = new Rect( position.x, position.y, position.width, position.height );
			if( GUI.Button( buttonRect, label.text ) )
			{
				System.Type eventOwnerType = prop.serializedObject.targetObject.GetType();
				string eventName = buttonAttribute.MethodName;

				if( _eventMethodInfo == null )
					_eventMethodInfo = eventOwnerType.GetMethod( eventName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic );

				if( _eventMethodInfo != null )
					_eventMethodInfo.Invoke( prop.serializedObject.targetObject, null );
				else
					Debug.LogWarning( string.Format( "InspectorButton: Unable to find method {0} in {1}", eventName, eventOwnerType ) );
			}
		}
	}
#endif
}