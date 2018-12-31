using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;

namespace Acxi.Adapter
{
    class BookedRideAdapter : RecyclerView.Adapter
    {
        public event EventHandler<BookedRideAdapterClickEventArgs> ItemClick;
        public event EventHandler<BookedRideAdapterClickEventArgs> ItemLongClick;
        string[] items;

        public BookedRideAdapter(string[] data)
        {
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            //var id = Resource.Layout.__YOUR_ITEM_HERE;
            //itemView = LayoutInflater.From(parent.Context).
            //       Inflate(id, parent, false);

            var vh = new BookedRideAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as BookedRideAdapterViewHolder;
            //holder.TextView.Text = items[position];
        }

        public override int ItemCount => items.Length;

        void OnClick(BookedRideAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(BookedRideAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class BookedRideAdapterViewHolder : RecyclerView.ViewHolder
    {
        //public TextView TextView { get; set; }


        public BookedRideAdapterViewHolder(View itemView, Action<BookedRideAdapterClickEventArgs> clickListener,
                            Action<BookedRideAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            //TextView = v;
            itemView.Click += (sender, e) => clickListener(new BookedRideAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new BookedRideAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class BookedRideAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}