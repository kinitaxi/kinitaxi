using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Acxi.DialogueFragment
{
    public class ReportProblemEventArgs : EventArgs
    {
        public string type { get; set; }
    }
    public class ReportAProblem_Dialogue : Android.Support.V4.App.DialogFragment
    {
        public event EventHandler<ReportProblemEventArgs> OnReported;
        LinearLayout laylostitem;
        LinearLayout layothers;
        LinearLayout layroute;
        LinearLayout layovercharged;
        LinearLayout laypromo;
        Button btnclose;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            View view = inflater.Inflate(Resource.Layout.report_problem, container, false);

            btnclose = (Button)view.FindViewById(Resource.Id.btnclose_report);
            laylostitem = (LinearLayout)view.FindViewById(Resource.Id.laylostitem_report);
            layothers = (LinearLayout)view.FindViewById(Resource.Id.layothers_report);
            layovercharged = (LinearLayout)view.FindViewById(Resource.Id.layovercharged_report);
            laypromo = (LinearLayout)view.FindViewById(Resource.Id.laypromo_report);
            layroute = (LinearLayout)view.FindViewById(Resource.Id.layroute_report);

            btnclose.Click += Btnclose_Click;
            laylostitem.Click += Laylostitem_Click;
            layothers.Click += Layothers_Click;
            laypromo.Click += Laypromo_Click;
            layroute.Click += Layroute_Click;
            layovercharged.Click += Layovercharged_Click;
            return view;
        }

        private void Layovercharged_Click(object sender, EventArgs e)
        {
            OnReported.Invoke(this, new ReportProblemEventArgs { type = "overcharge" });
            this.Dismiss();
        }

        private void Layroute_Click(object sender, EventArgs e)
        {
            OnReported.Invoke(this, new ReportProblemEventArgs { type = "longroute" });
            this.Dismiss();
        }

        private void Laypromo_Click(object sender, EventArgs e)
        {
            OnReported.Invoke(this, new ReportProblemEventArgs { type = "promo" });
            this.Dismiss();
        }

        private void Layothers_Click(object sender, EventArgs e)
        {
            OnReported.Invoke(this, new ReportProblemEventArgs { type = "others" });
            this.Dismiss();
        }

        private void Laylostitem_Click(object sender, EventArgs e)
        {
            OnReported.Invoke(this, new ReportProblemEventArgs { type = "lostitem" });
            this.Dismiss();
        }

        private void Btnclose_Click(object sender, EventArgs e)
        {
            this.Dismiss();
        }
    }
}