﻿namespace Tvl.VisualStudio.Text.Commenter.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using Microsoft.VisualStudio.Editor;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Operations;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Text.Commenter.Interfaces;

    /// <summary>
    /// This class provides the core implementation of the commenting service in Visual Studio
    /// by importing instances of <see cref="ICommenterProvider"/> and attaching a
    /// <see cref="CommenterFilter"/> to the text view if an <see cref="ICommenter"/> is
    /// available for the underlying text buffer of the text view.
    /// </summary>
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal sealed class AutoCommentService : IVsTextViewCreationListener
    {
        /// <summary>
        /// This is the backing field for the <see cref="EditorAdaptersFactoryService"/> property.
        /// </summary>
        private IVsEditorAdaptersFactoryService _editorAdaptersFactoryService;

        /// <summary>
        /// This is the backing field for the <see cref="EditorOperationsFactoryService"/> property.
        /// </summary>
        private IEditorOperationsFactoryService _editorOperationsFactoryService;

        /// <summary>
        /// This is the backing field for the <see cref="TextUndoHistoryRegistry"/> property.
        /// </summary>
        private ITextUndoHistoryRegistry _textUndoHistoryRegistry;

        /// <summary>
        /// This is the backing field for the <see cref="CommenterProviders"/> property.
        /// </summary>
        private List<Lazy<ICommenterProvider, IContentTypeMetadata>> _commenterProviders;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoCommentService"/> class with the specified components.
        /// </summary>
        /// <param name="editorAdaptersFactoryService">The global <see cref="IVsEditorAdaptersFactoryService"/>
        /// component provided by Visual Studio.</param>
        /// <param name="editorOperationsFactoryService">The global <see cref="IEditorOperationsFactoryService"/>
        /// component provided by Visual Studio.</param>
        /// <param name="textUndoHistoryRegistry">The global <see cref="ITextUndoHistoryRegistry"/> component provided
        /// by Visual Studio.</param>
        /// <param name="commenterProviders">A collection of <see cref="ICommenterProvider"/> components exported by
        /// this and other extensions, along with any content type metadata associated with the component through the
        /// use of <see cref="ContentTypeAttribute"/>.</param>
        [ImportingConstructor]
        public AutoCommentService(
            [Import] IVsEditorAdaptersFactoryService editorAdaptersFactoryService,
            [Import] IEditorOperationsFactoryService editorOperationsFactoryService,
            [Import] ITextUndoHistoryRegistry textUndoHistoryRegistry,
            [ImportMany] IEnumerable<Lazy<ICommenterProvider, IContentTypeMetadata>> commenterProviders)
        {
            _editorAdaptersFactoryService = editorAdaptersFactoryService;
            _editorOperationsFactoryService = editorOperationsFactoryService;
            _textUndoHistoryRegistry = textUndoHistoryRegistry;
            _commenterProviders = commenterProviders.ToList();
        }

        /// <summary>
        /// Gets the global <see cref="IVsEditorAdaptersFactoryService"/> component provided
        /// by Visual Studio.
        /// </summary>
        private IVsEditorAdaptersFactoryService EditorAdaptersFactoryService
        {
            get
            {
                return _editorAdaptersFactoryService;
            }
        }

        /// <summary>
        /// Gets the global <see cref="IEditorOperationsFactoryService"/> component provided
        /// by Visual Studio.
        /// </summary>
        private IEditorOperationsFactoryService EditorOperationsFactoryService
        {
            get
            {
                return _editorOperationsFactoryService;
            }
        }

        /// <summary>
        /// Gets the global <see cref="ITextUndoHistoryRegistry"/> component provided
        /// by Visual Studio.
        /// </summary>
        private ITextUndoHistoryRegistry TextUndoHistoryRegistry
        {
            get
            {
                return _textUndoHistoryRegistry;
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="ICommenterProvider"/> components exported by this and other extensions,
        /// along with any content type metadata associated with the component through the use of
        /// <see cref="ContentTypeAttribute"/>.
        /// </summary>
        private List<Lazy<ICommenterProvider, IContentTypeMetadata>> CommenterProviders
        {
            get
            {
                return _commenterProviders;
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>When a text view is created, this method first checks if the content type of the underlying
        /// <see cref="ITextBuffer"/> matches a content type associated with any of the
        /// <see cref="CommenterProviders"/>. If so, <see cref="ICommenterProvider.TryCreateCommenter"/> is called to
        /// obtain the <see cref="ICommenter"/> to associate with the text buffer for the view. The commenter is then
        /// used to initialize a <see cref="CommenterFilter"/> that provides support for the comment and uncomment
        /// commands for the text view.</para>
        ///
        /// <para>
        /// If any of these operations fails, no changes are applied to the text view.
        /// </para>
        ///
        /// <note type="note">
        /// <para>The current implementation does not support projection buffer scenarios involving multiple content
        /// types. However, the <see cref="ICommenterProvider"/> and <see cref="ICommenter"/> interfaces do not prevent
        /// such a feature from being implemented in a future release.</para>
        /// </note>
        /// </remarks>
        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            ITextView textView = EditorAdaptersFactoryService.GetWpfTextView(textViewAdapter);
            if (textView == null)
                return;

            var provider = CommenterProviders.FirstOrDefault(providerInfo => providerInfo.Metadata.ContentTypes.Any(contentType => textView.TextBuffer.ContentType.IsOfType(contentType)));
            if (provider == null)
                return;

            var commenter = provider.Value.TryCreateCommenter(textView.TextBuffer);
            if (commenter == null)
                return;

            IEditorOperations editorOperations = EditorOperationsFactoryService.GetEditorOperations(textView);

            CommenterFilter filter = new CommenterFilter(textViewAdapter, textView, commenter, editorOperations, TextUndoHistoryRegistry);
            filter.Enabled = true;
            textView.Properties.AddProperty(typeof(CommenterFilter), filter);
        }
    }
}
