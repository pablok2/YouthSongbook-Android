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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="items"></param>
        /// <param name="highContrast"></param>
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

        /// <summary>
        /// Selector object as an array
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public override string this[int position]
        {
            get { return items[position]; }
        }

        /// <summary>
        /// Count
        /// </summary>
        public override int Count
        {
            get { return items.Length; }
        }

        /// <summary>
        /// Returns the item id
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public override long GetItemId(int position)
        {
            return position;
        }

        /// <summary>
        /// Gets the table item
        /// </summary>
        /// <param name="position"></param>
        /// <param name="convertView"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Position for section
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public int GetPositionForSection(int section)
        {
            return alphaIndex[sections[section]];
        }

        /// <summary>
        /// Section for position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Java objects
        /// </summary>
        /// <returns></returns>
        Object[] ISectionIndexer.GetSections()
        {
            return sectionsObjects;
        }
    }
}