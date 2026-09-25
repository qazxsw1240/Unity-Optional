#if UNITY_EDITOR

using Unity.Utility.Functional;

using UnityEditor;

using UnityEngine;

namespace Unity.Utility.Editor
{
  [CustomPropertyDrawer(typeof(SerializableOptional<>))]
  internal sealed class SerializableOptionalEditor : PropertyDrawer
  {
    private const string k_HasValueProperty = "m_HasValue";
    private const string k_ValueProperty = "m_Value";

    private static readonly float s_EditorSpace = EditorGUIUtility.standardVerticalSpacing;
    private static readonly float s_DefaultHeight = EditorGUIUtility.singleLineHeight;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      SerializedProperty hasValueProperty = property.FindPropertyRelative(k_HasValueProperty);
      SerializedProperty valueProperty = property.FindPropertyRelative(k_ValueProperty);
      float height = EditorGUI.GetPropertyHeight(hasValueProperty, label, hasValueProperty.isExpanded);
      if (hasValueProperty.boolValue)
      {
        height += s_EditorSpace + EditorGUI.GetPropertyHeight(valueProperty, label, valueProperty.isExpanded);
      }
      return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      SerializedObject target = property.serializedObject;
      SerializedProperty hasValueProperty = property.FindPropertyRelative(k_HasValueProperty);
      SerializedProperty valueProperty = property.FindPropertyRelative(k_ValueProperty);
      EditorGUI.BeginProperty(position, label, property);
      GUI.SetNextControlName("PropertyField");
      EditorGUI.PrefixLabel(position, 0, label);
      GUIContent buttonContent = new(hasValueProperty.boolValue ? "Remove Value" : "Contain Value");

      float buttonWidth = 10f + GUI.skin.button.CalcSize(buttonContent).x;

      Rect buttonRect = new(position.x + position.width - buttonWidth, position.y, buttonWidth, s_DefaultHeight);
      if (EditorGUI.DropdownButton(buttonRect, buttonContent, FocusType.Passive))
      {
        EditorGUI.FocusTextInControl("PropertyField");
        hasValueProperty.boolValue = !hasValueProperty.boolValue;
      }
      if (hasValueProperty.boolValue)
      {
        position.y += EditorGUI.GetPropertyHeight(hasValueProperty, label, hasValueProperty.isExpanded);
        position.height = EditorGUI.GetPropertyHeight(valueProperty, valueProperty.isExpanded);
        EditorGUI.indentLevel++;
        EditorGUI.PropertyField(position, valueProperty, valueProperty.isExpanded);
        EditorGUI.indentLevel--;
      }
      EditorGUI.EndProperty();
      target.ApplyModifiedProperties();
    }
  }
}

#endif
