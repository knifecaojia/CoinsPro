using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reg
{
    class RegDBQueue
    {
        private readonly Queue<RegInfo> queue;
        private static readonly object sync = new object();
        public RegDBQueue()
        {
            this.queue = new Queue<RegInfo>();
        }
        public RegInfo Get()
        {
            lock (sync)
            {
                return this.queue.Count <= 0 ? null : this.queue.Dequeue();
            }
        }
        public void UpdateDB()
        {
            lock (sync)
            {
                RegInfo user= this.queue.Count <= 0 ? null : this.queue.Dequeue();
                if (user == null)
                {
                    return;
                }
                else
                {
                    user.UpdateDB();
                }
            }
        }
        public void Put(RegInfo user)
        {
            lock (sync)
            {
                this.queue.Enqueue(user);
            }
        }

    }
}
