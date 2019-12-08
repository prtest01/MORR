﻿using System;
using System.Collections.Generic;
using System.Text;
using MORR.Shared.Events.Queue;
using MORR.Modules.WebBrowser.Events;
using MORR.Shared.Events;
using System.Composition;
namespace MORR.Modules.WebBrowser.Producers
{
    [Export(typeof(CloseTabEventProducer))]
    [Export(typeof(EventQueue<CloseTabEvent>))]
    [Export(typeof(EventQueue<Event>))]
    public class CloseTabEventProducer : EventQueue<CloseTabEvent>
    {
        public override IAsyncEnumerable<CloseTabEvent> GetEvents()
        {
            throw new NotImplementedException();
        }

        protected override void Enqueue(CloseTabEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}