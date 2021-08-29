#if UNITY_EDITOR
namespace DudeiNoise.Editor
{
	public partial class NoiseGeneratorWindow 
	{
		private interface INoiseGeneratorTab
		{
			void OnTabEnter();
			
			void DrawTabContent();

			bool DrawButton();
		}
	}
}
#endif