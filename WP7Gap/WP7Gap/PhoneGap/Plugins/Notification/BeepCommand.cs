//-----------------------------------------------------------------------
// <copyright file="BeepCommand.cs">
//     Copyright © 2010. All rights reserved.
// </copyright>
// <author>Matt Lacey</author>
//-----------------------------------------------------------------------
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace PhoneGap.Plugins.Notification
{
    internal class BeepCommand : PhoneGapCommand
    {
        public override void Execute(params string[] args)
        {
            var times = int.Parse(args[0]);
            var played = 0;

            while (played++ < times)
            {
                using (var stream = TitleContainer.OpenStream("PhoneGap/Plugins/Notification/beep.wav"))
                {
                    var effect = SoundEffect.FromStream(stream);
                    effect.Play(); // (vol, 0, 0)

                    // This will pause while the beep plays but as it also blocks!
                    // Could do with finding a better solution if users want lots of beeps (or sound effect is changed to something longer)
                    if (times > 0)
                    {
                        Thread.Sleep(effect.Duration);
                    }
                }
            }
        }
    }
}