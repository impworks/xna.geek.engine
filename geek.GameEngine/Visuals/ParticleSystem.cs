using System;
using Microsoft.Xna.Framework;
using geek.GameEngine.Behaviours;
using geek.GameEngine.Utils;

namespace geek.GameEngine.Visuals
{
	/// <summary>
	/// The class that manages a swarm of particles.
	/// </summary>
	public class ParticleSystem : ObjectGroup
	{
		#region Constructors

		public ParticleSystem(int rate, Func<DynamicObject> generator = null)
		{
			ParticleAngle = new JitteryValue(0, MathHelper.Pi);
			Output = rate;
			Generator = generator;
			IsActive = true;
		}

		public ParticleSystem(int rate, string particleAssetName)
			: this(rate, () => createDefaultParticle(particleAssetName))
		{ }

		#endregion

		#region Fields

		/// <summary>
		/// Checks if the PS should create new particles.
		/// </summary>
		public bool IsActive;

		/// <summary>
		/// The location at which to create 
		/// </summary>
		public JitteryVector2 ParticleOrigin;

		/// <summary>
		/// The angle to propel objects to.
		/// </summary>
		public JitteryValue ParticleAngle;

		/// <summary>
		/// The strength of the propulsion.
		/// </summary>
		public JitteryValue ParticlePropulsion;

		/// <summary>
		/// The number of particles the system may produce before self-destruction.
		/// </summary>
		public int ParticleLimit;

		/// <summary>
		/// The generator of new particles.
		/// </summary>
		public Func<DynamicObject> Generator;

		/// <summary>
		/// Number of objects to create per second.
		/// </summary>
		public int Output
		{
			get { return _CreationRate.IsAlmostNull() ? 0 : (int) (1/_CreationRate); }
			set { _CreationRate = value == 0 ? 0 : 1.0f/value; }
		}

		/// <summary>
		/// Each particle's lifefime length before it gets destroyed.
		/// </summary>
		public JitteryValue TimeToLive;

		/// <summary>
		/// The amount of time between the creation of particles.
		/// </summary>
		private float _CreationRate;

		/// <summary>
		/// Time elapsed since last particle creation.
		/// </summary>
		private float _ElapsedTime;

		/// <summary>
		/// The number of particles the system has emitted up to date.
		/// </summary>
		private int _ParticlesCreated;

		#endregion

		#region Methods

		public override void Update()
		{
			if (IsActive)
			{
				_ElapsedTime += GameCore.Delta;
				if (_ElapsedTime > _CreationRate)
				{
					while (_ElapsedTime > _CreationRate)
					{
						_ElapsedTime -= _CreationRate;
						createParticle();
					}
				}
			}

			base.Update();
		}

		/// <summary>
		/// Create a new particle in the swarm.
		/// </summary>
		protected void createParticle()
		{
			if (ParticleLimit > 0)
			{
				_ParticlesCreated++;
				if (_ParticlesCreated >= ParticleLimit)
				{
					// stop creating particles when the limit has been reached
					// and remove the system itself when all the particles have disappeared as well
					if (Objects.Count == 0)
						Remove();

					return;
				}
			}

			if (Generator == null)
				throw new ArgumentException("Particle generator function is not provided!");

			var obj = Generator();
			tweakParticle(obj);

			AddChild(obj);
		}

		/// <summary>
		/// Set the particle's default properties.
		/// </summary>
		/// <param name="obj">Particle to tweak.</param>
		protected virtual void tweakParticle(DynamicObject obj)
		{
			obj.Position = ParticleOrigin.GetValue();
			obj.Momentum = Globals.CreateVector(ParticlePropulsion.GetValue(), ParticleAngle.GetValue());
			obj.Behaviours.Add(new TimebombBehaviour(TimeToLive.GetValue()));

			// speed optimization: disable touch support for particles
			var itb = obj as InteractableObject;
			if (itb != null)
				itb.IsTouchable = false;
		}

		/// <summary>
		/// Create a particle out of an asset.
		/// </summary>
		/// <param name="assetName">Asset name.</param>
		private static GameObject createDefaultParticle(string assetName)
		{
			var obj = new GameObject();
			obj.AddAnimation(assetName).SetHotSpot();
			return obj;
		}

		/// <summary>
		/// Reinitialize the particle system properties.
		/// </summary>
		public void Reset()
		{
			IsActive = true;
			_ElapsedTime = 0;
			_ParticlesCreated = 0;
			Clear();
		}

		#endregion
	}
}
