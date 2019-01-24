using UnityEngine;
using UnityEditor;

public class CustomEditorWindow : EditorWindow
{

  private bool needsRepaint;

  private void OnGUI()
  {
    if (!IsActive()) return;

    needsRepaint = false;

    EditorGUI.BeginChangeCheck();

    Setup();
    Draw();
    HandleInput();

    if (EditorGUI.EndChangeCheck())
    {
      MarkSceneDirty();
    }

    if (needsRepaint)
    {
      Repaint();
    }
  }

  virtual protected bool IsActive()
  {
    return true;
  }
  virtual protected void Setup() { }
  virtual protected void Draw() { }
  virtual protected void HandleInput() { }

  protected void MarkSceneDirty()
  {
    if (!EditorApplication.isPlaying)
    {
      UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }
  }

  protected void NeedsRepaint()
  {
    needsRepaint = true;
  }

  private void OnDisable()
  {
    MarkSceneDirty();
  }

}
