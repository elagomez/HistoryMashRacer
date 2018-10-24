using UnityEditor;
using UnityEngine;
using ELine.CustomAttributes;

namespace ELine
{
    [CustomPropertyDrawer(typeof(UneditableAttribute))]
    public class UneditableAttributePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var uneditableAttribute = (this.attribute as UneditableAttribute);

            // only disable the editablity of the field when appropiate
            if (uneditableAttribute.EffectiveWhen == UneditableAttribute.Effective.Always ||
                ((uneditableAttribute.EffectiveWhen == UneditableAttribute.Effective.OnlyWhilePlaying && EditorApplication.isPlayingOrWillChangePlaymode) == true) ||
                ((uneditableAttribute.EffectiveWhen == UneditableAttribute.Effective.OnlyWhileEditing && EditorApplication.isPlaying == false))
            )
            {
                GUI.enabled = false;
            }

            EditorGUI.PropertyField(position, property, label, true);

            GUI.enabled = true;
        }
    }
}