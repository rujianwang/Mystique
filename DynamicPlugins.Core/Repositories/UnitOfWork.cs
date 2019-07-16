﻿using DynamicPlugins.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Transactions;

namespace DynamicPlugins.Core.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private DbHelper _dbHelper = null;
        private IPluginRepository _pluginRepository = null;
        private List<string> _scripts;

        public UnitOfWork()
        {
            _scripts = new List<string>();
        }

        public IPluginRepository PluginRepository
        {
            get
            {
                if (_pluginRepository == null)
                {
                    _pluginRepository = new PluginRepository(_dbHelper);
                }

                return _pluginRepository;
            }
        }

        public void Commit()
        {
            using (var sqlConnection = new SqlConnection())
            {
                sqlConnection.Open();

                using (var scope = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        foreach (var script in _scripts)
                        {
                            var cmd = new SqlCommand(script, sqlConnection);
                            cmd.ExecuteNonQuery();
                        }

                        scope.Commit();
                    }
                    catch
                    {
                        scope.Rollback();
                    }
                }
            }
        }
    }
}