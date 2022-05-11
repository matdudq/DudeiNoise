#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace UNG.Editor
{
	public partial class NoiseGeneratorWindow
	{
		private class CustomSpaceTab : INoiseGeneratorTab
		{
			private NoiseGeneratorWindow owner;
			
			public CustomSpaceTab(NoiseGeneratorWindow owner)
			{
				this.owner = owner;
			}

			public void OnTabEnter()
			{
				owner.tillingEnabledSP.boolValue = false;
				owner.SetDirty();
				
				owner.RegenerateTextures();
			}
			
			public void DrawTabContent()
			{
				owner.DrawCustomSpaceSectionContent();
			}

			public bool DrawButton()
			{
				return GUILayout.Button(owner.customSpaceTabButtonGC);
			}
		}
	}
}
#endif