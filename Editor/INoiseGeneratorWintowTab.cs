#if UNITY_EDITOR
namespace UNG.Editor
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