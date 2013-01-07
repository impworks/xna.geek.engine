using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Devices.Sensors;

namespace geek.GameEngine
{
	/// <summary>
	/// The accelerometer manager.
	/// </summary>
	public static class AccelManager
	{
		#region Constructor

		static AccelManager()
		{
			_Accelerometer = new Accelerometer();
			_Accelerometer.CurrentValueChanged += accelChanged;
			Enabled = true;
		}

		#endregion

		#region Fields

		/// <summary>
		/// The accelerometer object.
		/// </summary>
		private static Accelerometer _Accelerometer;

		/// <summary>
		/// Accelerometer's X axis.
		/// </summary>
		public static float X { get; private set; }

		/// <summary>
		/// Accelerometer's Y axis.
		/// </summary>
		public static float Y { get; private set; }

		/// <summary>
		/// Accelerometer's Z axis.
		/// </summary>
		public static float Z { get; private set; }

		/// <summary>
		/// Checks if the readings are enabled or not.
		/// </summary>
		private static bool _Enabled;
		public static bool Enabled
		{
			get { return _Enabled; }
			set
			{
				if (value)
					tryEnable();
				else
					disable();
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Try to enable the accelerometer, if possible.
		/// </summary>
		private static void tryEnable()
		{
			if (_Enabled || !Accelerometer.IsSupported)
				return;

			try
			{
				_Accelerometer.Start();
				_Enabled = true;
			}
			catch { }
		}

		/// <summary>
		/// Disable the accelerometer readings.
		/// </summary>
		private static void disable()
		{
			if (!_Enabled)
				return;

			_Enabled = false;
			_Accelerometer.Stop();
		}

		/// <summary>
		/// Update the accelerometer readings in local fields.
		/// </summary>
		private static void accelChanged(object sender, SensorReadingEventArgs<AccelerometerReading> args)
		{
			X = args.SensorReading.Acceleration.X;
			Y = args.SensorReading.Acceleration.Y;
			Z = args.SensorReading.Acceleration.Z;
		}

		#endregion
	}
}
