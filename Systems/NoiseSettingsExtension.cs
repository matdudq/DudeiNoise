namespace DudeiNoise
{
	public static class NoiseSettingsExtension
	{
		#region Public methods

		public static NoiseMethod NoiseMethod(this NoiseSettings generatorSettings)
		{
			return Noise.methods[(int) generatorSettings.noiseType][generatorSettings.dimensions - 1];
		}

		#endregion Public methods
	}
}