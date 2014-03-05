﻿namespace Tvl.VisualStudio.Text
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Tvl.VisualStudio.Shell;

    using IOleCommandTarget = Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget;

    /// <summary>
    /// This class extends <see cref="CommandFilter"/> to support command filters for
    /// text view instances (<see cref="IVsTextView"/>).
    /// </summary>
    /// <threadsafety/>
    [ComVisible(true)]
    public abstract class TextViewCommandFilter : CommandFilter
    {
        /// <summary>
        /// This is the backing field for the <see cref="TextViewAdapter"/> property.
        /// </summary>
        private readonly IVsTextView _textViewAdapter;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextViewCommandFilter"/> class
        /// for the specified text view.
        /// </summary>
        /// <param name="textViewAdapter">The text view this command filter should attach to.</param>
        protected TextViewCommandFilter(IVsTextView textViewAdapter)
        {
            Contract.Requires<ArgumentNullException>(textViewAdapter != null, "textViewAdapter");

            _textViewAdapter = textViewAdapter;
        }

        /// <summary>
        /// Gets the <see cref="IVsTextView"/> which this command filter should attach to.
        /// </summary>
        protected IVsTextView TextViewAdapter
        {
            get
            {
                Contract.Ensures(Contract.Result<IVsTextView>() != null);

                return _textViewAdapter;
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This operation is implemented by calling <see cref="IVsTextView.AddCommandFilter"/>.
        /// </remarks>
        protected override IOleCommandTarget Connect()
        {
            IOleCommandTarget next;
            ErrorHandler.ThrowOnFailure(TextViewAdapter.AddCommandFilter(this, out next));
            return next;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This operation is implemented by calling <see cref="IVsTextView.RemoveCommandFilter"/>.
        /// </remarks>
        protected override void Disconnect()
        {
            ErrorHandler.ThrowOnFailure(TextViewAdapter.RemoveCommandFilter(this));
        }
    }
}
