using Autofac;
using Library.SystemModels;
using Npgsql;
using Server.DBModels;
using Server.Envir;
using Server.Repository;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Server
{
    public static class ServerContainerBuilder
    {
        public static ContainerBuilder RegisterDatabase(this ContainerBuilder builder)
        {
            var dbProvider = (DatabaseProvider)Config.DBProvider;

            switch (dbProvider)
            {
                case DatabaseProvider.File:
                    builder.Register(x =>
                    {
                        var session = new MirDB.Session(Config.DBConnStr);
                        session.Initialize(
                            Assembly.GetAssembly(typeof(ItemInfo)), // returns assembly LibraryCore
                            Assembly.GetAssembly(typeof(AccountInfo)) // returns assembly ServerLibrary
                        );
                        return session;
                    }).SingleInstance();
                    builder.RegisterType<Repository.File.AccountRepository>().As<IAccountRepository>().SingleInstance();
                    break;
                case DatabaseProvider.PostgreSQL:
                    builder.Register(x => new NpgsqlConnection(Config.DBConnStr)).SingleInstance();
                    builder.RegisterType<Repository.PostgreSQL.AccountRepository>().As<IAccountRepository>().SingleInstance();
                    break;
            }

            return builder;
        }
    }
}
