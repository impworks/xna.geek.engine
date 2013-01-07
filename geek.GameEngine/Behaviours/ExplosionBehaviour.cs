using System;
using geek.GameEngine.Visuals;

namespace geek.GameEngine.Behaviours
{
	/// <summary>
	/// The fade out effect that creates an explosion with a particle system
	/// </summary>
	public class ExplosionBehaviour<T> : IBehaviour, IFadeOut where T: VisualObjectBase, new()
	{
		#region IBehaviour implementation

		public void UpdateObjectState(DynamicObject obj)
		{
			//		,--.
			//	   (	)
			//		`--'\___
			//	       //\\--\
			//	    D_//  ||--\
			//		   ___|	\__\
			//		  /|	' ### -
			//		_/_|	  -###.
			//		 			 #.
			//					  '.
			//
			//	  Nothing to do here :)
		}

		#endregion

		#region IFadeOut implementation

		public void ActivateFadeOut(DynamicObject obj)
		{
			FadeOutFinished = true;

			var newObj = new T {Position = obj.GetPosition(true)};
			GameStoryBoard.CurrentScene.AddChildDeferred(newObj);
		}

		public bool FadeOutFinished { get; private set; }

		#endregion
	}

	/// <summary>
	/// The fade out effect that creates an explosion with a particle system
	/// </summary>
	public class ExplosionBehaviour : IBehaviour, IFadeOut
	{
		public ExplosionBehaviour(Func<VisualObjectBase> gen)
		{
			_Generator = gen;
		}

		private readonly Func<VisualObjectBase> _Generator;
		public bool FadeOutFinished { get; private set; }

		public void UpdateObjectState(DynamicObject obj)
		{ }

		public void ActivateFadeOut(DynamicObject obj)
		{
			FadeOutFinished = true;

			var newObj = _Generator();
			newObj.Position = obj.GetPosition(true);
			GameStoryBoard.CurrentScene.AddChildDeferred(newObj);
		}
	}
}
