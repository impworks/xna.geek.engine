namespace geek.GameEngine.AnimatedProperties
{
	/// <summary>
	/// The list of interpolation modes.
	/// </summary>
	public enum InterpolationMode
	{
		Linear,

		EaseInSoft,
		EaseOutSoft,
		EaseBothSoft,

		EaseInMedium,
		EaseOutMedium,
		EaseBothMedium,

		EaseInHard,
		EaseOutHard,
		EaseBothHard,

		BackIn,
		BackOut,
		BackBoth,

		SinePulse,
		SinePulseBack,

		Bounce,
		Elastic
	}
}
