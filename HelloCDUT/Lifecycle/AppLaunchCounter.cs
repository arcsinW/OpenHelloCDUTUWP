﻿//  Copyright (c) Microsoft Corporation.  All rights reserved.
// 
//  The MIT License (MIT)
// 
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//  ---------------------------------------------------------------------------------

using Windows.Storage;

namespace HelloCDUT.Lifecycle
{
    /// <summary>
    /// Manages how often the app has been launched.
    /// </summary>
    public class AppLaunchCounter
    {
        private const string LaunchCountKey = "LAUNCH_COUNT";

        /// <summary>
        /// Determines whether the app has been launched for the first time.
        /// </summary>
        /// <returns>True, if launched for first time. Otherwise, false.</returns>
        public static bool IsFirstLaunch()
        {
            var value = ApplicationData.Current.LocalSettings.Values[LaunchCountKey];

            return (value == null || (int)value <= 1);
        }

        /// <summary>
        /// Registers the app launch.
        /// </summary>
        public static void RegisterLaunch()
        {
            var value = ApplicationData.Current.LocalSettings.Values[LaunchCountKey];

            if (value == null)
            {
                ApplicationData.Current.LocalSettings.Values[LaunchCountKey] = 1;
            }
            else
            {
                var i = ((int)value);
                ApplicationData.Current.LocalSettings.Values[LaunchCountKey] = ++i;
            }
        }
    }
}