using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using Acxi.DataModels;

namespace Acxi.Adapter
{
    class WhereToAdapter : RecyclerView.Adapter
    {
        public event EventHandler<WhereToAdapterClickEventArgs> ItemClick;
        public event EventHandler<WhereToAdapterClickEventArgs> ItemLongClick;
        List<SavedPlaceItem> items;

        public WhereToAdapter(List<SavedPlaceItem> data)
        {
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            //Setup your layout here
            View itemView = null;
            //var id = Resource.Layout.__YOUR_ITEM_HERE;
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.saveplace_item2, parent, false);
            var vh = new WhereToAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as WhereToAdapterViewHolder;
            holder.txtTitle.Text = item.title;
            holder.txtAddress.Text = item.address;
            if (item.title == "Home")
            {
                holder.imgIcon.SetImageResource(Resource.Mipmap.ic_home_black_48dp);
            }
            else if (item.title == "Work")
            {
                holder.imgIcon.SetImageResource(Resource.Mipmap.ic_work_black_48dp);
            }
        }

        public override int ItemCount => items.Count;

        void OnClick(WhereToAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(WhereToAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class WhereToAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView txtTitle { get; set; }
        public TextView txtAddress { get; set; }
        public ImageView imgIcon { get; set; }

        public WhereToAdapterViewHolder(View itemView, Action<WhereToAdapterClickEventArgs> clickListener,
                            Action<WhereToAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            txtTitle = (TextView)itemView.FindViewById(Resource.Id.txttitle_wheretoitem);
            txtAddress = (TextView)itemView.FindViewById(Resource.Id.txtaddress_wheretoitem);
            imgIcon = (ImageView)itemView.FindViewById(Resource.Id.imgicon_wheretoitem);

            itemView.Click += (sender, e) => clickListener(new WhereToAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new WhereToAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class WhereToAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}