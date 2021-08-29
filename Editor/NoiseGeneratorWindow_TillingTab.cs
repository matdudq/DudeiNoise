#if UNITY_EDITOR
using UnityEngine;

namespace DudeiNoise.Editor
{
	public partial class NoiseGeneratorWindow
	{
		private class TillingTab : INoiseGeneratorTab
		{
			private NoiseGeneratorWindow owner;
			
			public TillingTab(NoiseGeneratorWindow owner)
			{
				this.owner = owner;
				owner.frequencyValue = owner.scaleOffsetSP.vector3Value.x;
			}
			
			public void OnTabEnter()
			{
				owner.tillingEnabledSP.boolValue = true;

				owner.SetDirty();
				
				owner.RegenerateTextures();
			}
			
			public void DrawTabContent()
			{
				owner.DrawTillingSectionContent();
			}

			public bool DrawButton()
			{
				return GUILayout.Button(owner.tillingTabButtonGC);
			}
		}
	}
}
#endif