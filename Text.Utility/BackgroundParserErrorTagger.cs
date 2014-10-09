﻿namespace Tvl.VisualStudio.Text
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Adornments;
    using Microsoft.VisualStudio.Text.Tagging;
    using ErrorHandler = Microsoft.VisualStudio.ErrorHandler;

    public class BackgroundParserErrorTagger : ITagger<IErrorTag>
    {
        /// <summary>
        /// This is the backing field for the <see cref="TextBuffer"/> property.
        /// </summary>
        private readonly ITextBuffer _textBuffer;
        /// <summary>
        /// This is the backing field for the <see cref="BackgroundParser"/> property.
        /// </summary>
        private readonly IBackgroundParser _backgroundParser;

        private ITagSpan<IErrorTag>[] _tags;

        /// <inheritdoc/>
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public BackgroundParserErrorTagger(ITextBuffer textBuffer, IBackgroundParser backgroundParser)
        {
            Contract.Requires<ArgumentNullException>(textBuffer != null, "textBuffer");
            Contract.Requires<ArgumentNullException>(backgroundParser != null, "backgroundParser");

            this._textBuffer = textBuffer;
            this._backgroundParser = backgroundParser;
            this._backgroundParser.ParseComplete += HandleBackgroundParserParseComplete;
            this._backgroundParser.RequestParse(false);
        }

        /// <summary>
        /// Gets the text buffer which the background parser reports errors for.
        /// </summary>
        public ITextBuffer TextBuffer
        {
            get
            {
                Contract.Ensures(Contract.Result<ITextBuffer>() != null);

                return _textBuffer;
            }
        }

        /// <summary>
        /// Gets the background parser which provides the error information used by this tagger.
        /// </summary>
        public IBackgroundParser BackgroundParser
        {
            get
            {
                Contract.Ensures(Contract.Result<IBackgroundParser>() != null);

                return _backgroundParser;
            }
        }

        public IEnumerable<ITagSpan<IErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            Contract.Requires<ArgumentNullException>(spans != null, "spans");
            Contract.Ensures(Contract.Result<IEnumerable<ITagSpan<IErrorTag>>>() != null);

            return _tags;
        }

        /// <inheritdoc/>
        IEnumerable<ITagSpan<IErrorTag>> ITagger<IErrorTag>.GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return GetTags(spans);
        }

        private void HandleBackgroundParserParseComplete(object sender, ParseResultEventArgs e)
        {
            var snapshot = TextBuffer.CurrentSnapshot;
            List<TagSpan<IErrorTag>> tags = new List<TagSpan<IErrorTag>>();
            foreach (var error in e.Errors)
            {
                try
                {
                    tags.Add(new TagSpan<IErrorTag>(new SnapshotSpan(e.Snapshot, error.Span).TranslateTo(snapshot, SpanTrackingMode.EdgeExclusive), new ErrorTag(PredefinedErrorTypeNames.SyntaxError, error.Message)));
                }
                catch (Exception ex)
                {
                    if (ErrorHandler.IsCriticalException(ex))
                        throw;
                }
            }

            _tags = tags.ToArray();
            OnTagsChanged(new SnapshotSpanEventArgs(new SnapshotSpan(snapshot, 0, snapshot.Length)));
        }

        private void OnTagsChanged(SnapshotSpanEventArgs e)
        {
            Contract.Requires(e != null);

            var t = TagsChanged;
            if (t != null)
                t(this, e);
        }
    }
}
