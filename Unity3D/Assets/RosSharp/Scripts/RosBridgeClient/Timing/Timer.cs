/*
© Siemens AG, 2017-2019
Author: Dr. Martin Bischoff (martin.bischoff@siemens.com)
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
<http://www.apache.org/licenses/LICENSE-2.0>.
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

// Adding Timestamp switching
// Shimadzu corp , 2019, Akira NODA (a-noda@shimadzu.co.jp / you.akira.noda@gmail.com)

// Removing MonoBehaviour inheritance
// Siemens AG , 2019, Berkay Alp Cakal (berkay_alp.cakal.ct@siemens.com) 

// Added allocation free alternatives
// UoK , 2019, Odysseas Doumas (od79@kent.ac.uk / odydoum@gmail.com)

// Added ability to specify simulation or real time (requires ClockPublisher script)
// 2020, Tyler Stephans (tbs5111@psu.edu)

using System;
using RosSharp.RosBridgeClient.MessageTypes.Std;

namespace RosSharp.RosBridgeClient
{
    public class Timer
    {
        public static DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        public enum timeOptions
        {
            Real,
            Simulation
        }
        public static timeOptions timeRef = timeOptions.Real;   // Default to unix epoch time

        public virtual MessageTypes.Std.Time Now()
        {
            Now(out uint secs, out uint nsecs);
            return new MessageTypes.Std.Time(secs, nsecs);
        }

        public virtual void Now(MessageTypes.Std.Time stamp)
        {
            uint secs; uint nsecs;
            Now(out secs, out nsecs);
            stamp.secs = secs; stamp.nsecs = nsecs;
        }

        private static void Now(out uint secs, out uint nsecs)
        {
            double msecs;
            // If using simulation time, then start the timer with when the game starts
            if (timeRef.Equals(timeOptions.Simulation))
            {
                msecs = UnityEngine.Time.fixedTime * 1000;
                //msecs = myTimer.simTime * 1000;
            }
            else
            {
                TimeSpan timeSpan = DateTime.Now.ToUniversalTime() - UNIX_EPOCH;
                msecs = timeSpan.TotalMilliseconds;
            }
            secs = (uint)(msecs / 1000);
            nsecs = (uint)((msecs / 1000 - secs) * 1e+9);
        }
    }
}
