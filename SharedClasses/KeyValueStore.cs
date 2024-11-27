using System.Collections.Generic;

using static CitizenFX.Core.Native.API;

#if CLIENT
namespace vMenuClient
#else
namespace vMenuServer
#endif
{
    public static partial class KeyValueStore
    {
        public static string GetString(string key)
        {
            try
            {
                return GetResourceKvpString(key);
            }
            catch
            {
                try
                {
                    return GetResourceKvpInt(key).ToString();
                }
                catch
                {
                    return GetResourceKvpFloat(key).ToString();
                }
            }
        }
        public static int GetInt(string key)
        {
            var ok = int.TryParse(GetString(key), out var result);
            return ok ? result : 0;
        }
        public static float GetFloat(string key)
        {
            var ok = float.TryParse(GetString(key), out var result);
            return ok ? result : 0;
        }

        public static Dictionary<string, string> GetAll() => GetAllWithPrefix("");

        public static Dictionary<string, string> GetAllWithPrefix(string keyPrefix)
        {
            var keyValues = new Dictionary<string,string>();

            var handle = StartFindKvp(keyPrefix);
            while (true)
            {
                var key = FindKvp(handle);
                if (key is "" or null or "NULL")
                    break;

                keyValues[key] = GetString(key);
            }
            EndFindKvp(handle);

            return keyValues;
        }
    }
}

namespace vMenuShared
{
    public static class KeyValueStoreSync
    {
        public struct Request
        {
            public enum RequestType
            {
                GetAll,
                Remove,
                Set,
                SetAll,
            }


            public struct RequestDataGetAll
            {
            }

            public struct RequestDataRemove
            {
                public string Key { get; set; }
            }

            public struct RequestDataSet
            {
                public string Key { get; set; }
                public string Value { get; set; }
            }
            public struct RequestDataSetAll
            {
                public Dictionary<string, string> KeyValues;
            }


            public ulong Id { get; set; }

            public RequestType Type { get; set; }

            public RequestDataGetAll? DataGetAll { get; set; }
            public RequestDataRemove? DataRemove { get; set; }
            public RequestDataSet? DataSet { get; set; }
            public RequestDataSetAll? DataSetAll { get; set; }
        }

        public struct Response
        {
            public enum ResponseType
            {
                Ok,
                Error,
                NoServerStore
            }


            public struct ResponseDataGetAll
            {
                public Dictionary<string, string> KeyValues { get; set; }
            }

            public struct ResponseDataRemove
            {
            }

            public struct ResponseDataSet
            {
            }
            public struct ResponseDataSetAll
            {
            }


            public ulong Id { get; set; }
            public string Error { get; set; }

            public ResponseType Type { get; set; }

            public ResponseDataGetAll? DataGetAll { get; set; }
            public ResponseDataRemove? DataRemove { get; set; }
            public ResponseDataSet? DataSet { get; set; }
            public ResponseDataSetAll? DataSetAll { get; set; }
        }
    }
}
