using AquaPark.Data;
using AquaPark.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace AquaPark.Services
{
    public static class AuditService
    {
        public static void EnsureCreated()
        {
            AppData.db.Database.ExecuteSqlRaw("""
IF OBJECT_ID(N'ActionLog', N'U') IS NULL
BEGIN
    CREATE TABLE ActionLog (
        action_log_id INT IDENTITY(1,1) PRIMARY KEY,
        user_id INT NULL,
        user_name NVARCHAR(150) NULL,
        action_date DATETIME NOT NULL DEFAULT GETDATE(),
        action_type NVARCHAR(50) NOT NULL,
        entity_name NVARCHAR(100) NOT NULL,
        entity_id INT NULL,
        details NVARCHAR(500) NULL,
        FOREIGN KEY (user_id) REFERENCES Users(user_id)
    );
END
""");
        }

        public static void Log(string actionType, string entityName, int? entityId = null, string? details = null)
        {
            try
            {
                EnsureCreated();

                User? user = AppData.CurrentUser;
                int? userId = user?.UserId;
                string userName = user?.FullName ?? "Система";

                AppData.db.Database.ExecuteSqlInterpolated($"""
INSERT INTO ActionLog (user_id, user_name, action_date, action_type, entity_name, entity_id, details)
VALUES ({userId}, {userName}, {DateTime.Now}, {actionType}, {entityName}, {entityId}, {details})
""");
            }
            catch
            {
                // Журнал не должен блокировать основную операцию пользователя.
            }
        }

        public static List<ActionLogEntry> GetLogs(string searchText, DateTime? dateFrom, DateTime? dateTo)
        {
            EnsureCreated();

            string sql = "SELECT action_log_id, user_id, user_name, action_date, action_type, entity_name, entity_id, details FROM ActionLog WHERE 1 = 1";
            List<SqlParameter> parameters = new();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                sql += " AND (LOWER(ISNULL(user_name, '')) LIKE @search OR LOWER(action_type) LIKE @search OR LOWER(entity_name) LIKE @search OR LOWER(ISNULL(details, '')) LIKE @search)";
                parameters.Add(new SqlParameter("@search", $"%{searchText.Trim().ToLower()}%"));
            }

            if (dateFrom.HasValue)
            {
                sql += " AND action_date >= @dateFrom";
                parameters.Add(new SqlParameter("@dateFrom", dateFrom.Value.Date));
            }

            if (dateTo.HasValue)
            {
                sql += " AND action_date < @dateTo";
                parameters.Add(new SqlParameter("@dateTo", dateTo.Value.Date.AddDays(1)));
            }

            sql += " ORDER BY action_date DESC";

            using var command = AppData.db.Database.GetDbConnection().CreateCommand();
            command.CommandText = sql;
            command.Parameters.AddRange(parameters.ToArray());

            if (command.Connection!.State != System.Data.ConnectionState.Open)
            {
                command.Connection.Open();
            }

            using var reader = command.ExecuteReader();
            List<ActionLogEntry> result = new();

            while (reader.Read())
            {
                result.Add(new ActionLogEntry
                {
                    ActionLogId = reader.GetInt32(0),
                    UserId = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                    UserName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    ActionDate = reader.GetDateTime(3),
                    ActionType = reader.GetString(4),
                    EntityName = reader.GetString(5),
                    EntityId = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                    Details = reader.IsDBNull(7) ? string.Empty : reader.GetString(7)
                });
            }

            return result;
        }
    }
}
