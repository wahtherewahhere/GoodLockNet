#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER || NET5_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Xunit;
using System.Security.Cryptography;

namespace wahtherewahhere.GoodLockNet.tests
{
    public partial class TestGoodSemaphoreSlim
    {
        [Theory]
        [InlineData(5, 5)]
        [InlineData(10, 5)]
        [InlineData(100, 5)]
        public async Task TestUseWaitAsync_Success(int taskNum, int taskDepth)
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
                    await RecursiveAsyncWithAsync(0, taskDepth);
                })
                );
            }

            await Task.WhenAll(tasks.ToArray());
            Assert.True(counter == expectNum);
        }

        private async Task RecursiveAsyncWithAsync(int taskid, int i)
        {
            if (i == 0)
            {
                return;
            }
            await using (goodSemaphoreSlim.UseWaitAsync())
            {
                counter++;
                await Task.Yield();
                await Task.Delay(RandomNumberGenerator.GetInt32(3));
                await RecursiveAsyncWithAsync(taskid, i - 1);
                await Task.Yield();
                await Task.Delay(RandomNumberGenerator.GetInt32(3));
                await RecursiveAsyncWithAsync(taskid, i - 1);
            }
        }
    }
}
#endif
