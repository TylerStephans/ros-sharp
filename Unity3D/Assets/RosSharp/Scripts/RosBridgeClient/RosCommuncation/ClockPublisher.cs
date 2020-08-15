// Publish clock messages to ROS. Can be real ot simulation time
// Based on DyncoClockPublisher from https://github.com/samiamlabs/ros-sharp/blob/new-warehouse/Unity3D/Assets/Dyno/Scripts/DynoClockPublisher.cs
// 2020, Tyler Stephans (tbs5111@psu.edu)

using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class ClockPublisher : UnityPublisher<MessageTypes.Rosgraph.Clock>
    {
        public Timer.timeOptions timeReference = Timer.timeOptions.Real;
        [Tooltip("Whether to publish clock message to ROS.")]
        public bool publishClock = false;
        public string publishedTime { get; private set; } = "...";
        private double nseconds;
        private MessageTypes.Rosgraph.Clock clock;

        public ClockPublisher()
        {
            Topic = "/clock";
        }
        
        protected override void Start()
        {
            Timer.timeRef = timeReference;  // Tell Timer what time reference to use
            base.Start();
            clock = new MessageTypes.Rosgraph.Clock();
        }

        public void UpdateClock()
        {
            // Use Standard Header Extension as source of truth for time
            MessageTypes.Std.Header header = new MessageTypes.Std.Header();
            header.Update();
            clock.clock = header.stamp;

            nseconds = (double)header.stamp.nsecs / 1.0e9;
            publishedTime = header.stamp.secs.ToString("F0") + nseconds.ToString("F9").Substring(1);

            if (publishClock)
                Publish(clock);
        }

        private void FixedUpdate()
        {
            UpdateClock();
        }

    }
}