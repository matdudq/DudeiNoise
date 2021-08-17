#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace DudeiNoise.Editor
{
	public partial class NoiseGeneratorWindow
	{
		#region Variables

		private GUIStyle sectionStyle;

		private bool stylesInitialized = false;

		#endregion Variables

		#region Private methods

		private void InitializeStyles()
		{
			sectionStyle = new GUIStyle(EditorStyles.helpBox);
		}

		#endregion Private methods
	}
}
#endif