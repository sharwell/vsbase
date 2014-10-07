namespace Tvl.VisualStudio.Text.EditorNavigation.Interfaces
{
    using Microsoft.VisualStudio.Text;

    public interface IEditorNavigationSourceAggregatorFactoryService
    {
        IEditorNavigationSourceAggregator CreateEditorNavigationSourceAggregator(ITextBuffer textBuffer);
    }
}
