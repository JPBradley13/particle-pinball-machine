//
// Copyright (c) LightBuzz Software.
// All rights reserved.
//
// http://lightbuzz.com
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions
// are met:
//
// 1. Redistributions of source code must retain the above copyright
//    notice, this list of conditions and the following disclaimer.
//
// 2. Redistributions in binary form must reproduce the above copyright
//    notice, this list of conditions and the following disclaimer in the
//    documentation and/or other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
// FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
// COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
// INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
// BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS
// OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED
// AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
// LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY
// WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.
//

using LightBuzz.Vitruvius.Gestures;
using Windows.Kinect;
using System;
using System.Linq;
using System.Collections.Generic;

namespace LightBuzz.Vitruvius
{
    /// <summary>
    /// Represents a gesture controller.
    /// </summary>
    public class GestureController : BaseController<Body>
    {
        #region Members

        /// <summary>
        /// A list of all the gestures the controller is searching for.
        /// </summary>
        private List<Gesture> _gestures = new List<Gesture>();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="GestureController"/> with all of the available gesture types.
        /// </summary>
        public GestureController()
        {
            foreach (GestureType t in Enum.GetValues(typeof(GestureType)))
            {
                AddGesture(t);
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="GestureController"/> with the specified gesture type.
        /// </summary>
        /// <param name="type">The gesture type to recognize.</param>
        public GestureController(GestureType type)
        {
            AddGesture(type);
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a gesture is recognized.
        /// </summary>
        public event EventHandler<GestureEventArgs> GestureRecognized;

        #endregion

        #region Properties

        public List<Gesture> GetGestures() { return _gestures; }

        #endregion

        #region Methods

        /// <summary>
        /// Updates all gestures.
        /// </summary>
        /// <param name="body">The body data to search for gestures.</param>
        public override void Update(Body body)
        {
            base.Update(body);

            foreach (Gesture gesture in _gestures)
            {
                gesture.Update(body);
            }
        }

        /// <summary>
        /// Adds the specified gesture for recognition.
        /// </summary>
        /// <param name="type">The predefined <see cref="GestureType" />.</param>
        public void AddGesture(GestureType type)
        {
            // Check whether the gesure is already added.
            if (_gestures.Where(g => g.GestureType == type).Count() > 0) return;

            IGestureSegment[] segments = null;

            switch (type)
            {
               case GestureType.SwipeRight:
                    segments = new IGestureSegment[] {
                        new SwipeRightSegment1(),
                        new SwipeRightSegment2(),
                        new SwipeRightSegment3()
                    };     
                    break;
                default:
                    break;
            }

            if (segments != null)
            {
                Gesture gesture = new Gesture(type, segments);
                gesture.GestureRecognized += OnGestureRecognized;

                _gestures.Add(gesture);
            }
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Handles the GestureRecognized event of the g control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="GestureEventArgs"/> instance containing the event data.</param>
        private void OnGestureRecognized(object sender, GestureEventArgs e)
        {
            if (GestureRecognized != null)
            {
                GestureRecognized(this, e);
            }

            foreach (Gesture gesture in _gestures)
            {
                gesture.Reset();
            }
        }

        #endregion
    }
}