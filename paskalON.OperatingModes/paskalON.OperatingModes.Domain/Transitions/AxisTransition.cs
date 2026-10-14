// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Transitions
{
    /// <summary>
    /// Bumpless transition of one power axis after changes of the operating modes.
    /// </summary>
    /// <remarks>
    /// The transition consists of one segment per changed operating mode. Each segment ramps the step of its operating mode to
    /// zero with the ramp of that operating mode, so several changes in the same cycle or while other segments are still running
    /// keep their own ramp policy. The applied target is the objective plus the sum of the segment offsets.
    /// Values are in the base unit (Watts or Vars).
    /// </remarks>
    public sealed class AxisTransition
    {
        /// <summary>
        /// Lock object for thread-safe access to the segments.
        /// </summary>
        private readonly object _dataLock = new object();


        /// <summary>
        /// Running segments in start order.
        /// </summary>
        private readonly List<TransitionSegment> _segments = new List<TransitionSegment>();


        /// <summary>
        /// Gets a snapshot of the running segments in start order.
        /// </summary>
        public IReadOnlyList<TransitionSegment> Segments
        {
            get { lock (_dataLock) { return _segments.ToArray(); } }
        }


        /// <summary>
        /// Gets the total offset between the applied target and the objective.
        /// </summary>
        public double Offset
        {
            get { lock (_dataLock) { return _segments.Sum(segment => segment.Offset); } }
        }


        /// <summary>
        /// Gets the kind of the latest running segment or none.
        /// </summary>
        public TransitionKind Kind
        {
            get { lock (_dataLock) { return _segments.Count == 0 ? TransitionKind.None : _segments[^1].Kind; } }
        }


        /// <summary>
        /// Gets the name of the operating mode of the latest running segment or null.
        /// </summary>
        public string? OperatingModeName
        {
            get { lock (_dataLock) { return _segments.Count == 0 ? null : _segments[^1].OperatingModeName; } }
        }


        /// <summary>
        /// Advances all running segments to the current time and removes the finished segments.
        /// </summary>
        /// <remarks>
        /// Advances (ramp controller calculate) all running segments to the current time and removes the finished segments.
        /// </remarks>
        /// <returns>The total offset of the running segments.</returns>
        public double Advance()
        {
            lock (_dataLock)
            {
                double offset = _segments.Sum(segment => segment.Advance());
                _segments.RemoveAll(segment => segment.IsFinished);

                return offset;
            }
        }


        /// <summary>
        /// Starts the segments of the changed operating modes so that the applied target stays continuous.
        /// </summary>
        /// <param name="requiredOffset">The offset of the new segments keeping the applied target continuous (origin - objective - offset of the previous cycle).</param>
        /// <param name="requests">The requests of the changed operating modes.</param>
        /// <remarks>
        /// Each request starts with its own step. The part of the required offset that is not explained by the steps of the
        /// changed operating modes (e.g. a limiting mode reacting to a replaced base mode) is assigned to the request of the
        /// operating mode owning the axis, otherwise to the request with the largest step. A request of an operating mode
        /// with a running segment is merged into that segment.
        /// 
        /// Required offset is what the new segments must cover together so the applied target doesn't jump.
        /// For example: requiredOffset=3 MW means the plant was applying 8 MW and the objective after the change is 5 MW, so 3 MW must be ramped away.
        /// If the requiredOffset is uneven the sum of all requests then the last requested in the list gets the reminder.        
        /// </remarks>
        public void Start(double requiredOffset, IReadOnlyList<TransitionRequest> requests)
        {
            ArgumentNullException.ThrowIfNull(requests);

            if (requests.Count == 0)
            {
                return;
            }

            lock (_dataLock)
            {
                double remainder = requiredOffset - requests.Sum(request => request.Offset);
                int absorbingIndex = GetAbsorbingIndex(requests);

                for (int i = 0; i < requests.Count; i++)
                {
                    TransitionRequest request = requests[i];
                    double offset = i == absorbingIndex ? request.Offset + remainder : request.Offset;
                    AddSegment(request, offset);
                }

                _segments.RemoveAll(segment => segment.IsFinished);
            }
        }


        /// <summary>
        /// Limits the total offset after a mandatory constraint limited the applied target.
        /// </summary>
        /// <remarks>
        /// The segments are scaled down if the allowed offset points in the same direction, otherwise they are cancelled.
        /// </remarks>
        /// <param name="offset">The allowed total offset (constrained applied target - objective).</param>
        public void Constrain(double offset)
        {
            lock (_dataLock)
            {
                double total = _segments.Sum(segment => segment.Offset);

                if (Math.Abs(total) < TransitionSegment.FinishTolerance || Math.Abs(total - offset) < TransitionSegment.FinishTolerance)
                {
                    return;
                }

                if (Math.Sign(total) == Math.Sign(offset) && Math.Abs(offset) < Math.Abs(total))
                {
                    double factor = offset / total;
                    _segments.ForEach(segment => segment.Scale(factor));
                    _segments.RemoveAll(segment => segment.IsFinished);

                    return;
                }

                CancelSegments();
            }
        }


        /// <summary>
        /// Cancels all running segments, e.g. when an override forces the applied target.
        /// </summary>
        public void Cancel()
        {
            lock (_dataLock)
            {
                CancelSegments();
            }
        }


        /// <summary>
        /// Gets the index of the request absorbing the remainder.
        /// </summary>
        /// <param name="requests">The requests.</param>
        /// <returns>The index of the last request owning the axis, otherwise of the request with the largest step.</returns>
        private static int GetAbsorbingIndex(IReadOnlyList<TransitionRequest> requests)
        {
            int absorbingIndex = 0;

            for (int i = 0; i < requests.Count; i++)
            {
                if (requests[i].OwnsAxis)
                {
                    absorbingIndex = i;
                }
                else if (requests[absorbingIndex].OwnsAxis is false && Math.Abs(requests[i].Offset) > Math.Abs(requests[absorbingIndex].Offset))
                {
                    absorbingIndex = i;
                }
            }

            return absorbingIndex;
        }


        /// <summary>
        /// Adds a segment for the request or merges it into the running segment of the same operating mode.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="offset">The offset of the request including an assigned remainder.</param>
        private void AddSegment(TransitionRequest request, double offset)
        {
            TransitionSegment? running = _segments.FirstOrDefault(segment => string.Equals(segment.OperatingModeName, request.OperatingModeName, StringComparison.Ordinal));

            if (running is not null)
            {
                running.Merge(request.Kind, offset, request.RampController);

                return;
            }

            if (Math.Abs(offset) < TransitionSegment.FinishTolerance)
            {
                return;
            }

            _segments.Add(new TransitionSegment(request.OperatingModeName, request.Kind, offset, request.RampController));
        }


        /// <summary>
        /// Stops and removes all segments.
        /// </summary>
        private void CancelSegments()
        {
            _segments.ForEach(segment => segment.Stop());
            _segments.Clear();
        }
    }
}
