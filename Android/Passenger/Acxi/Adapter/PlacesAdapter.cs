using System;
using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Acxi.DataModels;

namespace Acxi.Adapter
{
    class PlacesAdapter : RecyclerView.Adapter
    {
        public event EventHandler<PlacesAdapterClickEventArgs> ItemClick;
        public event EventHandler<PlacesAdapterClickEventArgs> ItemLongClick;
        public event EventHandler<PlacesAdapterClickEventArgs> ItemRemove;
        List<SavedPlaceItem> items;
        
        public PlacesAdapter(List<SavedPlaceItem> data)
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
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.saveplace_item, parent, false);
            var vh = new PlacesAdapterViewHolder(itemView, OnClick, OnLongClick, onRemove);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as PlacesAdapterViewHolder;
            holder.txtTitle.Text = item.title;
            holder.txtAddress.Text = item.address;
            if(item.title == "Home")
            {
                holder.imgDelete.Visibility = ViewStates.Invisible;
                holder.imgIcon.SetImageResource(Resource.Mipmap.ic_home_black_48dp);
            }
            else if (item.title == "Work")
            {
                holder.imgDelete.Visibility = ViewStates.Invisible;
                holder.imgIcon.SetImageResource(Resource.Mipmap.ic_work_black_48dp);
            }
            
        }

        public override int ItemCount => items.Count;

        void OnClick(PlacesAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(PlacesAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
        void onRemove(PlacesAdapterClickEventArgs args) => ItemRemove?.Invoke(this, args);
    }

    public class PlacesAdapterViewHolder : RecyclerView.ViewHolder
    {
        //public TextView TextView { get; set; }
        public TextView txtTitle { get; set; }
        public TextView txtAddress { get; set; }
        public ImageView imgIcon { get; set; }
        public ImageView imgDelete { get; set; }

        public PlacesAdapterViewHolder(View itemView, Action<PlacesAdapterClickEventArgs> clickListener,
                            Action<PlacesAdapterClickEventArgs> longClickListener, Action<PlacesAdapterClickEventArgs> itemRemoveClickListener) : base(itemView)
        {
            //TextView = v;
            txtTitle = (TextView)itemView.FindViewById(Resource.Id.txttitle_placeitem);
            txtAddress = (TextView)itemView.FindViewById(Resource.Id.txtaddress_placeitem);
            imgIcon = (ImageView)itemView.FindViewById(Resource.Id.imgicon_placeitem);
            imgDelete = (ImageView)itemView.FindViewById(Resource.Id.imgdelete_placeitem);

            itemView.Click += (sender, e) => clickListener(new PlacesAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new PlacesAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            imgDelete.Click += (sender, e) => itemRemoveClickListener(new PlacesAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class PlacesAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}