#if UNITY_EDITOR
namespace DudeiNoise.Editor
{
	public partial class NoiseGeneratorWindow 
	{
		private interface INoiseGeneratorModeTab
		{
			void OnTabEnter();
			
			void DrawInspector();

			bool DrawButton();
		}
	}
}
#endif