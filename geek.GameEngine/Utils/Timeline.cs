using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace geek.GameEngine.Utils
{
	/// <summary>
	/// A class that can execute events at given points of time.
	/// </summary>
	public class Timeline
    {
        #region Constants

        /// <summary>
        /// Number of events that can be skipped before list clean up is executed.
        /// </summary>
	    private int SKIPPED_EVENT_THRESHOLD = 3;

        #endregion

        #region Constructor

        public Timeline()
		{
			_KeyFrames = new List<TimelineRecord>();
		}

		#endregion

		#region Fields

        /// <summary>
        /// Gets or sets the paused flag.
        /// </summary>
	    public bool Paused;

		/// <summary>
		/// Gets or sets the looping flag.
		/// </summary>
		public bool IsLooped;

		/// <summary>
		/// Checks if the timeline has finished.
		/// </summary>
		public bool Finished { get { return !IsLooped && _Current == null; } }

		/// <summary>
		/// The current time of the timeline.
		/// </summary>
		public float CurrentTime { get; private set; }

		/// <summary>
		/// The current frame.
		/// </summary>
		private TimelineRecord _Current;

		/// <summary>
		/// The list of events to happen in time.
		/// </summary>
		private readonly List<TimelineRecord> _KeyFrames;

		/// <summary>
		/// The maximum record ID.
		/// </summary>
		private int _MaxRecordId;

		#endregion

		#region Methods

		/// <summary>
		/// Add a keyframe to the list.
		/// </summary>
		/// <param name="time">Desired time.</param>
		/// <param name="action">Desired action.</param>
		/// <param name="comment">Optional debugging comment.</param>
		public int Add(float time, Action action, string comment = null)
		{
		    return AddAbsolute(CurrentTime + time, action, comment);
		}

        public int AddAbsolute(float time, Action action, string comment = null)
		{
			var record = new TimelineRecord
			{
			    Time = time,
				Action = action,
				Comment = comment,
				RecordId = _MaxRecordId++
			};

			if (_Current == null || _Current.Time > time)
				_Current = record;

			for (var idx = 0; idx < _KeyFrames.Count; idx++)
			{
				var curr = _KeyFrames[idx];
				if (curr.Time > time)
				{
					_KeyFrames.Insert(idx, record);
					return record.RecordId;
				}
			}

			// the current item is the last
			_KeyFrames.Add(record);
			return record.RecordId;
		}

		/// <summary>
		/// Remove a keyframe by it's ID.
		/// </summary>
		/// <param name="id">Keyframe's unique ID in the timeline returned by 'Add' method.</param>
		public void Remove(int id)
		{
			TimelineRecord rec = null;
			foreach (var curr in _KeyFrames)
			{
				if (curr.RecordId == id)
				{
					rec = curr;
					break;
				}
			}

			if (rec == null)
				return;

			if(rec.RecordId == _Current.RecordId)
				shiftCurrent();

			_KeyFrames.Remove(rec);
		}

		/// <summary>
		/// Update the timeline and execute current action.
		/// </summary>
		public void Update()
		{
			if (Finished || Paused)
				return;

			CurrentTime += GameCore.Delta;
			if (CurrentTime <= _Current.Time)
				return;

			_Current.Action();
#if DEBUG

			if (!string.IsNullOrEmpty(_Current.Comment))
				Debug.WriteLine("Action executed: {0}", _Current.Comment);

#endif

            shiftCurrent();
		}

		private void shiftCurrent()
		{
			var nextIndex = _KeyFrames.IndexOf(_Current) + 1;
			if (nextIndex < _KeyFrames.Count)
			{
				_Current = _KeyFrames[nextIndex];

			    if (nextIndex > SKIPPED_EVENT_THRESHOLD && !IsLooped)
			        clearSkipped();
			}
			else
			{
				if (IsLooped)
				{
					CurrentTime = 0;
					_Current = _KeyFrames[0];
				}
				else
				{
					_Current = null;
				}
			}
		}

        private void clearSkipped()
        {
            var count = 0;
            foreach (var curr in _KeyFrames)
                if (curr.Time < CurrentTime)
                    count++;

            _KeyFrames.RemoveRange(0, count-1);
        }

		#endregion

		private class TimelineRecord
		{
			/// <summary>
			/// The record's unique ID.
			/// </summary>
			public int RecordId;

			/// <summary>
			/// Action to execute.
			/// </summary>
			public Action Action;

			/// <summary>
			/// Action's desired time.
			/// </summary>
			public float Time;

			/// <summary>
			/// An optional comment for debugging.
			/// </summary>
			public string Comment;
		}
	}
}
