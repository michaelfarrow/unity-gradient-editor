using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(CustomGradient))]
public class GradientPropertyDrawer : PropertyDrawer
{

  const float gradientPadding = 2;

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
  {
    Event guiEvent = Event.current;

    CustomGradient gradient = (CustomGradient)fieldInfo.GetValue(property.serializedObject.targetObject);

    float labelWidth = EditorGUIUtility.labelWidth;

    Rect labelRect = new Rect(position.x, position.y, labelWidth, position.height);
    Rect gradientRect = new Rect(position.x + labelWidth, position.y, position.width - labelWidth, position.height);
    Rect gradientInnerRect = new Rect(gradientRect.x + gradientPadding, gradientRect.y + gradientPadding, gradientRect.width - gradientPadding * 2, gradientRect.height - gradientPadding * 2);

    switch (guiEvent.type)
    {
      case EventType.Repaint:

        GUI.Label(labelRect, label);

        GUIStyle gradientStyle = new GUIStyle();
        gradientStyle.normal.background = gradient.GetTexture((int)gradientRect.width);

        GUI.TextField(gradientRect, "");
        GUI.Label(gradientInnerRect, GUIContent.none, gradientStyle);

        break;
      case EventType.MouseDown:
        if (guiEvent.button == 0 && gradientRect.Contains(guiEvent.mousePosition))
        {
          GradientEditorWindow editor = EditorWindow.GetWindow<GradientEditorWindow>();
          editor.SetGradient(gradient);
        }
        break;
    }
  }

}
