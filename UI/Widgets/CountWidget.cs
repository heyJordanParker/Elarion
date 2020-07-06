using Elarion.Workflows.Arrays;

namespace Elarion.UI.Widgets {
    public abstract class CountWidget<TSavedList, TSavedListItem> : BasicWidget<TSavedList, SavedList<TSavedListItem>>
        where TSavedList : SavedList<TSavedListItem> {
        protected override string StringifiedVariable => savedVariable.Count.ToString();
    }
}