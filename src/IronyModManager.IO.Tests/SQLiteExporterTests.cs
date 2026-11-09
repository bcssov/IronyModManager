// ***********************************************************************
// Assembly         : IronyModManager.IO.Tests
// ***********************************************************************

using System;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using IronyModManager.IO.Common;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Mods;
using IronyModManager.Models.Common;
using IronyModManager.Shared;
using IronyModManager.Shared.MessageBus;
using Microsoft.Data.Sqlite;
using Moq;
using Xunit;

namespace IronyModManager.IO.Tests
{
    public class SQLiteExporterTests
    {
        [Theory]
        [InlineData("initialSchema", true, "0000001000", "0000001001")]
        [InlineData("modifyPositionToInteger", true, "0", "1")]
        [InlineData("addModShortDescription", true, "0", "1")]
        [InlineData("addModShortDescription", false, "0", "1")]
        public async Task Should_update_only_position_and_append_new_playset_link(string migration, bool compositeKey, string existingPosition, string newPosition)
        {
            var directory = Directory.CreateTempSubdirectory("IronySQLiteExporterTests-");
            using var stable = new SqliteConnection($@"Data Source=""{Path.Combine(directory.FullName, "launcher-v2.sqlite")}""");
            using var beta = new SqliteConnection($@"Data Source=""{Path.Combine(directory.FullName, "launcher-v2_openbeta.sqlite")}""");
            try
            {
                CreateDatabase(stable, migration, compositeKey);
                CreateDatabase(beta, migration, compositeKey);
                new OrmInitialization().OnPostStartup();
                var writer = new ModWriter(Mock.Of<ILogger>(), Mock.Of<IMapper>(), Mock.Of<IDriveInfoProvider>(), Mock.Of<IMessageBus>());
                var mod = Mock.Of<IMod>(m => m.DescriptorFile == "mod/patch.mod" && m.FileName == "mod/patch" && m.Name == "Patch");

                Assert.True(await writer.ApplyModsAsync(new ModWriterParameters
                {
                    AppendOnly = true,
                    DescriptorType = DescriptorType.DescriptorMod,
                    RootDirectory = directory.FullName,
                    TopPriorityMods = [mod]
                }));

                AssertLinks(stable, existingPosition, newPosition);
                AssertLinks(beta, existingPosition, newPosition);
            }
            finally
            {
                stable.Close();
                beta.Close();
                SqliteConnection.ClearPool(stable);
                SqliteConnection.ClearPool(beta);
                directory.Delete(recursive: true);
            }
        }

        private void CreateDatabase(SqliteConnection connection, string migration, bool compositeKey)
        {
            connection.Open();
            using var command = connection.CreateCommand();
            var positionType = migration == "initialSchema" ? "TEXT" : "INTEGER";
            var primaryKey = compositeKey ? ", PRIMARY KEY (playsetId, modId)" : string.Empty;
            command.CommandText = $"""
                CREATE TABLE knex_migrations (id INTEGER PRIMARY KEY, name TEXT);
                INSERT INTO knex_migrations VALUES (1, $migration);
                CREATE TABLE mods (id TEXT NOT NULL PRIMARY KEY, gameRegistryId TEXT, status TEXT NOT NULL, source TEXT NOT NULL);
                INSERT INTO mods VALUES ('existing-mod', 'mod/existing.mod', 'ready_to_play', 'local');
                CREATE TABLE playsets (
                    id TEXT NOT NULL PRIMARY KEY, name TEXT NOT NULL, isActive BOOLEAN, loadOrder TEXT,
                    createdOn DATETIME NOT NULL, isRemoved BOOLEAN NOT NULL DEFAULT FALSE
                );
                INSERT INTO playsets (id, name, isActive, loadOrder, createdOn) VALUES ('irony-playset', 'IronyModManager', 1, 'custom', 0);
                CREATE TABLE playsets_mods (
                    playsetId TEXT NOT NULL REFERENCES playsets(id) ON DELETE CASCADE,
                    modId TEXT NOT NULL REFERENCES mods(id) ON DELETE CASCADE,
                    enabled BOOLEAN DEFAULT 1,
                    position {positionType}
                    {primaryKey}
                );
                INSERT INTO playsets_mods VALUES ('irony-playset', 'existing-mod', 0, 99);
                CREATE TRIGGER guard_playset_mod_columns
                    BEFORE UPDATE OF modId, playsetId, enabled ON playsets_mods
                    BEGIN
                        SELECT RAISE(ABORT, 'Reordering must update only position');
                    END;
                """;
            command.Parameters.AddWithValue("$migration", migration);
            command.ExecuteNonQuery();
            connection.Close();
        }

        private void AssertLinks(SqliteConnection connection, string existingPosition, string newPosition)
        {
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT pm.playsetId, pm.modId, pm.enabled, CAST(pm.position AS TEXT), m.gameRegistryId
                FROM playsets_mods pm JOIN mods m ON m.id = pm.modId
                ORDER BY pm.position
                """;
            using var rows = command.ExecuteReader();
            Assert.True(rows.Read());
            Assert.Equal("irony-playset", rows.GetString(0));
            Assert.Equal("existing-mod", rows.GetString(1));
            Assert.False(rows.GetBoolean(2));
            Assert.Equal(existingPosition, rows.GetString(3));

            Assert.True(rows.Read());
            Assert.Equal("irony-playset", rows.GetString(0));
            Assert.True(Guid.TryParse(rows.GetString(1), out _));
            Assert.True(rows.GetBoolean(2));
            Assert.Equal(newPosition, rows.GetString(3));
            Assert.Equal("mod/patch.mod", rows.GetString(4));
            Assert.False(rows.Read());
        }
    }
}
