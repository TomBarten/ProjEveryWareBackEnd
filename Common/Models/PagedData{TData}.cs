// <copyright file="PagedData{TData}.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Common.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents paged data.
    /// </summary>
    /// <typeparam name="TData">The type of data.</typeparam>
    public class PagedData<TData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PagedData{TData}" /> class.
        /// </summary>
        /// <param name="pageNumber">The value for <see cref="PageNumber" />.</param>
        /// <param name="totalPageCount">The value for <see cref="TotalPageCount" />.</param>
        /// <param name="dataPerPage">The value for <see cref="DataPerPage" />.</param>
        /// <param name="data">The value for <see cref="Data" />.</param>
        public PagedData(
            int pageNumber,
            int totalPageCount,
            int dataPerPage,
            IReadOnlyList<TData> data)
        {
            this.PageNumber = pageNumber;
            this.TotalPageCount = totalPageCount;
            this.DataPerPage = dataPerPage;
            this.Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        /// <summary>
        /// Gets the number of this page (starting from 0).
        /// </summary>
        public int PageNumber { get; }

        /// <summary>
        /// Gets the total amount of pages.
        /// </summary>
        public int TotalPageCount { get; }

        /// <summary>
        /// Gets the amount of data per page.
        /// Please note, there may be less items in this page, when
        /// this is the final page.
        /// </summary>
        public int DataPerPage { get; }

        /// <summary>
        /// Gets the data in the page.
        /// </summary>
        public IReadOnlyList<TData> Data { get; }
    }
}
