using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public class RenderTextureToPNG : EditorWindow {
  [MenuItem("Editor/RenderTextureToPNG")]
  static void Open() {
    GetWindow<RenderTextureToPNG>("RenderTextureToPNG");
  }

  RenderTexture renderTextureRef;
  string textureName;

  void OnGUI() {
    renderTextureRef = EditorGUILayout.ObjectField("RenderTexture", renderTextureRef, typeof(RenderTexture), true) as RenderTexture;
    textureName = EditorGUILayout.TextField("Texture Name", textureName);

    if (GUILayout.Button("Make"))
    {
      var path = GetPath(textureName);
      var bytes = MakePNGBytes(renderTextureRef);
      File.WriteAllBytes(path, bytes);
    }
  }

  static string GetPath(string name)
  {
    var projectWindowUtilType = typeof(ProjectWindowUtil);
    var getActiveFolderPathMethod = projectWindowUtilType.GetMethod("GetActiveFolderPath", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
    var pathToCurrentFolder = getActiveFolderPathMethod.Invoke(null, new object[0]).ToString();
    var directory = pathToCurrentFolder;
    directory = directory.Split('/').Skip(1).Aggregate((s1, s2) => $"{s1}/{s2}");
    return $"{Application.dataPath}/{directory}/{name}.png";
  }

  static byte[] MakePNGBytes(RenderTexture renderTextureRef)
  {
    RenderTexture.active = renderTextureRef;
    var tex = new Texture2D(renderTextureRef.width, renderTextureRef.height, TextureFormat.RGB24, false);
    tex.ReadPixels(new Rect(0, 0, renderTextureRef.width, renderTextureRef.height), 0, 0);
    tex.Apply();
    if (PlayerSettings.colorSpace == ColorSpace.Linear)
    {
      // ガンマ補正
      var color = tex.GetPixels();
      for (int i = 0; i < color.Length; i++)
      {
        color[i].r = Mathf.LinearToGammaSpace(color[i].r);
        color[i].g = Mathf.LinearToGammaSpace(color[i].g);
        color[i].b = Mathf.LinearToGammaSpace(color[i].b);
      }
      tex.SetPixels(color);
    }
    var bytes = tex.EncodeToPNG();
    Object.DestroyImmediate(tex);
    return bytes;
  }
}
#endif
