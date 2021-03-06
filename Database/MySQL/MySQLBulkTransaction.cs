/*
    Copyright 2011 MCForge
        
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.opensource.org/licenses/ecl2.php
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace MCGalaxy.SQL {
    
    public sealed class MySQLBulkTransaction : BulkTransaction {

        public MySQLBulkTransaction(string connString) {
            connection = new MySqlConnection(connString);
            connection.Open();
            connection.ChangeDatabase(Server.MySQLDatabaseName);

            transaction = connection.BeginTransaction();
        }

        public override IDbCommand CreateCommand(string query) {
            return new MySqlCommand(query, (MySqlConnection)connection, (MySqlTransaction)transaction);
        }
        
        public override IDataParameter CreateParam(string paramName, DbType type) {
            MySqlParameter arg = new MySqlParameter(paramName, null);
            arg.DbType = type;
            return arg;
        }
    }
}
