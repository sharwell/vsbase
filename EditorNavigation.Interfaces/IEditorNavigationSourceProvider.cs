namespace Tvl.VisualStudio.Text.EditorNavigation.Interfaces
{
    using Microsoft.VisualStudio.Text;

    public interface IEditorNavigationSourceProvider
    {
        IEditorNavigationSource TryCreateEditorNavigationSource(ITextBuffer textBuffer);
    }
}
