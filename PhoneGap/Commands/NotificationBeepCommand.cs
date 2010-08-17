//-----------------------------------------------------------------------
// <copyright file="NotificationBeepCommand.cs">
//     Copyright © 2010. All rights reserved.
// </copyright>
// <author>Matt Lacey</author>
//-----------------------------------------------------------------------
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace PhoneGap.Commands
{
    internal class NotificationBeepCommand : PhoneGapCommand
    {
        public override void Execute(params string[] args)
        {
            var times = int.Parse(args[0]);
            var played = 0;

            while (played++ < times)
            {
                using (var stream = TitleContainer.OpenStream("beep.wav"))
                {
                    var effect = SoundEffect.FromStream(stream);
                    effect.Play(); // (vol, 0, 0)
                    
                    // This will pause while the beep plays but as it also blocks!
                    // Could do with finding a better solution if users want lots of beeps (or sound effect is changed to something longer)
                    Thread.Sleep(effect.Duration);
                }
            }
        }
    }
}