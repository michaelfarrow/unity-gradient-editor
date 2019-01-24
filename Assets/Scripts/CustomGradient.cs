using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CustomGradient
{

  public enum BlendMode
  {
    Linear,
    Discrete
  }

  [System.Serializable]
  public struct ColourKey
  {
    [SerializeField]
    Color colour;
    [SerializeField]
    float time;

    public ColourKey(Color colour, float time)
    {
      this.colour = colour;
      this.time = time;
    }

    public Color Colour
    {
      get
      {
        return colour;
      }
    }

    public float Time
    {
      get
      {
        return time;
      }
    }

  }

  public BlendMode blendMode;

  [SerializeField]
  List<ColourKey> keys = new List<ColourKey>();

  public CustomGradient()
  {
    AddKey(Color.white, 0);
    AddKey(Color.black, 1);
  }

  public Color Evaluate(float time)
  {
    ColourKey keyLeft = keys[0];
    ColourKey keyRight = keys[keys.Count - 1];

    // for (int i = 0; i < keys.Count - 1; i++)
    // {
    //   if (keys[i].Time < time && keys[i + 1].Time >= time)
    //   {
    //     keyLeft = keys[i];
    //     keyRight = keys[i + 1];
    //     break;
    //   }
    // }

    for (int i = 0; i < keys.Count; i++)
    {
      ColourKey key = keys[i];
      if (key.Time <= time)
      {
        keyLeft = key;
      }
      if (key.Time >= time)
      {
        keyRight = key;
        break;
      }
    }

    if (blendMode == BlendMode.Linear)
    {
      float blendTime = Mathf.InverseLerp(keyLeft.Time, keyRight.Time, time);
      return Color.Lerp(keyLeft.Colour, keyRight.Colour, blendTime);
    }

    return keyRight.Colour;
  }

  public ColourKey GetKey(int i)
  {
    return keys[i];
  }

  public int KeyCount()
  {
    return keys.Count;
  }

  public int AddKey(Color colour, float time)
  {
    ColourKey key = new ColourKey(colour, time);

    for (int i = 0; i < keys.Count; i++)
    {
      if (key.Time < keys[i].Time)
      {
        keys.Insert(i, key);
        return i;
      }
    }

    keys.Add(key);
    return keys.Count - 1;
  }

  public void RemoveKey(int index)
  {
    if (keys.Count > 2)
    {
      keys.RemoveAt(index);
    }
  }

  public int UpdateKeyTime(int index, float time)
  {
    Color colour = GetKey(index).Colour;
    keys.RemoveAt(index);
    return AddKey(colour, time);
  }

  public void UpdateKeyColour(int index, Color colour)
  {
    ColourKey key = GetKey(index);
    keys[index] = new ColourKey(colour, key.Time);
  }

  public Texture2D GetTexture(int width)
  {
    Texture2D texture = new Texture2D(width, 1);
    Color[] colours = new Color[width];
    for (int i = 0; i < width; i++)
    {
      colours[i] = Evaluate((float)i / (width - 1));
    }
    texture.SetPixels(colours);
    texture.Apply();
    return texture;
  }

}
