using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate;
using NHibernate.Context;
using FluentNHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Portal.Entities;
using FluentNHibernate.Conventions;

namespace Portal
{
	public class NHibernateHelper
	{
		public class TableNameConvention : IClassConvention
		{
			public void Apply(FluentNHibernate.Conventions.Instances.IClassInstance instance) {
				instance.Table(MvcApplication.Config("table.Prefix") + instance.EntityType.Name);
			}
		}

		static readonly object _locker = new object();

		private static ISessionFactory sessionFactory;
		public static ISessionFactory SessionFactory {
			get {
				lock (_locker) {
					if (sessionFactory == null) {
						sessionFactory = Fluently.Configure()
							.Database(
								MsSqlConfiguration.MsSql2008.ConnectionString(x => x.FromConnectionStringWithKey("dbConnection")).ShowSql()
							)
							.Mappings(m =>
								m.FluentMappings.AddFromAssemblyOf<Page>()
								.Conventions.Add<TableNameConvention>()
							)
							.ExposeConfiguration(BuildSchema)
							.BuildSessionFactory();
					}
				}
				return sessionFactory;
			}
		}

		private static void BuildSchema(Configuration config) {
			new SchemaUpdate(config).Execute(false, true);
			config.SetProperty("current_session_context_class", "web");
		}

		public static ISession OpenSession() {
			return SessionFactory.OpenSession();
		}

		public static ISession GetCurrentSession() {
			if (!CurrentSessionContext.HasBind(SessionFactory)) {
				CurrentSessionContext.Bind(SessionFactory.OpenSession());
			}
			return SessionFactory.GetCurrentSession();
		}

		public static void DisposeSession() {
			var session = GetCurrentSession();
			session.Close();
			session.Dispose();
		}

		public static void BeginTransaction() {
			GetCurrentSession().BeginTransaction();
		}

		public static void CommitTransaction() {
			var session = GetCurrentSession();
			if (session.Transaction.IsActive)
				session.Transaction.Commit();
		}

		public static void RollbackTransaction() {
			var session = GetCurrentSession();
			if (session.Transaction.IsActive)
				session.Transaction.Rollback();
		}
	}
}