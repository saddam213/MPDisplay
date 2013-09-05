﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageFramework.DataObjects
{
    public class APIList
    {
        public APIListType ListType { get; set; }
        public List<APIListItem> ListItems { get; set; }
        public APIListLayout ListLayout { get; set; }
    }

    public class APIListItem
    {
        public int Index { get; set; }
        public string Label { get; set; }
        public string Label2 { get; set; }
        public string Label3 { get; set; }
        public byte[] Image { get; set; }
    }

    public enum APIListType
    {
        None,
        List,
        Menu,
        GroupMenu,
        DialogList
    }

    public enum APIListLayout
    {
        Vertical,
        Horizontal,
        CoverFlow
    }
}