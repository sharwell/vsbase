namespace Tvl.VisualStudio.Text.EditorNavigation.Implementation
{
    using System.Collections.Generic;

    public interface IEditorNavigationSourceMetadata
    {
        IEnumerable<string> ContentTypes
        {
            get;
        }

        //IEnumerable<string> EditorNavigationType
        //{
        //    get;
        //}
    }
}
