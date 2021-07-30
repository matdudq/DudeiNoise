#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace DudeiNoise.Editor
{
	public class TextureWindow : EditorWindow
	{
		#region Variables

		private NoiseTexture noiseTexture = null;

		private NoiseTextureChannel textureChannel = NoiseTextureChannel.RED;
		
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

		public static TextureWindow GetWindow(Vector2 position, string name, NoiseTexture noiseTexture)
		{
			TextureWindow window = GetWindow<TextureWindow>(name);
			window.titleContent = new GUIContent(name);
			window.position = new Rect(position.x,position.y,windowSize.x, windowSize.y);
			window.minSize = windowSize;
			window.maxSize = windowSize;
			window.noiseTexture = noiseTexture;
			
			window.Show();

			return window;
		}

		public void RepaintForChannel(NoiseTextureChannel textureChannel)
		{
			this.textureChannel = textureChannel;
			Repaint();
		}
		
		#endregion Public methods
		
		#region Private methods
		
		private void DisplayTextureOfType(Rect displayArea)
		{
			EditorGUI.DrawPreviewTexture(displayArea,noiseTexture.Texture,null ,ScaleMode.ScaleAndCrop,1,0,textureChannel.ToColorWriteMask());
		}

		#endregion Private methods
	}
}
#endif