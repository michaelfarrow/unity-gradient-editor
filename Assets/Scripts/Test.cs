using UnityEngine;

public class Test : MonoBehaviour
{

  public CustomGradient gradient;

  new private Renderer renderer;

  private void Start()
  {
    renderer = GetComponent<Renderer>();
  }

  private void Update()
  { 
    // Debug.Log(renderer.bounds.size.x);
    renderer.material.SetTexture("_MainTex", gradient.GetTexture(300));
  }

}
