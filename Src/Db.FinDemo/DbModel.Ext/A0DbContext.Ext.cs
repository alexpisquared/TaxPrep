using System;
using System.Data.Entity;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Diagnostics;

namespace Db.FinDemo.DbModel
{
  public partial class A0DbContext : DbContext
  {
    public static A0DbContext Create() => new A0DbContext();

    A0DbContext()
      : base() => Database.Connection.ConnectionString = ConStr; //tu: remove CREATE TABLE permissin neeed - Database.SetInitializer<A0DbContext>(null);//``System.Diagnostics.Trace.WriteLine($" ** db from con str:{DbName}");

    public static string DbName => ConStr.Split(';').ToList().FirstOrDefault(r => r.Split('=')[0].Equals("initial catalog", StringComparison.OrdinalIgnoreCase))?.Split('=')[1];

    // Azure SQL Database connection string does not work with .NET Framework 4 (OP 2025--04-12)
    static string ConStr_____ => JObject.Parse(File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @$"AppSettings\MinFin7.MNT\AppSettings.json")))["SqlConStrFormat"]?.ToString();
    static string ConStr
    {
      get
      {
#if DEBUG
        var dbg = "Dbg";
#else
        var dbg = ""; // == Rls
#endif

        return $@"Data Source=freedemosqlserver.database.windows.net;Initial Catalog=FreeFinDemoDb;User ID=OleksaAdmin;Password=Y3{Environment.GetEnvironmentVariable("freedemosqlserver", EnvironmentVariableTarget.User)}"; // 2024-07-20
        return $@"data source=.\sqlexpress;initial Catalog=4_FinDemo_Use_Azure_one_{dbg};integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";
      }
    }

    //todo: needs core: [DbFunction(Name = "SOUNDEX", IsBuiltIn = true)] public static string SoundsLike(string sounds) => throw new NotImplementedException(); //tu: SOUNDEX

    public static string SolutionCfg =>
#if DEBUG
        "dbg";
#else
        "RLS";
#endif
  }
}

/*USE [FinDemo]
GO

/* ***** Object:  Table [dbo].[BalAmtHist]    Script Date: 2015-09-30 20:35:27 ****** /
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[BalAmtHist](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TxMoneySrcId] [int] NOT NULL,
	[BalAmt] [money] NOT NULL,
	[BalTpe] [varchar](20) NOT NULL,
	[AsOfDate] [datetime] NOT NULL,
	[Notes] [varchar](2000) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[ModifiedAt] [datetime] NULL,
 CONSTRAINT [PK_BalAmtHist] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[BalAmtHist] ADD  CONSTRAINT [DF_BalAmtHist_TxSourceId]  DEFAULT ((0)) FOR [TxMoneySrcId]
GO

ALTER TABLE [dbo].[BalAmtHist] ADD  CONSTRAINT [DF_BalAmtHist_LoggetAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO

ALTER TABLE [dbo].[BalAmtHist]  WITH CHECK ADD  CONSTRAINT [FK_BalAmtHist_TxSource] FOREIGN KEY([TxMoneySrcId])
REFERENCES [dbo].[TxMoneySrc] ([Id])
GO

ALTER TABLE [dbo].[BalAmtHist] CHECK CONSTRAINT [FK_BalAmtHist_TxSource]
GO


*/
