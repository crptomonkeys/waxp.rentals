using System;
using System.Threading.Tasks;
using Nano.Net;
using Nano.Net.Numbers;
using WaxRentals.Banano.Config;

namespace WaxRentals.Banano.Transact
{
    public class WrappedAccount
    {

        private Account _account;

        public WrappedAccount(BananoSeed seed, uint index)
        {
            _account = new Account(seed.Seed, index, "ban");
        }

        public void Send(string target, decimal banano)
        {

        }

        public async Task<BigDecimal> Receive()
        {
            var client = new RpcClient("http://host-x86-01:17072"); // we need to change httpclient to longer timeout and precalc work
            var pending = await client.PendingBlocksAsync(_account.Address, int.MaxValue);

            BigDecimal result = 0;
            foreach (var block in pending.PendingBlocks)
            {
                try
                {
                    await client.UpdateAccountAsync(_account);
                    var work = await client.WorkGenerateAsync(_account.Frontier);
                    var receive = Block.CreateReceiveBlock(_account, block.Value, work.Work);
                    await client.ProcessAsync(receive);
                    result += BigDecimal.Parse(block.Value.Amount);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return result;
        }

    }
}
