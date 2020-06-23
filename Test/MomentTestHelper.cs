// <copyright file="MomentTestHelper.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test
{
    using System;
    using Fvect.Backend.Common.Abstraction;
    using Moq;

    /// <summary>
    /// Contains test helpers for the <see cref="IMoment"/> type.
    /// </summary>
    public static class MomentTestHelper
    {
        /// <summary>
        /// Creates a <see cref="Mock{T}"/> of type <see cref="IMoment"/> with the
        /// get operation of the <see cref="IMoment.UtcNow"/> property set to
        /// the specified value and marked as verifiable.
        /// </summary>
        /// <param name="nowValue">
        /// The now value, when <c>null</c>,
        /// will fallback to <see cref="DateTimeOffset.UtcNow"/>.
        /// </param>
        /// <returns>The created mock.</returns>
        public static Mock<IMoment> CreateMomentMock(DateTimeOffset? nowValue = default)
        {
            var mock = new Mock<IMoment>();
            mock.SetupGet(m => m.UtcNow).Returns(nowValue ?? DateTimeOffset.UtcNow).Verifiable();

            return mock;
        }
    }
}
