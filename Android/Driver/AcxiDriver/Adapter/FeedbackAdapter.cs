using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using AcxiDriver.DataModels;
using Acxi.Helpers;
using Android.Graphics;
using Android.Views.Animations;
using Android.Content;

namespace AcxiDriver.Adapter
{
    class FeedbackAdapter : RecyclerView.Adapter
    {
        public event EventHandler<FeedbackAdapterClickEventArgs> ItemClick;
        public event EventHandler<FeedbackAdapterClickEventArgs> ItemLongClick;
        List<Rating_Status> items;
        Context context;
        int mcurrentposition = -1;
        HelperFunctions helper = new HelperFunctions();
        public FeedbackAdapter(List<Rating_Status> data, Context mcontext)
        {
            //foreach(Rating_Status feed in data)
            //{
            //    if(string.IsNullOrEmpty(feed.created_at) || string.IsNullOrEmpty(feed.created_at))
            //    {
            //        data.Remove(feed);
            //    }
            //}
            items = data;
            context = mcontext;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
          
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.feedback_item, parent, false);
            var vh = new FeedbackAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];
            
            var holder = viewHolder as FeedbackAdapterViewHolder;
            holder.txtfeedback.Text = item.feedback;
            holder.txtname_feedback.Text = item.user_name;
            if (double.Parse(item.created_at) > 1)
            {
                holder.txtdate_feedback.Text = helper.ConvertToDate(double.Parse(item.created_at));
            }
            else
            {
                holder.txtdate_feedback.Text = "Not available";
            }
            holder.CalculateStar(item.rating);

            //holder.TextView.Text = items[position];

            if (position > mcurrentposition)
            {
                SetAnimation(holder.ItemView);
                mcurrentposition = position;
            }
        }

        public void SetAnimation(View view)
        {
            Animation anim = AnimationUtils.LoadAnimation(context, Resource.Animation.slide_up);
            view.StartAnimation(anim);
        }

        public override int ItemCount => items.Count;

        void OnClick(FeedbackAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(FeedbackAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class FeedbackAdapterViewHolder : RecyclerView.ViewHolder
    {
        //public TextView TextView { get; set; }

        public TextView txtdate_feedback;
        public TextView txtname_feedback;
        public TextView txtfeedback;
        public ImageView star1;
        public ImageView star2;
        public ImageView star3;
        public ImageView star4;
        public ImageView star5;

        public FeedbackAdapterViewHolder(View itemView, Action<FeedbackAdapterClickEventArgs> clickListener,
                            Action<FeedbackAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            //TextView = v;
            txtdate_feedback = (TextView)itemView.FindViewById(Resource.Id.txtdate_feedback);
            txtfeedback = (TextView)itemView.FindViewById(Resource.Id.txtfeed_feedback);
            txtname_feedback = (TextView)itemView.FindViewById(Resource.Id.txtname_feedback);
            star1 = (ImageView)itemView.FindViewById(Resource.Id.star1_feedback);
            star2 = (ImageView)itemView.FindViewById(Resource.Id.star2_feedback);
            star3 = (ImageView)itemView.FindViewById(Resource.Id.star3_feedback);
            star4 = (ImageView)itemView.FindViewById(Resource.Id.star4_feedback);
            star5 = (ImageView)itemView.FindViewById(Resource.Id.star5_feedback);

            itemView.Click += (sender, e) => clickListener(new FeedbackAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new FeedbackAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }

        public void CalculateStar(double ride_rating)
        {
            if (ride_rating == 1)
            {
                //REPLACE BORDERED IMAGE WITH FILLED IMAGE RESOURCE
                star1.SetImageResource(Resource.Mipmap.ic_star_black_48dp);

                //TINT THE IMAGE WITH ORANGE COLOR
                star1.SetColorFilter(Color.Rgb(238, 134, 31));
                star2.SetColorFilter(Color.Rgb(149, 152, 154));
                star3.SetColorFilter(Color.Rgb(149, 152, 154));
                star4.SetColorFilter(Color.Rgb(149, 152, 154));
                star5.SetColorFilter(Color.Rgb(149, 152, 154));
            }
            else if (ride_rating == 2)
            {
                star1.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
                star2.SetImageResource(Resource.Mipmap.ic_star_black_48dp);

                star1.SetColorFilter(Color.Rgb(238, 134, 31));
                star2.SetColorFilter(Color.Rgb(238, 134, 31));
                star3.SetColorFilter(Color.Rgb(149, 152, 154));
                star4.SetColorFilter(Color.Rgb(149, 152, 154));
                star5.SetColorFilter(Color.Rgb(149, 152, 154));
            }
            else if (ride_rating == 3)
            {
                star1.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
                star2.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
                star3.SetImageResource(Resource.Mipmap.ic_star_black_48dp);

                star1.SetColorFilter(Color.Rgb(238, 134, 31));
                star2.SetColorFilter(Color.Rgb(238, 134, 31));
                star3.SetColorFilter(Color.Rgb(238, 134, 31));
                star4.SetColorFilter(Color.Rgb(149, 152, 154));
                star5.SetColorFilter(Color.Rgb(149, 152, 154));
            }
            else if (ride_rating == 4)
            {
                star1.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
                star2.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
                star3.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
                star4.SetImageResource(Resource.Mipmap.ic_star_black_48dp);

                star1.SetColorFilter(Color.Rgb(238, 134, 31));
                star2.SetColorFilter(Color.Rgb(238, 134, 31));
                star3.SetColorFilter(Color.Rgb(238, 134, 31));
                star4.SetColorFilter(Color.Rgb(238, 134, 31));
                star5.SetColorFilter(Color.Rgb(149, 152, 154));
            }
            else if (ride_rating == 5)
            {
                star1.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
                star2.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
                star3.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
                star4.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
                star5.SetImageResource(Resource.Mipmap.ic_star_black_48dp);

                star1.SetColorFilter(Color.Rgb(238, 134, 31));
                star2.SetColorFilter(Color.Rgb(238, 134, 31));
                star3.SetColorFilter(Color.Rgb(238, 134, 31));
                star4.SetColorFilter(Color.Rgb(238, 134, 31));
                star4.SetColorFilter(Color.Rgb(238, 134, 31));

            }
        }

    }

    public class FeedbackAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}