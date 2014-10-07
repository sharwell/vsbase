namespace Tvl.VisualStudio.Text
{
    using System;

    public interface IBackgroundParser
    {
        event EventHandler<ParseResultEventArgs> ParseComplete;

        void RequestParse(bool forceReparse);
    }
}
