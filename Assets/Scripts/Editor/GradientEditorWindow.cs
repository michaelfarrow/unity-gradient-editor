using UnityEngine;
using UnityEditor;

public class GradientEditorWindow : CustomEditorWindow
{

  CustomGradient gradient;
  const int borderSize = 10;
  const int keyWidth = 10;
  const int keyHeight = 20;
  const int gradientHeight = 50;

  private Rect gradientPreviewRect;
  private Rect[] keyRects;
  private bool mouseDownOverKey;
  private int selectedKeyIndex;

  override protected bool IsActive()
  {
    return gradient != null;
  }

  override protected void Setup()
  {
    gradientPreviewRect = new Rect(borderSize, borderSize, position.width - borderSize * 2, gradientHeight);
  }

  override protected void Draw()
  {
    GUI.DrawTexture(gradientPreviewRect, gradient.GetTexture((int)gradientPreviewRect.width));

    keyRects = new Rect[gradient.KeyCount()];

    for (int i = 0; i < gradient.KeyCount(); i++)
    {
      CustomGradient.ColourKey key = gradient.GetKey(i);
      Rect keyRect = new Rect(gradientPreviewRect.x + gradientPreviewRect.width * key.Time - keyWidth / 2f, gradientPreviewRect.yMax + borderSize, keyWidth, keyHeight);

      if (i == selectedKeyIndex)
      {
        EditorGUI.DrawRect(new Rect(keyRect.x - 2, keyRect.y - 2, keyRect.width + 4, keyRect.height + 4), Color.black);
      }
      else
      {
        EditorGUI.DrawRect(new Rect(keyRect.x - 1, keyRect.y - 1, keyRect.width + 2, keyRect.height + 2), Color.grey);
      }

      EditorGUI.DrawRect(keyRect, key.Colour);
      keyRects[i] = keyRect;
    }

    Rect settingsRect = new Rect(borderSize, gradientPreviewRect.yMax + keyHeight + borderSize * 2, gradientPreviewRect.width, position.height);

    GUILayout.BeginArea(settingsRect);

    EditorGUI.BeginChangeCheck();
    Color newColour = EditorGUILayout.ColorField(gradient.GetKey(selectedKeyIndex).Colour);

    if (EditorGUI.EndChangeCheck())
    {
      gradient.UpdateKeyColour(selectedKeyIndex, newColour);
    }

    gradient.blendMode = (CustomGradient.BlendMode)EditorGUILayout.EnumPopup("Blend Mode", gradient.blendMode);

    GUILayout.EndArea();
  }

  override protected void HandleInput()
  {
    Event guiEvent = Event.current;

    if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
    {

      for (int i = 0; i < keyRects.Length; i++)
      {
        if (keyRects[i].Contains(guiEvent.mousePosition))
        {
          mouseDownOverKey = true;
          selectedKeyIndex = i;
          NeedsRepaint();
        }
      }

      if (gradientPreviewRect.Contains(guiEvent.mousePosition))
      {
        Color colour = gradient.Evaluate(Mathf.InverseLerp(gradientPreviewRect.x, gradientPreviewRect.xMax, guiEvent.mousePosition.x));
        float keyTime = Mathf.InverseLerp(gradientPreviewRect.x, gradientPreviewRect.xMax, guiEvent.mousePosition.x);
        selectedKeyIndex = gradient.AddKey(colour, keyTime);
        MarkSceneDirty();
        NeedsRepaint();
      }

    }

    if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0)
    {
      mouseDownOverKey = false;
    }

    if (guiEvent.type == EventType.MouseDrag && guiEvent.button == 0 && mouseDownOverKey)
    {
      float keyTime = Mathf.InverseLerp(gradientPreviewRect.x, gradientPreviewRect.xMax, guiEvent.mousePosition.x);
      selectedKeyIndex = gradient.UpdateKeyTime(selectedKeyIndex, keyTime);
      MarkSceneDirty();
      NeedsRepaint();
    }

    if (guiEvent.type == EventType.KeyDown && (guiEvent.keyCode == KeyCode.Backspace || guiEvent.keyCode == KeyCode.Delete))
    {
      gradient.RemoveKey(selectedKeyIndex);
      if (selectedKeyIndex >= gradient.KeyCount())
      {
        selectedKeyIndex--;
      }
      MarkSceneDirty();
      NeedsRepaint();
    }
  }

  public void SetGradient(CustomGradient gradient)
  {
    this.gradient = gradient;
  }

  private void OnEnable()
  {
    titleContent.text = "Gradient Editor";
    position.Set(position.x, position.y, 400, 150);
    minSize = new Vector2(200, 150);
    maxSize = new Vector2(1920, 150);
  }

}
