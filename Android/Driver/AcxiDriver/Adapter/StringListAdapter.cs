using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;

namespace AcxiDriver.Adapter
{
	public class StringListAdapter : BaseAdapter<String>
	{
		private Context context;
		private List<string> mItems;
		public StringListAdapter(Context _context, List<string>itemsm)
		{
			context = _context;
			mItems = itemsm;
		}

		public override string this[int position]
		{
			get
			{
				return mItems[position];
			}
		}

		public override int Count
		{
			get
			{
				return mItems.Count;
			}
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View row = LayoutInflater.From(context).Inflate(Resource.Layout.stringlist_row, parent, false);
			TextView txtfac = row.FindViewById<TextView>(Resource.Id.txtlistrow);
			txtfac.Text = mItems[position];

			//Typeface type1 = Typeface.CreateFromAsset(Application.Context.Assets, "Fonts/AvenirNext-Regular.ttf");
			//txtfac.SetTypeface(type1, TypefaceStyle.Normal);

			return row;
		}
	}
}
