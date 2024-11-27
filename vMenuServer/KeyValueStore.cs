using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CitizenFX.Core;

using MySql.Data.MySqlClient;

using Newtonsoft.Json;

using vMenuShared;

using static CitizenFX.Core.Native.API;

using static vMenuShared.KeyValueStoreSync;

namespace vMenuServer
{
    public static class DatabaseKeyValueStore
    {
        public static string Truncate(this string value, int length)
        {
            if (value.Length <= length)
                return value;

            return value.Substring(0, length);
        }

        public const string SERVER_LICENSE = "0000000000000000000000000000000000000000";

        public static string ConnectionString { get; private set; }
        private static MySqlConnection connection = null;


        private static void CreateTable()
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new MySqlCommand
                {
                    Connection = connection,
                    CommandText = "CREATE TABLE IF NOT EXISTS `vMenu` (`PlayerLicense` CHAR(40) NOT NULL, `Key` VARCHAR(64) NOT NULL, `Value` TEXT, PRIMARY KEY (`PlayerLicense`, `Key`))"
                };
                command.ExecuteNonQuery();
            }
        }

        public static void Connect()
        {
            var connectionStringVar = ConfigManager.GetSettingsString(ConfigManager.Setting.vmenu_mysql_connection_string_var);
            if (string.IsNullOrEmpty(connectionStringVar))
                return;

            ConnectionString = GetConvar(connectionStringVar, null);
            if (string.IsNullOrEmpty(ConnectionString))
            {
                Debug.WriteLine($"Invalid or missing MySQL connection string convar \"{connectionStringVar}\"");
                ConnectionString = null;
                return;
            }

            connection = new MySqlConnection(ConnectionString);

            SetConvarReplicated(ConfigManager.Setting.vmenu_server_store.ToString(), "false");

            try
            {
                connection.Open();
            }
            catch (MySqlException e)
            {
                Debug.WriteLine($"Could not open database connection to \"{ConnectionString}\": {e}");
                ConnectionString = null;
                connection = null;
                return;
            }

            try
            {
                CreateTable();
            }
            catch (MySqlException e)
            {
                Debug.WriteLine($"Could create key-value store table: {e}");
                ConnectionString = null;
                connection = null;
                return;
            }

            SetConvarReplicated(ConfigManager.Setting.vmenu_server_store.ToString(), "true");
        }


        public static async Task<Dictionary<string, string>> GetAll(string playerLicense)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                var command = new MySqlCommand
                {
                    Connection = connection,
                    CommandText = "SELECT `Key`, `Value` FROM `vMenu` WHERE `PlayerLicense`=@playerLicense",
                };
                command.Parameters.AddWithValue("@playerLicense", playerLicense);

                var keyValues = new Dictionary<string, string>();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var key = reader.GetFieldValue<string>(0);
                        var value = reader.GetFieldValue<string>(1);
                        keyValues[key] = value;
                    }
                }

                return keyValues;
            }
        }

        public static async Task<Dictionary<string, string>> GetAll()
        {
            if (string.IsNullOrEmpty(ConnectionString))
                return new Dictionary<string, string>();

            return await GetAll(SERVER_LICENSE);
        }

        public static async Task Remove(string playerLicense, string key)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                var command = new MySqlCommand
                {
                    Connection = connection,
                    CommandText = "DELETE FROM `vMenu` WHERE `PlayerLicense`=@playerLicense AND `Key`=@key"
                };
                command.Parameters.AddWithValue("@playerLicense", playerLicense);
                command.Parameters.AddWithValue("@key", key.Truncate(64));

                await command.ExecuteNonQueryAsync();
            }
        }

        public static async Task Remove(string key)
        {
            if (string.IsNullOrEmpty(ConnectionString))
                return;

            await Remove(SERVER_LICENSE, key);
        }

        public static async Task Set(string playerLicense, string key, string value)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                var command = new MySqlCommand
                {
                    Connection = connection,
                    CommandText = "INSERT INTO `vMenu` (`PlayerLicense`, `Key`, `Value`) VALUES (@playerLicense, @key, @value) ON DUPLICATE KEY UPDATE `Value`=@value"
                };
                command.Parameters.AddWithValue("@playerLicense", playerLicense);
                command.Parameters.AddWithValue("@key", key.Truncate(64));
                command.Parameters.AddWithValue("@value", value);

                await command.ExecuteNonQueryAsync();
            }
        }

        public static async Task Set(string key, string value)
        {
            if (string.IsNullOrEmpty(ConnectionString))
                return;

            await Set(SERVER_LICENSE, key, value);
        }

        public static async Task SetAll(string playerLicense, Dictionary<string,string> keyValues)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                var command = new MySqlCommand
                {
                    Connection = connection,
                    CommandText = "INSERT INTO `vMenu` (`PlayerLicense`, `Key`, `Value`) VALUES (@playerLicense, @key, @value) ON DUPLICATE KEY UPDATE `Value`=@value"
                };
                command.Parameters.AddWithValue("@playerLicense", playerLicense);
                command.Parameters.AddWithValue("@key", null);
                command.Parameters.AddWithValue("@value", null);
                command.Prepare();

                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        foreach (var kv in keyValues)
                        {
                            command.Parameters["@key"].Value = kv.Key.Truncate(64);
                            command.Parameters["@value"].Value = kv.Value;
                            await command.ExecuteNonQueryAsync();
                            await BaseScript.Delay(0);
                        }
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        throw e;
                    }
                }
            }
        }

        public static async Task SetAll(Dictionary<string,string> keyValues)
        {
            if (string.IsNullOrEmpty(ConnectionString))
                return;

            await SetAll(SERVER_LICENSE, keyValues);
        }
    }

    public static class ClientRemoteKeyValueStore
    {
        private static void SendResponse(Player player, Response response)
        {
            player.TriggerLatentEvent("vMenu:ServerKeyValueStoreResponse", 8192, JsonConvert.SerializeObject(response));
        }


        public static async Task HandleRequest(Player player, string json)
        {
            var request = JsonConvert.DeserializeObject<Request>(json);
            var response = await HandleRequest(player, request);
            SendResponse(player, response);
        }

        private static async Task<Response> HandleRequest(Player player, Request request)
        {
            if (string.IsNullOrEmpty(DatabaseKeyValueStore.ConnectionString))
            {
                return new Response
                {
                    Id = request.Id,
                    Type = Response.ResponseType.NoServerStore
                };
            }

            var license = player.Identifiers["license"].Truncate(40);

            Response response;
            try
            {
                switch (request.Type)
                {
                    case Request.RequestType.GetAll:
                        response = await HandleRequestGetAll(license, request);
                        break;
                    case Request.RequestType.Remove:
                        response = await HandleRequestRemove(license, request);
                        break;
                    case Request.RequestType.Set:
                        response = await HandleRequestSet(license, request);
                        break;
                    case Request.RequestType.SetAll:
                        response = await HandleRequestSetAll(license, request);
                        break;
                    default:
                        response = new Response
                        {
                            Type = Response.ResponseType.Error,
                            Error = "The request was invalid"
                        };
                        break;
                }
                response.Type = Response.ResponseType.Ok;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error handling database key-value store request {request.Type} (id {request.Id}) from player {player.Name} ({license}): {e}");
                response = new Response
                {
                    Type = Response.ResponseType.Error,
                    Error = $"request id {request.Id}"
                };
            }
            response.Id = request.Id;

            return response;
        }

        private static async Task<Response> HandleRequestGetAll(string playerLicense, Request _)
        {
            var keyValues = await DatabaseKeyValueStore.GetAll(playerLicense);
            return new Response
            {
                DataGetAll = new Response.ResponseDataGetAll
                {
                    KeyValues = keyValues
                }
            };
        }

        private static async Task<Response> HandleRequestRemove(string playerLicense, Request requestRemove)
        {
            await DatabaseKeyValueStore.Remove(playerLicense, requestRemove.DataRemove?.Key);
            return new Response
            {
                DataRemove = new Response.ResponseDataRemove
                {}
            };
        }

        private static async Task<Response> HandleRequestSet(string playerLicense, Request requestSet)
        {
            var data = requestSet.DataSet.Value;
            await DatabaseKeyValueStore.Set(playerLicense, data.Key, data.Value);
            return new Response
            {
                DataSet = new Response.ResponseDataSet
                {
                }
            };
        }

        private static async Task<Response> HandleRequestSetAll(string playerLicense, Request requestSetAll)
        {
            await DatabaseKeyValueStore.SetAll(playerLicense, requestSetAll.DataSetAll?.KeyValues);
            return new Response
            {
                DataSetAll = new Response.ResponseDataSetAll
                {
                }
            };
        }
    }

    public static partial class KeyValueStore
    {
        public async static Task SyncWithDatabase()
        {
            var localKvs = GetAll();
            var databaseKvs = await DatabaseKeyValueStore.GetAll();

            var localNonDatabaseKvs = localKvs
                .Where(kv => !databaseKvs.ContainsKey(kv.Key))
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            // We re-add local kv pairs so we always have string values
            foreach (var kv in localKvs)
            {
                SetResourceKvp(kv.Key, kv.Value);
            }
            foreach (var kv in databaseKvs)
            {
                SetResourceKvp(kv.Key, kv.Value);
            }
            try
            {
                await DatabaseKeyValueStore.SetAll(localNonDatabaseKvs);
            }
            catch (MySqlException e)
            {
                Debug.WriteLine($"Error setting multiple keys in database key-value store: {e.Message}");
            }
        }

        public async static Task Remove(string key)
        {
            DeleteResourceKvp(key);
            try
            {
                await DatabaseKeyValueStore.Remove(key);
            }
            catch (MySqlException e)
            {
                Debug.WriteLine($"Error removing key \"{key}\" from database key-value store: {e.Message}");
            }
        }

        public async static Task Set(string key, string value)
        {
            SetResourceKvp(key, value);
            try
            {
                await DatabaseKeyValueStore.Set(key, value);
            }
            catch (MySqlException e)
            {
                Debug.WriteLine($"Error setting \"{key}={value}\" in database key-value store: {e.Message}");
            }
        }
    }
}
