using DudeiNoise.Editor.Utilities;
using UnityEngine;

namespace DudeiNoise
{
	[CreateAssetMenu(fileName = "NoiseSettings", menuName = "Noise/Create Settings", order = 1)]
	public class NoiseTextureSettings : ScriptableObject
	{
		#region Variables
		
		[Tooltip("Folder where textures will be saved.")]
		public FolderReference exportFolder = new FolderReference();

		[Tooltip("Resolution of rendered texture.")]
		public int resolution = 256;
		
		[Tooltip("Filter mode of rendered texture. Good to see different ways of filtering to se how noise works.")]
		public FilterMode filterMode = FilterMode.Point;
		
		public NoiseSettings redChannelNoiseSettings = null;
		
		public NoiseSettings greenChannelNoiseSettings = null;
		
		public NoiseSettings blueChannelNoiseSettings = null;
		
		public NoiseSettings alphaChannelNoiseSettings = null;

		#endregion Variables

		#region Public methods

		public NoiseSettings GetNoiseSettingsForChannel(NoiseTextureChannel noiseTextureChannel)
		{
			switch (noiseTextureChannel)
			{
				case NoiseTextureChannel.RED:
					return redChannelNoiseSettings;
				case NoiseTextureChannel.GREEN:
					return greenChannelNoiseSettings;
				case NoiseTextureChannel.BLUE:
					return blueChannelNoiseSettings;
				case NoiseTextureChannel.ALPHA:
					return alphaChannelNoiseSettings;
				case NoiseTextureChannel.FULL:
					return redChannelNoiseSettings;
			}

			Debug.Log( $"Something goes wrong with defined channel {noiseTextureChannel}");
			return null;
		}

		#endregion Public methods
	}
}