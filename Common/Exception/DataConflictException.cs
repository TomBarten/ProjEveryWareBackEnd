// <copyright file="DataConflictException.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Common.Exception
{
    /// <summary>
    /// Represents an error that is caused by a data conflict
    /// (for example, a concurrent update error).
    /// </summary>
    /// <remarks>
    /// The database of the backend should not be viewed as
    /// an external data provider.
    /// </remarks>
    public class DataConflictException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataConflictException"/> class.
        /// </summary>
        public DataConflictException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataConflictException"/> class.
        /// </summary>
        /// <param name="message"> The message that describes the error.</param>
        public DataConflictException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataConflictException"/> class.
        /// </summary>
        /// <param name="message"> The message that describes the error.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.
        /// </param>
        public DataConflictException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
