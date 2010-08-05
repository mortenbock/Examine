﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Examine;
using LuceneExamine.DataServices;

namespace LuceneExamine.Config
{
    /// <summary>
    /// Extension methods for IndexSet
    /// </summary>
    public static class IndexSetExtensions
    {

        private static readonly object m_Locker = new object();

        /// <summary>
        /// Convert the indexset to indexerdata.
        /// This detects if there are no user/system fields specified and if not, uses the data service to look them 
        /// up and update the in memory IndexSet.
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public static IIndexCriteria ToIndexCriteria(this IndexSet set, IDataService svc)
        {
            if (set.IndexUserFields.Count == 0)
            {
                lock (m_Locker)
                {
                    //we need to add all user fields to the collection if it is empty (this is the default if none are specified)
                    var userFields = svc.ContentService.GetAllUserPropertyNames();
                    foreach (var u in userFields)
                    {
                        set.IndexUserFields.Add(new IndexField() { Name = u });
                    } 
                }
            }

            if (set.IndexAttributeFields.Count == 0)
            {
                lock (m_Locker)
                {
                    //we need to add all system fields to the collection if it is empty (this is the default if none are specified)
                    var sysFields = svc.ContentService.GetAllSystemPropertyNames();
                    foreach (var s in sysFields)
                    {
                        set.IndexAttributeFields.Add(new IndexField() { Name = s });
                    } 
                }
            }

            return new IndexCriteria(
                set.IndexAttributeFields.ToList().Select(x => x.Name).ToArray(),
                set.IndexUserFields.ToList().Select(x => x.Name).ToArray(),
                set.IncludeNodeTypes.ToList().Select(x => x.Name).ToArray(),
                set.ExcludeNodeTypes.ToList().Select(x => x.Name).ToArray(),
                set.IndexParentId);
        }

        /// <summary>
        /// Returns a string array of all fields that are indexed including Umbraco fields
        /// </summary>
        public static IEnumerable<IndexField> CombinedUmbracoFields(this IndexSet set, IDataService svc)
        {
            //if (set.IndexUserFields.Count == 0)
            //{
            //    //we need to add all user fields to the collection if it is empty (this is the default if none are specified)
            //    var userFields = svc.ContentService.GetAllUserPropertyNames();
            //    foreach (var u in userFields)
            //    {
            //        set.IndexUserFields.Add(new IndexField() { Name = u });
            //    }
            //}

            //if (set.IndexAttributeFields.Count == 0)
            //{
            //    //we need to add all system fields to the collection if it is empty (this is the default if none are specified)
            //    var sysFields = svc.ContentService.GetAllSystemPropertyNames();
            //    foreach (var s in sysFields)
            //    {
            //        set.IndexUserFields.Add(new IndexField() { Name = s });
            //    }
            //}

            return set.IndexUserFields.ToList()
                .Concat(set.IndexAttributeFields.ToList());
        }

      
    }
}