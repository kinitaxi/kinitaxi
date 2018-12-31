using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using Acxi.DataModels;
using System.Globalization;
using Android.Graphics;
using Android.Views.Animations;
using Android.Content;

namespace Acxi.Adapter
{
    class TripAdapter : RecyclerView.Adapter
    {
        public event EventHandler<TripAdapterClickEventArgs> ItemClick;
        public event EventHandler<TripAdapterClickEventArgs> ItemLongClick;
        List<rideHistory> items;

        Helpers.HelperFunctions helper = new Helpers.HelperFunctions();
        Context mcontext;
        int mcurrentposition = -1;
        public TripAdapter(List<rideHistory> data, Context context)
        {
            items = data;
            mcontext = context;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            //var id = Resource.Layout.__YOUR_ITEM_HERE;
            //itemView = LayoutInflater.From(parent.Context).
            //       Inflate(id, parent, false);
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.history_item, parent, false);
            var vh = new TripAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as TripAdapterViewHolder;
            //holder.TextView.Text = items[position];
            holder.triplocation.Text = items[position].pickup_address;
            holder.tripdestination.Text = items[position].destination_address;
            holder.tripfare.Text = helper.CurrencyConvert(double.Parse(items[position].total_fare));
           // holder.tripstatus.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(items[position].status);
            holder.tripdate.Text = (double.Parse(items[position].created_at) > 1) ? helper.ConvertToDate(double.Parse(items[position].created_at)) : "Not Available";

            if (items[position].status.Contains("cancelled"))
            {
                holder.tripstatus.SetTextColor(Color.Rgb(193, 7, 7));
                holder.tripstatus.Text = "Cancelled";
            }
            else if (items[position].status == "ended")
            {
                holder.tripstatus.Text = "Completed";
                holder.tripstatus.SetTextColor(Color.Rgb(9, 155, 11));
            }
            else
            {
                holder.tripstatus.Text = "Undefined";
            }

            if(position > mcurrentposition)
            {
                SetAnimation(holder.ItemView);
                mcurrentposition = position;
            }
        }

        public void SetAnimation(View view)
        {
            Animation anim = AnimationUtils.LoadAnimation(mcontext, Resource.Animation.slide_up);
            view.StartAnimation(anim);
        }
        public override int ItemCount => items.Count;

        void OnClick(TripAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(TripAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class TripAdapterViewHolder : RecyclerView.ViewHolder
    {
        //public TextView TextView { get; set; }
        public TextView triplocation;
        public TextView tripdestination;
        public TextView tripstatus;
        public TextView tripfare;
        public TextView tripdate;

        public TripAdapterViewHolder(View itemView, Action<TripAdapterClickEventArgs> clickListener,
                            Action<TripAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            //TextView = v;
            triplocation = (TextView)itemView.FindViewById(Resource.Id.txtlocation_historyitem);
            tripdestination = (TextView)itemView.FindViewById(Resource.Id.txtdestination_historyitem);
            tripstatus = (TextView)itemView.FindViewById(Resource.Id.txtstatus_historyitem);
            tripfare = (TextView)itemView.FindViewById(Resource.Id.txtfare_historyitem);
            tripdate = (TextView)itemView.FindViewById(Resource.Id.txtdate_historyitem);

            itemView.Click += (sender, e) => clickListener(new TripAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new TripAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class TripAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}