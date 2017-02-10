using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Cruise.DashboardBroker
{
    public class BackScheduler : IDisposable
    {
        private static BackScheduler _instance;
        private static readonly object Locker = new object();

        public enum ScheduleType
        {
            Periodical, Dynamic
        }

        public class Entry
        {
            public ScheduleType Type { get; set; }
            public int Period { get; set; }
            public DateTime? LastRun { get; set; }
            public IScheduleProcess Item { get; set; }

            private object _locker = new object();

            public DateTime NextRun
            {
                get
                {
                    switch (Type)
                    {
                        //case ScheduleType.Dynamic:
                        //    return ((IDynamicScheduleProcess) Item).Schedule;
                        case ScheduleType.Periodical:
                            return LastRun.HasValue ? LastRun.Value.AddMilliseconds(Period) : DateTime.MinValue;
                    }
                    return DateTime.MaxValue;
                }
            }

            public bool IsScheduled
            {
                get
                {
                    if (!Monitor.TryEnter(_locker)) return false;
                    try
                    {
                        return NextRun <= DateTime.Now;
                    }
                    finally
                    {
                        Monitor.Exit(_locker);
                    }
                }
            }

            public override string ToString()
            {
                return string.Format("Type:[{0}];Period:[{1}];NextRun:[{2}];Item:[{3}];", Type, Period, NextRun, Item);
            }

            ManualResetEventSlim _scheduleRunning = new ManualResetEventSlim(false);

            //TODO: Should be protection of reentry same Entry Callback, if ScheduleRun time is exceeds period time
            void Callback(object state)
            {
                if(_scheduleRunning.Wait(0)) return;
                lock (_locker)
                {
                    _scheduleRunning.Set();
                    try
                    {
                        Item.ScheduleRun();
                    }
                    catch (Exception x)
                    {
                    }
                    LastRun = DateTime.Now;
                    _scheduleRunning.Reset();
                }
            }

            public void ScheduleRun()
            {
                ThreadPool.QueueUserWorkItem(Callback);
                //(new Thread(() =>
                //{
                    
                //}) { Name = string.Format("ScheduleRun:[{0}]", this) }).Start();
            }
        }

        private readonly List<Entry> _items = new List<Entry>();
        private Thread _workerThread;
        public const int SleepSicle = 100;

        private BackScheduler()
        {
            StartProcess();
            AppDomain.CurrentDomain.DomainUnload += (s,e) => {
                this.Dispose();
            };
        }

        private List<Entry> GetScheduledItems()
        {
            lock (_items)
            {
                return _items.Where(i => i.IsScheduled).ToList();
            }
        }

        ManualResetEventSlim _shutdown = new ManualResetEventSlim(false);

        public void ThreadMethod()
        {
            try
            {
                while (!_shutdown.Wait(100))
                {
                    var scheduledItems = GetScheduledItems();

                    if ((scheduledItems == null) || (!scheduledItems.Any()))
                    {
                        Thread.Sleep(SleepSicle); 
                        continue;
                    }
                    foreach (var item in scheduledItems)
                    {
                        item.ScheduleRun();
                    }
                }
            }
            catch (Exception x)
            {
            }
        }

        public bool Contains(IScheduleProcess item)
        {
            lock (_items)
            {
                return _items.FirstOrDefault(i => i.Item == item) != null;
            }
        }

        public static int Minutes(int value)
        {
            return 60000*value;
        }

        public static int Seconds(int value)
        {
            return 1000 * value;
        }

        public void Add(IScheduleProcess item, int periodMillis = 0)
        {
            lock (_items)
            {
                if (item == null) throw new Exception("Can't add null item");
                if (Contains(item)) throw new Exception(string.Format("{0} allready added", item.GetType().Name));
                _items.Add(new Entry
                {
                    Item = item,
                    Period = periodMillis,
                    Type = (periodMillis == 0) ? ScheduleType.Dynamic : ScheduleType.Periodical
                });
            }
        }

        public void Remove(IScheduleProcess item, bool checkExists = true)
        {
            lock (_items)
            {
                if (item == null) throw new Exception("Can't find null item");
                if (!Contains(item) && checkExists) throw new Exception(string.Format("Item {0} is not in list", item.GetType().Name));
                _items.RemoveAll(i => i.Item == item);
            }
        }

        public void Clear()
        {
            lock (_items)
            {
                _items.Clear();
            }
        }

        private bool IsEmpty { get { lock (_items) { return !_items.Any(); } } }
        public int Count { get { lock (_items) { return _items.Count; } } }

        private void StopProcess()
        {
            if (_workerThread != null)
            {
                _shutdown.Set();
                _workerThread.Join();
                _workerThread = null;
            }
        }

        private void StartProcess()
        {
            _workerThread = new Thread(ThreadMethod) {Name = "BackScheduler", IsBackground = true};
            _workerThread.Start();
        }

        public static BackScheduler Instance
        {
            get
            {
                lock (Locker)
                {
                    return _instance ?? (_instance = new BackScheduler());
                }
            }
        }

        public void Dispose()
        {
            if (!IsEmpty)Clear();
            StopProcess();
        }
    }
}
