#region

using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using HIHSongbook;
using Object = Java.Lang.Object;
using Android.Graphics;

#endregion

namespace YouthSongbook
{
    /// <summary>
    ///     Custom SongListClass to add more functionality
    /// </summary>
    internal class SongListAdapter : BaseAdapter<string>, ISectionIndexer
    {
        private Activity context;
        private string[] items;
        private Dictionary<string, int> alphaIndex;
        private string[] sections;
        private Java.Lang.Object[] sectionsObjects;
        private bool highContrastFlag;

        public SongListAdapter(Activity context, string[] items, bool highContrast)
        {
            this.context = context;
            this.items = items;
            highContrastFlag = highContrast;

            alphaIndex = new Dictionary<string, int>();
            for (int i = 0; i < items.Length; i++)
            {
                var key = items[i][0].ToString();
                if (!alphaIndex.ContainsKey(key))
                    alphaIndex.Add(key, i);
            }

            sections = new string[alphaIndex.Keys.Count];
            alphaIndex.Keys.CopyTo(sections, 0); // convert letters list to string[]

            // Interface requires a Java.Lang.Object[], so we create one here
            sectionsObjects = new Java.Lang.Object[sections.Length];
            for (int i = 0; i < sections.Length; i++)
            {
                sectionsObjects[i] = new Java.Lang.String(sections[i]);
            }
        }

        public override string this[int position]
        {
            get { return items[position]; }
        }

        public override int Count
        {
            get { return items.Length; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView; // re-use an existing view, if one is available
            if (view == null) // otherwise create a new one
            {
                view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
            }

            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = items[position];
            view.FindViewById<TextView>(Android.Resource.Id.Text1).SetTextColor(highContrastFlag ? Color.White : Color.Black);
            view.FindViewById<TextView>(Android.Resource.Id.Text1).SetBackgroundColor(highContrastFlag ? Color.Black : Color.White);

            return view;
        }

        public int GetPositionForSection(int section)
        {
            return alphaIndex[sections[section]];
        }

        public int GetSectionForPosition(int position)
        {
            int prevSection = 0;
            for (int i = 0; i < sections.Length; i++)
            {
                if (GetPositionForSection(i) > position)
                {
                    break;
                }
                prevSection = i;
            }

            return prevSection;
        }

        Object[] ISectionIndexer.GetSections()
        {
            return sectionsObjects;
        }
    }
}