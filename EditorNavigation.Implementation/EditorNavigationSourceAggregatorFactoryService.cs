namespace Tvl.VisualStudio.Text.EditorNavigation.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Text.EditorNavigation.Interfaces;

    [Export(typeof(IEditorNavigationSourceAggregatorFactoryService))]
    internal class EditorNavigationSourceAggregatorFactoryService : IEditorNavigationSourceAggregatorFactoryService
    {
        [ImportMany]
        private IEnumerable<Lazy<IEditorNavigationSourceProvider, IEditorNavigationSourceMetadata>> NavigationSourceProviders
        {
            get;
            set;
        }

        public IEditorNavigationSourceAggregator CreateEditorNavigationSourceAggregator(ITextBuffer textBuffer)
        {
            var providers = NavigationSourceProviders.Where(provider => provider.Metadata.ContentTypes.Any(contentType => textBuffer.ContentType.IsOfType(contentType)));

            var sources =
                providers
                .Select(provider => provider.Value.TryCreateEditorNavigationSource(textBuffer))
                .Where(source => source != null)
                .ToArray();

            return new EditorNavigationSourceAggregator(sources);
        }
    }
}
