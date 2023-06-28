using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scanner_MAUI.Model;

namespace Scanner_MAUI.Helpers
{
    public class ObservableList: List<Network>, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public new void Add(Network item)
        {
            base.Add(item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        //public new void Remove(Network item)
        //{
        //    if (base.Remove(item))
        //    {
        //        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
        //    }
        //}

        // Implement other methods like Clear, Insert, etc., following the same pattern.
    }
}
