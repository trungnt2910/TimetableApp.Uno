using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml.Controls;

namespace TimetableApp.UI
{
    public class ToolStripItemCollection : IList<ToolStripItem>, IEnumerable<ToolStripItem>, ICollection<ToolStripItem>
    {
        private IList<MenuFlyoutItemBase> items_;
        private List<ToolStripItem> trueItems;

        public int Count => trueItems.Count;

        public bool IsReadOnly => false;

        public virtual ToolStripItem this[int index] 
        { 
            get => trueItems[index];
            set
            {
                items_[index] = value.ToItem();
                trueItems[index] = value;
            }
        }
        public void Add(ToolStripItem value)
        {
            trueItems.Add(value);
            items_.Add(value.ToItem());
        }
        public void AddRange(ToolStripItem[] toolStripItems)
        {
            foreach (var item in toolStripItems)
            {
                items_.Add(item.ToItem());
            }
            trueItems.AddRange(toolStripItems);
        }
        public void AddRange(ToolStripItemCollection toolStripItems)
        {
            foreach (var item in toolStripItems.trueItems)
            {
                items_.Add(item.ToItem());
            }
            trueItems.AddRange(toolStripItems.trueItems);
        }
        public virtual void Clear()
        {
            items_.Clear();
            trueItems.Clear();
        }
        public int IndexOf(ToolStripItem value)
        {
            return trueItems.IndexOf(value);
        }
        public bool Contains(ToolStripItem value)
        {
            return trueItems.Contains(value);
        }
        public void Insert(int index, ToolStripItem value)
        {
            trueItems.Insert(index, value);
            items_.Insert(index, value.ToItem());
        }
        public void RemoveAt(int index)
        {
            trueItems.RemoveAt(index);
            items_.RemoveAt(index);
        }

        IEnumerator<ToolStripItem> IEnumerable<ToolStripItem>.GetEnumerator() => trueItems.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => (trueItems as IEnumerable).GetEnumerator();

        public void CopyTo(ToolStripItem[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(ToolStripItem item)
        {
            var index = trueItems.IndexOf(item);
            if (index != -1)
            {
                trueItems.RemoveAt(index);
                items_.RemoveAt(index);
                return true;
            }
            return false;
        }

        internal ToolStripItemCollection(IList<MenuFlyoutItemBase> children)
        {
            items_ = children;
            trueItems = new List<ToolStripItem>();
        }
    }
}
