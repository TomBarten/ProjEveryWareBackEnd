// <copyright file="DataProviderException.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Common.Exception
{
    /// <summary>
    /// Represents an error that occurs when communicating with
    /// an external data provider.
    /// </summary>
    /// <remarks>
    /// The database of the backend should not be viewed as
    /// an external data provider.
    /// </remarks>
    public class DataProviderException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataProviderException"/> class.
        /// </summary>
        public DataProviderException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataProviderException"/> class.
        /// </summary>
        /// <param name="message"> The message that describes the error.</param>
        public DataProviderException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataProviderException"/> class.
        /// </summary>
        /// <param name="message"> The message that describes the error.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.
        /// </param>
        public DataProviderException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
