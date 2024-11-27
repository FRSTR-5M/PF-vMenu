using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CitizenFX.Core;

using Newtonsoft.Json;

using static CitizenFX.Core.Native.API;
using static vMenuClient.CommonFunctions;
using static vMenuShared.KeyValueStoreSync;

namespace vMenuClient
{
    internal static class RemoteKeyValueStore
    {
        private static ulong _nextId = 0;
        public static ulong NewId() => _nextId++;

        private static Dictionary<ulong, Response> responses = new Dictionary<ulong, Response>();

        public static void ReceiveResponse(string json)
        {
            var response = JsonConvert.DeserializeObject<Response>(json);
            responses[response.Id] = response;
        }

        private static async Task<Response> SendRequest(Request request)
        {
            var json = JsonConvert.SerializeObject(request);
            BaseScript.TriggerLatentServerEvent($"vMenu:ServerKeyValueStoreRequest", 8192, json);

            Response response;
            while (!responses.TryGetValue(request.Id, out response))
            {
                await Delay(0);
            }

            responses.Remove(response.Id);
            return response;
        }


        public static async Task Remove(string key)
        {
            var request = new Request
            {
                Id = NewId(),
                Type = Request.RequestType.Remove,
                DataRemove = new Request.RequestDataRemove
                {
                    Key = key
                }
            };
            var response = await SendRequest(request);
            if (response.Type == Response.ResponseType.Error)
            {
                Debug.WriteLine($"Error removing \"{key}\" from remote key-value store: {response.Error}");
            }
        }

        public static async Task Set(string key, string value)
        {
            var request = new Request
            {
                Id = NewId(),
                Type = Request.RequestType.Set,
                DataSet = new Request.RequestDataSet
                {
                    Key = key,
                    Value = value
                }
            };
            var response = await SendRequest(request);
            if (response.Type == Response.ResponseType.Error)
            {
                Debug.WriteLine($"Error setting \"{key}={value}\" in remote key-value store: {response.Error}");
            }
        }

        public static async Task SetAll(Dictionary<string,string> keyValues)
        {
            var request = new Request
            {
                Id = NewId(),
                Type = Request.RequestType.SetAll,
                DataSetAll = new Request.RequestDataSetAll
                {
                    KeyValues = keyValues
                }
            };
            var response = await SendRequest(request);
            if (response.Type == Response.ResponseType.Error)
            {
                Debug.WriteLine($"Error setting multiple keys in remote key-value store: {response.Error}");
            }
        }

        public static async Task<Dictionary<string,string>> GetAll()
        {
            var request = new Request
            {
                Id = NewId(),
                Type = Request.RequestType.GetAll,
                DataGetAll = new Request.RequestDataGetAll
                {
                }
            };
            var response = await SendRequest(request);
            switch (response.Type)
            {
                case Response.ResponseType.Error:
                    Debug.WriteLine($"Error setting multiple keys in remote key-value store: {response.Error}");
                    goto case Response.ResponseType.NoServerStore;
                case Response.ResponseType.NoServerStore:
                    return new Dictionary<string, string>();
                case Response.ResponseType.Ok:
                    return response.DataGetAll?.KeyValues;
                default:
                    Debug.WriteLine("BUG");
                    return new Dictionary<string, string>();
            }
        }
    }

    public static partial class KeyValueStore
    {
        public static async Task<bool> SyncWithServer()
        {
            var localKvs = GetAll();
            var remoteKvs = await RemoteKeyValueStore.GetAll();

            var localNonServerKvs = localKvs
                .Where(kv => !remoteKvs.ContainsKey(kv.Key))
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            var remoteDiffLocal = remoteKvs.Where(kv => !localKvs.ContainsKey(kv.Key) || localKvs[kv.Key] != kv.Value);

            // We re-add local kv pairs so we always have string values
            foreach (var kv in localKvs)
            {
                SetResourceKvp(kv.Key, kv.Value);
            }
            foreach (var kv in remoteKvs)
            {
                SetResourceKvp(kv.Key, kv.Value);
            }
            await RemoteKeyValueStore.SetAll(localNonServerKvs);

            return remoteDiffLocal.Any();
        }

        public static async Task RemoveAsync(string key)
        {
            DeleteResourceKvp(key);
            await RemoteKeyValueStore.Remove(key);
        }
        public static void Remove(string key) => _ = RemoveAsync(key);

        public static async Task SetAsync(string key, string value)
        {
            SetResourceKvp(key, value);
            await RemoteKeyValueStore.Set(key, value);
        }
        public static void Set(string key, string value) => _ = SetAsync(key, value);

        public static async Task SetAsync(string key, int value) => await SetAsync(key, value.ToString());
        public static void Set(string key, int value) => _ = SetAsync(key, value);

        public static async Task SetAsync(string key, float value) => await SetAsync(key, value.ToString());
        public static void Set(string key, float value) => _ = SetAsync(key, value);
    }
}
