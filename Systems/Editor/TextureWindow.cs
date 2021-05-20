#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace DudeiNoise.Editor
{
	public class TextureWindow : EditorWindow
	{
		#region Variables

		private Texture2D texture = null;
		
		private static Vector2 windowSize = new Vector2(400, 400);

		#endregion Variables

		#region Unity methods
		
		private void OnGUI()
		{
			Rect displayArea = new Rect(0, 0, position.width, position.height);
			
			EditorGUILayout.BeginVertical();
			GUILayout.BeginArea(displayArea);
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			DisplayTextureOfType(displayArea);
			
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.EndArea();
			
			
			EditorGUILayout.EndVertical();
		}

		#endregion Unity methods
		
		#region Public methods

		public static TextureWindow GetWindow(Vector2 position, string name, int resolution, FilterMode mode)
		{
			TextureWindow window = GetWindow<TextureWindow>(name);
			window.titleContent = new GUIContent(name);
			window.position = new Rect(position.x,position.y,windowSize.x, windowSize.y);
			window.minSize = windowSize;
			window.maxSize = windowSize;
			window.texture = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false)
			{
				name = "Noise",
				filterMode = mode,
				wrapMode = TextureWrapMode.Clamp
			};
			
			window.Show();

			return window;
		}

		public void UpdateTexture(Color[] textureArray, int resolution, FilterMode mode)
		{
			if (texture.filterMode != mode)
			{
				texture.filterMode = mode;
			}
			
			if (texture.width != resolution)
			{
				texture.Resize( resolution, resolution);
			}
			
			texture.SetPixels(textureArray);
			texture.Apply();
		}

		#endregion Public methods
		
		#region Private methods

		private void DisplayTextureOfType(Rect displayArea)
		{
			EditorGUI.DrawPreviewTexture(displayArea,texture);
		}

		#endregion Private methods
	}
}
#endif