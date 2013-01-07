using geek.GameEngine.Visuals;

namespace geek.GameEngine.Behaviours
{
	/// <summary>
	/// The class that defines a typical behaviour for a dynamic object.
	/// </summary>
	public interface IBehaviour
	{
		/// <summary>
		/// Apply the behaviour to the object.
		/// </summary>
		/// <param name="obj">Object to modify.</param>
		void UpdateObjectState(DynamicObject obj);
	}
}
