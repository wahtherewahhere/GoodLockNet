using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;
using wahtherewahhere.GoodLockNet;
using Xunit;

namespace wahtherewahhere.GoodLockNet.tests
{
    public partial class TestGoodSemaphoreSlim
    {
        [Theory]
        [InlineData(5, 5)]
        [InlineData(10, 5)]
        [InlineData(100, 5)]
        public async Task TestUseWait_Success(int taskNum, int taskDepth)
        {
            long expectNum = ((long)Math.Pow(2, taskDepth) - 1) * taskNum;
            List<Task> tasks = new List<Task>();
            Barrier barrier = new Barrier(taskNum);
            for (int i = 0; i < taskNum; i++)
            {
                int tmp = i;
                tasks.Add(
                Task.Run(async () =>
                    {
                        barrier.SignalAndWait();
                        await RecursiveSyncWithAsync(0, taskDepth);
                    })
                );
            }

            await Task.WhenAll(tasks.ToArray());
            Assert.True(counter == expectNum);
        }

        

        private GoodSemaphoreSlim goodSemaphoreSlim = new GoodSemaphoreSlim();
        private int counter = 0;
        private async Task RecursiveSyncWithAsync(int taskid, int i)
        {
            if (i == 0)
            {
                return;
            }
            using (goodSemaphoreSlim.UseWait())
            {
                counter++;
                await Task.Yield();
                await Task.Delay(3);
                await RecursiveSyncWithAsync(taskid, i - 1);
                await Task.Yield();
                await Task.Delay(3);
                await RecursiveSyncWithAsync(taskid, i - 1);
            }
        }
    }
}