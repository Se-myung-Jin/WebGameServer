using StackExchange.Redis;

#pragma warning disable CS1998

namespace Common
{
    public class PubSubManager
    {
        private HashSet<PubSubCommandProtocol> m_commonHandle = new HashSet<PubSubCommandProtocol>
        {
            PubSubCommandProtocol.TableRefresh,
        };

        public virtual async Task InitializeAsync()
        {
            await RegisterSubScribeAsync();
        }

        public virtual void Send(SUBSCRIPTION_COMMAND command)
        {
            throw new NotImplementedException();
        }


        public virtual async Task SendAsync(SUBSCRIPTION_COMMAND command)
        {
            throw new NotImplementedException();
        }

        public void Send(int id, string channel, SUBSCRIPTION_COMMAND command)
        {
            DatabaseContextContainer.Instance.Redis.Publish(id, channel, command.WriteMemoryPack(), true);
        }

        public async Task SendAsync(int id, string channel, SUBSCRIPTION_COMMAND command)
        {
            await DatabaseContextContainer.Instance.Redis.PublishAsync(id, channel, command.WriteMemoryPack(), true);
        }

        protected virtual async Task RegisterSubScribeAsync()
        {
            //await DatabaseContextContainer.Instance.Redis.RegisterSubscibeAsync(GLOBAL_VALUES.GLOBAL_PUBSUB, GLOBAL_VALUES.SUBSCRIBER_CHANNEL_WORLDSTATE, SubscribeRedis);
            await Task.FromResult(0);
        }

        protected virtual async void SubscribeRedis(RedisChannel channel, RedisValue value)
        {
            LogSystem.Log.DebugBuildTrace($"{GetType().Name} SUBSCRIBER {channel} SIZE[{value.Length()}]");
            try
            {

                byte[] body = value;
                await using var stream = SystemGlobal.Instance.RecycleMemory.GetStream(body);
                var command = await stream.ReadMemoryPackAsync<SUBSCRIPTION_COMMAND>();

                if (command == null)
                {
                    LogSystem.Log.Error($"{GetType().Name} SUBSCRIBER {channel} Command Parsing Fail");
                    return;
                }

                OnExecute(command);
            }
            catch (Exception ex)
            {
                LogSystem.Log.Error(ex);
            }
        }

        protected virtual async void OnExecute(SUBSCRIPTION_COMMAND command)
        {
            if (m_commonHandle.Contains(command.Protocol))
            {
                CommonHandle(command);
                return;
            }

            ExternalHandle(command);
        }

        protected virtual async void CommonHandle(SUBSCRIPTION_COMMAND command)
        {
            switch (command.Protocol)
            {
                case PubSubCommandProtocol.TableRefresh:
                    {
                        var realCommand = command as COMMAND_TABLE_REFRESH;
                        
                    } break;
            }
        }

        protected virtual async void ExternalHandle(SUBSCRIPTION_COMMAND command)
        {
            await Task.FromResult(0);
        }
    }
}
