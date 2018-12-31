using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using AcxiDriver.DataModels;
using Acxi.Helpers;
using Android.Graphics;

namespace AcxiDriver.Adapter
{
    class TicketAdapter : RecyclerView.Adapter
    {
        public event EventHandler<TicketAdapterClickEventArgs> ItemClick;
        public event EventHandler<TicketAdapterClickEventArgs> ItemLongClick;
       List<TicketDetailsFull> items;

        public TicketAdapter(List<TicketDetailsFull> data)
        {
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
           // View itemView = null;
            //var id = Resource.Layout.__YOUR_ITEM_HERE;
            var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ticket_item, parent, false);
            var vh = new TicketAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];
            HelperFunctions helpers = new HelperFunctions();
            var holder = viewHolder as TicketAdapterViewHolder;
            holder.txttitle.Text = item.title;
            holder.txtdate.Text = helpers.ConvertToDate(item.created_at);
            holder.txtbody.Text = item.message;
            holder.txtcategory.Text = item.category;
            if(item.status == "active")
            {
                holder.txtstatus.SetTextColor(Color.Rgb(6, 144, 193));
                holder.txtstatus.Text = "Active";
            }
            else if (item.status == "solved")
            {
                holder.txtstatus.SetTextColor(Color.Rgb(70, 188, 82));
                holder.txtstatus.Text = "Solved";
            }
            else if (item.status == "closed")
            {
                holder.txtstatus.SetTextColor(Color.Rgb(128, 128, 128));
                holder.txtstatus.Text = "Closed";
            }
          
        }

        public override int ItemCount => items.Count;

        void OnClick(TicketAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(TicketAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class TicketAdapterViewHolder : RecyclerView.ViewHolder
    {
        //public TextView TextView { get; set; }
        public TextView txttitle { get; set; }
        public TextView txtdate { get; set; }
        public TextView txtbody { get; set; }
        public TextView txtcategory { get; set; }

        public TextView txtstatus { get; set; }

        public TicketAdapterViewHolder(View itemView, Action<TicketAdapterClickEventArgs> clickListener,
                            Action<TicketAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            //TextView = v;
            txttitle = (TextView)itemView.FindViewById(Resource.Id.txttitle_ticketitem);
            txtdate = (TextView)itemView.FindViewById(Resource.Id.txtdate_ticketitem);
            txtbody = (TextView)itemView.FindViewById(Resource.Id.txtbody_ticketitem);
            txtcategory = (TextView)itemView.FindViewById(Resource.Id.txtcategory_ticketitem);
            txtstatus = (TextView)itemView.FindViewById(Resource.Id.txtstatus_ticketitem);
            itemView.Click += (sender, e) => clickListener(new TicketAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new TicketAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class TicketAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}