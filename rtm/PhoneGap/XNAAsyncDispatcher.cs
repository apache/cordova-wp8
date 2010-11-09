//-----------------------------------------------------------------------
// <copyright file="XnaAsyncDispatcher.cs">
//     Copyright © 2010. All rights reserved.
// </copyright>
// <author>Matt Lacey</author>
//-----------------------------------------------------------------------
using System;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Xna.Framework;

namespace PhoneGap
{
    public sealed class XnaAsyncDispatcher : IApplicationService
    {
        private readonly DispatcherTimer frameworkDispatcherTimer;

        public XnaAsyncDispatcher(TimeSpan dispatchInterval)
        {
            this.frameworkDispatcherTimer = new DispatcherTimer();
            this.frameworkDispatcherTimer.Tick += frameworkDispatcherTimer_Tick;
            this.frameworkDispatcherTimer.Interval = dispatchInterval;
        }

        void IApplicationService.StartService(ApplicationServiceContext context)
        {
            this.frameworkDispatcherTimer.Start();
        }

        void IApplicationService.StopService()
        {
            this.frameworkDispatcherTimer.Stop();
        }

        public static void frameworkDispatcherTimer_Tick(object sender, EventArgs e)
        {
            FrameworkDispatcher.Update();
        }
    }
}