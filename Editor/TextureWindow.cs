#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace UNG.Editor
{
	public class TextureWindow : EditorWindow
	{
		#region Variables

		private NoiseTexture noiseTexture = null;

		private Material displayMaterial = null;
		
		private static Vector2 windowSize = new Vector2(400, 400);

		private static string displayMaterialAssetName = "TextureWindowMaterial";
		private static string channelMaskMaterialProp = "_ChannelMask";

		private static Color[] channelMasks = new[]
		{
			new Color(1, 0, 0, 0),
			new Color(0, 1, 0, 0),
			new Color(0, 0, 1, 0),
			new Color(0, 0, 0, 1)
		};
		
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
		
		public static TextureWindow GetWindow(Vector2 position, string name, NoiseTexture noiseTexture)
		{
			TextureWindow window = GetWindow<TextureWindow>(name);
			window.titleContent = new GUIContent(name);
			window.position = new Rect(position.x,position.y,windowSize.x, windowSize.y);
			window.minSize = windowSize;
			window.maxSize = windowSize;
			window.noiseTexture = noiseTexture;
			window.displayMaterial = Instantiate(LoadDisplayMaterial());
			window.Show();

			return window;
		}

		public void RepaintForChannel(NoiseTextureChannel textureChannel)
		{
			displayMaterial.SetColor(channelMaskMaterialProp, channelMasks[(int)textureChannel]);
			Repaint();
		}
		
		#endregion Public methods
		
		#region Private methods
		
		private void DisplayTextureOfType(Rect displayArea)
		{
			EditorGUI.DrawPreviewTexture(displayArea,noiseTexture.Texture, displayMaterial);
		}
		
		private static Material LoadDisplayMaterial()
		{
			string[] materialGUIDs = AssetDatabase.FindAssets(displayMaterialAssetName);

			if (materialGUIDs.Length == 0)
			{
				Debug.LogWarning(" There is no one material called {displayMaterialAssetName}. Texture Window can display texture incorrectly!");
				return null;
			}
			
			if (materialGUIDs.Length > 1)
			{
				Debug.LogWarning(" There are more than one materials called {displayMaterialAssetName}. Texture Window can display texture incorrectly!");
			}

			string materialPath = AssetDatabase.GUIDToAssetPath(materialGUIDs[0]);
			return AssetDatabase.LoadAssetAtPath<Material>(materialPath);
		}
		
		#endregion Private methods
	}
}
#endif