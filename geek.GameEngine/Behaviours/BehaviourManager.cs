using System;
using System.Collections;
using System.Collections.Generic;
using geek.GameEngine.Visuals;

namespace geek.GameEngine.Behaviours
{
	public class BehaviourManager : IEnumerable<IBehaviour>
	{
		#region Fields

		/// <summary>
		/// The list of behaviours
		/// </summary>
		private readonly Dictionary<Type, IBehaviour> _Behaviours = new Dictionary<Type, IBehaviour>();

		#endregion

		#region Methods

		/// <summary>
		/// Apply the behaviours to the object.
		/// </summary>
		/// <param name="obj">Object to modify.</param>
		public void UpdateObjectState(DynamicObject obj)
		{
			var hasFadeOut = false;
			var mustRemove = true;
			foreach (var curr in _Behaviours)
			{
				curr.Value.UpdateObjectState(obj);

				var fadeOut = curr.Value as IFadeOut;
				if (fadeOut != null)
				{
					hasFadeOut = true;
					mustRemove &= fadeOut.FadeOutFinished;
				}
			}

			if(hasFadeOut && mustRemove)
				obj.Remove(true);
		}

		/// <summary>
		/// Add a behaviour to the list.
		/// </summary>
		/// <param name="behaviour">Behaviour.</param>
		public void Add(IBehaviour behaviour)
		{
			var type = behaviour.GetType();
			if (!_Behaviours.ContainsKey(type))
				_Behaviours.Add(type, behaviour);
		}

		/// <summary>
		/// Add multiple behaviours to the object.
		/// </summary>
		/// <param name="behaviours">Set of behaviours to add.</param>
		public void Add(params IBehaviour[] behaviours)
		{
			foreach(var curr in behaviours)
				Add(curr);
		}

		/// <summary>
		/// Gets a specific behaviour if it exists in the list.
		/// </summary>
		/// <typeparam name="T">Behaviour type</typeparam>
		public T Get<T>() where T: IBehaviour
		{
			IBehaviour result;
			_Behaviours.TryGetValue(typeof (T), out result);
			return (T)result;
		}

		/// <summary>
		/// Checks whether the object implements a specific behaviour.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public bool Contains<T>() where T: class, IBehaviour
		{
			return Get<T>() != null;
		}

		/// <summary>
		/// Remove a behaviour from the list.
		/// </summary>
		/// <typeparam name="T">Behaviour type.</typeparam>
		public void Remove<T>() where T: IBehaviour
		{
			var type = typeof (T);

			if(_Behaviours.ContainsKey(type))
				_Behaviours.Remove(type);
		}

		/// <summary>
		/// Remove any behaviour that is a subclass of the given behaviour type.
		/// </summary>
		/// <typeparam name="T">Interface type.</typeparam>
		public void RemoveAny<T>()
		{
			var type = typeof (T);
			var typesToRemove = new List<Type>();
			foreach (var curr in _Behaviours.Keys)
			{
				foreach(var currInter in curr.GetInterfaces())
					if(currInter == type)
						typesToRemove.Add(curr);
			}

			foreach (var curr in typesToRemove)
				_Behaviours.Remove(curr);
		}

		/// <summary>
		/// Remove all behaviours from the object.
		/// </summary>
		public void Clear()
		{
			_Behaviours.Clear();
		}

		#endregion

		#region IEnumerable<Behaviour> implementation

		public IEnumerator<IBehaviour> GetEnumerator()
		{
			return (_Behaviours.Values as IEnumerable<IBehaviour>).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _Behaviours.Values.GetEnumerator();
		}

		#endregion
	}
}
